using EntitiesLayer.Entities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesLayer.Entities;

namespace DataAccessLayer.Repositories
{
    public class ExerciseRepository
    {
        public async Task<List<Exercise>> GetAllAsync()
        {
            using (var context = new ZeonDbContext())
            {
                return await context.Exercises.ToListAsync();
            }
        }

        public async Task<Exercise> GetByIdAsync(int exerciseId)
        {
            using (var context = new ZeonDbContext())
            {
                return await context.Exercises.FindAsync(exerciseId);
            }
        }

        public async Task<List<Exercise_Workout>> GetExercisesForWorkoutAsync(int workoutId)
        {
            using (var context = new ZeonDbContext())
            {
                return await context.Exercise_Workout
                    .Include(ew => ew.Exercise)
                    .Where(ew => ew.WorkoutId == workoutId)
                    .ToListAsync();
            }
        }

        public async Task<bool> AddExerciseToWorkoutAsync(Exercise_Workout exerciseWorkout)
        {
            using (var context = new ZeonDbContext())
            {
                context.Exercise_Workout.Add(exerciseWorkout);
                await context.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> UpdateExerciseWorkoutAsync(Exercise_Workout exerciseWorkout)
        {
            using (var context = new ZeonDbContext())
            {
                var existing = await context.Exercise_Workout
                    .FirstOrDefaultAsync(ew => ew.ExerciseId == exerciseWorkout.ExerciseId
                                            && ew.WorkoutId == exerciseWorkout.WorkoutId);
                if (existing == null)
                    return false;

                existing.sets = exerciseWorkout.sets;
                existing.reps = exerciseWorkout.reps;
                existing.weight = exerciseWorkout.weight;

                await context.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> RemoveExerciseFromWorkoutAsync(int exerciseId, int workoutId)
        {
            using (var context = new ZeonDbContext())
            {
                var item = await context.Exercise_Workout
                    .FirstOrDefaultAsync(ew => ew.ExerciseId == exerciseId && ew.WorkoutId == workoutId);
                if (item == null)
                    return false;

                context.Exercise_Workout.Remove(item);
                await context.SaveChangesAsync();
                return true;
            }
        }
    }
}
