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
    public partial class UcUpdateTemplate : UserControl
    {
        private TemplateService templateService = new TemplateService();
        private ExerciseService exerciseService = new ExerciseService();
        private int trainerId;
        private int? templateId;
        private List<Exercise> allExercises;
        private List<WorkoutItem> workoutItems = new List<WorkoutItem>();

        public event Action OnSaved;

        public UcUpdateTemplate(int trainerId, int? templateId)
        {
            InitializeComponent();
            this.trainerId = trainerId;
            this.templateId = templateId;
            this.Loaded += UcUpdateTemplate_Loaded;
        }

        private async void UcUpdateTemplate_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadAllExercises();

            if (templateId.HasValue)
            {
                txtTitle.Text = "Uredi predložak";
                await LoadExistingTemplate();
            }

            UpdateEmptyStates();
        }

        private async Task LoadAllExercises()
        {
            try
            {
                allExercises = await exerciseService.GetAllExercisesAsync();
                dgAllExercises.ItemsSource = allExercises;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadExistingTemplate()
        {
            try
            {
                var templates = await Task.Run(() => templateService.GetTemplates(trainerId));
                var template = templates.FirstOrDefault(t => t.id_workout_template == templateId.Value);
                if (template == null) return;

                txtTemplateName.Text = template.workout_template_name;

                workoutItems.Clear();
                var ordered = template.Workout_Workout_plan_template
                    .OrderBy(ww => ww.workout_plan_day);

                foreach (var ww in ordered)
                {
                    workoutItems.Add(new WorkoutItem
                    {
                        WorkoutId = ww.WorkoutId,
                        Day = ww.workout_plan_day,
                        WorkoutName = ww.Workout?.workout_name ?? "Trening",
                        DisplayName = $"Dan {ww.workout_plan_day} - {ww.Workout?.workout_name ?? "Trening"}"
                    });
                }

                RefreshWorkoutDgv();
                if (dgWorkouts.Items.Count > 0)
                    dgWorkouts.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshWorkoutDgv()
        {
            dgWorkouts.ItemsSource = null;
            dgWorkouts.ItemsSource = workoutItems;
            UpdateEmptyStates();
        }

        private void UpdateEmptyStates()
        {
            txtNoWorkouts.Visibility = workoutItems.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            dgWorkouts.Visibility = workoutItems.Count == 0 ? Visibility.Collapsed : Visibility.Visible;

            var exercises = dgvExercises.ItemsSource as List<ExerciseDisplayItem>;
            bool hasExercises = exercises != null && exercises.Count > 0;
            txtNoExercises.Visibility = hasExercises ? Visibility.Collapsed : Visibility.Visible;
        }

        private async void dgWorkouts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = dgWorkouts.SelectedItem as WorkoutItem;
            if (selected == null)
            {
                dgvExercises.ItemsSource = null;
                UpdateEmptyStates();
                return;
            }

            await LoadExercisesForWorkout(selected.WorkoutId);
        }

        private async Task LoadExercisesForWorkout(int workoutId)
        {
            try
            {
                var exercises = await Task.Run(() => templateService.GetExercisesForWorkout(workoutId));

                var displayList = exercises.Select(ew => new ExerciseDisplayItem
                {
                    ExerciseId = ew.ExerciseId,
                    WorkoutId = ew.WorkoutId,
                    ExerciseName = ew.Exercise?.exercise_name ?? "",
                    Muscle = ew.Exercise?.muscle ?? "",
                    Sets = ew.sets?.ToString() ?? "-",
                    Reps = ew.reps?.ToString() ?? "-",
                    Weight = ew.weight?.ToString("0.#") ?? "-"
                }).ToList();

                dgvExercises.ItemsSource = displayList;
                UpdateEmptyStates();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (templateId.HasValue)
                {
                    var template = new Workout_plan_template
                    {
                        id_workout_template = templateId.Value,
                        workout_template_name = txtTemplateName.Text
                    };
                    await Task.Run(() => templateService.UpdateTemplate(template));
                    MessageBox.Show("Predložak uspješno ažuriran!", "Uspjeh", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    var template = new Workout_plan_template
                    {
                        workout_template_name = txtTemplateName.Text,
                        TrainerId = trainerId
                    };
                    bool created = await Task.Run(() => templateService.CreateTemplate(template));
                    if (created)
                    {
                        templateId = template.id_workout_template;
                        txtTitle.Text = "Uredi predložak";
                        MessageBox.Show("Predložak uspješno kreiran! Sada možete dodavati treninge i vježbe.", "Uspjeh", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }

                OnSaved?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.ShowUserControl(new UcWorkoutTemplates(trainerId));
            }
        }

        private async Task<bool> EnsureTemplateSaved()
        {
            if (templateId.HasValue) return true;

            string name = txtTemplateName.Text?.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Unesite naziv predloška prije dodavanja treninga.", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            try
            {
                var template = new Workout_plan_template
                {
                    workout_template_name = name,
                    TrainerId = trainerId
                };
                bool created = await Task.Run(() => templateService.CreateTemplate(template));
                if (created)
                {
                    templateId = template.id_workout_template;
                    txtTitle.Text = "Uredi predložak";
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return false;
        }

        private async void btnAddWorkout_Click(object sender, RoutedEventArgs e)
        {
            if (!await EnsureTemplateSaved()) return;

            var dialog = new AddWorkoutDialog();
            dialog.Owner = Window.GetWindow(this);
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    Workout workout;

                    if (dialog.IsExistingWorkout)
                    {
                        workout = dialog.SelectedWorkout;
                    }
                    else
                    {
                        var newWorkout = new Workout
                        {
                            workout_name = dialog.WorkoutName,
                            muscle_group = dialog.MuscleGroup
                        };
                        workout = await Task.Run(() => templateService.CreateWorkout(newWorkout));
                    }

                    int day = dialog.SelectedDay > 0 ? dialog.SelectedDay : workoutItems.Count + 1;
                    await Task.Run(() => templateService.AddWorkoutToTemplate(workout.id_workout, templateId.Value, day));

                    workoutItems.Add(new WorkoutItem
                    {
                        WorkoutId = workout.id_workout,
                        Day = day,
                        WorkoutName = workout.workout_name,
                        DisplayName = $"Dan {day} - {workout.workout_name}"
                    });

                    RefreshWorkoutDgv();
                    dgWorkouts.SelectedIndex = workoutItems.Count - 1;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void btnRemoveWorkoutRow_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var selected = button?.DataContext as WorkoutItem;
            if (selected == null || !templateId.HasValue) return;

            var result = MessageBox.Show(
                $"Jeste li sigurni da želite ukloniti \"{selected.DisplayName}\"?",
                "Potvrda", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await Task.Run(() => templateService.RemoveWorkoutFromTemplate(selected.WorkoutId, templateId.Value));
                    workoutItems.Remove(selected);
                    RefreshWorkoutDgv();
                    dgvExercises.ItemsSource = null;
                    UpdateEmptyStates();
                    if (dgWorkouts.Items.Count > 0)
                        dgWorkouts.SelectedIndex = 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnAddExercise_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgWorkouts.SelectedItem as WorkoutItem;
            if (selected == null)
            {
                MessageBox.Show("Prvo odaberite trening iz popisa.", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            panelExercisePicker.Visibility = Visibility.Visible;
            panelExercisePicker.BringIntoView();
        }

        private void btnCloseExercisePicker_Click(object sender, RoutedEventArgs e)
        {
            panelExercisePicker.Visibility = Visibility.Collapsed;
        }

        private void txtSearchExercise_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (allExercises == null) return;
            string search = txtSearchExercise.Text.ToLower().Trim();
            if (string.IsNullOrEmpty(search))
            {
                dgAllExercises.ItemsSource = allExercises;
            }
            else
            {
                dgAllExercises.ItemsSource = allExercises
                    .Where(ex => (ex.exercise_name ?? "").ToLower().Contains(search)
                              || (ex.muscle ?? "").ToLower().Contains(search))
                    .ToList();
            }
        }

        private async void btnPickExercise_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var exercise = button?.DataContext as Exercise;
            var selectedWorkout = dgWorkouts.SelectedItem as WorkoutItem;

            if (exercise == null || selectedWorkout == null) return;

            try
            {
                int.TryParse(txtSets.Text, out int sets);
                int.TryParse(txtReps.Text, out int reps);
                decimal.TryParse(txtWeight.Text, out decimal weight);

                await Task.Run(() => templateService.AddExerciseToWorkout(
                    exercise.id_exercise,
                    selectedWorkout.WorkoutId,
                    sets > 0 ? sets : (int?)null,
                    reps > 0 ? reps : (int?)null,
                    weight >= 0 ? weight : (decimal?)null));

                await LoadExercisesForWorkout(selectedWorkout.WorkoutId);
                panelExercisePicker.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnRemoveExercise_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var item = button?.DataContext as ExerciseDisplayItem;
            if (item == null) return;

            try
            {
                await Task.Run(() => templateService.RemoveExerciseFromWorkout(item.ExerciseId, item.WorkoutId));
                await LoadExercisesForWorkout(item.WorkoutId);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void dgvExercises_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Cancel) return;

            var item = e.Row.Item as ExerciseDisplayItem;
            if (item == null) return;

            var textBox = e.EditingElement as TextBox;
            if (textBox == null) return;

            string newValue = textBox.Text;
            string columnHeader = (e.Column as DataGridTextColumn)?.Header?.ToString() ?? "";

            int.TryParse(item.Sets, out int sets);
            int.TryParse(item.Reps, out int reps);
            decimal.TryParse(item.Weight, out decimal weight);

            if (columnHeader == "SERIJE") int.TryParse(newValue, out sets);
            else if (columnHeader == "PONAVLJANJA") int.TryParse(newValue, out reps);
            else if (columnHeader == "TEŽINA (kg)") decimal.TryParse(newValue, out weight);

            try
            {
                await exerciseService.UpdateExerciseWorkoutAsync(
                    item.ExerciseId,
                    item.WorkoutId,
                    sets > 0 ? sets : (int?)null,
                    reps > 0 ? reps : (int?)null,
                    weight >= 0 ? weight : (decimal?)null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void dgvExercises_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
    }

    public class WorkoutItem
    {
        public int WorkoutId { get; set; }
        public int Day { get; set; }
        public string WorkoutName { get; set; }
        public string DisplayName { get; set; }
    }

    public class ExerciseDisplayItem
    {
        public int ExerciseId { get; set; }
        public int WorkoutId { get; set; }
        public string ExerciseName { get; set; }
        public string Muscle { get; set; }
        public string Sets { get; set; }
        public string Reps { get; set; }
        public string Weight { get; set; }
    }
}
