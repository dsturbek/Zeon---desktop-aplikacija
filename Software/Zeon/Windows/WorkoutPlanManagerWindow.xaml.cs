using BussinessLogicLayer;
using EntitiesLayer.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PresentationLayer.Windows
{
    public partial class WorkoutPlanManagerWindow : UserControl
    {
        private readonly Client _client;
        private Workout_plan_assigned _plan;
        private readonly int _trainerId;
        private readonly ContentControl _contentPanel;
        private readonly WorkoutPlanService _workoutPlanService;
        private readonly WorkoutService _workoutService;
        private readonly ExerciseService _exerciseService;

        private List<Workout> _allWorkouts;
        private List<Exercise> _allExercises;
        private ObservableCollection<PlanWorkoutItem> _planWorkouts;
        private PlanWorkoutItem _selectedWorkoutItem;

        public WorkoutPlanManagerWindow(Client client, Workout_plan_assigned existingPlan, int trainerId, ContentControl contentPanel)
        {
            InitializeComponent();
            _client = client;
            _plan = existingPlan;
            _trainerId = trainerId;
            _contentPanel = contentPanel;
            _workoutPlanService = new WorkoutPlanService();
            _workoutService = new WorkoutService();
            _exerciseService = new ExerciseService();
            _planWorkouts = new ObservableCollection<PlanWorkoutItem>();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _allWorkouts = await _workoutService.GetAllWorkoutsAsync();
            _allExercises = await _exerciseService.GetAllExercisesAsync();

            cmbWorkouts.ItemsSource = _allWorkouts;
            cmbExercises.ItemsSource = _allExercises;

            if (_plan != null)
            {
                txtPlanName.Text = _plan.workout_assigned_name;
                dpStartDate.SelectedDate = _plan.date_start;
                dpEndDate.SelectedDate = _plan.date_end;
                btnDeletePlan.Visibility = Visibility.Visible;

                foreach (var pw in _plan.Workout_plan_assigned_Workout.OrderBy(w => w.workout_plan_day))
                {
                    _planWorkouts.Add(new PlanWorkoutItem
                    {
                        Day = pw.workout_plan_day,
                        DayLabel = $"Dan {pw.workout_plan_day}",
                        WorkoutId = pw.WorkoutId,
                        WorkoutName = pw.Workout?.workout_name ?? "-",
                        Workout = pw.Workout
                    });
                }
            }
            else
            {
                dpStartDate.SelectedDate = DateTime.Today;
                btnDeletePlan.Visibility = Visibility.Collapsed;
            }

            lbWorkouts.ItemsSource = _planWorkouts;
        }

        private async void btnAddWorkout_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(txtDay.Text, out int day) || day <= 0)
            {
                MessageBox.Show("Unesite ispravan dan!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedWorkout = cmbWorkouts.SelectedItem as Workout;
            if (selectedWorkout == null)
            {
                MessageBox.Show("Odaberite trening!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_plan != null)
            {
                try
                {
                    await _workoutPlanService.AddWorkoutToPlanAsync(_plan.id_workout_assigned, selectedWorkout.id_workout, day);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            _planWorkouts.Add(new PlanWorkoutItem
            {
                Day = day,
                DayLabel = $"Dan {day}",
                WorkoutId = selectedWorkout.id_workout,
                WorkoutName = selectedWorkout.workout_name,
                Workout = selectedWorkout
            });

            var sorted = _planWorkouts.OrderBy(w => w.Day).ToList();
            _planWorkouts.Clear();
            foreach (var item in sorted)
                _planWorkouts.Add(item);
        }

        private async void btnRemoveWorkout_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var item = button?.Tag as PlanWorkoutItem;
            if (item == null) return;

            if (_plan != null)
            {
                try
                {
                    await _workoutPlanService.RemoveWorkoutFromPlanAsync(_plan.id_workout_assigned, item.WorkoutId, item.Day);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            _planWorkouts.Remove(item);
        }

        private async void lbWorkouts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedWorkoutItem = lbWorkouts.SelectedItem as PlanWorkoutItem;
            if (_selectedWorkoutItem == null)
            {
                txtSelectedWorkout.Text = "";
                dgExercises.ItemsSource = null;
                return;
            }

            txtSelectedWorkout.Text = $"- {_selectedWorkoutItem.WorkoutName}";

            var exercises = await _exerciseService.GetExercisesForWorkoutAsync(_selectedWorkoutItem.WorkoutId);
            var exerciseItems = exercises.Select(ew => new ExerciseWorkoutItem
            {
                ExerciseId = ew.ExerciseId,
                WorkoutId = ew.WorkoutId,
                ExerciseName = ew.Exercise?.exercise_name ?? "-",
                Sets = ew.sets,
                Reps = ew.reps,
                Weight = ew.weight
            }).ToList();

            dgExercises.ItemsSource = exerciseItems;
        }

        private async void btnAddExercise_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedWorkoutItem == null)
            {
                MessageBox.Show("Prvo odaberite trening s lijeve strane!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedExercise = cmbExercises.SelectedItem as Exercise;
            if (selectedExercise == null)
            {
                MessageBox.Show("Odaberite vježbu!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                await _exerciseService.AddExerciseToWorkoutAsync(
                    selectedExercise.id_exercise,
                    _selectedWorkoutItem.WorkoutId,
                    3, 10, null);

                lbWorkouts_SelectionChanged(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void dgExercises_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Cancel) return;

            var item = e.Row.Item as ExerciseWorkoutItem;
            if (item == null) return;

            var textBox = e.EditingElement as TextBox;
            if (textBox == null) return;

            var columnHeader = e.Column.Header.ToString();
            var newValue = textBox.Text.Trim();

            if (columnHeader == "Serije" || columnHeader == "Ponavljanja")
            {
                if (!string.IsNullOrEmpty(newValue))
                {
                    if (!int.TryParse(newValue, out int intValue) || intValue < 0)
                    {
                        MessageBox.Show($"{columnHeader} moraju biti pozitivan cijeli broj!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                        e.Cancel = true;
                        return;
                    }
                }
            }
            else if (columnHeader == "Tezina (kg)")
            {
                if (!string.IsNullOrEmpty(newValue))
                {
                    if (!decimal.TryParse(newValue, out decimal decValue) || decValue < 0)
                    {
                        MessageBox.Show("Težina mora biti pozitivan broj!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                        e.Cancel = true;
                        return;
                    }
                }
            }

            await System.Threading.Tasks.Task.Delay(100);

            try
            {
                await _exerciseService.UpdateExerciseWorkoutAsync(
                    item.ExerciseId,
                    item.WorkoutId,
                    item.Sets,
                    item.Reps,
                    item.Weight);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPlanName.Text))
            {
                MessageBox.Show("Unesite naziv plana!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (dpStartDate.SelectedDate == null)
            {
                MessageBox.Show("Odaberite datum početka!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (_plan == null)
                {
                    var newPlan = new Workout_plan_assigned
                    {
                        workout_assigned_name = txtPlanName.Text,
                        date_start = dpStartDate.SelectedDate,
                        date_end = dpEndDate.SelectedDate,
                        ClientId = _client.id_client
                    };

                    int planId = await _workoutPlanService.CreateWorkoutPlanAsync(newPlan);

                    foreach (var item in _planWorkouts)
                    {
                        await _workoutPlanService.AddWorkoutToPlanAsync(planId, item.WorkoutId, item.Day);
                    }

                    MessageBox.Show("Plan treninga uspješno kreiran!", "Uspjeh", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    _plan.workout_assigned_name = txtPlanName.Text;
                    _plan.date_start = dpStartDate.SelectedDate;
                    _plan.date_end = dpEndDate.SelectedDate;

                    await _workoutPlanService.UpdateWorkoutPlanAsync(_plan);
                    MessageBox.Show("Plan treninga uspješno ažuriran!", "Uspjeh", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                NavigateToClientDetails();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnDeletePlan_Click(object sender, RoutedEventArgs e)
        {
            if (_plan == null) return;

            var result = MessageBox.Show("Jeste li sigurni da želite obrisati ovaj plan treninga?",
                "Potvrda brisanja", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                await _workoutPlanService.DeleteWorkoutPlanAsync(_plan.id_workout_assigned);
                MessageBox.Show("Plan treninga uspješno obrisan!", "Uspjeh", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigateToClientDetails();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigateToClientDetails();
        }

        private void NavigateToClientDetails()
        {
            if (_contentPanel != null)
            {
                _contentPanel.Content = new ClientDetailsWindow(_client, _trainerId, _contentPanel);
            }
        }
    }

    public class PlanWorkoutItem
    {
        public int Day { get; set; }
        public string DayLabel { get; set; }
        public int WorkoutId { get; set; }
        public string WorkoutName { get; set; }
        public Workout Workout { get; set; }
    }

    public class ExerciseWorkoutItem
    {
        public int ExerciseId { get; set; }
        public int WorkoutId { get; set; }
        public string ExerciseName { get; set; }
        public int? Sets { get; set; }
        public int? Reps { get; set; }
        public decimal? Weight { get; set; }
    }
}
