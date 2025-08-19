using System;

namespace TurističkaOrganizacija.Application
{
    public static class EventBus
    {
        public static event Action<int> ReservationsChanged; // payload: clientId
        public static event Action ClientsChanged;
        public static event Action PackagesChanged;

        public static void PublishReservationsChanged(int clientId)
        {
            var handler = ReservationsChanged;
            if (handler != null) handler(clientId);
        }

        public static void PublishClientsChanged()
        {
            var handler = ClientsChanged;
            if (handler != null) handler();
        }

        public static void PublishPackagesChanged()
        {
            var handler = PackagesChanged;
            if (handler != null) handler();
        }
    }
}


