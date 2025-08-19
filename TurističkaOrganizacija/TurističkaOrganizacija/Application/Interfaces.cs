using System.Collections.Generic;
using TurističkaOrganizacija.Domain;

namespace TurističkaOrganizacija.Application
{
    public interface IClientRepository
    {
        Client GetById(int id);
        IEnumerable<Client> Search(string firstName, string lastName, string passportLike);
        void Add(Client client);
        void Update(Client client);
        void Delete(int id);
        bool ExistsPassport(int passportNumber, int? excludeClientId = null);
    }

    public interface IPackageRepository
    {
        TravelPackage GetById(int id);
        IEnumerable<TravelPackage> GetAll(PackageType? type = null);
        void Add(TravelPackage package);
        void Update(TravelPackage package);
        void Delete(int id);
    }

    public interface IReservationRepository
    {
        IEnumerable<Reservation> GetByClient(int clientId);
        void Add(Reservation reservation);
        void Update(Reservation reservation);
        void Delete(int id);
    }

    public interface ISecurityService
    {
        string Encrypt(string plaintext);
        string Decrypt(string ciphertext);
    }

    public interface IBackupService
    {
        void RunBackup();
    }
}


