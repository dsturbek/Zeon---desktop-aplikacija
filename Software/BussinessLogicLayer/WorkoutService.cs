using DataAccessLayer.Repositories;
using EntitiesLayer.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BussinessLogicLayer
{
    public class WorkoutService
    {
        private readonly WorkoutRepository _repository = new WorkoutRepository();

        public async Task<List<Workout>> GetAllWorkoutsAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Workout> GetWorkoutByIdAsync(int workoutId)
        {
            if (workoutId <= 0)
                throw new ArgumentException("ID treninga nije valjan!");

            return await _repository.GetByIdAsync(workoutId);
        }

        public async Task<int> CreateWorkoutAsync(Workout workout)
        {
            if (string.IsNullOrWhiteSpace(workout.workout_name))
                throw new ArgumentException("Naziv treninga je obavezan!");

            return await _repository.AddAsync(workout);
        }

        public async Task<bool> UpdateWorkoutAsync(Workout workout)
        {
            if (string.IsNullOrWhiteSpace(workout.workout_name))
                throw new ArgumentException("Naziv treninga je obavezan!");

            return await _repository.UpdateAsync(workout);
        }

        public async Task<bool> DeleteWorkoutAsync(int workoutId)
        {
            if (workoutId <= 0)
                throw new ArgumentException("ID treninga nije valjan!");

            return await _repository.DeleteAsync(workoutId);
        }
    }
}
