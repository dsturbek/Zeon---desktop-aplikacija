using DataAccessLayer.Repositories;
using EntitiesLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace BussinessLogicLayer
{
    public class FoodOrRecipeItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public decimal? Kcal { get; set; }
        public Food Food { get; set; } 
        public Recipe Recipe { get; set; } 
    }

    public class FoodPlanService
    {
        private FoodPlanRepository repository=new FoodPlanRepository();
        private RecipeRepository recipeRepository = new RecipeRepository();


        public async Task<List<Food_plan>> GetAllFoodPlansByTrainerId(int trainerId)
        {
            if (trainerId <= 0) 
                return new List<Food_plan>();
            return await repository.FindAllByTrainerId(trainerId);
        }

        public async Task<List<Client>> GetAllClientsForTrainer(int trainerId)
        {
            if (trainerId <= 0)
                return new List<Client>();
            return await repository.FindClientsForTrainer(trainerId);
        }

        public async Task<bool> SavePlan(Food_plan plan)
        {
            if (plan == null) 
                throw new ArgumentNullException(nameof(plan));

            if (string.IsNullOrWhiteSpace(plan.food_plan_name))
                throw new Exception("Naziv plana prehrane je obavezan.");

            if (plan.date_start >= plan.date_end)
                throw new Exception("Datum početka plana mora biti raniji od datuma završetka.");

            if (plan.ClientId <= 0)
                throw new Exception("Plan mora biti dodijeljen klijentu.");

            if (plan.id_food_plan == 0)
                return await repository.InsertFoodPlan(plan);

            return await repository.UpdateFoodPlan(plan);
        }

        public async Task<bool> DeleteFoodPlan(int planId)
        {
            if (planId <= 0) 
                return false;
            return await repository.DeleteFoodPlan(planId);
        }

        public async Task<List<Food>> SearchFood(string query)
        {
            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
                return new List<Food>();

            return await repository.SearchFood(query);
        }

        public async Task<List<FoodOrRecipeItem>> SearchFoodAndRecipes(string query)
        {
            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
                return new List<FoodOrRecipeItem>();

            var results = new List<FoodOrRecipeItem>();

            var foods = await repository.SearchFood(query);
            foreach (var food in foods)
            {
                results.Add(new FoodOrRecipeItem
                {
                    Id = food.id_food,
                    Name = food.name ?? food.name,
                    Type = "Food",
                    Kcal = food.kCal ?? food.kCal,
                    Food = food
                });
            }

            var recipes = await recipeRepository.FindAll();
            var matchingRecipes = recipes.Where(r => r.recipe_name.ToLower().Contains(query.ToLower())).ToList();

            foreach (var recipe in matchingRecipes)
            {
                var ingredients = await recipeRepository.GetFoodsForRecipe(recipe.id_recipe);
                decimal totalKcal = ingredients.Sum(i => ((i.Food?.kCal ?? 0) / 100m) * (i.amount_grams ?? 0));

                results.Add(new FoodOrRecipeItem
                {
                    Id = recipe.id_recipe,
                    Name = recipe.recipe_name,
                    Type = "Recipe",
                    Kcal = totalKcal,
                    Recipe = recipe
                });
            }

            return results;
        }

        public async Task<Planned_meal> AddMealToPlan(Planned_meal meal)
        {
            if (meal == null) 
                throw new ArgumentNullException(nameof(meal));

            return await repository.InsertPlannedMeal(meal);
        }

        public async Task<bool> AttachFoodToMeal(Planned_meal_Food mealFood)
        {
            if (mealFood == null) 
                throw new ArgumentNullException(nameof(mealFood));

            if (mealFood.amount <= 0)
                throw new Exception("Količina namirnice mora biti veća od nule.");

            return await repository.InsertPlannedMealFood(mealFood);
        }

        public async Task<List<Planned_meal>> GetMealsForPlan(int planId)
        {
            if (planId <= 0) 
                return new List<Planned_meal>();

            return await repository.GetMealsForPlan(planId);
        }

        public async Task<bool> DeleteMeal(int mealId)
        {
            if (mealId <= 0)
                return false;

            try
            {
                await repository.DeleteFoodsForMeal(mealId);

                return await repository.DeletePlannedMeal(mealId);
            }
            catch (Exception ex)
            {
                throw new Exception("Greška prilikom brisanja obroka: " + ex.Message);
            }
        }

        public async Task<List<Planned_meal_Food>> GetFoodsForMeal(int mealId)
        {
            if (mealId <= 0) 
                return new List<Planned_meal_Food>();

            return await repository.GetFoodsForMeal(mealId);
        }

        public async Task<bool> ClearFoodsFromMeal(int mealId)
        {
            if (mealId <= 0) return false;
            return await repository.DeleteFoodsForMeal(mealId);
        }

    }
}
