using EntitiesLayer.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class GoalRepository
    {
        public async Task<Goal> GetGoalByClientIdAsync(int clientId)
        {
            using (var context = new ZeonDbContext())
            {
                var query = from g in context.Goals
                            where g.ClientId == clientId
                            select g;
                return await query.FirstOrDefaultAsync();
            }
        }

        public async Task<bool> AddGoalAsync(Goal goal)
        {
            using (var context = new ZeonDbContext())
            {
                context.Goals.Add(goal);
                await context.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> UpdateGoalAsync(Goal goal)
        {
            using (var context = new ZeonDbContext())
            {
                context.Goals.Attach(goal);
                context.Entry(goal).State = EntityState.Modified;

                await context.SaveChangesAsync();
                return true;
            }
        }
    }
}
