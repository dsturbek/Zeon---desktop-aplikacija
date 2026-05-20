using DataAccessLayer.Repositories;
using EntitiesLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessLogicLayer.Services
{
    public class ClientService
    {
        /// <summary>
        /// Vraća trenerove klijente
        /// vraća List<Client>
        public List<Client> GetClientsForTrainer(int trainerId)
        {
            try
            {
                using (var repo = new ClientRepository())
                {
                    return repo.FindClientsByTrainer(trainerId).ToList();
                }
            }
            catch
            {
                return new List<Client>();
            }
        }
        /// <summary>
        /// Pretraga samo po imenu
        /// </summary>
        public List<Client> SearchClientsByName(int trainerId, string name)
        {
            try
            {
                using (var repo = new ClientRepository())
                {
                    return repo.SearchTrainerClientsByName(trainerId, name).ToList();
                }
            }
            catch
            {
                return new List<Client>();
            }
        }
        /// <summary>
        /// Pretraga svih klijenata (u svrhu dodavanja)
        /// </summary>
        public List<Client> SearchAllClientsToAdd(string search)
        {
            try
            {
                using (var repo = new ClientRepository())
                {
                    return repo.SearchAllClients(search).ToList();
                }
            }
            catch
            {
                return new List<Client>();
            }
        }
        /// <summary>
        /// Detalji klijenta
        /// </summary>
        public Client GetClientDetails(int clientId)
        {
            try
            {
                using (var repo = new ClientRepository())
                {
                    return repo.FindAll().FirstOrDefault(c => c.id_client == clientId);
                }
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// Dodavanje klijenta treneru
        /// </summary>
        public bool AddClientToTrainer(int clientId, int trainerId)
        {
            //try
            //{
                using (var repo = new ClientRepository())
                {
                    var client = repo.FindAll().FirstOrDefault(c => c.id_client == clientId);
                    if (client == null) return false;

                    if (client.TrainerId != null && client.TrainerId != trainerId)
                        return false;

                    var changed = repo.AddClientToTrainer(clientId, trainerId);
                    return changed > 0;
                }
            //}
            //catch
            //{
                //return false;
            //}
        }
        /// <summary>
        /// Makivanje klijenta od trenera
        /// </summary>
        public bool RemoveClientFromTrainer(int trainerId, int clientId)
        {
            try
            {
                using (var repo = new ClientRepository())
                {
                    var changed = repo.RemoveClientFromTrainer(clientId, trainerId);
                    return changed > 0;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
