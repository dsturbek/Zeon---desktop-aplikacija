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
    public class FoodWithAmount
    {
        public Food Food { get; set; }
        public decimal amount { get; set; }
        public decimal TotalKcal => Food != null ? (Food.kCal ?? 0) * amount / 100 : 0;
    }

    public partial class AddMeal : UserControl
    {
        private FoodPlanService service;
        private int planId;
        private ObservableCollection<FoodWithAmount> addedFoods;
        private Planned_meal _editingMeal;

        public event Action OnSaved;

        public AddMeal(int planId, Planned_meal meal=null)
        {
            InitializeComponent();
            this.planId = planId;
            this._editingMeal = meal;
            service = new FoodPlanService();
            addedFoods = new ObservableCollection<FoodWithAmount>();
            dgAddedFoods.ItemsSource = addedFoods;

            
            if (_editingMeal != null)
            {
                LoadMealData();
            }
            else
            {
                dtpMealDate.SelectedDate = DateTime.Now;
            }

        }

        private async void LoadMealData()
        {
            dtpMealDate.SelectedDate = _editingMeal.date;

            foreach (var item in cmbMealType.Items)
            {
                if (item is ComboBoxItem cbItem && cbItem.Content.ToString() == _editingMeal.meal_type)
                {
                    cmbMealType.SelectedItem = cbItem;
                    break;
                }
            }

            try
            {
                var mealFoods = await service.GetFoodsForMeal(_editingMeal.id_planned_meal);

                foreach (var mf in mealFoods)
                {
                    addedFoods.Add(new FoodWithAmount
                    {
                        Food = mf.Food, 
                        amount = (int)mf.amount
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greška pri učitavanju namirnica: " + ex.Message);
            }
        }

        private async void TxtSearchFood_TextChanged(object sender, TextChangedEventArgs e)
        {
            string query = txtSearchFood.Text?.Trim();
            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            {
                popResults.IsOpen = false;
                return;
            }

            try
            {
                var results = await service.SearchFoodAndRecipes(query);
                if (results != null && results.Count > 0)
                {
                    lstSearchResults.ItemsSource = results;
                    popResults.IsOpen = true;
                }
                else
                {
                    popResults.IsOpen = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri pretraživanju: {ex.Message}", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LstSearchResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstSearchResults.SelectedItem is FoodOrRecipeItem selectedItem)
            {
                txtSearchFood.Text = selectedItem.Name;
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            OnSaved?.Invoke();
        }

        private async void BtnAddFood_Click(object sender, RoutedEventArgs e)
        {
            if (lstSearchResults.SelectedItem == null)
            {
                MessageBox.Show("Odaberite namirnicu iz rezultata pretrage.", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtAmount.Text, out decimal amount) || amount <= 0)
            {
                MessageBox.Show("Unesite ispravnu količinu (broj veći od 0).", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedItem = (FoodOrRecipeItem)lstSearchResults.SelectedItem;

            if (selectedItem.Type == "Recipe")
            {
                var recipeService = new RecipeService();
                var ingredients = await recipeService.GetIngredientsForRecipeAsync(selectedItem.Id);

                if (ingredients == null || !ingredients.Any()) return;

                decimal totalRecipeGrams = ingredients.Sum(i => i.amount_grams ?? 0);

                if (totalRecipeGrams == 0) totalRecipeGrams = 100;

                decimal scalingFactor = amount / totalRecipeGrams;

                foreach (var ingredient in ingredients)
                {
                    decimal calculatedIngredientAmount = (ingredient.amount_grams ?? 0) * scalingFactor;

                    var existing = addedFoods.FirstOrDefault(f => f.Food.id_food == ingredient.FoodId);
                    if (existing != null)
                    {
                        existing.amount += calculatedIngredientAmount;
                    }
                    else
                    {
                        addedFoods.Add(new FoodWithAmount
                        {
                            Food = ingredient.Food,
                            amount = calculatedIngredientAmount 
                        });
                    }
                }

                MessageBox.Show($"Dodane sve namirnice iz recepta '{selectedItem.Name}'.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else 
            {
                var selectedFood = selectedItem.Food; 

                if (addedFoods.Any(f => f.Food.id_food == selectedFood.id_food))
                {
                    MessageBox.Show("Ova namirnica je već dodana u obrok.", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                addedFoods.Add(new FoodWithAmount
                {
                    Food = selectedFood,
                    amount = amount
                });
            }

            popResults.IsOpen = false;
            txtSearchFood.Clear();
            txtAmount.Text = "100";
            lstSearchResults.SelectedItem = null;

        }

        private void BtnRemoveFood_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn?.DataContext is FoodWithAmount item)
            {
                addedFoods.Remove(item);
            }
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {

            if (dtpMealDate.SelectedDate == null)
            {
                MessageBox.Show("Odaberite datum obroka.", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (addedFoods.Count == 0)
            {
                MessageBox.Show("Dodajte barem jednu namirnicu u obrok.", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                int savedMealId;

                if (_editingMeal != null)
                {
                    _editingMeal.meal_type = ((ComboBoxItem)cmbMealType.SelectedItem).Content.ToString();
                    _editingMeal.date = dtpMealDate.SelectedDate.Value;
                    
                    await service.AddMealToPlan(_editingMeal);
                    savedMealId = _editingMeal.id_planned_meal;

                    await service.ClearFoodsFromMeal(savedMealId);
                }
                else
                {
                    var meal = new Planned_meal
                    {
                        meal_type = ((ComboBoxItem)cmbMealType.SelectedItem).Content.ToString(),
                        date = dtpMealDate.SelectedDate.Value,
                        Food_planId = planId
                    };

                    var savedMeal = await service.AddMealToPlan(meal);
                    savedMealId = savedMeal.id_planned_meal;
                }

                foreach (var foodItem in addedFoods)
                {
                    var mealFood = new Planned_meal_Food
                    {
                        Planned_mealId = savedMealId,
                        FoodId = foodItem.Food.id_food,
                        amount = (int)foodItem.amount
                    };

                    await service.AttachFoodToMeal(mealFood);
                }

                MessageBox.Show("Obrok uspješno spremljen.", "Uspjeh", MessageBoxButton.OK, MessageBoxImage.Information);
                OnSaved?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri spremanju obroka: {ex.Message}", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
