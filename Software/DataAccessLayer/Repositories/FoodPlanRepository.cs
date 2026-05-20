using EntitiesLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class FoodPlanRepository
    {
        public async Task<List<Food_plan>> FindAllByTrainerId(int trainerId)
        {
            using (var context = new ZeonDbContext())
            {
                var query = from fp in context.Food_plan.Include(f => f.Client)
                            where fp.Client.TrainerId == trainerId
                            select fp;
                return await query.ToListAsync();
            }
        }

        public async Task<List<Client>> FindClientsForTrainer(int trainerId)
        {
            using (var context = new ZeonDbContext())
            {
                var query = from c in context.Clients
                            where c.TrainerId == trainerId
                            select c;
                return await query.ToListAsync();
            }
        }

        public async Task<bool> InsertFoodPlan(Food_plan plan)
        {
            using (var context = new ZeonDbContext())
            {
                context.Food_plan.Add(plan);
                return await context.SaveChangesAsync() > 0;
            }
        }

        public async Task<bool> UpdateFoodPlan(Food_plan plan)
        {
            using (var context = new ZeonDbContext())
            {
                var existing = await context.Food_plan.FindAsync(plan.id_food_plan);
                if (existing == null) return false;

                existing.food_plan_name = plan.food_plan_name;
                existing.food_plan_description = plan.food_plan_description;
                existing.date_start = plan.date_start;
                existing.date_end = plan.date_end;
                existing.ClientId = plan.ClientId;

                return await context.SaveChangesAsync() > 0;
            }
        }

        /// <summary> Briše plan prehrane i sve povezane obroke/hranu 
        /// (ručni Cascade Delete). </summary>
        public async Task<bool> DeleteFoodPlan(int planId)
        {
            using (var context = new ZeonDbContext())
            {
                var plan = await context.Food_plan.FindAsync(planId);
                if (plan == null) return false;

                var meals = await context.Planned_meal
                                   .Where(m => m.Food_planId == planId)
                                   .ToListAsync();

                foreach (var meal in meals)
                {
                    var mealFoods = context.Planned_meal_Food
                                           .Where(mf => mf.Planned_mealId == meal.id_planned_meal);
                    context.Planned_meal_Food.RemoveRange(mealFoods);
                }

                context.Planned_meal.RemoveRange(meals);
                context.Food_plan.Remove(plan);

                return await context.SaveChangesAsync() > 0;
            }
        }

        public async Task<List<Food>> SearchFood(string query)
        {
            using (var context = new ZeonDbContext())
            {
                var q = from f in context.Foods
                        where f.name.Contains(query)
                        select f;
                return await q.ToListAsync();
            }
        }

        /// <summary> Kreira novi obrok (npr. Doručak) 
        /// unutar plana i vraća kreirani objekt s ID-em. </summary>
        public async Task<Planned_meal> InsertPlannedMeal(Planned_meal meal)
        {
            using (var context = new ZeonDbContext())
            {
                context.Planned_meal.Add(meal);
                await context.SaveChangesAsync();
                return meal;
            }
        }

        /// <summary> Povezuje konkretnu namirnicu i 
        /// njezinu količinu s određenim obrokom. </summary>
        public async Task<bool> InsertPlannedMealFood(Planned_meal_Food mealFood)
        {
            using (var context = new ZeonDbContext())
            {
                context.Planned_meal_Food.Add(mealFood);
                return await context.SaveChangesAsync() > 0;
            }
        }

        /// <summary> Dohvaća sve planirane obroke za zadani plan. </summary>
        public async Task<List<Planned_meal>> GetMealsForPlan(int planId)
        {
            using (var context = new ZeonDbContext())
            {
                var query = from pm in context.Planned_meal
                            where pm.Food_planId == planId
                            select pm;
                return await query.ToListAsync();
            }
        }

        /// <summary> Briše sve namirnice povezane s jednim konkretnim obrokom. </summary>
        public async Task<bool> DeleteFoodsForMeal(int mealId)
        {
            using (var context = new ZeonDbContext())
            {
                var foods = context.Planned_meal_Food
                                   .Where(mf => mf.Planned_mealId == mealId);

                context.Planned_meal_Food.RemoveRange(foods);
                return await context.SaveChangesAsync() > 0;
            }
        }

        /// <summary> Briše sam obrok iz baze. </summary>
        public async Task<bool> DeletePlannedMeal(int mealId)
        {
            using (var context = new ZeonDbContext())
            {
                var meal = await context.Planned_meal.FindAsync(mealId);
                if (meal == null) return false;

                context.Planned_meal.Remove(meal);
                return await context.SaveChangesAsync() > 0;
            }
        }

        /// <summary> Dohvaća sve namirnice s detaljima za određeni obrok. </summary>
        public async Task<List<Planned_meal_Food>> GetFoodsForMeal(int mealId)
        {
            using (var context = new ZeonDbContext())
            {
                var query = from pmf in context.Planned_meal_Food.Include(p => p.Food)
                            where pmf.Planned_mealId == mealId
                            select pmf;
                return await query.ToListAsync();
            }
        }


    }
}
