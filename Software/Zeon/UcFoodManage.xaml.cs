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
    /// <summary>
    /// Interaction logic for UcFoodManage.xaml
    /// </summary>
    public partial class UcFoodManage : UserControl
    {
        private FoodService foodService;
        private List<Food> allFoods;

        public UcFoodManage()
        {
            InitializeComponent();
            foodService = new FoodService();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadFoodsAsync();
        }

        private async Task LoadFoodsAsync()
        {
            try
            {
                allFoods = await Task.Run(() => foodService.GetAllFoodsAsync());
                DisplayFoods(allFoods);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri učitavanju hrane: {ex.Message}",
                    "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DisplayFoods(List<Food> foods)
        {
            spFoodCards.Children.Clear();

            if (foods == null || foods.Count == 0)
            {
                var noDataText = new TextBlock
                {
                    Text = "Nema hrane u bazi podataka.",
                    FontSize = 16,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7A7A7A")),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 50, 0, 0)
                };
                spFoodCards.Children.Add(noDataText);
                return;
            }

            int columns = 3;
            int rows = (int)Math.Ceiling(foods.Count / (double)columns);

            Grid mainGrid = new Grid();
            mainGrid.HorizontalAlignment = HorizontalAlignment.Stretch;

            for (int i = 0; i < columns; i++)
            {
                mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            }

            for (int i = 0; i < rows; i++)
            {
                mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }

            for (int i = 0; i < foods.Count; i++)
            {
                var food = foods[i];
                int row = i / columns;
                int col = i % columns;

                var card = CreateFoodCard(food);
                Grid.SetRow(card, row);
                Grid.SetColumn(card, col);

                mainGrid.Children.Add(card);
            }

            spFoodCards.Children.Add(mainGrid);
        }

        private Border CreateFoodCard(Food food)
        {
            var card = new Border
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1A1A1A")),
                CornerRadius = new CornerRadius(8),
                Margin = new Thickness(0, 0, 15, 15),
                Padding = new Thickness(20)
            };

            var mainStack = new StackPanel();

            var nameText = new TextBlock
            {
                Text = food.name,
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFD")),
                Margin = new Thickness(0, 0, 0, 15)
            };
            mainStack.Children.Add(nameText);

            var badgesStack = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 0, 0, 15)
            };

            var caloriesBorder = new Border
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF0000")),
                CornerRadius = new CornerRadius(5),
                Padding = new Thickness(10,5,10, 5),
                Margin = new Thickness(0, 0, 10, 0)
            };
            var caloriesText = new TextBlock
            {
                Text = $"{food.kCal} kcal",
                FontSize = 12,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White
            };
            caloriesBorder.Child = caloriesText;
            badgesStack.Children.Add(caloriesBorder);

            var portionBorder = new Border
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00AA00")),
                CornerRadius = new CornerRadius(5),
                Padding = new Thickness(10,5,10, 5)
            };
            var portionText = new TextBlock
            {
                Text = "100g",
                FontSize = 12,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White
            };
            portionBorder.Child = portionText;
            badgesStack.Children.Add(portionBorder);

            mainStack.Children.Add(badgesStack);

            var macrosGrid = new Grid { Margin = new Thickness(0, 0, 0, 15) };
            macrosGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            macrosGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(10) });
            macrosGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            macrosGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            macrosGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(10) });
            macrosGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var proteinsBox = CreateMacroBox("PROTEINI", food.proteins.ToString() + "g", "#FF0000");
            Grid.SetRow(proteinsBox, 0);
            Grid.SetColumn(proteinsBox, 0);
            macrosGrid.Children.Add(proteinsBox);

            var carbsBox = CreateMacroBox("UGLJIKOHIDRATI", food.carbohydrates.ToString() + "g", "#0066FF");
            Grid.SetRow(carbsBox, 0);
            Grid.SetColumn(carbsBox, 2);
            macrosGrid.Children.Add(carbsBox);

            var fatsBox = CreateMacroBox("MASTI", food.fat.ToString() + "g", "#FFB800");
            Grid.SetRow(fatsBox, 2);
            Grid.SetColumn(fatsBox, 0);
            macrosGrid.Children.Add(fatsBox);

            var fiberBox = CreateMacroBox("VLAKNA", "0g", "#00AA00");
            Grid.SetRow(fiberBox, 2);
            Grid.SetColumn(fiberBox, 2);
            macrosGrid.Children.Add(fiberBox);

            mainStack.Children.Add(macrosGrid);

            var separator = new Border
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2A2A2A")),
                Height = 1,
                Margin = new Thickness(0, 10, 0, 15)
            };
            mainStack.Children.Add(separator);

            var buttonsGrid = new Grid();

            var btnEdit = new Button
            {
                Content = "Uredi",
                Height = 40,
                FontSize = 13,
                FontWeight = FontWeights.Bold,
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF0000")),
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                Cursor = System.Windows.Input.Cursors.Hand,
                Tag = food.id_food
            };

            btnEdit.Click += btnEditFood_Click;

            var template = new ControlTemplate(typeof(Button));
            var factory = new FrameworkElementFactory(typeof(Border));
            factory.SetValue(Border.BackgroundProperty, new TemplateBindingExtension(Button.BackgroundProperty));
            factory.SetValue(Border.CornerRadiusProperty, new CornerRadius(5));

            var contentPresenter = new FrameworkElementFactory(typeof(ContentPresenter));
            contentPresenter.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            contentPresenter.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);
            factory.AppendChild(contentPresenter);

            template.VisualTree = factory;
            btnEdit.Template = template;

            buttonsGrid.Children.Add(btnEdit);
            mainStack.Children.Add(buttonsGrid);

            card.Child = mainStack;
            return card;
        }

        private Border CreateMacroBox(string label, string value, string borderColor)
        {
            var box = new Border
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2A2A2A")),
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(borderColor)),
                BorderThickness = new Thickness(0, 0, 0, 3),
                CornerRadius = new CornerRadius(5),
                Padding = new Thickness(15,10,15, 10)
            };

            var stack = new StackPanel();

            var labelText = new TextBlock
            {
                Text = label,
                FontSize = 10,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7A7A7A")),
                Margin = new Thickness(0, 0, 0, 5)
            };

            var valueText = new TextBlock
            {
                Text = value,
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFD"))
            };

            stack.Children.Add(labelText);
            stack.Children.Add(valueText);
            box.Child = stack;

            return box;
        }

        private void txtSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                txtSearchPlaceholder.Visibility = Visibility.Collapsed;
            }
        }

        private void txtSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                txtSearchPlaceholder.Visibility = Visibility.Visible;
            }
        }

        private async void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchTerm = txtSearch.Text;

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                DisplayFoods(allFoods);
                return;
            }

            try
            {
                var filteredFoods = await Task.Run(() => foodService.SearchFoodsAsync(searchTerm));
                DisplayFoods(filteredFoods);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri pretraživanju: {ex.Message}", "Greška");
            }
        }

        private void btnAddFood_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddEditFoodWindow();

            if (addWindow.ShowDialog() == true)
            {
                _ = LoadFoodsAsync();
            }
        }

        private void btnEditFood_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            int foodId = (int)button.Tag;

            var editWindow = new AddEditFoodWindow(foodId);

            if (editWindow.ShowDialog() == true)
            {
                _ = LoadFoodsAsync();
            }
        }
    }
}
