using System;
using TurističkaOrganizacija.Domain;

namespace TurističkaOrganizacija.Application
{
    public interface IPricingStrategy
    {
        decimal Calculate(TravelPackage travelPackage, int passengerCount);
    }

    public class BasePriceStrategy : IPricingStrategy
    {
        public decimal Calculate(TravelPackage travelPackage, int passengerCount)
        {
            return travelPackage.Price * Math.Max(1, passengerCount);
        }
    }

    public class EarlyBirdDiscountStrategy : IPricingStrategy
    {
        private readonly IPricingStrategy _inner;
        public EarlyBirdDiscountStrategy(IPricingStrategy inner)
        {
            _inner = inner;
        }
        public decimal Calculate(TravelPackage travelPackage, int passengerCount)
        {
            decimal total = _inner.Calculate(travelPackage, passengerCount);
            if (travelPackage.DepartureDate.HasValue && (travelPackage.DepartureDate.Value - DateTime.Today).TotalDays >= 60)
            {
                total *= 0.9m; // 10% popust
            }
            return total;
        }
    }

    /// <summary>
    /// Decorator pattern - Group discount for multiple passengers
    /// </summary>
    public class GroupDiscountStrategy : IPricingStrategy
    {
        private readonly IPricingStrategy _inner;
        public GroupDiscountStrategy(IPricingStrategy inner)
        {
            _inner = inner;
        }
        public decimal Calculate(TravelPackage travelPackage, int passengerCount)
        {
            decimal total = _inner.Calculate(travelPackage, passengerCount);
            if (passengerCount >= 4)
            {
                total *= 0.85m; // 15% popust za grupe od 4+ osoba
            }
            else if (passengerCount >= 2)
            {
                total *= 0.95m; // 5% popust za 2-3 osobe
            }
            return total;
        }
    }

    /// <summary>
    /// Decorator pattern - Seasonal pricing based on package type
    /// </summary>
    public class SeasonalPricingStrategy : IPricingStrategy
    {
        private readonly IPricingStrategy _inner;
        public SeasonalPricingStrategy(IPricingStrategy inner)
        {
            _inner = inner;
        }
        public decimal Calculate(TravelPackage travelPackage, int passengerCount)
        {
            decimal total = _inner.Calculate(travelPackage, passengerCount);
            var currentMonth = DateTime.Now.Month;
            
            // Summer premium for sea packages (June-August)
            if (travelPackage.Type == PackageType.Sea && currentMonth >= 6 && currentMonth <= 8)
            {
                total *= 1.2m; // 20% premium
            }
            // Winter premium for mountain packages (December-February)
            else if (travelPackage.Type == PackageType.Mountain && (currentMonth == 12 || currentMonth <= 2))
            {
                total *= 1.15m; // 15% premium
            }
            
            return total;
        }
    }

    /// <summary>
    /// Decorator pattern - VIP client discount
    /// </summary>
    public class VipDiscountStrategy : IPricingStrategy
    {
        private readonly IPricingStrategy _inner;
        private readonly Func<bool> _isVipClient;
        
        public VipDiscountStrategy(IPricingStrategy inner, Func<bool> isVipClient)
        {
            _inner = inner;
            _isVipClient = isVipClient;
        }
        
        public decimal Calculate(TravelPackage travelPackage, int passengerCount)
        {
            decimal total = _inner.Calculate(travelPackage, passengerCount);
            if (_isVipClient())
            {
                total *= 0.8m; // 20% popust za VIP klijente
            }
            return total;
        }
    }
}


