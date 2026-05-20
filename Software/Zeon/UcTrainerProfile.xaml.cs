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
    public partial class UcTrainerProfile : UserControl
    {
        private ProfileService profileService = new ProfileService();
        private int trainerId;
        private Trainer currentTrainer;

        public UcTrainerProfile(int trainerId)
        {
            InitializeComponent();
            this.trainerId = trainerId;
            this.Loaded += UcTrainerProfile_Loaded;
        }

        private async void UcTrainerProfile_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadTrainerData();
        }

        private async Task LoadTrainerData()
        {
            try
            {
                currentTrainer = await Task.Run(() => profileService.GetTrainerProfile(trainerId));
                Load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void Load()
        {
            if (currentTrainer == null) return;

            txtNameSurname.Text = currentTrainer.name_surname ?? "";
            txtEmail.Text = currentTrainer.email ?? "";
            txtUsername.Text = currentTrainer.username ?? "";
            dpEmploymentDate.SelectedDate = currentTrainer.employment_date;

            var profile = currentTrainer.Trainer_profile?.FirstOrDefault();
            txtDescription.Text = profile?.description ?? "";

            string initials = GetInitials(currentTrainer.name_surname);
            txtAvatarInitials.Text = initials;
            txtHeaderInitials.Text = initials;
            txtHeaderName.Text = currentTrainer.name_surname ?? "";
            txtProfileName.Text = currentTrainer.name_surname ?? "";

            int clientCount = await Task.Run(() => profileService.GetClientCount(trainerId));
            txtStatClients.Text = clientCount.ToString();
            txtStatRating.Text = profile?.rating != null ? profile.rating.Value.ToString("0.0") : "-";
            txtStatPrice.Text = profile?.price != null ? profile.price.Value.ToString("0") + " EUR" : "-";

            txtPrice.Text = profile?.price != null ? profile.price.Value.ToString("0.##") : "";

            if (currentTrainer.employment_date.HasValue)
            {
                int years = DateTime.Now.Year - currentTrainer.employment_date.Value.Year;
                txtStatExperience.Text = years + " god";
                txtExperience.Text = years.ToString();
            }
            else
            {
                txtExperience.Text = "";
            }

            SetSpecializationCombo(cmbPrimarySpec, currentTrainer.specialization);
        }

        private string GetInitials(string nameSurname)
        {
            if (string.IsNullOrWhiteSpace(nameSurname)) return "?";
            var parts = nameSurname.Trim().Split(' ');
            if (parts.Length >= 2)
                return (parts[0][0].ToString() + parts[parts.Length - 1][0].ToString()).ToUpper();
            return parts[0][0].ToString().ToUpper();
        }

        private void SetSpecializationCombo(ComboBox combo, string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return;
            foreach (ComboBoxItem item in combo.Items)
            {
                if (item.Content.ToString() == value)
                {
                    combo.SelectedItem = item;
                    return;
                }
            }
        }

        private async void btnSavePersonal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (currentTrainer == null) return;

                currentTrainer.name_surname = txtNameSurname.Text;
                currentTrainer.email = txtEmail.Text;

                var selectedSpec = cmbPrimarySpec.SelectedItem as ComboBoxItem;
                currentTrainer.specialization = selectedSpec?.Content.ToString();

                var profile = currentTrainer.Trainer_profile?.FirstOrDefault();
                if (profile == null)
                {
                    profile = new Trainer_profile { TrainerId = trainerId };
                    currentTrainer.Trainer_profile.Add(profile);
                }
                profile.description = txtDescription.Text;

                if (decimal.TryParse(txtPrice.Text, out decimal price))
                    profile.price = price;
                else
                    profile.price = null;

                if (dpEmploymentDate.SelectedDate.HasValue)
                    currentTrainer.employment_date = dpEmploymentDate.SelectedDate.Value;
                else if (int.TryParse(txtExperience.Text, out int experience) && experience >= 0)
                    currentTrainer.employment_date = DateTime.Now.AddYears(-experience);

                bool result = await Task.Run(() => profileService.UpdateProfile(currentTrainer));

                if (result)
                {
                    MessageBox.Show("Profil uspješno ažuriran!", "Uspjeh", MessageBoxButton.OK, MessageBoxImage.Information);
                    await LoadTrainerData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnCancelPersonal_Click(object sender, RoutedEventArgs e)
        {
            await LoadTrainerData();
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Jeste li sigurni da se želite odjaviti?", "Odjava", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                var loginWindow = new LoginWindow();
                loginWindow.Show();
                Window.GetWindow(this)?.Close();
            }
        }

        private void btnCancelPassword_Click(object sender, RoutedEventArgs e)
        {
            txtOldPassword.Clear();
            txtNewPassword.Clear();
            txtConfirmPassword.Clear();
        }

        private async void btnChangePassword_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string oldPass = txtOldPassword.Password;
                string newPass = txtNewPassword.Password;
                string confirmPass = txtConfirmPassword.Password;

                bool result = await Task.Run(() => profileService.ChangePassword(trainerId, oldPass, newPass, confirmPass));

                if (result)
                {
                    MessageBox.Show("Lozinka uspješno promijenjena!", "Uspjeh", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtOldPassword.Clear();
                    txtNewPassword.Clear();
                    txtConfirmPassword.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
