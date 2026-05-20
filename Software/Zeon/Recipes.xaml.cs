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

    public partial class Recipes : UserControl
    {
        private RecipeService recipeService = new RecipeService();
        private int _trainerId;

        public Recipes(int trainerId)
        {
            InitializeComponent();
            _trainerId = trainerId;
            InitializeFoodPlansTab();
            InitializeFoodManageTab();
            LoadRecipesAsync();
        }

        public Recipes() : this(0)
        {
        }

        private void InitializeFoodPlansTab()
        {
            foodPlansContent.Content = new FoodPlan(_trainerId);
        }

        private void InitializeFoodManageTab()
        {
            foodManageContent.Content = new UcFoodManage();
        }

        private async void LoadRecipesAsync()
        {
            try
            {
                var recipes = await recipeService.GetAllRecipesWithKcalAsync();
                dgRecipes.ItemsSource = recipes;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri učitavanju recepata: {ex.Message}", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgRecipes.SelectedItem is Recipe selectedRecipe)
            {
                var result = MessageBox.Show($"Jeste li sigurni da želite obrisati recept '{selectedRecipe.recipe_name}'?",
                                           "Potvrda brisanja", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    bool success = await recipeService.DeleteRecipeAsync(selectedRecipe.id_recipe);
                    if (success)
                    {
                        LoadRecipesAsync();
                    }
                    else
                    {
                        MessageBox.Show("Greška pri brisanju recepta.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Odaberite recept za brisanje.");
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (dgRecipes.SelectedItem is Recipe selectedRecipe)
            {
                OpenRecipeDetails(selectedRecipe);
            }
            else
            {
                MessageBox.Show("Molimo odaberite recept koji želite urediti.", "Obavijest", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            OpenRecipeDetails(null);
        }

        private void OpenRecipeDetails(Recipe recipe)
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            if (mainWindow != null)
            {
                var recipeCreateUC = new RecipeCreate(recipe);
                recipeCreateUC.OnSaved += () => LoadRecipesAsync();
                mainWindow.ShowUserControl(recipeCreateUC);
            }
        }

        private void DgRecipes_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            BtnEdit_Click(sender, e);
        }
    }
}
