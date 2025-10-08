using System;
using System.Linq;
using System.Windows.Forms;
using TurističkaOrganizacija.Application;
using TurističkaOrganizacija.Domain;
using TurističkaOrganizacija.Infrastructure.Repositories.SqlClient;
// removed TemplateMethod usage
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
        private Label lblReservations = new Label();
        private Label lblPackages = new Label();
        private Button btnReserve = new Button();
        private Button btnCancel = new Button();
        private Button btnUpdate = new Button();
        private Label lblPassengers = new Label();
        private NumericUpDown nudPassengers = new NumericUpDown();
        private Client preselectedClient;
        private Button btnSea = new Button();
        private Button btnMountain = new Button();
        private Button btnExcursion = new Button();
        private Button btnCruise = new Button();
        private PackageType currentPackageType = PackageType.Sea;

        public ReservationForm()
        {
            this.Text = "Rezervacije";
            this.Width = 1100;
            this.Height = 700;

            gridClients.Location = new System.Drawing.Point(12, 12);
            gridClients.Size = new System.Drawing.Size(520, 500);
            gridClients.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;

            lblReservations.Text = "Rezervacije korisnika";
            lblReservations.AutoSize = true;
            lblReservations.Location = new System.Drawing.Point(12, 12);
            lblReservations.Anchor = AnchorStyles.Top | AnchorStyles.Left;

            lblPackages.Text = "Dostupni paketi";
            lblPackages.AutoSize = true;
            lblPackages.Location = new System.Drawing.Point(550, 12);
            lblPackages.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            // Package type filter buttons under action buttons
            int rightX = 550;
            int bottomY = 570;
            btnSea.Text = "More";
            btnSea.Location = new System.Drawing.Point(rightX, bottomY);
            btnSea.Size = new System.Drawing.Size(80, 28);
            btnSea.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            btnMountain.Text = "Planina";
            btnMountain.Location = new System.Drawing.Point(rightX + 90, bottomY);
            btnMountain.Size = new System.Drawing.Size(80, 28);
            btnMountain.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            btnExcursion.Text = "Ekskurzija";
            btnExcursion.Location = new System.Drawing.Point(rightX + 180, bottomY);
            btnExcursion.Size = new System.Drawing.Size(90, 28);
            btnExcursion.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            btnCruise.Text = "Krstarenje";
            btnCruise.Location = new System.Drawing.Point(rightX + 280, bottomY);
            btnCruise.Size = new System.Drawing.Size(90, 28);
            btnCruise.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            gridPackages.Location = new System.Drawing.Point(550, 32);
            gridPackages.Size = new System.Drawing.Size(520, 480);
            gridPackages.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;

            gridReservations.Location = new System.Drawing.Point(12, 32);
            gridReservations.Size = new System.Drawing.Size(520, 480);
            gridReservations.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;

            btnReserve.Text = "Rezerviši";
            btnReserve.Location = new System.Drawing.Point(550, 530);
            btnCancel.Text = "Otkaži";
            btnCancel.Location = new System.Drawing.Point(650, 530);
            btnUpdate.Text = "Ažuriraj";
            btnUpdate.Location = new System.Drawing.Point(750, 530);
            lblPassengers.Text = "Broj putnika";
            lblPassengers.AutoSize = true;
            lblPassengers.Location = new System.Drawing.Point(12, 540);
            nudPassengers.Minimum = 1;
            nudPassengers.Maximum = 100;
            nudPassengers.Value = 1;
            nudPassengers.Location = new System.Drawing.Point(12, 560);

            this.Controls.Add(lblReservations);
            this.Controls.Add(lblPackages);
            this.Controls.Add(gridClients);
            this.Controls.Add(gridPackages);
            this.Controls.Add(btnSea);
            this.Controls.Add(btnMountain);
            this.Controls.Add(btnExcursion);
            this.Controls.Add(btnCruise);
            this.Controls.Add(btnReserve);
            this.Controls.Add(btnCancel);
            this.Controls.Add(btnUpdate);
            this.Controls.Add(lblPassengers);
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
                gridReservations,
                cellBackColor: ColorTranslator.FromHtml("#9fd1b9"),
                fontColor: Color.Black,
                gridLineColor: Color.Black,
                keepSelectionHighlight: false);
            UiKit.StyleGridSolid(
                gridPackages,
                cellBackColor: ColorTranslator.FromHtml("#9fd1b9"),
                fontColor: Color.Black,
                gridLineColor: Color.Black,
                keepSelectionHighlight: false);
            UiKit.StyleButtons(ColorTranslator.FromHtml("#9d00ff"), 96, btnCancel, btnReserve, btnUpdate);
            UiKit.StyleButtons(ColorTranslator.FromHtml("#9d00ff"), 80, btnSea, btnMountain, btnExcursion, btnCruise);
        }

        public ReservationForm(Client client) : this()
        {
            preselectedClient = client;
            if (preselectedClient != null)
            {
                this.Text = $"Rezervacije - {preselectedClient.FirstName} {preselectedClient.LastName}";
                gridClients.Visible = false;
                gridClients.Enabled = false;
                LoadReservations(preselectedClient.Id);
                // Delay highlighting until form is fully loaded
                this.Load += (s, e) => MarkReservedPackages();
            }
        }

        private void LoadData()
        {
            var facade = BuildFacade();
            if (preselectedClient == null)
            {
                gridClients.DataSource = facade.GetAllClients().ToList();
            }
            else
            {
                // Keep data source minimal; not shown when preselected
                gridClients.DataSource = new[] { preselectedClient };
            }
            // Default load per current type
            LoadPackagesByType(currentPackageType);
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
            btnUpdate.Click += (s, e) => UpdatePassengers();
            btnSea.Click += (s, e) => { currentPackageType = PackageType.Sea; LoadPackagesByType(currentPackageType); MarkReservedPackages(); };
            btnMountain.Click += (s, e) => { currentPackageType = PackageType.Mountain; LoadPackagesByType(currentPackageType); MarkReservedPackages(); };
            btnExcursion.Click += (s, e) => { currentPackageType = PackageType.Excursion; LoadPackagesByType(currentPackageType); MarkReservedPackages(); };
            btnCruise.Click += (s, e) => { currentPackageType = PackageType.Cruise; LoadPackagesByType(currentPackageType); MarkReservedPackages(); };
            TurističkaOrganizacija.Application.EventBus.ReservationsChanged += (clientId) =>
            {
                var client = GetActiveClient();
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

        private void LoadPackagesByType(PackageType type)
        {
            var facade = BuildFacade();
            var list = facade.GetPackagesByType(type).ToList();

            gridPackages.AutoGenerateColumns = false;
            gridPackages.Columns.Clear();

            // Common columns
            gridPackages.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(TravelPackage.Name), HeaderText = "Naziv" });
            gridPackages.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(TravelPackage.Price), HeaderText = "Cena" });

            if (type == PackageType.Sea)
            {
                gridPackages.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(TravelPackage.Destination), HeaderText = "Destinacija" });
                gridPackages.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(TravelPackage.TransportType), HeaderText = "Prevoz" });
                gridPackages.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(TravelPackage.AccommodationType), HeaderText = "Smeštaj" });
            }
            else if (type == PackageType.Mountain)
            {
                gridPackages.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(TravelPackage.Destination), HeaderText = "Destinacija" });
                gridPackages.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(TravelPackage.TransportType), HeaderText = "Prevoz" });
                gridPackages.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(TravelPackage.AccommodationType), HeaderText = "Smeštaj" });
                
                gridPackages.Columns.Add(new DataGridViewTextBoxColumn { Name = "Aktivnosti", HeaderText = "Aktivnosti" });
            }
            else if (type == PackageType.Excursion)
            {
                gridPackages.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(TravelPackage.Destination), HeaderText = "Destinacija" });
                gridPackages.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(TravelPackage.TransportType), HeaderText = "Prevoz" });
                gridPackages.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(TravelPackage.GuideName), HeaderText = "Vodič" });
                gridPackages.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(TravelPackage.DurationDays), HeaderText = "Trajanje (dana)" });
            }
            else if (type == PackageType.Cruise)
            {
                gridPackages.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(TravelPackage.ShipName), HeaderText = "Brod" });
                gridPackages.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(TravelPackage.Route), HeaderText = "Ruta" });
                gridPackages.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(TravelPackage.DepartureDate), HeaderText = "Polazak" });
                gridPackages.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(TravelPackage.CabinType), HeaderText = "Kabina" });
            }

            gridPackages.DataSource = list;
            
            gridPackages.CellFormatting -= GridPackages_CellFormatting;
            gridPackages.CellFormatting += GridPackages_CellFormatting;
        }

        private void GridPackages_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (gridPackages.Columns[e.ColumnIndex].Name != "Aktivnosti") return;
            var item = gridPackages.Rows[e.RowIndex].DataBoundItem as MountainPackage;
            if (item == null)
            {
                e.Value = string.Empty;
                e.FormattingApplied = true;
                return;
            }
            if (item.Activities == null || item.Activities.Count == 0)
            {
                e.Value = string.Empty;
            }
            else
            {
                e.Value = string.Join(", ", item.Activities);
            }
            e.FormattingApplied = true;
        }

        private void Reserve()
        {
            if (gridPackages.CurrentRow == null) return;
            var client = GetActiveClient();
            var pack = gridPackages.CurrentRow.DataBoundItem as TravelPackage;
            if (client == null || pack == null) return;

            IPricingStrategy strategy = new BasePriceStrategy();
            decimal total = strategy.Calculate(pack, (int)nudPassengers.Value);

            var service = new ReservationService(new ReservationRepositorySql(), new PackageRepositorySql());
            service.Reserve(client.Id, pack, (int)nudPassengers.Value, total);
            MessageBox.Show("Rezervacija uspešna.");
        }

        private void Cancel()
        {
            if (gridPackages.CurrentRow == null) return;
            var client = GetActiveClient();
            var pack = gridPackages.CurrentRow.DataBoundItem as TravelPackage;
            if (client == null || pack == null) return;

            var service = new ReservationService(new ReservationRepositorySql(), new PackageRepositorySql());
            service.Cancel(client.Id, pack.Id);
            MessageBox.Show("Rezervacija otkazana (ako je postojala).");

            LoadReservations(client.Id);
            MarkReservedPackages();
            gridPackages.Refresh();
        }

        private void UpdatePassengers()
        {
            if (gridPackages.CurrentRow == null) return;
            var client = GetActiveClient();
            var pack = gridPackages.CurrentRow.DataBoundItem as TravelPackage;
            if (client == null || pack == null) return;

            var service = new ReservationService(new ReservationRepositorySql(), new PackageRepositorySql());
            service.UpdatePassengers(client.Id, pack.Id, (int)nudPassengers.Value);
            MessageBox.Show("Rezervacija ažurirana (ako je postojala).");

            LoadReservations(client.Id);
            MarkReservedPackages();
            gridPackages.Refresh();
        }

        private void MarkReservedPackages()
        {
            var client = GetActiveClient();
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
            var packageRepo = new PackageRepositorySql();

            var view = list.Select(r =>
            {
                var p = packageRepo.GetById(r.PackageId);
                string vrsta = p == null ? string.Empty : (p.Type == PackageType.Sea ? "more" : p.Type == PackageType.Mountain ? "planina" : p.Type == PackageType.Excursion ? "ekskurzija" : "krstarenje");
                decimal cena = (p?.Price ?? 0) * r.PassengerCount;
                return new
                {
                    Naziv = p?.Name,
                    Cena = cena,
                    Vrsta = vrsta,
                    BrojPutnika = r.PassengerCount
                };
            }).ToList();

            gridReservations.AutoGenerateColumns = true;
            gridReservations.DataSource = view;
        }

        private Client GetActiveClient()
        {
            if (preselectedClient != null) return preselectedClient;
            if (gridClients.CurrentRow == null) return null;
            return gridClients.CurrentRow.DataBoundItem as Client;
        }
    }
}


