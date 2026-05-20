using DataAccessLayer.Repositories;
using EntitiesLayer.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BussinessLogicLayer
{
    public class WorkoutPlanService
    {
        private readonly WorkoutPlanRepository _repository = new WorkoutPlanRepository();

        public async Task<Workout_plan_assigned> GetWorkoutPlanForClientAsync(int clientId)
        {
            if (clientId <= 0)
                throw new ArgumentException("ID klijenta nije valjan!");

            return await _repository.GetWorkoutPlanByClientIdAsync(clientId);
        }

        public async Task<List<Workout_plan_assigned>> GetAllWorkoutPlansForClientAsync(int clientId)
        {
            if (clientId <= 0)
                throw new ArgumentException("ID klijenta nije valjan!");

            return await _repository.GetAllWorkoutPlansForClientAsync(clientId);
        }

        public async Task<int> CreateWorkoutPlanAsync(Workout_plan_assigned plan)
        {
            if (string.IsNullOrWhiteSpace(plan.workout_assigned_name))
                throw new ArgumentException("Naziv plana je obavezan!");

            if (plan.ClientId == null || plan.ClientId <= 0)
                throw new ArgumentException("Klijent mora biti odabran!");

            if (plan.date_start == null)
                throw new ArgumentException("Datum početka je obavezan!");

            if (plan.date_end != null && plan.date_end < plan.date_start)
                throw new ArgumentException("Datum završetka ne može biti prije datuma početka!");

            return await _repository.AddWorkoutPlanAsync(plan);
        }

        public async Task<bool> UpdateWorkoutPlanAsync(Workout_plan_assigned plan)
        {
            if (string.IsNullOrWhiteSpace(plan.workout_assigned_name))
                throw new ArgumentException("Naziv plana je obavezan!");

            if (plan.date_end != null && plan.date_end < plan.date_start)
                throw new ArgumentException("Datum završetka ne može biti prije datuma početka!");

            return await _repository.UpdateWorkoutPlanAsync(plan);
        }

        public async Task<bool> DeleteWorkoutPlanAsync(int planId)
        {
            if (planId <= 0)
                throw new ArgumentException("Plan nije valjan!");

            return await _repository.DeleteWorkoutPlanAsync(planId);
        }

        public async Task<bool> AddWorkoutToPlanAsync(int planId, int workoutId, int day)
        {
            if (planId <= 0)
                throw new ArgumentException("Plan nije valjan!");

            if (workoutId <= 0)
                throw new ArgumentException("Trening nije valjan!");

            if (day <= 0)
                throw new ArgumentException("Dan mora biti pozitivan broj!");

            var planWorkout = new Workout_plan_assigned_Workout
            {
                Workout_plan_assignedId = planId,
                WorkoutId = workoutId,
                workout_plan_day = day
            };

            return await _repository.AddWorkoutToPlanAsync(planWorkout);
        }

        public async Task<int> ImportFromTemplateAsync(int templateId, int clientId, string planName, DateTime dateStart, DateTime? dateEnd = null)
        {
            if (templateId <= 0)
                throw new ArgumentException("Predložak nije valjan!");

            if (clientId <= 0)
                throw new ArgumentException("Klijent nije valjan!");

            if (string.IsNullOrWhiteSpace(planName))
                throw new ArgumentException("Naziv plana je obavezan!");

            if (dateEnd.HasValue && dateEnd.Value < dateStart)
                throw new ArgumentException("Datum završetka ne može biti prije datuma početka!");

            return await _repository.ImportFromTemplateAsync(templateId, clientId, planName, dateStart, dateEnd);
        }

        public async Task<bool> RemoveWorkoutFromPlanAsync(int planId, int workoutId, int day)
        {
            if (planId <= 0 || workoutId <= 0 || day <= 0)
                throw new ArgumentException("Neispravni parametri!");

            return await _repository.RemoveWorkoutFromPlanAsync(planId, workoutId, day);
        }
    }
}
