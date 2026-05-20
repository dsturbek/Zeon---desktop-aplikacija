using EntitiesLayer.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class ReportRepository
    {
        public async Task<List<Client>> FindAllByTrainerId(int trainerId)
        {
            using (var context=new ZeonDbContext())
            {
                var query=from c in context.Clients
                          where c.TrainerId == trainerId
                          select c;
                return await query.ToListAsync();
            }
        }

        public async Task<List<Client_measurement>> GetMeasurements(int clientId, DateTime fromDate, DateTime to)
        {
            using (var context = new ZeonDbContext())
            {
                var profileId = (from p in context.Client_profile
                            where p.ClientId == clientId
                            select p.id_client_profile).FirstOrDefault();

                var query = from m in context.Client_measurement
                            where m.Client_profileId == profileId
                            && m.measurement_date >= fromDate
                            && m.measurement_date <= to
                            orderby m.measurement_date
                            select m;

                return await query.ToListAsync();
            }
        }

        public async Task<List<Personal_record>> GetPersonalRecords(int clientId)
        {
            using (var context = new ZeonDbContext())
            {
                var profileId = (from p in context.Client_profile
                                 where p.ClientId == clientId
                                 select p.id_client_profile).FirstOrDefault();

                var query = from pr in context.Personal_record
                                .Include(pr => pr.Exercise)
                            where pr.Client_profileId == profileId
                            orderby pr.date_achievement descending
                            select pr;
                return await query.ToListAsync();
            }
        }

        public async Task<List<Feedback_training>> GetTrainingData(int clientId, DateTime from, DateTime to)
        {
            using (var context = new ZeonDbContext())
            {
                var query = from ft in context.Feedback_training
                                .Include(f => f.Client)
                                .Include(f => f.Exercise_Workout)
                                .Include(f => f.Exercise_Workout.Workout)
                                .Include(f => f.Exercise_Workout.Exercise)
                            where ft.ClientId == clientId
                            select ft;
                return await query.ToListAsync();
            }
        }
    }
}
