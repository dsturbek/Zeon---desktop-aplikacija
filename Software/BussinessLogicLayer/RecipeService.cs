using DataAccessLayer.Repositories;
using EntitiesLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessLogicLayer
{
    public class RecipeService
    {
        private RecipeRepository repository = new RecipeRepository();

        public async Task<List<Recipe>> GetAllRecipesWithKcalAsync()
        {
            var recipes = await repository.FindAll();

            foreach (var recipe in recipes)
            {
                var ingredients = await repository.GetFoodsForRecipe(recipe.id_recipe);
                recipe.TotalKcal = (double)ingredients.Sum(i =>((i.Food?.kCal ?? 0) / 100m) * (i.amount_grams ?? 0));
            }

            return recipes;
        }
        public async Task<bool> SaveFullRecipeAsync(Recipe recipe, List<Food_Recipe> ingredients)
        {
            if (string.IsNullOrWhiteSpace(recipe.recipe_name))
                throw new Exception("Naziv recepta je obavezan.");

            if (ingredients == null || !ingredients.Any())
                throw new Exception("Recept mora imati barem jednu namirnicu.");

            if (recipe.id_recipe == 0)
            {
                var savedRecipe = await repository.Insert(recipe);
                recipe.id_recipe = savedRecipe.id_recipe; 
            }
            else
            {
                await repository.ClearFoodsFromRecipe(recipe.id_recipe);
                await repository.Update(recipe);
            }

            foreach (var ing in ingredients)
            {
                if (ing.amount_grams > 0)
                {
                    var newRelation = new Food_Recipe
                    {
                        RecipeId = recipe.id_recipe, 
                        FoodId = ing.FoodId,
                        amount_grams = ing.amount_grams
                    };
                    await repository.AddFoodToRecipe(newRelation);
                }
            }

            return true;
        }

        public async Task<List<Food_Recipe>> GetIngredientsForRecipeAsync(int recipeId)
        {
            if (recipeId <= 0) return new List<Food_Recipe>();
            return await repository.GetFoodsForRecipe(recipeId);
        }

        public async Task<bool> DeleteRecipeAsync(int recipeId)
        {
            if (recipeId <= 0) return false;
            return await repository.Delete(recipeId);
        }
    }
}

