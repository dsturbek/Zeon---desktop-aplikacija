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

namespace PresentationLayer.Windows
{
    /// <summary>
    /// Interaction logic for FrmAddClient.xaml
    /// </summary>
    public partial class FrmAddClient : UserControl
    {
        private readonly int _trainerId;
        private readonly ClientService _clientService = new ClientService();

        public event EventHandler ClientAssigned;

        public FrmAddClient(int trainerId)
        {
            InitializeComponent();
            _trainerId = trainerId;
            LoadAllClients();
        }

        public void LoadAllClients()
        {
            dgAllClients.ItemsSource = _clientService.SearchAllClientsToAdd(string.Empty);
        }

        private Client GetSelectedClient()
        {
            return dgAllClients.SelectedItem as Client;
        }

        private void txtSearchAll_TextChanged(object sender, TextChangedEventArgs e)
        {
            dgAllClients.ItemsSource = _clientService.SearchAllClientsToAdd(txtSearchAll.Text.Trim());
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            txtSearchAll.Clear();
            LoadAllClients();
        }

        private void btnAssign_Click(object sender, RoutedEventArgs e)
        {
               var selected = GetSelectedClient();
               if (selected == null)
               {
                   MessageBox.Show("Molimo odaberite klijenta iz liste.", "Obavijest", MessageBoxButton.OK, MessageBoxImage.Information);
                   return;
               }

               var ok = _clientService.AddClientToTrainer(selected.id_client, _trainerId);
               if (!ok)
               {
                   MessageBox.Show("Došlo je do greške prilikom dodavanja klijenta. Pokušajte ponovno.", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                   return;
               }

               MessageBox.Show("Klijent je uspješno dodan.", "Obavijest", MessageBoxButton.OK, MessageBoxImage.Information);
               ClientAssigned?.Invoke(this, EventArgs.Empty);
        }
    }
}
