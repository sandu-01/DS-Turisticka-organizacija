using System;
using System.Collections.Generic;
using TurističkaOrganizacija.Domain;

namespace TurističkaOrganizacija.Application
{
    public class ReservationBuilder
    {
        private readonly Reservation _reservation = new Reservation();

        public ReservationBuilder ForClient(int clientId)
        {
            _reservation.ClientId = clientId;
            return this;
        }

        public ReservationBuilder ForPackage(int packageId)
        {
            _reservation.PackageId = packageId;
            return this;
        }

        public ReservationBuilder WithPassengers(int passengerCount)
        {
            _reservation.PassengerCount = passengerCount;
            return this;
        }

        public ReservationBuilder ReservedAt(DateTime at)
        {
            _reservation.ReservedAt = at;
            return this;
        }

        public ReservationBuilder WithStatus(ReservationStatus status)
        {
            _reservation.Status = status;
            return this;
        }

        public ReservationBuilder WithTotal(decimal total)
        {
            _reservation.TotalPrice = total;
            return this;
        }

        public Reservation Build()
        {
            return _reservation;
        }
    }
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

        public void Add(TravelPackage travelPackage)
        {
            _packageRepository.Add(travelPackage);
            TurističkaOrganizacija.Application.EventBus.PublishPackagesChanged();
        }

        public void Update(TravelPackage travelPackage)
        {
            _packageRepository.Update(travelPackage);
            TurističkaOrganizacija.Application.EventBus.PublishPackagesChanged();
        }

        public void Delete(int packageId)
        {
            _packageRepository.Delete(packageId);
            TurističkaOrganizacija.Application.EventBus.PublishPackagesChanged();
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
            var reservation = new ReservationBuilder()
                .ForClient(clientId)
                .ForPackage(travelPackage.Id)
                .WithPassengers(passengerCount)
                .ReservedAt(DateTime.UtcNow)
                .WithStatus(ReservationStatus.Confirmed)
                .WithTotal(totalPrice)
                .Build();
            _reservationRepository.Add(reservation);
            TurističkaOrganizacija.Application.EventBus.PublishReservationsChanged(clientId);
        }

        public void Cancel(int clientId, int packageId)
        {
            _reservationRepository.Delete(clientId, packageId);
            TurističkaOrganizacija.Application.EventBus.PublishReservationsChanged(clientId);
        }

        public void UpdatePassengers(int clientId, int packageId, int passengerCount)
        {
            var reservation = new ReservationBuilder()
                .ForClient(clientId)
                .ForPackage(packageId)
                .WithPassengers(passengerCount)
                .ReservedAt(DateTime.UtcNow)
                .Build();
            _reservationRepository.Update(reservation);
            TurističkaOrganizacija.Application.EventBus.PublishReservationsChanged(clientId);
        }
    }
}


