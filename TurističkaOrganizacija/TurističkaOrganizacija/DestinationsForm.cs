using System;
using System.Linq;
using System.Windows.Forms;
using TurističkaOrganizacija.Application.Facade;
using TurističkaOrganizacija.Infrastructure.Adapters;
using TurističkaOrganizacija.Infrastructure.Repositories.SqlClient;
using TurističkaOrganizacija.Application;

namespace TurističkaOrganizacija
{
    public class DestinationsForm : Form
    {
        private readonly ListBox list = new ListBox();

        public DestinationsForm()
        {
            this.Text = "Destinacije";
            this.Width = 400;
            this.Height = 500;
            list.Dock = DockStyle.Fill;
            this.Controls.Add(list);
            LoadData();
        }

        private TouristAgencyFacade BuildFacade()
        {
            var adapter = new SqlServerDatabaseAdapter(new ClientRepositorySql(), new PackageRepositorySql(), new ReservationRepositorySql());
            var clientService = new ClientService(new ClientRepositorySql(), new TurističkaOrganizacija.Infrastructure.Security.SecurityService());
            var packageService = new PackageService(new PackageRepositorySql());
            var reservationService = new ReservationService(new ReservationRepositorySql(), new PackageRepositorySql());
            var security = new TurističkaOrganizacija.Infrastructure.Security.SecurityService();
            return new TouristAgencyFacade(adapter, clientService, packageService, reservationService, security);
        }

        private void LoadData()
        {
            var facade = BuildFacade();
            var items = facade.GetAllDestinations().OrderBy(x => x).ToList();
            list.DataSource = items;
        }
    }
}


