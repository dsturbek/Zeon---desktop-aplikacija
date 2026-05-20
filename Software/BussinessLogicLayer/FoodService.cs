using DataAccessLayer.Repositories;
using EntitiesLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessLogicLayer
{
    public class FoodService
    {
        private FoodRepository foodRepository;

        public FoodService()
        {
            foodRepository = new FoodRepository();
        }

        public async Task<List<Food>> GetAllFoodsAsync()
        {
            return await foodRepository.GetAllAsync();
        }

        public async Task<Food> GetFoodByIdAsync(int foodId)
        {
            if (foodId <= 0)
            {
                throw new Exception("ID hrane je neispravan!");
            }

            return await foodRepository.GetByIdAsync(foodId);
        }

        public async Task<List<Food>> SearchFoodsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetAllFoodsAsync();
            }

            return await foodRepository.SearchByNameAsync(searchTerm);
        }

        public async Task<bool> AddFoodAsync(Food food)
        {
            ValidateFood(food);

            var existingFood = await foodRepository.GetByNameAsync(food.name);

            if (existingFood != null)
            {
                throw new Exception($"Hrana '{food.name}' već postoji u sustavu!");
            }

            return await foodRepository.AddAsync(food);
        }

        public async Task<bool> UpdateFoodAsync(Food food)
        {
            ValidateFood(food);

            if (food.id_food <= 0)
            {
                throw new Exception("ID hrane je neispravan!");
            }

            var existingFood = await foodRepository.GetByNameAsync(food.name);

            if (existingFood != null && existingFood.id_food != food.id_food)
            {
                throw new Exception($"Hrana s nazivom '{food.name}' već postoji!");
            }

            return await foodRepository.UpdateAsync(food);
        }

        public async Task<bool> DeleteFoodAsync(int foodId)
        {
            if (foodId <= 0)
            {
                throw new Exception("ID hrane je neispravan!");
            }

            return await foodRepository.DeleteAsync(foodId);
        }

        private void ValidateFood(Food food)
        {
            if (string.IsNullOrWhiteSpace(food.name))
            {
                throw new Exception("Naziv hrane je obavezan!");
            }

            if (food.kCal < 0)
            {
                throw new Exception("Kalorije ne mogu biti negativne!");
            }

            if (food.proteins < 0)
            {
                throw new Exception("Proteini ne mogu biti negativni!");
            }

            if (food.carbohydrates < 0)
            {
                throw new Exception("Ugljikohidrati ne mogu biti negativni!");
            }

            if (food.fat < 0)
            {
                throw new Exception("Masti ne mogu biti negativne!");
            }
        }
    }
}
