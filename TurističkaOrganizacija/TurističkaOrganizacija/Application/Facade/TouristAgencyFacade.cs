using System;
using System.Collections.Generic;
using TurističkaOrganizacija.Domain;
using TurističkaOrganizacija.Infrastructure.Adapters;

namespace TurističkaOrganizacija.Application.Facade
{
    /// <summary>
    /// Facade pattern - provides a simplified interface to complex subsystem
    /// Hides the complexity of multiple services behind a single interface
    /// </summary>
    public class TouristAgencyFacade
    {
        private readonly IDatabaseAdapter _databaseAdapter;
        private readonly ClientService _clientService;
        private readonly PackageService _packageService;
        private readonly ReservationService _reservationService;
        private readonly ISecurityService _securityService;

        public TouristAgencyFacade(
            IDatabaseAdapter databaseAdapter,
            ClientService clientService,
            PackageService packageService,
            ReservationService reservationService,
            ISecurityService securityService)
        {
            _databaseAdapter = databaseAdapter;
            _clientService = clientService;
            _packageService = packageService;
            _reservationService = reservationService;
            _securityService = securityService;
        }

        /// <summary>
        /// Simplified method to get all clients with their basic information
        /// </summary>
        public IEnumerable<Client> GetAllClients()
        {
            return _databaseAdapter.GetClients(null, null, null);
        }

        /// <summary>
        /// Simplified method to get all packages by type
        /// </summary>
        public IEnumerable<TravelPackage> GetPackagesByType(PackageType type)
        {
            return _packageService.GetAll(type);
        }

        /// <summary>
        /// Simplified method to get all available destinations
        /// </summary>
        public IEnumerable<string> GetAllDestinations()
        {
            var packages = _packageService.GetAll();
            var destinations = new HashSet<string>();
            
            foreach (var package in packages)
            {
                if (!string.IsNullOrEmpty(package.Destination))
                {
                    destinations.Add(package.Destination);
                }
            }
            
            return destinations;
        }

        /// <summary>
        /// Simplified method to get all reservations for a specific client
        /// </summary>
        public IEnumerable<Reservation> GetClientReservations(int clientId)
        {
            return _databaseAdapter.GetReservationsByClient(clientId);
        }

        /// <summary>
        /// Simplified method to create a complete booking (client + package + reservation)
        /// </summary>
        public bool CreateCompleteBooking(
            string clientFirstName, 
            string clientLastName, 
            int passportNumber, 
            DateTime dateOfBirth, 
            string email, 
            string phone,
            int packageId, 
            int passengerCount)
        {
            try
            {
                // Create client
                var client = new Client
                {
                    FirstName = clientFirstName,
                    LastName = clientLastName,
                    PassportNumber = passportNumber,
                    DateOfBirth = dateOfBirth,
                    Email = email,
                    Phone = phone
                };

                // Validate and add client
                var passportPlain = passportNumber.ToString();
                _clientService.Create(client, passportPlain);

                // Get package
                var package = _databaseAdapter.GetPackageById(packageId);
                if (package == null)
                {
                    throw new ArgumentException("Package not found");
                }

                // Create reservation
                var reservation = new Reservation
                {
                    ClientId = client.Id,
                    PackageId = packageId,
                    PassengerCount = passengerCount,
                    ReservedAt = DateTime.UtcNow,
                    Status = ReservationStatus.Confirmed,
                    TotalPrice = package.Price * passengerCount
                };

                _databaseAdapter.AddReservation(reservation);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Simplified method to cancel a booking
        /// </summary>
        public bool CancelBooking(int clientId, int packageId)
        {
            try
            {
                _reservationService.Cancel(clientId, packageId);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Simplified method to update booking (change passenger count)
        /// </summary>
        public bool UpdateBooking(int clientId, int packageId, int newPassengerCount)
        {
            try
            {
                var reservations = _databaseAdapter.GetReservationsByClient(clientId);
                foreach (var reservation in reservations)
                {
                    if (reservation.PackageId == packageId)
                    {
                        var package = _databaseAdapter.GetPackageById(packageId);
                        reservation.PassengerCount = newPassengerCount;
                        reservation.TotalPrice = package.Price * newPassengerCount;
                        _databaseAdapter.UpdateReservation(reservation);
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Simplified method to search clients
        /// </summary>
        public IEnumerable<Client> SearchClients(string firstName = null, string lastName = null, string passportLike = null)
        {
            return _databaseAdapter.GetClients(firstName, lastName, passportLike);
        }

        /// <summary>
        /// Simplified method to get client statistics
        /// </summary>
        public Tuple<int, int, int> GetStatistics()
        {
            var clients = _databaseAdapter.GetClients(null, null, null);
            var packages = _packageService.GetAll();
            var totalReservations = 0;

            foreach (var client in clients)
            {
                var clientReservations = _databaseAdapter.GetReservationsByClient(client.Id);
                totalReservations += System.Linq.Enumerable.Count(clientReservations);
            }

            return Tuple.Create(System.Linq.Enumerable.Count(clients), System.Linq.Enumerable.Count(packages), totalReservations);
        }
    }
}
