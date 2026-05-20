using ApiServiceLayer.Services;
using System;
using System.Windows;

namespace Zeon
{
    public partial class LoginWindow : Window
    {
        private readonly AuthApiService _authApiService = new AuthApiService();

        public LoginWindow()
        {
            InitializeComponent();
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                btnLogin.IsEnabled = false;
                btnLogin.Content = "Prijava...";

                string username = txtUsername.Text;
                string password = txtPassword.Password;

                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    MessageBox.Show("Unesite korisničko ime i lozinku.", "Upozorenje",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var response = await _authApiService.LoginAsync(username, password);

                if (response.User != null && response.User.IdTrainer.HasValue)
                {
                    MainWindow main = new MainWindow(response.User.IdTrainer.Value);
                    main.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show(response.Error ?? "Prijava nije uspjela.", "Greška",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (ApiException ex)
            {
                MessageBox.Show("Pogrešno korisničko ime ili lozinka.", "Greška",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greška pri povezivanju sa serverom. Provjerite internetsku vezu.", "Greška",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                btnLogin.IsEnabled = true;
                btnLogin.Content = "Prijava";
            }
        }

        private void lnkRegister_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow register = new RegisterWindow();
            this.Hide();
            register.ShowDialog();
        }
    }
}
