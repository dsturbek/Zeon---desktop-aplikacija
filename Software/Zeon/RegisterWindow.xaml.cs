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
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        TrainerService trainerService = new TrainerService();
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private async void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(txtPassword.Password != txtConfirmPassword.Password)
                {
                    MessageBox.Show("Lozinke se ne podudaraju!", "Greška",
                MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var trainer = new Trainer
                {
                    username = txtUsername.Text,
                    password = txtPassword.Password,
                    email = txtEmail.Text,
                    name_surname = txtName_Surename.Text
                };
                bool success = await Task.Run(() => trainerService.TrainerRegister(trainer));
                if (success)
                {
                    MainWindow main = new MainWindow(trainer.id_trainer);
                    main.Show();
                    this.Close();
                    foreach (Window window in Application.Current.Windows)
                    {
                        if (window is LoginWindow)
                            window.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Greška",
                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void lnkLogin_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            foreach (Window window in Application.Current.Windows)
            {
                if (window is LoginWindow)
                {
                    window.Show();
                    window.Activate();
                    break;
                }
            }
        }
    }
}
