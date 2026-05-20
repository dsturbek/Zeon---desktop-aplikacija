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
    public partial class UcWorkoutTemplates : UserControl
    {
        private TemplateService templateService = new TemplateService();
        private int trainerId;

        public UcWorkoutTemplates(int trainerId)
        {
            InitializeComponent();
            this.trainerId = trainerId;
            InitializeReportsTab();
            this.Loaded += UcWorkoutTemplates_Loaded;
        }

        private void InitializeReportsTab()
        {
            reportsContent.Content = new ReportOverview(trainerId);
        }

        private async void UcWorkoutTemplates_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadTemplates();
        }

        private async Task LoadTemplates()
        {
            try
            {
                var templates = await Task.Run(() => templateService.GetTemplates(trainerId));

                var displayList = templates.Select(t => new
                {
                    t.id_workout_template,
                    t.workout_template_name,
                    WorkoutCount = t.Workout_Workout_plan_template.Count()
                }).ToList();

                dgTemplates.ItemsSource = displayList;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnNewTemplate_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            if (mainWindow != null)
            {
                var uc = new UcUpdateTemplate(trainerId, null);
                uc.OnSaved += () => mainWindow.ShowUserControl(new UcWorkoutTemplates(trainerId));
                mainWindow.ShowUserControl(uc);
            }
        }

        private void OpenTemplate(object dataContext)
        {
            if (dataContext == null) return;

            int templateId = (int)dataContext.GetType().GetProperty("id_workout_template").GetValue(dataContext);

            var mainWindow = Window.GetWindow(this) as MainWindow;
            if (mainWindow != null)
            {
                var uc = new UcUpdateTemplate(trainerId, templateId);
                uc.OnSaved += () => mainWindow.ShowUserControl(new UcWorkoutTemplates(trainerId));
                mainWindow.ShowUserControl(uc);
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            OpenTemplate(button?.DataContext);
        }

        private void TemplateName_Click(object sender, MouseButtonEventArgs e)
        {
            var textBlock = sender as TextBlock;
            OpenTemplate(textBlock?.DataContext);
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var item = button?.DataContext;
            if (item == null) return;

            int templateId = (int)item.GetType().GetProperty("id_workout_template").GetValue(item);
            string templateName = (string)item.GetType().GetProperty("workout_template_name").GetValue(item);

            var result = MessageBox.Show(
                $"Jeste li sigurni da želite obrisati predložak \"{templateName}\"?",
                "Potvrda brisanja",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var template = new Workout_plan_template { id_workout_template = templateId };
                    await Task.Run(() => templateService.RemoveTemplate(template));
                    await LoadTemplates();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
