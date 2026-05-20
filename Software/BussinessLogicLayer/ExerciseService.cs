using DataAccessLayer.Repositories;
using EntitiesLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessLogicLayer
{
    public class ExerciseService
    {
        private readonly ExerciseRepository _repository = new ExerciseRepository();

        public async Task<List<Exercise>> GetAllExercisesAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Exercise> GetExerciseByIdAsync(int exerciseId)
        {
            if (exerciseId <= 0)
                throw new ArgumentException("ID vježbe nije valjan!");

            return await _repository.GetByIdAsync(exerciseId);
        }

        public async Task<List<Exercise_Workout>> GetExercisesForWorkoutAsync(int workoutId)
        {
            if (workoutId <= 0)
                throw new ArgumentException("ID treninga nije valjan!");

            return await _repository.GetExercisesForWorkoutAsync(workoutId);
        }

        public async Task<bool> AddExerciseToWorkoutAsync(int exerciseId, int workoutId, int? sets, int? reps, decimal? weight)
        {
            if (exerciseId <= 0)
                throw new ArgumentException("ID vježbe nije valjan!");

            if (workoutId <= 0)
                throw new ArgumentException("ID treninga nije valjan!");

            var exerciseWorkout = new Exercise_Workout
            {
                ExerciseId = exerciseId,
                WorkoutId = workoutId,
                sets = sets,
                reps = reps,
                weight = weight
            };

            return await _repository.AddExerciseToWorkoutAsync(exerciseWorkout);
        }

        public async Task<bool> UpdateExerciseWorkoutAsync(int exerciseId, int workoutId, int? sets, int? reps, decimal? weight)
        {
            if (exerciseId <= 0 || workoutId <= 0)
                throw new ArgumentException("Neispravni parametri!");

            var exerciseWorkout = new Exercise_Workout
            {
                ExerciseId = exerciseId,
                WorkoutId = workoutId,
                sets = sets,
                reps = reps,
                weight = weight
            };

            return await _repository.UpdateExerciseWorkoutAsync(exerciseWorkout);
        }

        public async Task<bool> RemoveExerciseFromWorkoutAsync(int exerciseId, int workoutId)
        {
            if (exerciseId <= 0 || workoutId <= 0)
                throw new ArgumentException("Neispravni parametri!");

            return await _repository.RemoveExerciseFromWorkoutAsync(exerciseId, workoutId);
        }
    }
}
