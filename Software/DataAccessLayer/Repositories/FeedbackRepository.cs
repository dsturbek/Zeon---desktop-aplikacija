using EntitiesLayer.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class FeedbackRepository
    {
        public async Task<List<Feedback_training>> GetAllTrainingFeedbacksAsync(int trainerId)
        {
            using (var context = new ZeonDbContext())
            {
                var query = from ft in context.Feedback_training
                            .Include(f => f.Client)
                            .Include(f => f.Exercise_Workout.Exercise)
                            .Include(f => f.Exercise_Workout.Workout)
                            where ft.Client.TrainerId == trainerId
                            select ft;
                return await query.ToListAsync();
            }
        }

        public async Task<List<Feedback_training>> GetAllTrainingFeedbackByClientAsnyc(int clientId)
        {
            using (var context = new ZeonDbContext())
            {
                var query=from ft in context.Feedback_training
                          .Include(f=>f.Client)
                          .Include(f=>f.Exercise_Workout)
                          .Include(f=>f.Exercise_Workout.Workout)
                          .Include(f=>f.Exercise_Workout.Exercise)
                          where ft.ClientId==clientId
                          select ft;
                return await query.ToListAsync();
            }
        }

        public async Task<List<Feedback_meal>> GetAllMealFeedbacksAsync(int trainerId)
        {
            using (var context = new ZeonDbContext())
            {
                var query = from fm in context.Feedback_meal
                            .Include(f => f.Planned_meal.Food_plan.Client)
                            .Include(f => f.Planned_meal)
                            where fm.Planned_meal.Food_plan.Client.TrainerId == trainerId
                            select fm;
                return await query.ToListAsync();
            }
        }

        public async Task<List<Feedback_meal>> GetAllMealFeedbackByClientAsnyc(int clientId)
        {
            using (var context = new ZeonDbContext())
            {
                var query = from fm in context.Feedback_meal
                          .Include(f => f.Planned_meal)
                          .Include(f => f.Planned_meal.Food_plan)
                          .Include(f => f.Planned_meal.Food_plan.Client)
                           where fm.Planned_meal.Food_plan.ClientId == clientId
                           select fm;
                return await query.ToListAsync();
            }
        }
    }
}
