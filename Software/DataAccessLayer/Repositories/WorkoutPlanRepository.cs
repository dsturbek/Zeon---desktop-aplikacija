using EntitiesLayer.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class WorkoutPlanRepository
    {
        public async Task<Workout_plan_assigned> GetWorkoutPlanByClientIdAsync(int clientId)
        {
            using (var context = new ZeonDbContext())
            {
                return await context.Workout_plan_assigned
                    .Include(wp => wp.Workout_plan_assigned_Workout.Select(wpw => wpw.Workout.Exercise_Workout.Select(ew => ew.Exercise)))
                    .Where(wp => wp.ClientId == clientId)
                    .OrderByDescending(wp => wp.date_start)
                    .FirstOrDefaultAsync();
            }
        }

        public async Task<List<Workout_plan_assigned>> GetAllWorkoutPlansForClientAsync(int clientId)
        {
            using (var context = new ZeonDbContext())
            {
                return await context.Workout_plan_assigned
                    .Where(wp => wp.ClientId == clientId)
                    .OrderByDescending(wp => wp.date_start)
                    .ToListAsync();
            }
        }

        public async Task<int> AddWorkoutPlanAsync(Workout_plan_assigned plan)
        {
            using (var context = new ZeonDbContext())
            {
                context.Workout_plan_assigned.Add(plan);
                await context.SaveChangesAsync();
                return plan.id_workout_assigned;
            }
        }

        public async Task<bool> UpdateWorkoutPlanAsync(Workout_plan_assigned plan)
        {
            using (var context = new ZeonDbContext())
            {
                var existing = await context.Workout_plan_assigned.FindAsync(plan.id_workout_assigned);
                if (existing == null)
                    return false;

                existing.workout_assigned_name = plan.workout_assigned_name;
                existing.date_start = plan.date_start;
                existing.date_end = plan.date_end;

                await context.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> DeleteWorkoutPlanAsync(int planId)
        {
            using (var context = new ZeonDbContext())
            {
                var workoutsInPlan = await context.Workout_plan_assigned_Workout
                    .Where(w => w.Workout_plan_assignedId == planId)
                    .ToListAsync();

                context.Workout_plan_assigned_Workout.RemoveRange(workoutsInPlan);

                var plan = await context.Workout_plan_assigned.FindAsync(planId);
                if (plan == null)
                    return false;

                context.Workout_plan_assigned.Remove(plan);
                await context.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> AddWorkoutToPlanAsync(Workout_plan_assigned_Workout planWorkout)
        {
            using (var context = new ZeonDbContext())
            {
                context.Workout_plan_assigned_Workout.Add(planWorkout);
                await context.SaveChangesAsync();
                return true;
            }
        }

        public async Task<int> ImportFromTemplateAsync(int templateId, int clientId, string planName, DateTime dateStart, DateTime? dateEnd = null)
        {
            using (var context = new ZeonDbContext())
            {
                var templateWorkouts = await context.Workout_Workout_plan_template
                    .Where(ww => ww.Workout_plan_templateId == templateId)
                    .ToListAsync();

                var plan = new Workout_plan_assigned
                {
                    workout_assigned_name = planName,
                    ClientId = clientId,
                    Workout_plan_templateId = templateId,
                    date_start = dateStart,
                    date_end = dateEnd
                };

                context.Workout_plan_assigned.Add(plan);
                await context.SaveChangesAsync();

                foreach (var tw in templateWorkouts)
                {
                    var assignedWorkout = new Workout_plan_assigned_Workout
                    {
                        Workout_plan_assignedId = plan.id_workout_assigned,
                        WorkoutId = tw.WorkoutId,
                        workout_plan_day = tw.workout_plan_day
                    };
                    context.Workout_plan_assigned_Workout.Add(assignedWorkout);
                }

                await context.SaveChangesAsync();
                return plan.id_workout_assigned;
            }
        }

        public async Task<bool> RemoveWorkoutFromPlanAsync(int planId, int workoutId, int day)
        {
            using (var context = new ZeonDbContext())
            {
                var item = await context.Workout_plan_assigned_Workout
                    .FirstOrDefaultAsync(w => w.Workout_plan_assignedId == planId
                                           && w.WorkoutId == workoutId
                                           && w.workout_plan_day == day);
                if (item == null)
                    return false;

                context.Workout_plan_assigned_Workout.Remove(item);
                await context.SaveChangesAsync();
                return true;
            }
        }
    }
}
