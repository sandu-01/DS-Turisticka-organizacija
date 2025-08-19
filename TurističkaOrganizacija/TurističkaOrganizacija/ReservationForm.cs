using System;
using System.Linq;
using System.Windows.Forms;
using TurističkaOrganizacija.Application;
using TurističkaOrganizacija.Domain;
using TurističkaOrganizacija.Infrastructure.Repositories.SqlClient;

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
        }

        private void LoadData()
        {
            var clientService = new ClientService(new ClientRepositorySql(), new TurističkaOrganizacija.Infrastructure.Security.SecurityService());
            var packageService = new PackageService(new PackageRepositorySql());

            gridClients.DataSource = clientService.Search(null, null, null).ToList();
            var allPackages = packageService.GetAll(null).ToList();
            gridPackages.DataSource = allPackages;
            gridClients.SelectionChanged += (s, e) => MarkReservedPackages();
            MarkReservedPackages();
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
        }

        private void Reserve()
        {
            if (gridClients.CurrentRow == null || gridPackages.CurrentRow == null) return;
            var client = gridClients.CurrentRow.DataBoundItem as Client;
            var pack = gridPackages.CurrentRow.DataBoundItem as TravelPackage;
            if (client == null || pack == null) return;

            IPricingStrategy strategy = new EarlyBirdDiscountStrategy(new BasePriceStrategy());
            decimal total = strategy.Calculate(pack, (int)nudPassengers.Value);

            var service = new ReservationService(new ReservationRepositorySql(), new PackageRepositorySql());
            service.Reserve(client.Id, pack, (int)nudPassengers.Value, total);
            MessageBox.Show("Rezervacija uspešna.");
        }

        private void Cancel()
        {
            // Minimal: delete reservation if exists by composite key
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
            var repo = new ReservationRepositorySql();
            var reserved = repo.GetByClient(client.Id).Select(r => r.PackageId).ToHashSet();

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


