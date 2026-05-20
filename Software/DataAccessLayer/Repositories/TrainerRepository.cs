using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesLayer.Entities;

namespace DataAccessLayer.Repositories
{
    public class TrainerRepository
    {
        public Trainer Login(string username, string password)
        {
            using (var context = new ZeonDbContext())
            {
                var query = from t in context.Trainers
                            where t.username == username && t.password == password
                            select t;
                return query.FirstOrDefault();
            }
        }

        public int Register(Trainer trainer)
        {
            using (var context = new ZeonDbContext())
            {
                context.Trainers.Add(trainer);
                context.SaveChanges();
                return trainer.id_trainer;
            }
        }
        public bool CheckUsername(string username)
        {
            using (var context = new ZeonDbContext())
            {
                return context.Trainers.Any(t => t.username == username);
            }
        }

        public Trainer GetTrainerById(int trainerId)
        {
            using (var context = new ZeonDbContext())
            {
                return context.Trainers
                    .Include(t => t.Trainer_profile)
                    .FirstOrDefault(t => t.id_trainer == trainerId);
            }
        }

        public bool UpdateTriner(Trainer trainer)
        {
            using (var context = new ZeonDbContext())
            {
                var existing = context.Trainers
                    .Include(t => t.Trainer_profile)
                    .FirstOrDefault(t => t.id_trainer == trainer.id_trainer);

                if (existing == null) return false;

                existing.name_surname = trainer.name_surname;
                existing.email = trainer.email;
                existing.specialization = trainer.specialization;
                existing.employment_date = trainer.employment_date;
                if(existing.password != trainer.password)
                {
                    existing.password = trainer.password;
                }

                var existingProfile = existing.Trainer_profile.FirstOrDefault();
                var newProfile = trainer.Trainer_profile.FirstOrDefault();

                if (existingProfile != null && newProfile != null)
                {
                    existingProfile.description = newProfile.description;
                    existingProfile.price = newProfile.price;
                    existingProfile.rating = newProfile.rating;
                    existingProfile.number_clients = newProfile.number_clients;
                }
                else if (existingProfile == null && newProfile != null)
                {
                    newProfile.TrainerId = trainer.id_trainer;
                    context.Trainer_profile.Add(newProfile);
                }

                context.SaveChanges();
                return true;
            }
        }

        public bool CheckEmail(string email)
        {
            using (var context = new ZeonDbContext())
            {
                return context.Trainers.Any(t => t.email == email);
            }
        }

        public void CreateTrainer(Trainer trainer)
        {
            using (var context = new ZeonDbContext())
            {
                context.Trainers.Add(trainer);
                context.SaveChanges();

                var profile = new Trainer_profile
                {
                    TrainerId = trainer.id_trainer
                };
                context.Trainer_profile.Add(profile);
                context.SaveChanges();
            }
        }
    }
}
