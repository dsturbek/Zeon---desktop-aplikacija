using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesLayer.Entities;

namespace DataAccessLayer.Repositories
{
    public class TemplateRepository
    {
        public List<Workout_plan_template> GetTrainerTemplates(int trainerId)
        {
            using (var context = new ZeonDbContext())
            {
                return context.Workout_plan_template
                    .Include(t => t.Workout_Workout_plan_template.Select(ww => ww.Workout))
                    .Where(t => t.TrainerId == trainerId)
                    .OrderBy(t => t.workout_template_name)
                    .ToList();
            }
        }

        public bool CreateNewTemplate(Workout_plan_template template)
        {
            using (var context = new ZeonDbContext())
            {
                context.Workout_plan_template.Add(template);
                int rows = context.SaveChanges();
                return rows > 0;
            }
        }

        public void AddWorkoutToTemplate(int workoutId, int templateId, int day)
        {
            using (var context = new ZeonDbContext())
            {
                var entry = new Workout_Workout_plan_template
                {
                    WorkoutId = workoutId,
                    Workout_plan_templateId = templateId,
                    workout_plan_day = day
                };
                context.Workout_Workout_plan_template.Add(entry);
                context.SaveChanges();
            }
        }

        public void AddExerciseToWorkout(int exerciseId, int workoutId, int? sets, int? reps, decimal? weight)
        {
            using (var context = new ZeonDbContext())
            {
                var entry = new Exercise_Workout
                {
                    ExerciseId = exerciseId,
                    WorkoutId = workoutId,
                    sets = sets,
                    reps = reps,
                    weight = weight
                };
                context.Exercise_Workout.Add(entry);
                context.SaveChanges();
            }
        }

        public int Delete(Workout_plan_template template)
        {
            using (var context = new ZeonDbContext())
            {
                var existing = context.Workout_plan_template
                    .Include(t => t.Workout_Workout_plan_template)
                    .FirstOrDefault(t => t.id_workout_template == template.id_workout_template);

                if (existing == null) return 0;

                context.Workout_Workout_plan_template.RemoveRange(existing.Workout_Workout_plan_template);
                context.Workout_plan_template.Remove(existing);
                return context.SaveChanges();
            }
        }

        public int Update(Workout_plan_template template)
        {
            using (var context = new ZeonDbContext())
            {
                var existing = context.Workout_plan_template
                    .FirstOrDefault(t => t.id_workout_template == template.id_workout_template);

                if (existing == null) return 0;

                existing.workout_template_name = template.workout_template_name;
                return context.SaveChanges();
            }
        }

        public List<Workout> GetAllWorkouts()
        {
            using (var context = new ZeonDbContext())
            {
                return context.Workouts
                    .OrderBy(w => w.workout_name)
                    .ToList();
            }
        }

        public Workout CreateWorkout(Workout workout)
        {
            using (var context = new ZeonDbContext())
            {
                context.Workouts.Add(workout);
                context.SaveChanges();
                return workout;
            }
        }

        public List<Exercise_Workout> GetExercisesForWorkout(int workoutId)
        {
            using (var context = new ZeonDbContext())
            {
                return context.Exercise_Workout
                    .Include(ew => ew.Exercise)
                    .Where(ew => ew.WorkoutId == workoutId)
                    .ToList();
            }
        }

        public void RemoveExerciseFromWorkout(int exerciseId, int workoutId)
        {
            using (var context = new ZeonDbContext())
            {
                var entry = context.Exercise_Workout
                    .FirstOrDefault(ew => ew.ExerciseId == exerciseId && ew.WorkoutId == workoutId);

                if (entry != null)
                {
                    context.Exercise_Workout.Remove(entry);
                    context.SaveChanges();
                }
            }
        }

        public void RemoveWorkoutFromTemplate(int workoutId, int templateId)
        {
            using (var context = new ZeonDbContext())
            {
                var entries = context.Workout_Workout_plan_template
                    .Where(ww => ww.WorkoutId == workoutId && ww.Workout_plan_templateId == templateId)
                    .ToList();

                context.Workout_Workout_plan_template.RemoveRange(entries);
                context.SaveChanges();
            }
        }
    }
}
