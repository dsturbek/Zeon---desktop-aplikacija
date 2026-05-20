using BussinessLogicLayer.Services;
using EntitiesLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Zeon
{
    public partial class NewConversationDialog : Window
    {
        private readonly ClientService _clientService = new ClientService();
        private readonly int _trainerId;
        private List<Client> _allClients;

        public Client SelectedClient { get; private set; }

        public NewConversationDialog(int trainerId)
        {
            InitializeComponent();
            _trainerId = trainerId;
            this.Loaded += NewConversationDialog_Loaded;
        }

        private void NewConversationDialog_Loaded(object sender, RoutedEventArgs e)
        {
            _allClients = _clientService.GetClientsForTrainer(_trainerId);
            lstClients.ItemsSource = _allClients;
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_allClients == null) return;
            string search = txtSearch.Text.ToLower().Trim();
            if (string.IsNullOrEmpty(search))
            {
                lstClients.ItemsSource = _allClients;
            }
            else
            {
                lstClients.ItemsSource = _allClients
                    .Where(c => (c.name_surname ?? "").ToLower().Contains(search))
                    .ToList();
            }
        }

        private void lstClients_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = lstClients.SelectedItem as Client;
            if (selected == null) return;

            SelectedClient = selected;
            DialogResult = true;
        }
    }
}
