using DataAccessLayer.Repositories;
using EntitiesLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessLogicLayer
{
    public class TrainerService
    {
        TrainerRepository trainerRepository = new TrainerRepository();
        public Trainer TrainerLogin(string username, string password)
        {
            if (username == null)
            {
                throw new Exception("Korisničko ime ne može biti prazano!");
            }
            if (password == null)
            {
                throw new Exception("Lozinka ne može biti prazna!");
            }

            var trainer = trainerRepository.Login(username, password);

            if (trainer == null)
            {
                throw new Exception("Pogrešno korisničko ime ili lozinka");
            }

            return trainer;
        }

        public bool TrainerRegister(Trainer trainer)
        {
            if (string.IsNullOrWhiteSpace(trainer.email))
            {
                throw new Exception("Email je obavezan!");
            }
            if (string.IsNullOrWhiteSpace(trainer.username))
            {
                throw new Exception("Korisničko ime je obavezno!");
            }
            if (string.IsNullOrWhiteSpace(trainer.password))
            {
                throw new Exception("Lozinka je obavezna!");
            }
            if (string.IsNullOrWhiteSpace(trainer.name_surname))
            {
                throw new Exception("Ime i prezime su obavezni!");
            }


            var checkUsername = trainerRepository.CheckUsername(trainer.username);
            int id;
            if (checkUsername == true)
            {
                throw new Exception("Korisničko ime već postoji!");
            }


            id = trainerRepository.Register(trainer);
            if (id > 0)
            {
                return true;
            }
            else
            {
                throw new Exception("Registracija nije uspješna!");
            }
        }
    }
}
