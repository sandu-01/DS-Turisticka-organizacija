using System;
using TurističkaOrganizacija.Domain;

namespace TurističkaOrganizacija.Application
{
    /// <summary>
    /// Factory Method pattern for creating different types of travel packages
    /// </summary>
    public abstract class PackageFactory
    {
        public abstract TravelPackage CreatePackage(string name, decimal price, string destination, string transportType, string accommodationType);
        
        public static PackageFactory GetFactory(PackageType type)
        {
            switch (type)
            {
                case PackageType.Sea:
                    return new SeaPackageFactory();
                case PackageType.Mountain:
                    return new MountainPackageFactory();
                case PackageType.Excursion:
                    return new ExcursionPackageFactory();
                case PackageType.Cruise:
                    return new CruisePackageFactory();
                default:
                    throw new ArgumentException($"Unknown package type: {type}");
            }
        }
    }

    public class SeaPackageFactory : PackageFactory
    {
        public override TravelPackage CreatePackage(string name, decimal price, string destination, string transportType, string accommodationType)
        {
            return new SeaPackage
            {
                Name = name,
                Price = price,
                Destination = destination,
                TransportType = transportType,
                AccommodationType = accommodationType
            };
        }
    }

    public class MountainPackageFactory : PackageFactory
    {
        public override TravelPackage CreatePackage(string name, decimal price, string destination, string transportType, string accommodationType)
        {
            return new MountainPackage
            {
                Name = name,
                Price = price,
                Destination = destination,
                TransportType = transportType,
                AccommodationType = accommodationType
            };
        }
    }

    public class ExcursionPackageFactory : PackageFactory
    {
        public override TravelPackage CreatePackage(string name, decimal price, string destination, string transportType, string accommodationType)
        {
            return new ExcursionPackage
            {
                Name = name,
                Price = price,
                Destination = destination,
                TransportType = transportType,
                AccommodationType = accommodationType
            };
        }
    }

    public class CruisePackageFactory : PackageFactory
    {
        public override TravelPackage CreatePackage(string name, decimal price, string destination, string transportType, string accommodationType)
        {
            return new CruisePackage
            {
                Name = name,
                Price = price,
                Destination = destination,
                TransportType = transportType,
                AccommodationType = accommodationType
            };
        }
    }
}
