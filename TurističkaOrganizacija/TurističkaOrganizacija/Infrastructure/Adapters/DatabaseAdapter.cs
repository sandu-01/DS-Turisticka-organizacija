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
            _reservationRepository.Delete(clientId, packageId);
        }
    }

    // Removed SqliteDatabaseAdapter placeholder
}
