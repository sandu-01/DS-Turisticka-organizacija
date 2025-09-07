using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TurističkaOrganizacija.Backend;
using TurističkaOrganizacija.Infrastructure.Backup;
using TurističkaOrganizacija.Infrastructure.Repositories.SqlClient;
using TurističkaOrganizacija.Application;
using TurističkaOrganizacija.Application.Commands;
using TurističkaOrganizacija.Application.Facade;
using TurističkaOrganizacija.Infrastructure.Adapters;
using TurističkaOrganizacija.GUI;

namespace TurističkaOrganizacija
{
    public partial class Form1 : Form
    {
        private readonly Timer backupTimer = new Timer();
        private BindingSource clientsBinding = new BindingSource();
        private readonly CommandInvoker commandInvoker = new CommandInvoker();
        private TouristAgencyFacade BuildFacade()
        {
            var adapter = new SqlServerDatabaseAdapter(new ClientRepositorySql(), new PackageRepositorySql(), new ReservationRepositorySql());
            var clientService = new ClientService(new ClientRepositorySql(), new TurističkaOrganizacija.Infrastructure.Security.SecurityService());
            var packageService = new PackageService(new PackageRepositorySql());
            var reservationService = new ReservationService(new ReservationRepositorySql(), new PackageRepositorySql());
            var security = new TurističkaOrganizacija.Infrastructure.Security.SecurityService();
            return new TouristAgencyFacade(adapter, clientService, packageService, reservationService, security);
        }

        public Form1()
        {
            InitializeComponent();
            UiKitForm1.ClientsDesign(txtIme, txtPrezime, txtEmail, txtTelefon, txtPasos, 
                RefreshClients, dtpRodjenje);
            // Grid changes its size according to content
            UiKit.WireAutoSizeGrid(dataGridView1, this, maxW: 0.9, maxH: 0.6);
            //form bc color
            UiKit.StyleForm(this, ColorTranslator.FromHtml("#9fd1b9"), 1);
            //cells bc color
            UiKit.StyleGridSolid(
                dataGridView1,
                cellBackColor: ColorTranslator.FromHtml("#c7e3b3"),
                fontColor: Color.Black,
                gridLineColor: Color.Black,
                keepSelectionHighlight: false);
            UiKit.StyleButtons(ColorTranslator.FromHtml("#2563eb"), 96, btnDodaj, btnIzmeni, btnObrisi, btnPaketi, btnDestinacije);

            this.Text = AppConfig.Instance.AgencyName;

            // Schedule automatic backup every 24h
            backupTimer.Interval = (int)System.TimeSpan.FromHours(24).TotalMilliseconds;
            backupTimer.Tick += (s, e) =>
            {
                try
                {
                    new BackupService().RunBackup();
                }
                catch
                {
                    // swallow for now; later log/display
                }
            };
            backupTimer.Start();

            var facade = BuildFacade();
            clientsBinding.DataSource = facade.GetAllClients();
            dataGridView1.DataSource = clientsBinding;


            btnDodaj.Click += (s, e) => btnAddClient_Click(s,e);
            btnIzmeni.Click += (s, e) => btnIzmena_Click(s,e);
            btnObrisi.Click += (s, e) => DeleteClient();
            btnPaketi.Click += (s, e) =>
            {
                using (var f = new PackagesForm())
                {
                    f.ShowDialog(this);
                }
            };

            btnDestinacije.Click += (s, e) =>
            {
                using (var f = new DestinationsForm())
                {
                    f.ShowDialog(this);
                }
            };
        
            dataGridView1.DoubleClick += (s, e) =>
            {
                using (var f = new ReservationForm())
                {
                    f.ShowDialog(this);
                }
            };

            TurističkaOrganizacija.Application.EventBus.ClientsChanged += () =>
            {
                try { RefreshClients(); } catch { }
            };
        }

        private ClientService BuildClientService()
        {
            var repo = new ClientRepositorySql();
            return new ClientService(repo, new TurističkaOrganizacija.Infrastructure.Security.SecurityService());
        }


        private void btnAddClient_Click(object sender, EventArgs e)
        {
            using (var dlg = new AddClientForm(BuildClientService(), commandInvoker))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    // EventBus.ClientsChanged već gađa RefreshClients(); ovo je dodatna sigurnost
                    try { RefreshClients(); } catch { }
                }
            }
        }

        private void btnIzmena_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null) return;
            if (!(dataGridView1.CurrentRow.DataBoundItem is TurističkaOrganizacija.Domain.Client selected)) return;

            using (var dlg = new EditClientForm(selected, BuildClientService(), commandInvoker))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    try { RefreshClients(); } catch { }
                }
            }
        }

        private void DeleteClient()
        {
            if (dataGridView1.CurrentRow == null) return;
            if (dataGridView1.CurrentRow.DataBoundItem is TurističkaOrganizacija.Domain.Client selected)
            {
                var service = BuildClientService();
                var cmd = new DeleteClientCommand(selected, service);
                commandInvoker.ExecuteCommand(cmd);

            }
        }

        private bool ValidateInputs(out TurističkaOrganizacija.Domain.Client client, out string passportPlain)
        {
            client = null;
            passportPlain = null;
            if (string.IsNullOrWhiteSpace(txtIme.Text) || string.IsNullOrWhiteSpace(txtPrezime.Text)) return false;
            if (!int.TryParse(txtPasos.Text, out int pasos)) return false;
            if (string.IsNullOrWhiteSpace(txtEmail.Text) || !txtEmail.Text.Contains("@")) return false;
            if (string.IsNullOrWhiteSpace(txtTelefon.Text)) return false;
            passportPlain = txtPasos.Text.Trim();
            client = new TurističkaOrganizacija.Domain.Client
            {
                FirstName = txtIme.Text.Trim(),
                LastName = txtPrezime.Text.Trim(),
                PassportNumber = pasos,
                DateOfBirth = dtpRodjenje.Value.Date,
                Email = txtEmail.Text.Trim(),
                Phone = txtTelefon.Text.Trim()
            };
            IValidationRule chain = new RequiredFieldsRule();
            chain.SetNext(new EmailFormatRule());
            chain.Validate(client);
            return true;
        }


        private void RefreshClients()
        {
            var facade = BuildFacade();
            string ime = string.IsNullOrWhiteSpace(txtIme.Text) ? null : txtIme.Text.Trim();
            string prezime = string.IsNullOrWhiteSpace(txtPrezime.Text) ? null : txtPrezime.Text.Trim();
            string pasosLike = string.IsNullOrWhiteSpace(txtPasos.Text) ? null : txtPasos.Text.Trim();
            var baseSet = facade.SearchClients(ime, prezime, pasosLike);
            TurističkaOrganizacija.GUI.UiKitForm1.RefreshClientsAddon(
                clientsBinding,
                baseSet,
                txtEmail,
                txtTelefon,
                dtpRodjenje.Value.Date,
                dataGridView1);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
