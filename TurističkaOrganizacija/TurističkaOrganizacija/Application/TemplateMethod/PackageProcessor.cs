using System;
using System.Collections.Generic;
using TurističkaOrganizacija.Domain;

namespace TurističkaOrganizacija.Application.TemplateMethod
{
    /// <summary>
    /// Template Method pattern for processing different types of travel packages
    /// Defines the skeleton of an algorithm while allowing subclasses to override specific steps
    /// </summary>
    public abstract class PackageProcessor
    {
        /// <summary>
        /// Template method that defines the algorithm structure
        /// </summary>
        public void ProcessPackage(TravelPackage package)
        {
            ValidatePackage(package);
            CalculatePricing(package);
            PrepareDocumentation(package);
            NotifyStakeholders(package);
            LogProcessing(package);
        }

        /// <summary>
        /// Step 1: Validate package-specific requirements
        /// </summary>
        protected abstract void ValidatePackage(TravelPackage package);

        /// <summary>
        /// Step 2: Calculate pricing based on package type
        /// </summary>
        protected abstract void CalculatePricing(TravelPackage package);

        /// <summary>
        /// Step 3: Prepare package-specific documentation
        /// </summary>
        protected abstract void PrepareDocumentation(TravelPackage package);

        /// <summary>
        /// Step 4: Notify relevant stakeholders
        /// </summary>
        protected virtual void NotifyStakeholders(TravelPackage package)
        {
            // Default implementation - can be overridden
            Console.WriteLine($"Notifying stakeholders about {package.Name}");
        }

        /// <summary>
        /// Step 5: Log processing information
        /// </summary>
        protected virtual void LogProcessing(TravelPackage package)
        {
            // Default implementation - can be overridden
            Console.WriteLine($"Package {package.Name} processed successfully");
        }
    }

    /// <summary>
    /// Concrete implementation for Sea packages
    /// </summary>
    public class SeaPackageProcessor : PackageProcessor
    {
        protected override void ValidatePackage(TravelPackage package)
        {
            if (string.IsNullOrEmpty(package.Destination))
                throw new ArgumentException("Sea package must have a destination");
            
            if (string.IsNullOrEmpty(package.AccommodationType))
                throw new ArgumentException("Sea package must specify accommodation type");
            
            Console.WriteLine($"Validating sea package: {package.Name}");
        }

        protected override void CalculatePricing(TravelPackage package)
        {
            // Sea packages might have seasonal pricing
            var currentMonth = DateTime.Now.Month;
            if (currentMonth >= 6 && currentMonth <= 8) // Summer months
            {
                package.Price *= 1.2m; // 20% summer premium
            }
            
            Console.WriteLine($"Calculated pricing for sea package: {package.Price:C}");
        }

        protected override void PrepareDocumentation(TravelPackage package)
        {
            Console.WriteLine($"Preparing sea package documentation: beach guides, water activities, sun protection info");
        }

        protected override void NotifyStakeholders(TravelPackage package)
        {
            base.NotifyStakeholders(package);
            Console.WriteLine("Notifying beach resort and water activity providers");
        }
    }

    /// <summary>
    /// Concrete implementation for Mountain packages
    /// </summary>
    public class MountainPackageProcessor : PackageProcessor
    {
        protected override void ValidatePackage(TravelPackage package)
        {
            if (string.IsNullOrEmpty(package.Destination))
                throw new ArgumentException("Mountain package must have a destination");
            
            var mountainPackage = package as MountainPackage;
            if (mountainPackage != null && mountainPackage.Activities.Count == 0)
                throw new ArgumentException("Mountain package must have at least one activity");
            
            Console.WriteLine($"Validating mountain package: {package.Name}");
        }

        protected override void CalculatePricing(TravelPackage package)
        {
            // Mountain packages might have equipment rental costs
            var mountainPackage = package as MountainPackage;
            if (mountainPackage != null)
            {
                var equipmentCost = mountainPackage.Activities.Count * 50m; // 50 per activity
                package.Price += equipmentCost;
            }
            
            Console.WriteLine($"Calculated pricing for mountain package: {package.Price:C}");
        }

        protected override void PrepareDocumentation(TravelPackage package)
        {
            Console.WriteLine($"Preparing mountain package documentation: hiking maps, equipment lists, safety guidelines");
        }

        protected override void NotifyStakeholders(TravelPackage package)
        {
            base.NotifyStakeholders(package);
            Console.WriteLine("Notifying mountain guides and equipment rental services");
        }
    }

    /// <summary>
    /// Concrete implementation for Excursion packages
    /// </summary>
    public class ExcursionPackageProcessor : PackageProcessor
    {
        protected override void ValidatePackage(TravelPackage package)
        {
            if (string.IsNullOrEmpty(package.Destination))
                throw new ArgumentException("Excursion package must have a destination");
            
            if (string.IsNullOrEmpty(package.GuideName))
                throw new ArgumentException("Excursion package must have a guide assigned");
            
            Console.WriteLine($"Validating excursion package: {package.Name}");
        }

        protected override void CalculatePricing(TravelPackage package)
        {
            // Excursion packages include guide fees
            package.Price += 100m; // Guide fee
            
            Console.WriteLine($"Calculated pricing for excursion package: {package.Price:C}");
        }

        protected override void PrepareDocumentation(TravelPackage package)
        {
            Console.WriteLine($"Preparing excursion package documentation: itinerary, guide contact info, cultural information");
        }

        protected override void NotifyStakeholders(TravelPackage package)
        {
            base.NotifyStakeholders(package);
            Console.WriteLine($"Notifying guide: {package.GuideName}");
        }
    }

    /// <summary>
    /// Concrete implementation for Cruise packages
    /// </summary>
    public class CruisePackageProcessor : PackageProcessor
    {
        protected override void ValidatePackage(TravelPackage package)
        {
            if (string.IsNullOrEmpty(package.ShipName))
                throw new ArgumentException("Cruise package must specify ship name");
            
            if (string.IsNullOrEmpty(package.Route))
                throw new ArgumentException("Cruise package must specify route");
            
            if (!package.DepartureDate.HasValue)
                throw new ArgumentException("Cruise package must have departure date");
            
            Console.WriteLine($"Validating cruise package: {package.Name}");
        }

        protected override void CalculatePricing(TravelPackage package)
        {
            // Cruise packages might have cabin type pricing
            if (!string.IsNullOrEmpty(package.CabinType))
            {
                switch (package.CabinType.ToLower())
                {
                    case "suite":
                        package.Price *= 1.5m;
                        break;
                    case "balcony":
                        package.Price *= 1.2m;
                        break;
                    case "interior":
                        package.Price *= 0.9m;
                        break;
                }
            }
            
            Console.WriteLine($"Calculated pricing for cruise package: {package.Price:C}");
        }

        protected override void PrepareDocumentation(TravelPackage package)
        {
            Console.WriteLine($"Preparing cruise package documentation: ship layout, port information, onboard activities");
        }

        protected override void NotifyStakeholders(TravelPackage package)
        {
            base.NotifyStakeholders(package);
            Console.WriteLine($"Notifying cruise line and port authorities for {package.ShipName}");
        }
    }

    /// <summary>
    /// Factory for creating appropriate package processors
    /// </summary>
    public static class PackageProcessorFactory
    {
        public static PackageProcessor CreateProcessor(PackageType packageType)
        {
            switch (packageType)
            {
                case PackageType.Sea:
                    return new SeaPackageProcessor();
                case PackageType.Mountain:
                    return new MountainPackageProcessor();
                case PackageType.Excursion:
                    return new ExcursionPackageProcessor();
                case PackageType.Cruise:
                    return new CruisePackageProcessor();
                default:
                    throw new ArgumentException($"Unknown package type: {packageType}");
            }
        }
    }
}
