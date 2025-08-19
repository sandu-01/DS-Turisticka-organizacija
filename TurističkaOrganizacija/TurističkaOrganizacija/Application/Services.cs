using System;
using System.Collections.Generic;
using TurističkaOrganizacija.Domain;

namespace TurističkaOrganizacija.Application
{
    public class ClientService
    {
        private readonly IClientRepository _clientRepository;
        private readonly ISecurityService _securityService;

        public ClientService(IClientRepository clientRepository, ISecurityService securityService)
        {
            _clientRepository = clientRepository;
            _securityService = securityService;
        }

        public void Create(Client client, string passportPlaintext)
        {
            client.PassportNumberEncrypted = _securityService.Encrypt(passportPlaintext);
            _clientRepository.Add(client);
            TurističkaOrganizacija.Application.EventBus.PublishClientsChanged();
        }

        public void Update(Client client, string passportPlaintext)
        {
            client.PassportNumberEncrypted = _securityService.Encrypt(passportPlaintext);
            _clientRepository.Update(client);
            TurističkaOrganizacija.Application.EventBus.PublishClientsChanged();
        }

        public void Delete(int clientId)
        {
            _clientRepository.Delete(clientId);
            TurističkaOrganizacija.Application.EventBus.PublishClientsChanged();
        }

        public IEnumerable<Client> Search(string firstName, string lastName, string passportLike)
        {
            return _clientRepository.Search(firstName, lastName, passportLike);
        }
    }

    public class PackageService
    {
        private readonly IPackageRepository _packageRepository;

        public PackageService(IPackageRepository packageRepository)
        {
            _packageRepository = packageRepository;
        }

        public IEnumerable<TravelPackage> GetAll(PackageType? type = null)
        {
            return _packageRepository.GetAll(type);
        }
    }

    public class ReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IPackageRepository _packageRepository;

        public ReservationService(IReservationRepository reservationRepository, IPackageRepository packageRepository)
        {
            _reservationRepository = reservationRepository;
            _packageRepository = packageRepository;
        }

        public void Reserve(int clientId, TravelPackage travelPackage, int passengerCount, decimal totalPrice)
        {
            var reservation = new Reservation
            {
                ClientId = clientId,
                PackageId = travelPackage.Id,
                PassengerCount = passengerCount,
                ReservedAt = DateTime.UtcNow,
                Status = ReservationStatus.Confirmed,
                TotalPrice = totalPrice
            };
            _reservationRepository.Add(reservation);
            TurističkaOrganizacija.Application.EventBus.PublishReservationsChanged(clientId);
        }

        public void Cancel(int clientId, int packageId)
        {
            if (_reservationRepository is TurističkaOrganizacija.Infrastructure.Repositories.SqlClient.ReservationRepositorySql repo)
            {
                repo.Delete(clientId, packageId);
                TurističkaOrganizacija.Application.EventBus.PublishReservationsChanged(clientId);
            }
        }
    }
}


