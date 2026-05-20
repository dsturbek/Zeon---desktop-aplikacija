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
    public partial class FoodPlan : UserControl
    {
        private FoodPlanService service;
        private int trainerId;
        public FoodPlan(int trainerId)
        {
            InitializeComponent();
            this.trainerId = trainerId;
            service = new FoodPlanService();
            Load();
        }

        private async void Load()
        {
            try
            {
                var plans = await service.GetAllFoodPlansByTrainerId(trainerId);
                dgPlans.ItemsSource = plans;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri učitavanju planova: {ex.Message}", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            if (mainWindow != null)
            {
                var uc = new FoodPlanCreate(trainerId, null);
                uc.OnSaved += () => mainWindow.ShowUserControl(new FoodPlan(trainerId));
                mainWindow.ShowUserControl(uc);
            }

        }

        private void DgPlans_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            BtnEdit_Click(sender, e);
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
           
            if (dgPlans.SelectedItem == null)
            {
                MessageBox.Show("Odaberite plan koji želite urediti.", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selected = (Food_plan)dgPlans.SelectedItem;
            var mainWindow = Window.GetWindow(this) as MainWindow;

            if (mainWindow != null)
            {
                var uc = new FoodPlanCreate(trainerId, selected);

                uc.OnSaved += () => mainWindow.ShowUserControl(new FoodPlan(trainerId));

                mainWindow.ShowUserControl(uc);
            }
            Load();
            
        }

        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgPlans.SelectedItem == null)
            {
                MessageBox.Show("Odaberite plan koji želite izbrisati.", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show("Jeste li sigurni da želite izbrisati odabrani plan?",
                "Potvrda brisanja", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var selected = (Food_plan)dgPlans.SelectedItem;
                    bool success = await service.DeleteFoodPlan(selected.id_food_plan);

                    if (success)
                    {
                        MessageBox.Show("Plan uspješno izbrisan.", "Uspjeh", MessageBoxButton.OK, MessageBoxImage.Information);
                        Load();
                    }
                    else
                    {
                        MessageBox.Show("Brisanje plana nije uspjelo.", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Greška pri brisanju plana: {ex.Message}", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
