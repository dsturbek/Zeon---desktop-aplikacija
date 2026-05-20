using EntitiesLayer.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class FoodRepository
    {
        public async Task<List<Food>> GetAllAsync()
        {
            using (var context = new ZeonDbContext())
            {
                return await context.Foods.ToListAsync();
            }
        }

        public async Task<Food> GetByIdAsync(int foodId)
        {
            using (var context = new ZeonDbContext())
            {
                return await context.Foods.FindAsync(foodId);
            }
        }

        public async Task<Food> GetByNameAsync(string name)
        {
            using (var context = new ZeonDbContext())
            {
                return await context.Foods
                    .Where(f => f.name.ToLower() == name.ToLower())
                    .FirstOrDefaultAsync();
            }
        }

        public async Task<List<Food>> SearchByNameAsync(string searchTerm)
        {
            using (var context = new ZeonDbContext())
            {
                return await context.Foods
                    .Where(f => f.name.ToLower().Contains(searchTerm.ToLower()))
                    .ToListAsync();
            }
        }

        public async Task<bool> AddAsync(Food food)
        {
            using (var context = new ZeonDbContext())
            {
                context.Foods.Add(food);
                await context.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> UpdateAsync(Food food)
        {
            using (var context = new ZeonDbContext())
            {
                context.Foods.Attach(food);
                context.Entry(food).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return true;
            }
        }
        public async Task<bool> DeleteAsync(int foodId)
        {
            using (var context = new ZeonDbContext())
            {
                var food = await context.Foods
                    .Include(f => f.Food_Food_diary)
                    .Include(f => f.Food_Recipe)
                    .Include(f => f.Planned_meal_Food)
                    .FirstOrDefaultAsync(f => f.id_food == foodId);

                if (food == null)
                {
                    throw new Exception("Hrana nije pronađena!");
                }

                if (food.Food_Food_diary.Any())
                {
                    throw new Exception("Ne možete obrisati hranu koja se koristi u dnevnicima prehrane!");
                }

                if (food.Food_Recipe.Any())
                {
                    throw new Exception("Ne možete obrisati hranu koja se koristi u receptima!");
                }

                if (food.Planned_meal_Food.Any())
                {
                    throw new Exception("Ne možete obrisati hranu koja se koristi u planovima obroka!");
                }

                context.Foods.Remove(food);
                await context.SaveChangesAsync();

                return true;
            }
        }
    }
}
