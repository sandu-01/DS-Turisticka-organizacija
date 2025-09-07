using System;
using System.Linq;
using System.Windows.Forms;
using TurističkaOrganizacija.Application;
using TurističkaOrganizacija.Domain;
using TurističkaOrganizacija.Infrastructure.Repositories.SqlClient;
using TurističkaOrganizacija.Application.TemplateMethod;
using TurističkaOrganizacija.Application.Facade;
using TurističkaOrganizacija.Infrastructure.Adapters;
using System.Drawing;
using TurističkaOrganizacija.GUI;

namespace TurističkaOrganizacija
{
    public class ReservationForm : Form
    {
        private DataGridView gridClients = new DataGridView();
        private DataGridView gridPackages = new DataGridView();
        private DataGridView gridReservations = new DataGridView();
        private Button btnReserve = new Button();
        private Button btnCancel = new Button();
        private NumericUpDown nudPassengers = new NumericUpDown();

        public ReservationForm()
        {
            this.Text = "Rezervacije";
            this.Width = 1100;
            this.Height = 700;

            gridClients.Location = new System.Drawing.Point(12, 12);
            gridClients.Size = new System.Drawing.Size(520, 500);
            gridClients.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;

            gridPackages.Location = new System.Drawing.Point(550, 12);
            gridPackages.Size = new System.Drawing.Size(520, 500);
            gridPackages.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;

            gridReservations.Location = new System.Drawing.Point(12, 560);
            gridReservations.Size = new System.Drawing.Size(1058, 100);
            gridReservations.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

            btnReserve.Text = "Rezerviši";
            btnReserve.Location = new System.Drawing.Point(550, 530);
            btnCancel.Text = "Otkaži";
            btnCancel.Location = new System.Drawing.Point(650, 530);
            nudPassengers.Minimum = 1;
            nudPassengers.Maximum = 100;
            nudPassengers.Value = 1;
            nudPassengers.Location = new System.Drawing.Point(12, 530);

            this.Controls.Add(gridClients);
            this.Controls.Add(gridPackages);
            this.Controls.Add(btnReserve);
            this.Controls.Add(btnCancel);
            this.Controls.Add(nudPassengers);
            this.Controls.Add(gridReservations);

            LoadData();
            WireEvents();

            //UiKit.WireAutoSizeGrid(gridClients, this, maxW: 0.45, maxH: 0.6);
            //UiKit.WireAutoSizeGrid(gridPackages, this, maxW: 0.9, maxH: 0.6);
            //form bc color
            UiKit.StyleForm(this, ColorTranslator.FromHtml("#42ad94"), 0.96);
            //cells bc color
            UiKit.StyleGridSolid(
                gridClients,
                cellBackColor: ColorTranslator.FromHtml("#9fd1b9"),
                fontColor: Color.Black,
                gridLineColor: Color.Black,
                keepSelectionHighlight: false);
            UiKit.StyleGridSolid(
                gridReservations,
                cellBackColor: ColorTranslator.FromHtml("#9fd1b9"),
                fontColor: Color.Black,
                gridLineColor: Color.Black,
                keepSelectionHighlight: false);
            UiKit.StyleButtons(ColorTranslator.FromHtml("#9d00ff"), 96, btnCancel, btnReserve);
        }

        private void LoadData()
        {
            var facade = BuildFacade();
            gridClients.DataSource = facade.GetAllClients().ToList();
            var allPackages = facade.GetPackagesByType(PackageType.Sea)
                .Concat(facade.GetPackagesByType(PackageType.Mountain))
                .Concat(facade.GetPackagesByType(PackageType.Excursion))
                .Concat(facade.GetPackagesByType(PackageType.Cruise))
                .ToList();
            gridPackages.DataSource = allPackages;
            gridClients.SelectionChanged += (s, e) => MarkReservedPackages();
            MarkReservedPackages();
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

        private void WireEvents()
        {
            btnReserve.Click += (s, e) => Reserve();
            btnCancel.Click += (s, e) => Cancel();
            TurističkaOrganizacija.Application.EventBus.ReservationsChanged += (clientId) =>
            {
                if (gridClients.CurrentRow == null) return;
                var client = gridClients.CurrentRow.DataBoundItem as Domain.Client;
                if (client == null) return;
                if (client.Id == clientId)
                {
                    LoadReservations(client.Id);
                    MarkReservedPackages();
                }
            };


            TurističkaOrganizacija.Application.EventBus.ClientsChanged += () =>
            {
                try { LoadData(); } catch { }
            };
        }

        private void Reserve()
        {
            if (gridClients.CurrentRow == null || gridPackages.CurrentRow == null) return;
            var client = gridClients.CurrentRow.DataBoundItem as Client;
            var pack = gridPackages.CurrentRow.DataBoundItem as TravelPackage;
            if (client == null || pack == null) return;

            IPricingStrategy strategy = new BasePriceStrategy();
            strategy = new EarlyBirdDiscountStrategy(strategy);
            strategy = new GroupDiscountStrategy(strategy);
            strategy = new SeasonalPricingStrategy(strategy);
            decimal total = strategy.Calculate(pack, (int)nudPassengers.Value);

            var service = new ReservationService(new ReservationRepositorySql(), new PackageRepositorySql());
            var cmd = new TurističkaOrganizacija.Application.Commands.MakeReservationCommand(client.Id, pack, (int)nudPassengers.Value, total, service);
            new TurističkaOrganizacija.Application.Commands.CommandInvoker().ExecuteCommand(cmd);
            MessageBox.Show("Rezervacija uspešna.");
        }

        private void Cancel()
        {
            if (gridClients.CurrentRow == null || gridPackages.CurrentRow == null) return;
            var client = gridClients.CurrentRow.DataBoundItem as Client;
            var pack = gridPackages.CurrentRow.DataBoundItem as TravelPackage;
            if (client == null || pack == null) return;

            var service = new ReservationService(new ReservationRepositorySql(), new PackageRepositorySql());
            service.Cancel(client.Id, pack.Id);
            MessageBox.Show("Rezervacija otkazana (ako je postojala).");
        }

        private void MarkReservedPackages()
        {
            if (gridClients.CurrentRow == null) return;
            var client = gridClients.CurrentRow.DataBoundItem as Client;
            if (client == null) return;
            var adapter = new SqlServerDatabaseAdapter(new ClientRepositorySql(), new PackageRepositorySql(), new ReservationRepositorySql());
            var reserved = adapter.GetReservationsByClient(client.Id).Select(r => r.PackageId).ToHashSet();

            foreach (DataGridViewRow row in gridPackages.Rows)
            {
                var pack = row.DataBoundItem as TravelPackage;
                if (pack == null) continue;
                row.DefaultCellStyle.BackColor = reserved.Contains(pack.Id) ? System.Drawing.Color.LightGreen : System.Drawing.Color.White;
            }
        }

        private void LoadReservations(int clientId)
        {
            var repo = new ReservationRepositorySql();
            var list = repo.GetByClient(clientId).ToList();
            gridReservations.DataSource = list;
        }
    }
}


