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
    /// <summary>
    /// Interaction logic for AddEditFoodWindow.xaml
    /// </summary>
    public partial class AddEditFoodWindow : Window
    {
        private FoodService foodService;
        private int? foodId;
        private Food existingFood;

        public AddEditFoodWindow()
        {
            InitializeComponent();

            foodService = new FoodService();
            foodId = null;

            txtTitle.Text = "Dodaj hranu";
            btnSave.Content = "Spremi";
            btnDelete.Visibility = Visibility.Collapsed;
        }

        public AddEditFoodWindow(int id)
        {
            InitializeComponent();

            foodService = new FoodService();
            foodId = id;

            txtTitle.Text = "Uredi hranu";
            btnSave.Content = "Ažuriraj";
            btnDelete.Visibility = Visibility.Visible;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (foodId.HasValue)
            {
                await LoadFoodDataAsync(foodId.Value);
            }
        }

        private async Task LoadFoodDataAsync(int id)
        {
            try
            {
                existingFood = await Task.Run(() => foodService.GetFoodByIdAsync(id));

                if (existingFood == null)
                {
                    MessageBox.Show("Hrana nije pronađena!", "Greška",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Close();
                    return;
                }

                txtFoodName.Text = existingFood.name;
                txtCalories.Text = existingFood.kCal.ToString();
                txtProteins.Text = existingFood.proteins.ToString();
                txtCarbohydrates.Text = existingFood.carbohydrates.ToString();
                txtFat.Text = existingFood.fat.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri učitavanju: {ex.Message}", "Greška",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtFoodName.Text))
                {
                    MessageBox.Show("Unesite naziv hrane!", "Upozorenje",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtFoodName.Focus();
                    return;
                }

                if (!int.TryParse(txtCalories.Text, out int calories) || calories < 0)
                {
                    MessageBox.Show("Unesite ispravne kalorije!", "Upozorenje",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtCalories.Focus();
                    return;
                }

                if (!decimal.TryParse(txtProteins.Text, out decimal proteins) || proteins < 0)
                {
                    MessageBox.Show("Unesite ispravne proteine!", "Upozorenje",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtProteins.Focus();
                    return;
                }

                if (!decimal.TryParse(txtCarbohydrates.Text, out decimal carbs) || carbs < 0)
                {
                    MessageBox.Show("Unesite ispravne ugljikohidrate!", "Upozorenje",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtCarbohydrates.Focus();
                    return;
                }

                if (!decimal.TryParse(txtFat.Text, out decimal fat) || fat < 0)
                {
                    MessageBox.Show("Unesite ispravne masti!", "Upozorenje",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtFat.Focus();
                    return;
                }

                btnSave.IsEnabled = false;
                string actionText = foodId.HasValue ? "Ažuriram..." : "Spremam...";
                btnSave.Content = actionText;

                Food food = new Food
                {
                    id_food = existingFood?.id_food ?? 0,
                    name = txtFoodName.Text.Trim(),
                    kCal = calories,
                    proteins = proteins,
                    carbohydrates = carbs,
                    fat = fat
                };

                bool success;

                if (foodId.HasValue)
                {
                    success = await Task.Run(() => foodService.UpdateFoodAsync(food));
                }
                else
                {
                    success = await Task.Run(() => foodService.AddFoodAsync(food));
                }

                if (success)
                {
                    string successMessage = foodId.HasValue
                        ? "Hrana uspješno ažurirana!"
                        : "Hrana uspješno dodana!";

                    MessageBox.Show(
                        successMessage,
                        "Uspjeh",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    this.DialogResult = true;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška: {ex.Message}", "Greška",
                    MessageBoxButton.OK, MessageBoxImage.Error);

                btnSave.IsEnabled = true;
                btnSave.Content = foodId.HasValue ? "Ažuriraj" : "Spremi";
            }
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (!foodId.HasValue)
                return;

            var result = MessageBox.Show(
                $"Jeste li sigurni da želite obrisati '{txtFoodName.Text}'?\n\n" +
                "Ova radnja se ne može poništiti!",
                "Potvrda brisanja",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                btnDelete.IsEnabled = false;
                btnDelete.Content = "Brišem...";

                bool success = await Task.Run(() => foodService.DeleteFoodAsync(foodId.Value));

                if (success)
                {
                    MessageBox.Show(
                        "Hrana uspješno obrisana!",
                        "Uspjeh",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    this.DialogResult = true;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri brisanju: {ex.Message}", "Greška",
                    MessageBoxButton.OK, MessageBoxImage.Error);

                btnDelete.IsEnabled = true;
                btnDelete.Content = "Obriši";
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Jeste li sigurni da želite odustati?\n" +
                "Sve nespremljene promjene će biti izgubljene.",
                "Potvrda",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                this.DialogResult = false;
                this.Close();
            }
        }
    }
}
