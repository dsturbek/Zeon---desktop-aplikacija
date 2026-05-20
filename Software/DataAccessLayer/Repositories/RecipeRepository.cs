using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EntitiesLayer.Entities;
using System.Data.Entity;

namespace DataAccessLayer.Repositories
{
    public class RecipeRepository
    {
        public async Task<List<Recipe>> FindAll()
        {
            using (var context = new ZeonDbContext())
            {
                var query = from r in context.Recipes
                            orderby r.recipe_name
                            select r;
                return await query.ToListAsync();
            }
        }

        public async Task<Recipe> FindById(int recipeId)
        {
            using (var context = new ZeonDbContext())
            {
                return await context.Recipes.FindAsync(recipeId);
            }
        }

        public async Task<Recipe> Insert(Recipe recipe)
        {
            using (var context = new ZeonDbContext())
            {
                context.Recipes.Add(recipe);
                await context.SaveChangesAsync();
                return recipe;
            }
        }

        public async Task<bool> Update(Recipe recipe)
        {
            using (var context = new ZeonDbContext())
            {
                var existing = await context.Recipes.FindAsync(recipe.id_recipe);
                if (existing == null) return false;

                existing.recipe_name = recipe.recipe_name;
                existing.recipe_description = recipe.recipe_description;
                existing.instructions = recipe.instructions;

                return await context.SaveChangesAsync() > 0;
            }
        }

        public async Task<bool> Delete(int recipeId)
        {
            using (var context = new ZeonDbContext())
            {
                var foodRecipes = context.Food_Recipe.Where(fr => fr.RecipeId == recipeId);
                context.Food_Recipe.RemoveRange(foodRecipes);

                var recipe = await context.Recipes.FindAsync(recipeId);
                if (recipe == null) return false;

                context.Recipes.Remove(recipe);
                return await context.SaveChangesAsync() > 0;
            }
        }

        public async Task<List<Food_Recipe>> GetFoodsForRecipe(int recipeId)
        {
            using (var context = new ZeonDbContext())
            {
                var query = from fr in context.Food_Recipe.Include(f => f.Food)
                            where fr.RecipeId == recipeId
                            select fr;
                return await query.ToListAsync();
            }
        }

        public async Task<bool> AddFoodToRecipe(Food_Recipe foodRecipe)
        {
            using (var context = new ZeonDbContext())
            {
                context.Food_Recipe.Add(foodRecipe);
                return await context.SaveChangesAsync() > 0;
            }
        }

        public async Task<bool> ClearFoodsFromRecipe(int recipeId)
        {
            using (var context = new ZeonDbContext())
            {
                var items = context.Food_Recipe.Where(fr => fr.RecipeId == recipeId);
                context.Food_Recipe.RemoveRange(items);
                return await context.SaveChangesAsync() > 0;
            }
        }
    }
}