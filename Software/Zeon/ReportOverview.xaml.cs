using BussinessLogicLayer;
using EntitiesLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Zeon
{

    public partial class ReportOverview : UserControl
    {
        private ReportService reportService;
        private int _trainerId;

        public ReportOverview(int trainerId)
        {
            InitializeComponent();
            _trainerId = trainerId;
            reportService = new ReportService();
            Load();
        }

        private async void Load()
        {
            try
            {
                var clients = await reportService.GetClientsByTrainerId(_trainerId);
                cmbClients.ItemsSource = clients;
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void ShowError(string message)
        {
            txtError.Text = message;
            txtError.Visibility = Visibility.Visible;
        }

        private async void BtnGenerate_Click(object sender, RoutedEventArgs e)
        {
            txtError.Visibility = Visibility.Collapsed;
            stackReport.Visibility = Visibility.Collapsed;

            if (cmbClients.SelectedItem == null || dtpFrom.SelectedDate == null || dtpTo.SelectedDate == null)
            {
                ShowError("Mora se odabrati klijent i valjan vremenski period.");
                return;
            }

            var client = (Client)cmbClients.SelectedItem;
            var from = dtpFrom.SelectedDate.Value;
            var to = dtpTo.SelectedDate.Value;

            if (!reportService.Validate(client.id_client, from, to))
            {
                ShowError("Neispravan unos — početni datum mora biti prije završnog.");
                return;
            }

            try
            {
                var report = await reportService.GenerateReport(client.id_client, from, to);

                ShowReport(report);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void ShowReport(TrainingReportDTO report)
        {
            if (report.WeightValues.Count >= 2)
            {
                double first = report.WeightValues.First();
                double last = report.WeightValues.Last();
                double change = last - first;
                txtWeightChange.Text = $"{change:+0.0;-0.0;0.0} kg";
                txtWeightChange.Foreground = change <= 0
                    ? (SolidColorBrush)new BrushConverter().ConvertFrom("#FF0000")
                    : (SolidColorBrush)new BrushConverter().ConvertFrom("#44FF44");
                txtWeightDetail.Text = $"Početak: {first} kg → Kraj: {last} kg";
            }
            else
            {
                txtWeightChange.Text = "N/A";
                txtWeightDetail.Text = "Nema dovoljno podataka";
            }

            txtTotalTrainings.Text = report.TotalTrainings.ToString();
            txtTrainingDetail.Text = "U odabranom periodu";

            txtPersonalRecords.Text = report.PersonalRecords.Count.ToString();
            txtPRDetail.Text = report.PersonalRecords.Count > 0
                ? $"Posljednji: {report.PersonalRecords.First().Exercise?.exercise_name ?? "—"}"
                : "Nema rekorda";

            Nacrtaj_GrafTezine(report);
            Nacrtaj_GrafPlaniranoStvarno(report);

            stackReport.Visibility = Visibility.Visible;
        }

        private void Nacrtaj_GrafTezine(TrainingReportDTO report)
        {
            canvasWeight.Children.Clear();
            if (report.WeightValues.Count < 2) return;

            double canvasW = canvasWeight.Width > 0 ? canvasWeight.Width : 800;
            double canvasH = 220;
            double padL = 45, padR = 15, padT = 15, padB = 35;
            double plotW = canvasW - padL - padR;
            double plotH = canvasH - padT - padB;

            double minW = report.WeightValues.Min() - 1;
            double maxW = report.WeightValues.Max() + 1;
            double range = maxW - minW;
            if (range == 0) range = 1;

            int count = report.WeightValues.Count;

            for (int i = 0; i <= 3; i++)
            {
                double val = minW + (range / 3) * i;
                double y = padT + plotH - (val - minW) / range * plotH;

                var gridLine = new Line
                {
                    X1 = padL, Y1 = y,
                    X2 = padL + plotW, Y2 = y,
                    Stroke = (SolidColorBrush)new BrushConverter().ConvertFrom("#2A2A2A")
                };
                canvasWeight.Children.Add(gridLine);

                var yLabel = new TextBlock
                {
                    Text = $"{val:0.0}",
                    FontSize = 9,
                    Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#7A7A7A"),
                    Width = 38,
                    TextAlignment = TextAlignment.Right
                };
                Canvas.SetLeft(yLabel, 2);
                Canvas.SetTop(yLabel, y - 6);
                canvasWeight.Children.Add(yLabel);
            }

            var points = new List<System.Windows.Point>();
            for (int i = 0; i < count; i++)
            {
                double x = padL + (count > 1 ? (plotW / (count - 1)) * i : 0);
                double y = padT + plotH - (report.WeightValues[i] - minW) / range * plotH;
                points.Add(new System.Windows.Point(x, y));

                if (i < report.WeightDates.Count)
                {
                    var xLabel = new TextBlock
                    {
                        Text = report.WeightDates[i].ToString("dd.MM"),
                        FontSize = 9,
                        Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#7A7A7A"),
                        Width = 38,
                        TextAlignment = TextAlignment.Center
                    };
                    Canvas.SetLeft(xLabel, x - 19);
                    Canvas.SetTop(xLabel, canvasH - padB + 5);
                    canvasWeight.Children.Add(xLabel);
                }

                var valLabel = new TextBlock
                {
                    Text = $"{report.WeightValues[i]} kg",
                    FontSize = 9,
                    Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFFFFD"),
                    Width = 42,
                    TextAlignment = TextAlignment.Center
                };
                Canvas.SetLeft(valLabel, x - 21);
                Canvas.SetTop(valLabel, y - 20);
                canvasWeight.Children.Add(valLabel);

                var dot = new Ellipse
                {
                    Width = 7, Height = 7,
                    Fill = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF0000")
                };
                Canvas.SetLeft(dot, x - 3.5);
                Canvas.SetTop(dot, y - 3.5);
                canvasWeight.Children.Add(dot);
            }

            for (int i = 0; i < points.Count - 1; i++)
            {
                var line = new Line
                {
                    X1 = points[i].X,
                    Y1 = points[i].Y,
                    X2 = points[i + 1].X,
                    Y2 = points[i + 1].Y,
                    Stroke = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF0000"),
                };
                canvasWeight.Children.Add(line);
            }
        }

        private void Nacrtaj_GrafPlaniranoStvarno(TrainingReportDTO report)
        {
            canvasPlanStvarno.Children.Clear();
            if (report.PlannedWeights.Count == 0) return;

            double canvasW = canvasPlanStvarno.Width > 0 ? canvasPlanStvarno.Width : 800;
            double canvasH = 220;
            double padL = 50, padR = 15, padT = 15, padB = 40;
            double plotW = canvasW - padL - padR;
            double plotH = canvasH - padT - padB;

            int count = report.PlannedWeights.Count;

            var differences = new List<double>();
            for (int i = 0; i < count; i++)
            {
                differences.Add(report.ActualWeights[i] - report.PlannedWeights[i]);
            }

            double maxPos = differences.Where(d => d > 0).DefaultIfEmpty(0).Max();
            double maxNeg = differences.Where(d => d < 0).DefaultIfEmpty(0).Min();
            double maxAbs = Math.Max(Math.Abs(maxPos), Math.Abs(maxNeg));
            if (maxAbs == 0) maxAbs = 10;
            maxAbs += 5;

            double zeroY = padT + plotH / 2;
            double halfH = plotH / 2;

            var zeroLine = new Line
            {
                X1 = padL,
                Y1 = zeroY,
                X2 = padL + plotW,
                Y2 = zeroY,
                Stroke = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFFFFD"),
            };
            canvasPlanStvarno.Children.Add(zeroLine);

            var zeroLabel = new TextBlock
            {
                Text = "0",
                FontSize = 9,
                Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFFFFD"),
                Width = 38,
                TextAlignment = TextAlignment.Right
            };
            Canvas.SetLeft(zeroLabel, 8);
            Canvas.SetTop(zeroLabel, zeroY - 6);
            canvasPlanStvarno.Children.Add(zeroLabel);

            var gridVals = new[] { maxAbs / 2, maxAbs };
            foreach (var gv in gridVals)
            {
                double yPos = zeroY - (gv / maxAbs) * halfH;
                canvasPlanStvarno.Children.Add(new Line { X1 = padL, Y1 = yPos, X2 = padL + plotW, Y2 = yPos, Stroke = (SolidColorBrush)new BrushConverter().ConvertFrom("#2A2A2A") });
                var lblPos = new TextBlock { Text = $"+{gv:0.0}", FontSize = 9, Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#7A7A7A"), Width = 38, TextAlignment = TextAlignment.Right };
                Canvas.SetLeft(lblPos, 8); Canvas.SetTop(lblPos, yPos - 6);
                canvasPlanStvarno.Children.Add(lblPos);

                double yNeg = zeroY + (gv / maxAbs) * halfH;
                canvasPlanStvarno.Children.Add(new Line { X1 = padL, Y1 = yNeg, X2 = padL + plotW, Y2 = yNeg, Stroke = (SolidColorBrush)new BrushConverter().ConvertFrom("#2A2A2A") });
                var lblNeg = new TextBlock { Text = $"-{gv:0.0}", FontSize = 9, Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#7A7A7A"), Width = 38, TextAlignment = TextAlignment.Right };
                Canvas.SetLeft(lblNeg, 8); Canvas.SetTop(lblNeg, yNeg - 6);
                canvasPlanStvarno.Children.Add(lblNeg);
            }

            double groupW = plotW / count;
            double barW = groupW * 0.55;

            for (int i = 0; i < count; i++)
            {
                double diff = differences[i];
                double x = padL + i * groupW + (groupW - barW) / 2;
                double barH = (Math.Abs(diff) / maxAbs) * halfH;

                if (diff >= 0)
                {
                    var rect = new Rectangle { Width = barW, Height = barH > 0 ? barH : 2, Fill = (SolidColorBrush)new BrushConverter().ConvertFrom("#44FF44") };
                    Canvas.SetLeft(rect, x); Canvas.SetTop(rect, zeroY - barH);
                    canvasPlanStvarno.Children.Add(rect);

                    var valLbl = new TextBlock { Text = $"+{diff:0.0} kg", FontSize = 9, Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#44FF44"), Width = barW, TextAlignment = TextAlignment.Center };
                    Canvas.SetLeft(valLbl, x); Canvas.SetTop(valLbl, zeroY - barH - 14);
                    canvasPlanStvarno.Children.Add(valLbl);
                }
                else
                {
                    var rect = new Rectangle { Width = barW, Height = barH > 0 ? barH : 2, Fill = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF0000") };
                    Canvas.SetLeft(rect, x); Canvas.SetTop(rect, zeroY);
                    canvasPlanStvarno.Children.Add(rect);

                    var valLbl = new TextBlock { Text = $"{diff:0.0} kg", FontSize = 9, Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF0000"), Width = barW, TextAlignment = TextAlignment.Center };
                    Canvas.SetLeft(valLbl, x); Canvas.SetTop(valLbl, zeroY + barH + 2);
                    canvasPlanStvarno.Children.Add(valLbl);
                }

                var xLabel = new TextBlock { Text = report.TrainingLabels[i], FontSize = 9, Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#7A7A7A"), Width = groupW, TextAlignment = TextAlignment.Center, TextWrapping = TextWrapping.Wrap };
                Canvas.SetLeft(xLabel, padL + i * groupW); Canvas.SetTop(xLabel, canvasH - padB + 5);
                canvasPlanStvarno.Children.Add(xLabel);
            }
        }
    }
}
