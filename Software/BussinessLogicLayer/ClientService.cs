using DataAccessLayer.Repositories;
using EntitiesLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessLogicLayer
{
    public class ClientService
    {
        private ClientRepository clientRepository;
        
        public ClientService()
        {
            clientRepository = new ClientRepository();
        }

        public async Task<List<Client>> GetClientsByTrainerAsync(int trainerId)
        {
            if(trainerId <= 0)
            {
                throw new Exception("ID trenera je neispravan!");
            }

            return await clientRepository.ClientsByTrainerAsync(trainerId);
        }

        public async Task<Client_profile> GetClientProfileAsync(int clientId)
        {
            if (clientId <= 0)
            {
                throw new Exception("ID klijenta nije ispravan!");
            }
            var client_profile = await clientRepository.GetClientProfileAsync(clientId);

            if (client_profile == null)
            {
                throw new Exception("Odabrani klijent ne postoji!");
            }
            return client_profile;
        }
    }
}
