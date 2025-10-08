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
}


