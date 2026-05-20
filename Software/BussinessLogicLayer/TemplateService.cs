using DataAccessLayer.Repositories;
using EntitiesLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessLogicLayer
{
    public class TemplateService
    {
        TemplateRepository templateRepository = new TemplateRepository();

        public List<Workout_plan_template> GetTemplates(int trainerId)
        {
            return templateRepository.GetTrainerTemplates(trainerId);
        }

        public bool CreateTemplate(Workout_plan_template template)
        {
            if (string.IsNullOrWhiteSpace(template.workout_template_name))
            {
                throw new Exception("Naziv predloška je obavezan!");
            }

            if (template.workout_template_name.Length > 50)
            {
                throw new Exception("Naziv predloška ne smije biti dulji od 50 znakova!");
            }

            return templateRepository.CreateNewTemplate(template);
        }

        public bool UpdateTemplate(Workout_plan_template template)
        {
            if (string.IsNullOrWhiteSpace(template.workout_template_name))
            {
                throw new Exception("Naziv predloška je obavezan!");
            }

            if (template.workout_template_name.Length > 50)
            {
                throw new Exception("Naziv predloška ne smije biti dulji od 50 znakova!");
            }

            int rows = templateRepository.Update(template);
            return rows > 0;
        }

        public void RemoveTemplate(Workout_plan_template template)
        {
            templateRepository.Delete(template);
        }

        public void AddWorkoutToTemplate(int workoutId, int templateId, int day)
        {
            if (day < 1)
            {
                throw new Exception("Dan treninga mora biti veći od 0!");
            }

            templateRepository.AddWorkoutToTemplate(workoutId, templateId, day);
        }

        public void AddExerciseToWorkout(int exerciseId, int workoutId, int? sets, int? reps, decimal? weight)
        {
            if (sets.HasValue && sets.Value <= 0)
            {
                throw new Exception("Broj serija mora biti veći od 0!");
            }

            if (reps.HasValue && reps.Value <= 0)
            {
                throw new Exception("Broj ponavljanja mora biti veći od 0!");
            }

            if (weight.HasValue && weight.Value < 0)
            {
                throw new Exception("Težina ne može biti negativna!");
            }

            templateRepository.AddExerciseToWorkout(exerciseId, workoutId, sets, reps, weight);
        }

        public List<Workout> GetAllWorkouts()
        {
            return templateRepository.GetAllWorkouts();
        }

        public Workout CreateWorkout(Workout workout)
        {
            if (string.IsNullOrWhiteSpace(workout.workout_name))
            {
                throw new Exception("Naziv treninga je obavezan!");
            }

            if (workout.workout_name.Length > 50)
            {
                throw new Exception("Naziv treninga ne smije biti dulji od 50 znakova!");
            }

            return templateRepository.CreateWorkout(workout);
        }

        public List<Exercise_Workout> GetExercisesForWorkout(int workoutId)
        {
            return templateRepository.GetExercisesForWorkout(workoutId);
        }

        public void RemoveExerciseFromWorkout(int exerciseId, int workoutId)
        {
            templateRepository.RemoveExerciseFromWorkout(exerciseId, workoutId);
        }

        public void RemoveWorkoutFromTemplate(int workoutId, int templateId)
        {
            templateRepository.RemoveWorkoutFromTemplate(workoutId, templateId);
        }
    }
}
