using DataAccessLayer.Repositories;
using EntitiesLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessLogicLayer
{
    public class GoalService
    {
        private GoalRepository goalRepository = new GoalRepository();

        public async Task<Goal> GetGoalByIdAsync(int clientId)
        {
            if(clientId == 0)
            {
                throw new Exception("ID Klijenta je 0!");
            }
            return await goalRepository.GetGoalByClientIdAsync(clientId);
        }

        public async Task<bool> SaveGoalAsync(Goal goal)
        {
            if (string.IsNullOrWhiteSpace(goal.goal_name))
            {
                throw new Exception("Naziv cilja je obavezno!");
            }
            if (string.IsNullOrWhiteSpace(goal.goal_description))
            {
                throw new Exception("Opis cilja je obavezan!");
            }
            if (goal.ClientId == 0)
            {
                throw new Exception("ID klijenta je neispravan!");
            }
            if (goal.goal_cal <= 0)
            {
                throw new Exception("Cilj za kalorije mora biti pozitivan broj!");
            }
            if (goal.goal_water <= 0)
            {
                throw new Exception("Cilj za vodu mora biti pozitivan broj!");
            }

            var existingGoal = await goalRepository.GetGoalByClientIdAsync((int)goal.ClientId);

            bool success;

            if (existingGoal != null)
            {
                goal.id_goal = existingGoal.id_goal;

                success = await goalRepository.UpdateGoalAsync(goal);
            }
            else
            {
                success = await goalRepository.AddGoalAsync(goal);
            }

            if (!success)
            {
                throw new Exception("Spremanje cilja nije uspjelo!");
            }

            return true;
        }
    }
}
