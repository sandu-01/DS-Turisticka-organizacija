using System;
using System.Collections.Generic;

namespace TurističkaOrganizacija.Domain
{
    public enum PackageType
    {
        Sea,
        Mountain,
        Excursion,
        Cruise
    }

    public class Client
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int PassportNumber { get; set; }
        public string PassportNumberEncrypted { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }

    public abstract class TravelPackage
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public PackageType Type { get; protected set; }

        // Common fields
        public string Destination { get; set; }
        public string AccommodationType { get; set; }
        public string TransportType { get; set; }

        // Specific fields (leave optional)
        public string GuideName { get; set; }
        public int? DurationDays { get; set; }

        public string ShipName { get; set; }
        public string Route { get; set; }
        public DateTime? DepartureDate { get; set; }
        public string CabinType { get; set; }
    }

    public class SeaPackage : TravelPackage
    {
        public SeaPackage() { Type = PackageType.Sea; }
    }

    public class MountainPackage : TravelPackage
    {
        public List<string> Activities { get; set; } = new List<string>();
        public string ActivitiesDisplay
        {
            get
            {
                if (Activities == null || Activities.Count == 0) return string.Empty;
                return string.Join(", ", Activities);
            }
        }
        public MountainPackage() { Type = PackageType.Mountain; }
    }

    public class ExcursionPackage : TravelPackage
    {
        public ExcursionPackage() { Type = PackageType.Excursion; }
    }

    public class CruisePackage : TravelPackage
    {
        public CruisePackage() { Type = PackageType.Cruise; }
    }

    public enum ReservationStatus
    {
        Draft,
        Confirmed,
        Canceled
    }

    public class Reservation
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int PackageId { get; set; }
        public int PassengerCount { get; set; }
        public DateTime ReservedAt { get; set; }
        public ReservationStatus Status { get; set; }
        public decimal TotalPrice { get; set; }
    }
}


