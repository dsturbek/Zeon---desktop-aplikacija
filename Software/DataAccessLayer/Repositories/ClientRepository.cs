using EntitiesLayer.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class ClientRepository : IDisposable
    {
        private readonly ZeonDbContext _dbContext;
        private bool _disposed;

        public ClientRepository()
        {
            _dbContext = new ZeonDbContext();
        }

        ///<summary>
        /// Vraća sve klijente iz baze
        /// </summary>
        public IQueryable<Client> FindAll()
        {
            return _dbContext.Clients;
        }
        ///<summary>
        /// vraća sve klijente određenog trenera
        /// </summary>
        public IQueryable<Client> FindClientsByTrainer(int trainerId)
        {
            return _dbContext.Clients.Where(c => c.TrainerId == trainerId);
        }
        ///<summary>
        /// Pretraga klijenata po imenu i prezimenu
        /// </summary>
        public IQueryable<Client> SearchTrainerClientsByName(int trainerId, string nameSurname)
        {
            nameSurname = (nameSurname ?? string.Empty).Trim();

            var query = _dbContext.Clients.Where(c => c.TrainerId == trainerId);

            if (nameSurname.Length == 0)
                return query;

            var nameSurnameToLower = nameSurname.ToLower();
            return query.Where(c => (c.name_surname ?? string.Empty).ToLower().Contains(nameSurnameToLower));
        }
        ///<summary>
        /// Pretraga svih klijenata
        /// name_surname, username ili email
        /// </summary>
        public IQueryable<Client> SearchAllClients(string search)
        {
            search = (search ?? string.Empty).Trim();

            if (search.Length == 0)
                return _dbContext.Clients;

            var searchToLower = search.ToLower();

            return _dbContext.Clients.Where(c =>
                (c.name_surname ?? string.Empty).ToLower().Contains(searchToLower) ||
                (c.username ?? string.Empty).ToLower().Contains(searchToLower) ||
                (c.email ?? string.Empty).ToLower().Contains(searchToLower)
            );
        }
        ///<summary>
        /// Dodavanje klijenta
        /// Vraća broj promijenjenih redova
        /// </summary>
        public int AddClientToTrainer(int clientId, int trainerId)
        {
            try
            {
                var client = _dbContext.Clients.SingleOrDefault(c => c.id_client == clientId);
                if (client == null) return 0;

                if (client.TrainerId == trainerId)
                    return 0;

                client.TrainerId = trainerId;
                return _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Greška pri dodavanju klijenta treneru: " + ex.Message);
            }
        }
        ///<summary>
        /// Uklanjanje klijenta od trenera
        /// Vraća broj promijenjenih redova
        /// </summary>
        public int RemoveClientFromTrainer(int clientId, int trainerId)
        {
            try
            {
                var client = _dbContext.Clients.SingleOrDefault(c => c.id_client == clientId);
                if (client == null) return 0;

                if (client.TrainerId != trainerId)
                    return 0;

                client.TrainerId = null;
                return _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Greška pri uklanjanju klijenta od trenera: " + ex.Message);
            }
        }

        public int GetClientCountForTrainer(int trainerId)
        {
            using (var context = new ZeonDbContext())
            {
                return context.Clients.Count(c => c.TrainerId == trainerId);
            }
        }

        public async Task<List<Client>> ClientsByTrainerAsync(int trainerId)
        {
            using (var context = new ZeonDbContext())
            {
                var query = from c in context.Clients
                            where c.TrainerId == trainerId
                            select c;
                return await query.ToListAsync();
            }
        }

        public async Task<Client_profile> GetClientProfileAsync(int clientId)
        {
            using (var context = new ZeonDbContext())
            {
                var query = from c in context.Client_profile
                            where c.ClientId == clientId
                            select c;
                return await query.FirstOrDefaultAsync();
            }
        }

        public void Dispose()
        {
            if (_disposed)
                return;
            _dbContext.Dispose();
            _disposed = true;
        }
    }
}
