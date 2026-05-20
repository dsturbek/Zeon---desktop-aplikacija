using DataAccessLayer.Repositories;
using EntitiesLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BussinessLogicLayer
{
    public class ProfileService
    {
        TrainerRepository trainerRepository = new TrainerRepository();
        ClientRepository clientRepository = new ClientRepository();

        public int GetClientCount(int trainerId)
        {
            return clientRepository.GetClientCountForTrainer(trainerId);
        }

        public Trainer GetTrainerProfile(int trainerId)
        {
            var trainer = trainerRepository.GetTrainerById(trainerId);

            if (trainer == null)
            {
                throw new Exception("Trener nije pronađen!");
            }

            return trainer;
        }

        public bool UpdateProfile(Trainer trainer)
        {
            if (string.IsNullOrWhiteSpace(trainer.name_surname))
            {
                throw new Exception("Ime i prezime su obavezni!");
            }

            if (string.IsNullOrWhiteSpace(trainer.email))
            {
                throw new Exception("Email je obavezan!");
            }

            if (!Regex.IsMatch(trainer.email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                throw new Exception("Email adresa nije ispravna!");
            }

            if (trainer.specialization != null && trainer.specialization.Length > 50)
            {
                throw new Exception("Specijalizacija ne smije biti duža od 50 znakova!");
            }

            var profile = trainer.Trainer_profile?.FirstOrDefault();
            if (profile != null && profile.description != null && profile.description.Length > 255)
            {
                throw new Exception("Opis ne smije biti dulji od 255 znakova!");
            }

            return trainerRepository.UpdateTriner(trainer);
        }

        public void CreateProfile()
        {
            trainerRepository.CreateTrainer(new Trainer());
        }

        public bool ChangePassword(int trainerId, string oldPass, string newPass, string newPassCheck)
        {
            var trainer = trainerRepository.GetTrainerById(trainerId);
            if (string.IsNullOrWhiteSpace(oldPass))
            {
                throw new Exception("Stara lozinka je obavezna!");
            }

            if (string.IsNullOrWhiteSpace(newPass))
            {
                throw new Exception("Nova lozinka je obavezna!");
            }

            if (trainer.password != oldPass)
            {
                throw new Exception("Stara lozinka nije ispravna!");
            }

            if (newPass != newPassCheck)
            {
                throw new Exception("Nove lozinke se ne podudaraju!");
            }

            if (newPass.Length < 6)
            {
                throw new Exception("Nova lozinka mora imati najmanje 6 znakova!");
            }

            if (trainer == null)
            {
                throw new Exception("Trener nije pronađen!");
            }

            trainer.password = newPass;
            return trainerRepository.UpdateTriner(trainer);
        }

        public bool EmailExists(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            return trainerRepository.CheckEmail(email);
        }
    }
}
