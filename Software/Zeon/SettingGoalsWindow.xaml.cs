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
    /// Interaction logic for SettingGoalsWindow.xaml
    /// </summary>
    public partial class SettingGoalsWindow : Window
    {
        private int client_id;
        private ClientService clientService;
        private Client_profile clientProfile;
        private GoalService goalService;
        private Goal existingGoal;
        public SettingGoalsWindow(int clientID)
        {
            InitializeComponent();
            client_id = clientID;
            clientService = new ClientService();
            goalService = new GoalService();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            clientProfile = await clientService.GetClientProfileAsync(client_id);
            LoadData(clientProfile);
        }

        private void Calculate()
        {
            try
            {
                decimal height = decimal.Parse(txtHeight.Text);
                decimal weight = decimal.Parse(txtWeight.Text);
                int age = IzracunajGodine(clientProfile);
                char gender = char.Parse(txtGender.Text);
                var selectedItem = cmbActivityLevel.SelectedItem as ComboBoxItem;
                string activityLevel = selectedItem.Content.ToString();
                var selectedItemCilj = cmbOdabirCilja.SelectedItem as ComboBoxItem;
                string cilj = selectedItemCilj.Content.ToString();

                decimal bmr;

                if (gender == 'M')
                {
                    bmr = (10 * weight) + (6.25m * height) - (5 * age) + 5;
                }
                else
                {
                    bmr = (10 * weight) + (6.25m * height) - (5 * age) - 161;
                }

                decimal tdeeCoefficient;

                switch (activityLevel)
                {
                    case "Sjedilački (bez vježbanja)":
                        tdeeCoefficient = 1.2m;
                        break;
                    case "Lagana aktivnost (1-3 dana/tjedan)":
                        tdeeCoefficient = 1.375m;
                        break;
                    case "Umjerena aktivnost (3-5 dana/tjedan)":
                        tdeeCoefficient = 1.55m;
                        break;
                    case "Intenzivna aktivnost (6-7 dana/tjedan)":
                        tdeeCoefficient = 1.725m;
                        break;
                    case "Ekstremna aktivnost (2x dnevno)":
                        tdeeCoefficient = 1.9m;
                        break;
                    default:
                        tdeeCoefficient = 1.2m;
                        break;
                }

                decimal tdee;
                switch (cilj)
                {
                    case "Gubiti težinu":
                        tdee = bmr * tdeeCoefficient * 0.85m;
                        break;
                    case "Dobiti na težini":
                        tdee = bmr * tdeeCoefficient * 1.15m;
                        break;
                    case "Graditi mišiće":
                        tdee = bmr * tdeeCoefficient;
                        break;
                    default:
                        tdee = bmr * tdeeCoefficient * 0.85m;
                        break;
                }

                decimal waterIntake = weight * 0.033m;

                txtBMR.Text = $"{Math.Round(bmr, 0)} kcal";
                txtTDEE.Text = $"{Math.Round(tdee, 0)} kcal";
                txtTargetCalories.Text = Math.Round(tdee, 0).ToString();
                txtGoalWater.Text = Math.Round(waterIntake, 0).ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void LoadData(Client_profile clientProfile)
        {
            txtAge.Text = IzracunajGodine(clientProfile).ToString();
            txtGender.Text = clientProfile.gender;
            txtHeight.Text = clientProfile.height.ToString();
            txtWeight.Text = clientProfile.weight.ToString();
        }

        private int IzracunajGodine(Client_profile osoba)
        {
            DateTime drodenja = (DateTime)osoba.birth_date;
            DateTime danas = DateTime.Today;
            int godine = danas.Year - drodenja.Year;
            if(drodenja.Date > danas.AddYears(-godine))
            {
                godine--;
            }
            return godine;
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(txtTargetCalories.Text, out int targetCal))
            {
                MessageBox.Show("Unesite ispravan kalorijski cilj!", "Greška");
                return;
            }
            if (!int.TryParse(txtGoalWater.Text, out int targetWater))
            {
                MessageBox.Show("Unesite ispravan cilj za unos vode!", "Greška");
                return;
            }
            Goal goal = new Goal
            {
                goal_name = txtGoalName.Text,
                goal_description = txtGoalDescription.Text,
                goal_cal = targetCal,
                goal_water = targetWater,
                ClientId = client_id
            };
            try
            {
                if (await goalService.SaveGoalAsync(goal))
                {
                    MessageBox.Show(
                        "Ciljevi za klijenta uspješno dodani!",
                        "Uspjeh",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnCalulate_Click(object sender, RoutedEventArgs e)
        {
            Calculate();
        }
    }
}
