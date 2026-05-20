using BussinessLogicLayer;
using EntitiesLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Zeon;

namespace PresentationLayer.Windows
{
    public partial class ClientDetailsWindow : UserControl
    {
        private readonly Client _client;
        private readonly int _trainerId;
        private readonly ContentControl _contentPanel;
        private readonly GoalService _goalService;
        private readonly WorkoutPlanService _workoutPlanService;
        private Goal _currentGoal;
        private Workout_plan_assigned _currentPlan;

        public ClientDetailsWindow(Client client, int trainerId, ContentControl contentPanel)
        {
            InitializeComponent();
            _client = client;
            _trainerId = trainerId;
            _contentPanel = contentPanel;
            _goalService = new GoalService();
            _workoutPlanService = new WorkoutPlanService();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            txtClientName.Text = _client.name_surname;
            txtClientEmail.Text = _client.email;

            await LoadGoalAsync();
            await LoadWorkoutPlanAsync();
        }

        private async System.Threading.Tasks.Task LoadGoalAsync()
        {
            try
            {
                _currentGoal = await _goalService.GetGoalByIdAsync(_client.id_client);

                if (_currentGoal != null)
                {
                    txtGoalName.Text = _currentGoal.goal_name;
                    txtGoalDescription.Text = _currentGoal.goal_description;
                    txtGoalCalories.Text = $"{_currentGoal.goal_cal} kcal";
                    txtGoalWater.Text = $"{_currentGoal.goal_water} L";

                    pnlGoalExists.Visibility = Visibility.Visible;
                    pnlNoGoal.Visibility = Visibility.Collapsed;
                }
                else
                {
                    pnlGoalExists.Visibility = Visibility.Collapsed;
                    pnlNoGoal.Visibility = Visibility.Visible;
                }
            }
            catch (Exception)
            {
                pnlGoalExists.Visibility = Visibility.Collapsed;
                pnlNoGoal.Visibility = Visibility.Visible;
            }
        }

        private async System.Threading.Tasks.Task LoadWorkoutPlanAsync()
        {
            try
            {
                _currentPlan = await _workoutPlanService.GetWorkoutPlanForClientAsync(_client.id_client);

                if (_currentPlan != null)
                {
                    txtPlanName.Text = _currentPlan.workout_assigned_name;

                    var startDate = _currentPlan.date_start?.ToString("dd.MM.yyyy") ?? "-";
                    var endDate = _currentPlan.date_end?.ToString("dd.MM.yyyy") ?? "-";
                    txtPlanDates.Text = $"{startDate} - {endDate}";

                    var workoutItems = _currentPlan.Workout_plan_assigned_Workout
                        .OrderBy(w => w.workout_plan_day)
                        .Select(w => new WorkoutDisplayItem
                        {
                            DayLabel = $"Dan {w.workout_plan_day}",
                            WorkoutName = w.Workout?.workout_name ?? "-",
                            MuscleGroup = w.Workout?.muscle_group ?? "",
                            ExercisesSummary = GetExercisesSummary(w.Workout)
                        })
                        .ToList();

                    icWorkouts.ItemsSource = workoutItems;

                    pnlPlanExists.Visibility = Visibility.Visible;
                    pnlNoPlan.Visibility = Visibility.Collapsed;
                }
                else
                {
                    pnlPlanExists.Visibility = Visibility.Collapsed;
                    pnlNoPlan.Visibility = Visibility.Visible;
                }
            }
            catch (Exception)
            {
                pnlPlanExists.Visibility = Visibility.Collapsed;
                pnlNoPlan.Visibility = Visibility.Visible;
            }
        }

        private string GetExercisesSummary(Workout workout)
        {
            if (workout?.Exercise_Workout == null || !workout.Exercise_Workout.Any())
                return "Nema vježbi";

            var exercises = workout.Exercise_Workout
                .Select(ew => ew.Exercise?.exercise_name ?? "")
                .Where(name => !string.IsNullOrEmpty(name))
                .Take(3)
                .ToList();

            var summary = string.Join(", ", exercises);
            if (workout.Exercise_Workout.Count > 3)
                summary += $" (+{workout.Exercise_Workout.Count - 3})";

            return summary;
        }

        private async void btnSetGoal_Click(object sender, RoutedEventArgs e)
        {
            var settingGoalsWindow = new SettingGoalsWindow(_client.id_client);
            settingGoalsWindow.Owner = Window.GetWindow(this);
            settingGoalsWindow.ShowDialog();

            await LoadGoalAsync();
        }

        private async void btnImportTemplate_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ImportTemplateDialog(_trainerId);
            dialog.Owner = Window.GetWindow(this);
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    await _workoutPlanService.ImportFromTemplateAsync(
                        dialog.SelectedTemplate.id_workout_template,
                        _client.id_client,
                        dialog.PlanName,
                        dialog.StartDate,
                        dialog.EndDate);

                    MessageBox.Show("Plan uspješno uvezen iz predloška!", "Uspjeh", MessageBoxButton.OK, MessageBoxImage.Information);
                    await LoadWorkoutPlanAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnManagePlan_Click(object sender, RoutedEventArgs e)
        {
            if (_contentPanel != null)
            {
                _contentPanel.Content = new WorkoutPlanManagerWindow(_client, _currentPlan, _trainerId, _contentPanel);
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if (_contentPanel != null)
            {
                _contentPanel.Content = new FrmClients(_trainerId, _contentPanel);
            }
        }
    }

    public class WorkoutDisplayItem
    {
        public string DayLabel { get; set; }
        public string WorkoutName { get; set; }
        public string MuscleGroup { get; set; }
        public string ExercisesSummary { get; set; }
    }
}
