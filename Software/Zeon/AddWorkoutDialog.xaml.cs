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
using System.Windows.Shapes;

namespace Zeon
{
    public partial class AddWorkoutDialog : Window
    {
        private TemplateService templateService = new TemplateService();

        public bool IsExistingWorkout { get; private set; }
        public Workout SelectedWorkout { get; private set; }
        public string WorkoutName { get; private set; }
        public string MuscleGroup { get; private set; }
        public int SelectedDay { get; private set; }

        public AddWorkoutDialog()
        {
            InitializeComponent();
            this.Loaded += AddWorkoutDialog_Loaded;
        }

        private async void AddWorkoutDialog_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadWorkouts();
        }

        private async System.Threading.Tasks.Task LoadWorkouts()
        {
            try
            {
                var workouts = await System.Threading.Tasks.Task.Run(() => templateService.GetAllWorkouts());
                cmbExistingWorkouts.ItemsSource = workouts;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void rbMode_Checked(object sender, RoutedEventArgs e)
        {
            if (panelExisting == null || panelNew == null) return;

            if (rbExisting.IsChecked == true)
            {
                panelExisting.Visibility = Visibility.Visible;
                panelNew.Visibility = Visibility.Collapsed;
            }
            else
            {
                panelExisting.Visibility = Visibility.Collapsed;
                panelNew.Visibility = Visibility.Visible;
            }
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            if (rbExisting.IsChecked == true)
            {
                var selected = cmbExistingWorkouts.SelectedItem as Workout;
                if (selected == null)
                {
                    MessageBox.Show("Odaberite trening iz popisa!", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                IsExistingWorkout = true;
                SelectedWorkout = selected;
                WorkoutName = selected.workout_name;
                MuscleGroup = selected.muscle_group;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(txtWorkoutName.Text))
                {
                    MessageBox.Show("Naziv treninga je obavezan!", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var selectedMuscle = cmbMuscleGroup.SelectedItem as ComboBoxItem;
                if (selectedMuscle == null)
                {
                    MessageBox.Show("Odaberite mišićnu skupinu!", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                IsExistingWorkout = false;
                WorkoutName = txtWorkoutName.Text.Trim();
                MuscleGroup = selectedMuscle.Content.ToString();
            }

            if (!int.TryParse(txtDay.Text, out int day) || day < 1)
            {
                MessageBox.Show("Dan programa mora biti pozitivan broj!", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            SelectedDay = day;

            DialogResult = true;
        }

        private void btnDialogCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
