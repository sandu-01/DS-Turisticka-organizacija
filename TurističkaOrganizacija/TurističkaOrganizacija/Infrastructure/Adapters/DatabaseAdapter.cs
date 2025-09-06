using System;
using System.Collections.Generic;
using System.Data.Common;
using TurističkaOrganizacija.Application;
using TurističkaOrganizacija.Domain;

namespace TurističkaOrganizacija.Infrastructure.Adapters
{
    /// <summary>
    /// Adapter pattern for different database providers
    /// Allows switching between SQL Server, SQLite, etc. without changing business logic
    /// </summary>
    public interface IDatabaseAdapter
    {
        IEnumerable<Client> GetClients(string firstName, string lastName, string passportLike);
        Client GetClientById(int id);
        void AddClient(Client client);
        void UpdateClient(Client client);
        void DeleteClient(int id);
        bool ClientPassportExists(int passportNumber, int? excludeClientId = null);
        
        IEnumerable<TravelPackage> GetPackages(PackageType? type = null);
        TravelPackage GetPackageById(int id);
        void AddPackage(TravelPackage package);
        void UpdatePackage(TravelPackage package);
        void DeletePackage(int id);
        
        IEnumerable<Reservation> GetReservationsByClient(int clientId);
        void AddReservation(Reservation reservation);
        void UpdateReservation(Reservation reservation);
        void DeleteReservation(int clientId, int packageId);
    }

    /// <summary>
    /// SQL Server implementation of database adapter
    /// </summary>
    public class SqlServerDatabaseAdapter : IDatabaseAdapter
    {
        private readonly IClientRepository _clientRepository;
        private readonly IPackageRepository _packageRepository;
        private readonly IReservationRepository _reservationRepository;

        public SqlServerDatabaseAdapter(
            IClientRepository clientRepository,
            IPackageRepository packageRepository,
            IReservationRepository reservationRepository)
        {
            _clientRepository = clientRepository;
            _packageRepository = packageRepository;
            _reservationRepository = reservationRepository;
        }

        public IEnumerable<Client> GetClients(string firstName, string lastName, string passportLike)
        {
            return _clientRepository.Search(firstName, lastName, passportLike);
        }

        public Client GetClientById(int id)
        {
            return _clientRepository.GetById(id);
        }

        public void AddClient(Client client)
        {
            _clientRepository.Add(client);
        }

        public void UpdateClient(Client client)
        {
            _clientRepository.Update(client);
        }

        public void DeleteClient(int id)
        {
            _clientRepository.Delete(id);
        }

        public bool ClientPassportExists(int passportNumber, int? excludeClientId = null)
        {
            return _clientRepository.ExistsPassport(passportNumber, excludeClientId);
        }

        public IEnumerable<TravelPackage> GetPackages(PackageType? type = null)
        {
            return _packageRepository.GetAll(type);
        }

        public TravelPackage GetPackageById(int id)
        {
            return _packageRepository.GetById(id);
        }

        public void AddPackage(TravelPackage package)
        {
            _packageRepository.Add(package);
        }

        public void UpdatePackage(TravelPackage package)
        {
            _packageRepository.Update(package);
        }

        public void DeletePackage(int id)
        {
            _packageRepository.Delete(id);
        }

        public IEnumerable<Reservation> GetReservationsByClient(int clientId)
        {
            return _reservationRepository.GetByClient(clientId);
        }

        public void AddReservation(Reservation reservation)
        {
            _reservationRepository.Add(reservation);
        }

        public void UpdateReservation(Reservation reservation)
        {
            _reservationRepository.Update(reservation);
        }

        public void DeleteReservation(int clientId, int packageId)
        {
            _reservationRepository.Delete(clientId);
        }
    }

    /// <summary>
    /// Future SQLite implementation - placeholder for demonstration
    /// </summary>
    public class SqliteDatabaseAdapter : IDatabaseAdapter
    {
        // This would implement the same interface but use SQLite instead of SQL Server
        // Currently just throws NotImplementedException to show the pattern
        
        public IEnumerable<Client> GetClients(string firstName, string lastName, string passportLike)
        {
            throw new NotImplementedException("SQLite adapter not yet implemented");
        }

        public Client GetClientById(int id)
        {
            throw new NotImplementedException("SQLite adapter not yet implemented");
        }

        public void AddClient(Client client)
        {
            throw new NotImplementedException("SQLite adapter not yet implemented");
        }

        public void UpdateClient(Client client)
        {
            throw new NotImplementedException("SQLite adapter not yet implemented");
        }

        public void DeleteClient(int id)
        {
            throw new NotImplementedException("SQLite adapter not yet implemented");
        }

        public bool ClientPassportExists(int passportNumber, int? excludeClientId = null)
        {
            throw new NotImplementedException("SQLite adapter not yet implemented");
        }

        public IEnumerable<TravelPackage> GetPackages(PackageType? type = null)
        {
            throw new NotImplementedException("SQLite adapter not yet implemented");
        }

        public TravelPackage GetPackageById(int id)
        {
            throw new NotImplementedException("SQLite adapter not yet implemented");
        }

        public void AddPackage(TravelPackage package)
        {
            throw new NotImplementedException("SQLite adapter not yet implemented");
        }

        public void UpdatePackage(TravelPackage package)
        {
            throw new NotImplementedException("SQLite adapter not yet implemented");
        }

        public void DeletePackage(int id)
        {
            throw new NotImplementedException("SQLite adapter not yet implemented");
        }

        public IEnumerable<Reservation> GetReservationsByClient(int clientId)
        {
            throw new NotImplementedException("SQLite adapter not yet implemented");
        }

        public void AddReservation(Reservation reservation)
        {
            throw new NotImplementedException("SQLite adapter not yet implemented");
        }

        public void UpdateReservation(Reservation reservation)
        {
            throw new NotImplementedException("SQLite adapter not yet implemented");
        }

        public void DeleteReservation(int clientId, int packageId)
        {
            throw new NotImplementedException("SQLite adapter not yet implemented");
        }
    }
}
