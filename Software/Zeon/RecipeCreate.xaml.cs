using BussinessLogicLayer;
using EntitiesLayer.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    
    public partial class RecipeCreate : UserControl
    {

        private RecipeService service = new RecipeService();
        private FoodPlanService foodPlanService = new FoodPlanService();
        private Recipe currentRecipe;
        public ObservableCollection<Food_Recipe> Ingredients { get; set; } = new ObservableCollection<Food_Recipe>();
        public event Action OnSaved;
        public RecipeCreate(Recipe recipe = null)
        {
            InitializeComponent();

            currentRecipe = recipe ?? new Recipe();
            dgIngredients.ItemsSource = Ingredients;

            if (currentRecipe.id_recipe != 0) LoadData();
        }

        private async void LoadData()
        {
            txtHeader.Text = "Uredi Recept";
            txtRecipeName.Text = currentRecipe.recipe_name;
            txtDescription.Text = currentRecipe.recipe_description;
            txtInstructions.Text = currentRecipe.instructions;

            var existingIngredients = await service.GetIngredientsForRecipeAsync(currentRecipe.id_recipe);
            Ingredients.Clear();
            foreach (var item in existingIngredients) Ingredients.Add(item);
            UpdateTotalKcal();
        }

        private async void TxtSearchFood_TextChanged(object sender, TextChangedEventArgs e)
        {
            string query = txtSearchFood.Text;
            if (query.Length > 1)
            {
                var results = await foodPlanService.SearchFood(query);
                var cleanResults = results?.GroupBy(f => f.name).Select(g => g.First()).ToList();

                lbFoodResults.ItemsSource = cleanResults;
                popResults.IsOpen = cleanResults != null && cleanResults.Any();
            }
            else popResults.IsOpen = false;
        }

        private void LbFoodResults_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (lbFoodResults.SelectedItem is Food selectedFood)
            {
                if (!Ingredients.Any(i => i.FoodId == selectedFood.id_food))
                {
                    Ingredients.Add(new Food_Recipe { Food = selectedFood, FoodId = selectedFood.id_food, amount_grams = 100 });
                    UpdateTotalKcal();
                }
                popResults.IsOpen = false;
                txtSearchFood.Clear();
            }
        }

        private void UpdateTotalKcal()
        {
            decimal total = Ingredients.Sum(i => ((i.Food?.kCal ?? 0) / 100m) * (i.amount_grams ?? 0));
            lblTotalKcal.Text = $"Ukupno: {total:N0} kcal";
        }

        private void BtnRemoveIngredient_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button).DataContext as Food_Recipe;
            Ingredients.Remove(item);
            UpdateTotalKcal();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow.ShowUserControl(new Recipes());
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            dgIngredients.CommitEdit(DataGridEditingUnit.Row, true);
            try
            {
                currentRecipe.recipe_name = txtRecipeName.Text;
                currentRecipe.recipe_description = txtDescription.Text;
                currentRecipe.instructions = txtInstructions.Text;

                bool success = await service.SaveFullRecipeAsync(currentRecipe, Ingredients.ToList());
                if (success)
                {
                    MessageBox.Show("Recept uspješno spremljen!");
                    OnSaved?.Invoke();
                    BtnCancel_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DgIngredients_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() => UpdateTotalKcal()), System.Windows.Threading.DispatcherPriority.Background);
        }
    }
}
