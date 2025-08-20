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
}


