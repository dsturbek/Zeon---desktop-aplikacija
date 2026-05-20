using BussinessLogicLayer.Services;
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
using PresentationLayer.Windows;

namespace PresentationLayer.Windows
{
    /// <summary>
    /// Interaction logic for FrmClients.xaml
    /// </summary>
    public partial class FrmClients : UserControl
    {
        private readonly int _trainerId;
        private readonly ClientService _clientService = new ClientService();
        private readonly ContentControl _contentPanel;
        private FrmAddClient _addClientControl;

        public FrmClients(int trainerId, ContentControl contentPanel = null)
        {
            InitializeComponent();
            _trainerId = trainerId;
            _contentPanel = contentPanel;
            InitializeAddClientTab();
            LoadClients();
        }

        private void InitializeAddClientTab()
        {
            _addClientControl = new FrmAddClient(_trainerId);
            _addClientControl.ClientAssigned += (s, e) => LoadClients();
            addClientContent.Content = _addClientControl;
        }

        private void LoadClients()
        {
            dgClients.ItemsSource = _clientService.GetClientsForTrainer(_trainerId);
        }

        private Client GetSelectedClient()
        {
            return dgClients.SelectedItem as Client;
        }

        private void txtSearchName_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = txtSearchName.Text.Trim();
            dgClients.ItemsSource = _clientService.SearchClientsByName(_trainerId, searchText);
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            txtSearchName.Clear();
            LoadClients();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            _addClientControl.LoadAllClients();
            tabClients.SelectedItem = tabAddClient;
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            var selected = GetSelectedClient();
            if (selected == null)
            {
                MessageBox.Show("Odaberite klijenta za brisanje.", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var confirm = MessageBox.Show($"Maknuti klijenta '{selected.name_surname}' iz liste vaših klijenata?", "Potvrda", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirm != MessageBoxResult.Yes)
                return;

            var ok = _clientService.RemoveClientFromTrainer(_trainerId, selected.id_client);

            if (!ok)
            {
                MessageBox.Show("Nije moguće maknuti klijenta. Pokušajte ponovo.", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            LoadClients();
        }

        private void btnDetails_Click(object sender, RoutedEventArgs e)
        {
            var selected = GetSelectedClient();
            if (selected == null)
            {
                MessageBox.Show("Odaberite klijenta za prikaz detalja.", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_contentPanel != null)
            {
                _contentPanel.Content = new ClientDetailsWindow(selected, _trainerId, _contentPanel);
            }
        }

        private void tabClients_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source != tabClients) return;

            if (tabClients.SelectedItem != tabAddClient)
            {
                LoadClients();
            }
        }
    }
}
