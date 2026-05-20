using BussinessLogicLayer;
using EntitiesLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
using System.Xml.Linq;

namespace Zeon
{

    public partial class FoodPlanCreate : UserControl
    {
        private FoodPlanService service;
        private int trainerId;
        private Food_plan currentPlan;
        private List<Planned_meal> meals;

        public event Action OnSaved;

        public FoodPlanCreate(int trainerId, Food_plan plan)
        {
            InitializeComponent();
            this.trainerId = trainerId;
            this.currentPlan = plan;
            service = new FoodPlanService();
            meals = new List<Planned_meal>();
            Load();
        }

        private async void Load()
        {
            try
            {
                var clients = await service.GetAllClientsForTrainer(trainerId);
                cmbClient.ItemsSource = clients;
                cmbClient.SelectedValuePath = "id_client";
                cmbClient.DisplayMemberPath = "name_surname";

                if (currentPlan != null)
                {
                    txtTitle.Text = "Uređivanje plana prehrane";
                    txtName.Text = currentPlan.food_plan_name;
                    txtDescription.Text = currentPlan.food_plan_description;
                    dtpStart.SelectedDate = currentPlan.date_start;
                    dtpEnd.SelectedDate = currentPlan.date_end;
                    cmbClient.SelectedValue = currentPlan.ClientId;

                    await RefreshMealsGrid();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri učitavanju podataka: {ex.Message}", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task RefreshMealsGrid()
        {
            if (currentPlan != null && currentPlan.id_food_plan != 0)
            {
                meals = await service.GetMealsForPlan(currentPlan.id_food_plan);
                dgMeals.ItemsSource = null;
                dgMeals.ItemsSource = meals;
            }
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Naziv plana je obavezan.", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (dtpStart.SelectedDate == null || dtpEnd.SelectedDate == null)
            {
                MessageBox.Show("Odaberite datume početka i kraja plana.", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (dtpStart.SelectedDate > dtpEnd.SelectedDate)
            {
                MessageBox.Show("Datum početka mora biti prije datuma kraja.", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (cmbClient.SelectedItem == null)
            {
                MessageBox.Show("Odaberite klijenta.", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var plan = currentPlan ?? new Food_plan();
                plan.food_plan_name = txtName.Text;
                plan.food_plan_description = txtDescription.Text;
                plan.date_start = dtpStart.SelectedDate ?? DateTime.Now;
                plan.date_end = dtpEnd.SelectedDate ?? DateTime.Now;
                plan.ClientId = ((Client)cmbClient.SelectedItem).id_client;

                bool success = await service.SavePlan(plan);
                if (success)
                {
                    currentPlan = plan;
                    MessageBox.Show("Plan uspješno spremljen.");
                    OnSaved?.Invoke(); 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri spremanju: {ex.Message}");
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            OnSaved?.Invoke();
        }

        private async void BtnRemoveMeal_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var mealToRemove = button?.DataContext as Planned_meal;

            if (mealToRemove != null)
            {
                var result = MessageBox.Show($"Jeste li sigurni da želite obrisati obrok '{mealToRemove.meal_type}'?",
                    "Potvrda brisanja", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        bool deleted = await service.DeleteMeal(mealToRemove.id_planned_meal);
                        if (deleted)
                        {
                            await RefreshMealsGrid(); 
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Greška pri brisanju obroka: {ex.Message}");
                    }
                }
            }
        }

        private void BtnAddMeal_Click(object sender, RoutedEventArgs e)
        {
            if (currentPlan == null || currentPlan.id_food_plan == 0)
            {
                MessageBox.Show("Prvo spremite plan prehrane prije dodavanja obroka.", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var mainWindow = Window.GetWindow(this) as MainWindow;
            if (mainWindow != null)
            {
                var uc = new AddMeal(currentPlan.id_food_plan);
                uc.OnSaved += async () =>
                {
                    await RefreshMealsGrid();
                    mainWindow.ShowUserControl(this);
                };
                mainWindow.ShowUserControl(uc);
            }
        }

        private void dgMeals_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgMeals.SelectedItem is Planned_meal selectedMeal)
            {
                var mainWindow = Window.GetWindow(this) as MainWindow;
                if (mainWindow != null)
                {
                    var uc = new AddMeal(currentPlan.id_food_plan, selectedMeal);

                    uc.OnSaved += async () =>
                    {
                        await RefreshMealsGrid();
                        mainWindow.ShowUserControl(this);
                    };

                    mainWindow.ShowUserControl(uc);
                }
            }
        }

    }
}
