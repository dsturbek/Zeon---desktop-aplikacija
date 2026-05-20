using EntitiesLayer.Entities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class WorkoutRepository
    {
        public async Task<List<Workout>> GetAllAsync()
        {
            using (var context = new ZeonDbContext())
            {
                return await context.Workouts
                    .Include(w => w.Exercise_Workout.Select(ew => ew.Exercise))
                    .ToListAsync();
            }
        }

        public async Task<Workout> GetByIdAsync(int workoutId)
        {
            using (var context = new ZeonDbContext())
            {
                return await context.Workouts
                    .Include(w => w.Exercise_Workout.Select(ew => ew.Exercise))
                    .FirstOrDefaultAsync(w => w.id_workout == workoutId);
            }
        }

        public async Task<int> AddAsync(Workout workout)
        {
            using (var context = new ZeonDbContext())
            {
                context.Workouts.Add(workout);
                await context.SaveChangesAsync();
                return workout.id_workout;
            }
        }

        public async Task<bool> UpdateAsync(Workout workout)
        {
            using (var context = new ZeonDbContext())
            {
                var existing = await context.Workouts.FindAsync(workout.id_workout);
                if (existing == null)
                    return false;

                existing.workout_name = workout.workout_name;
                existing.muscle_group = workout.muscle_group;

                await context.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> DeleteAsync(int workoutId)
        {
            using (var context = new ZeonDbContext())
            {
                var exercisesInWorkout = await context.Exercise_Workout
                    .Where(ew => ew.WorkoutId == workoutId)
                    .ToListAsync();
                context.Exercise_Workout.RemoveRange(exercisesInWorkout);

                var planWorkouts = await context.Workout_plan_assigned_Workout
                    .Where(pw => pw.WorkoutId == workoutId)
                    .ToListAsync();
                context.Workout_plan_assigned_Workout.RemoveRange(planWorkouts);

                var workout = await context.Workouts.FindAsync(workoutId);
                if (workout == null)
                    return false;

                context.Workouts.Remove(workout);
                await context.SaveChangesAsync();
                return true;
            }
        }
    }
}
