using BussinessLogicLayer;
using EntitiesLayer.Entities;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Zeon
{
    public partial class ImportTemplateDialog : Window
    {
        private readonly TemplateService _templateService = new TemplateService();
        private readonly int _trainerId;

        public Workout_plan_template SelectedTemplate { get; private set; }
        public string PlanName { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }

        public ImportTemplateDialog(int trainerId)
        {
            InitializeComponent();
            _trainerId = trainerId;
            dpStartDate.SelectedDate = DateTime.Today;
            this.Loaded += ImportTemplateDialog_Loaded;
        }

        private void ImportTemplateDialog_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var templates = _templateService.GetTemplates(_trainerId);
                cmbTemplates.ItemsSource = templates;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            var selected = cmbTemplates.SelectedItem as Workout_plan_template;
            if (selected == null)
            {
                MessageBox.Show("Odaberite predložak.", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPlanName.Text))
            {
                MessageBox.Show("Unesite naziv plana.", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!dpStartDate.SelectedDate.HasValue)
            {
                MessageBox.Show("Odaberite datum početka.", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (dpEndDate.SelectedDate.HasValue && dpEndDate.SelectedDate.Value < dpStartDate.SelectedDate.Value)
            {
                MessageBox.Show("Datum završetka ne može biti prije datuma početka.", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SelectedTemplate = selected;
            PlanName = txtPlanName.Text.Trim();
            StartDate = dpStartDate.SelectedDate.Value;
            EndDate = dpEndDate.SelectedDate;
            DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
