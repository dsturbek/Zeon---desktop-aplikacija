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
    /// <summary>
    /// Interaction logic for FeedbackOverview.xaml
    /// </summary>
    public partial class FeedbackOverview : UserControl
    {

        private FeedbackService feedbackService;
        private ClientService clientService;
        private List<Feedback_training> allTrainingFeedbacks;
        private List<Feedback_meal> allMealFeedbacks;
        private int _trainerId;

        public FeedbackOverview(int trainerId)
        {
            InitializeComponent();
            _trainerId=trainerId;
            feedbackService = new FeedbackService();
            clientService = new ClientService();
            LoadDataAsync();

        }

        private async void LoadDataAsync()
        {
            try
            {
                allTrainingFeedbacks = await feedbackService.GetAllTrainingFeedbacksAsync(_trainerId);
                allMealFeedbacks = await feedbackService.GetAllMealFeedbacksAsync(_trainerId);
                var clients = await clientService.GetClientsByTrainerAsync(_trainerId);
                cmbClients.ItemsSource = clients;
                PrikaziKartice();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška: {ex.Message}", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PrikaziKartice()
        {
            stackKartice.Children.Clear();

            bool pokazujiTraining = rbAll.IsChecked == true || rbTraining.IsChecked == true;
            bool pokazujiMeal = rbAll.IsChecked == true || rbMeal.IsChecked == true;

            int? selectedClientId = null;
            if (cmbClients.SelectedItem is Client selected)
                selectedClientId = selected.id_client;

            if (pokazujiTraining && allTrainingFeedbacks != null)
            {
                var filtered = allTrainingFeedbacks;
                if (selectedClientId.HasValue)
                    filtered = filtered.Where(f => f.ClientId == selectedClientId.Value).ToList();

                foreach (var fb in filtered)
                    stackKartice.Children.Add(KreiraTrainingKartica(fb));
            }

            if (pokazujiMeal && allMealFeedbacks != null)
            {
                var filtered = allMealFeedbacks;
                if (selectedClientId.HasValue)
                    filtered = filtered.Where(f => f.Planned_meal?.Food_plan?.ClientId == selectedClientId.Value).ToList();

                foreach (var fb in filtered)
                    stackKartice.Children.Add(KreiraMealKartica(fb));
            }
        }

        private Border KreiraMealKartica(Feedback_meal fb)
        {
            var stack = new StackPanel();

            stack.Children.Add(CreateText(
            $"{fb.Planned_meal?.Food_plan?.Client?.name_surname ?? "Nepoznat"} - {fb.Planned_meal?.meal_type}",
            15, "#FFFFFD", true, new Thickness(0, 0, 0, 8)));

            stack.Children.Add(CreateText(
            $"\"{fb.comment}\"",
            14, "#FFFFFD", false, new Thickness(0, 0, 0, 8)));

            stack.Children.Add(CreateTag());

            return CreateCard(stack);

        }

        private Border KreiraTrainingKartica(Feedback_training fb)
        {
            var stack=new StackPanel();

            stack.Children.Add(CreateText(
            $"{fb.Client?.name_surname ?? "Nepoznat"} - {fb.Exercise_Workout?.Workout?.workout_name}",
            15, "#FFFFFD", true, new Thickness(0, 0, 0, 4)));

            stack.Children.Add(CreateText(
            fb.Exercise_Workout?.Exercise?.exercise_name ?? "",
            13, "#FF0000", false, new Thickness(0, 0, 0, 8)));

            stack.Children.Add(CreateText(
            $"\"{fb.feedback_message}\"",
            14, "#FFFFFD", false, new Thickness(0, 0, 0, 8)));

            stack.Children.Add(CreateTag());

            return CreateCard(stack);

        }

        private TextBlock CreateText(string text,int size, string color, bool bold=false, Thickness? margin = null)
        {
            return new TextBlock
            {
                Text = text,
                FontSize = size,
                FontWeight = bold ? FontWeights.Bold : FontWeights.Normal,
                Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom(color),
                TextWrapping = TextWrapping.Wrap,
                Margin = margin ?? new Thickness(0)
            };
        }

        private Border CreateTag()
        {
            return new Border
            {
                Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF0000"),
                CornerRadius = new CornerRadius(3),
                Padding = new Thickness(8, 3, 8, 3),
                Margin = new Thickness(0, 10, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left,
            };
        }

        private Border CreateCard(StackPanel content)
        {
            return new Border
            {
                Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#1A1A1A"),
                BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#3A3A3A"),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(20),
                Margin = new Thickness(0, 0, 0, 12),
                Child = content
            };
        }

        private void FilterChanged(object sender, RoutedEventArgs e)
        {
            if (stackKartice == null) return;
            PrikaziKartice();
        }

        private void CmbClients_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PrikaziKartice();
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            rbAll.IsChecked = true;
            cmbClients.SelectedItem = null;
            PrikaziKartice();
        }
    }
}
