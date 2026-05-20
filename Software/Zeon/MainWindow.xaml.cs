using BussinessLogicLayer;
using ApiServiceLayer.Services;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using PresentationLayer.Windows;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Zeon
{
    public partial class MainWindow : Window
    {
        private int trainerId;
        private readonly NotificationApiService _notificationService = new NotificationApiService();
        private DispatcherTimer _notificationTimer;

        public MainWindow(int trainerId)
        {
            InitializeComponent();
            this.trainerId = trainerId;
            this.Loaded += MainWindow_Loaded;
            this.Closed += MainWindow_Closed;
        }

        public MainWindow() : this(0)
        {
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadTrainerHeader();
            await CheckNotifications();
            StartNotificationPolling();
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            StopNotificationPolling();
        }

        private void StartNotificationPolling()
        {
            _notificationTimer = new DispatcherTimer();
            _notificationTimer.Interval = TimeSpan.FromSeconds(60);
            _notificationTimer.Tick += async (s, e) => await CheckNotifications();
            _notificationTimer.Start();
        }

        private void StopNotificationPolling()
        {
            if (_notificationTimer != null)
            {
                _notificationTimer.Stop();
                _notificationTimer = null;
            }
        }

        private async Task CheckNotifications()
        {
            try
            {
                var countResponse = await _notificationService.GetNotificationCountAsync(trainerId);

                Dispatcher.Invoke(() =>
                {
                    if (countResponse.TotalNewCount > 0)
                    {
                        txtNotificationCount.Text = countResponse.TotalNewCount.ToString();
                        badgeNotifications.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        badgeNotifications.Visibility = Visibility.Collapsed;
                    }
                });
            }
            catch
            {
                // Ignoriramo greške pri dohvatu obavijesti
            }
        }

        public void RefreshNotificationBadge()
        {
            _ = CheckNotifications();
        }

        private async Task LoadTrainerHeader()
        {
            try
            {
                var profileService = new ProfileService();
                var trainer = await Task.Run(() => profileService.GetTrainerProfile(trainerId));
                if (trainer != null)
                {
                    txtDashboardName.Text = trainer.name_surname ?? "";
                    txtDashboardInitials.Text = GetInitials(trainer.name_surname);
                }
            }
            catch { }
        }

        private string GetInitials(string nameSurname)
        {
            if (string.IsNullOrWhiteSpace(nameSurname)) return "?";
            var parts = nameSurname.Trim().Split(' ');
            if (parts.Length >= 2)
                return (parts[0][0].ToString() + parts[parts.Length - 1][0].ToString()).ToUpper();
            return parts[0][0].ToString().ToUpper();
        }

        private void ShowDashboard()
        {
            gridDashboard.Visibility = Visibility.Visible;
            contentMain.Visibility = Visibility.Collapsed;
            contentMain.Content = null;
        }

        public void ShowUserControl(UserControl control)
        {
            gridDashboard.Visibility = Visibility.Collapsed;
            contentMain.Content = control;
            contentMain.Visibility = Visibility.Visible;
        }

        private void btnDashboard_Click(object sender, RoutedEventArgs e)
        {
            ShowDashboard();
        }

        private void ProfileHeader_Click(object sender, MouseButtonEventArgs e)
        {
            var uc = new UcTrainerProfile(trainerId);
            ShowUserControl(uc);
        }

        private void btnMyProfile_Click(object sender, MouseButtonEventArgs e)
        {
            var uc = new UcTrainerProfile(trainerId);
            ShowUserControl(uc);
        }

        private void btnPredefinedPlans_Click(object sender, RoutedEventArgs e)
        {
            var uc = new UcWorkoutTemplates(trainerId);
            ShowUserControl(uc);
        }

        private void btnChat_Click(object sender, RoutedEventArgs e)
        {
            var uc = new UcCommunication(trainerId);
            ShowUserControl(uc);
        }

        private void btnNotifications_Click(object sender, RoutedEventArgs e)
        {
            var uc = new UcCommunication(trainerId);
            uc.Loaded += (s, args) =>
            {
                uc.tabCommunication.SelectedIndex = 1;
            };
            ShowUserControl(uc);
        }

        private void btnShowAllClients_Click(object sender, MouseButtonEventArgs e)
        {
            var uc = new FrmClients(trainerId, contentMain);
            ShowUserControl(uc);
        }

        private void btnShowAllWorkouts_Click(object sender, MouseButtonEventArgs e)
        {
            var uc = new UcWorkoutTemplates(trainerId);
            ShowUserControl(uc);
        }

        private void btnPopisKlijenata_Click(object sender, RoutedEventArgs e)
        {
            var uc = new FrmClients(trainerId, contentMain);
            ShowUserControl(uc);
        }

        private void btnDodajKlijenta_Click(object sender, RoutedEventArgs e)
        {
            var uc = new FrmClients(trainerId, contentMain);
            uc.Loaded += (s, args) =>
            {
                uc.tabClients.SelectedIndex = 1;
            };
            ShowUserControl(uc);
        }

        private void BtnFeedback_Click(object sender, RoutedEventArgs e)
        {
            var uc = new UcCommunication(trainerId);
            uc.Loaded += (s, args) =>
            {
                uc.tabCommunication.SelectedIndex = 2;
            };
            ShowUserControl(uc);
        }

        private void btnIzvjestaji_Click(object sender, RoutedEventArgs e)
        {
            var uc = new UcWorkoutTemplates(trainerId);
            uc.Loaded += (s, args) =>
            {
                uc.tabWorkouts.SelectedIndex = 1;
            };
            ShowUserControl(uc);
        }

        private void btnFoodPlans_Click(object sender, RoutedEventArgs e)
        {
            var uc = new Recipes(trainerId);
            uc.Loaded += (s, args) =>
            {
                uc.tabNutrition.SelectedItem = uc.tabFoodPlans;
            };
            ShowUserControl(uc);
        }

        private void btnRecipes_Click(object sender, RoutedEventArgs e)
        {
            var uc = new Recipes(trainerId);
            uc.Loaded += (s, args) =>
            {
                uc.tabNutrition.SelectedItem = uc.tabRecipes;
            };
            ShowUserControl(uc);
        }

        private void FoodManage_Click(object sender, RoutedEventArgs e)
        {
            var uc = new Recipes(trainerId);
            uc.Loaded += (s, args) =>
            {
                uc.tabNutrition.SelectedItem = uc.tabFoodManage;
            };
            ShowUserControl(uc);
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
            {
                OpenUserDocumentation();
                e.Handled = true;
            }
        }

        private void OpenUserDocumentation()
        {
            var pdfPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resursi", "Korisnicka_dokumentacija.pdf");

            if (!File.Exists(pdfPath))
            {
                MessageBox.Show("Korisnička dokumentacija nije pronađena.", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = pdfPath,
                UseShellExecute = true
            });
        }
    }
}
