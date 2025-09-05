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

namespace TurističkaOrganizacija
{
    public partial class Form1 : Form
    {
        private readonly Timer backupTimer = new Timer();
        private BindingSource clientsBinding = new BindingSource();

        public Form1()
        {
            InitializeComponent();
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

            // Load clients into grid as MVP
            var repo = new ClientRepositorySql();
            var service = new ClientService(repo, new TurističkaOrganizacija.Infrastructure.Security.SecurityService());
            var clients = service.Search(null, null, null);
            clientsBinding.DataSource = clients;
            dataGridView1.DataSource = clientsBinding;

            // Wire buttons
            btnDodaj.Click += (s, e) => AddClient();
            btnIzmeni.Click += (s, e) => UpdateClient();
            btnObrisi.Click += (s, e) => DeleteClient();
            btnOsvezi.Click += (s, e) => RefreshClients();
            btnPaketi.Click += (s, e) =>
            {
                using (var f = new PackagesForm())
                {
                    f.ShowDialog(this);
                }
            };

            // Double-click opens reservation form
            dataGridView1.DoubleClick += (s, e) =>
            {
                using (var f = new ReservationForm())
                {
                    f.ShowDialog(this);
                }
            };
        }

        private ClientService BuildClientService()
        {
            var repo = new ClientRepositorySql();
            return new ClientService(repo, new TurističkaOrganizacija.Infrastructure.Security.SecurityService());
        }

        

        private void AddClient()
        {
            if (!ValidateInputs(out var client, out var passportPlain))
            {
                MessageBox.Show("Popunite obavezna polja: Ime, Prezime, Pasoš, Email, Telefon.", "Validacija", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                var repo = new ClientRepositorySql();
                IValidationRule chain = new RequiredFieldsRule();
                chain.SetNext(new EmailFormatRule());
                chain.SetNext(new UniquePassportRule(repo, null));
                chain.Validate(client);

                var service = new ClientService(repo, new TurističkaOrganizacija.Infrastructure.Security.SecurityService());
                service.Create(client, passportPlain);
                MessageBox.Show("Klijent uspešno dodat.", "Uspeh", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            RefreshClients();
        }

        private void UpdateClient()
        {
            if (dataGridView1.CurrentRow == null) return;
            if (!ValidateInputs(out var client, out var passportPlain))
            {
                MessageBox.Show("Popunite obavezna polja i označite klijenta.", "Validacija", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (dataGridView1.CurrentRow.DataBoundItem is TurističkaOrganizacija.Domain.Client selected)
            {
                try
                {
                    client.Id = selected.Id;
                    var repo = new ClientRepositorySql();
                    IValidationRule chain = new RequiredFieldsRule();
                    chain.SetNext(new EmailFormatRule());
                    chain.SetNext(new UniquePassportRule(repo, client.Id));
                    chain.Validate(client);

                    var service = new ClientService(repo, new TurističkaOrganizacija.Infrastructure.Security.SecurityService());
                    service.Update(client, passportPlain);
                    MessageBox.Show("Klijent uspešno izmenjen.", "Uspeh", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
                RefreshClients();
            }
        }

        private void DeleteClient()
        {
            if (dataGridView1.CurrentRow == null) return;
            if (dataGridView1.CurrentRow.DataBoundItem is TurističkaOrganizacija.Domain.Client selected)
            {
                var service = BuildClientService();
                service.Delete(selected.Id);
                RefreshClients();
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
            // Validation chain
            IValidationRule chain = new RequiredFieldsRule();
            chain.SetNext(new EmailFormatRule());
            chain.Validate(client);
            return true;
        }

        // Search filters on refresh
        private void RefreshClients()
        {
            var service = BuildClientService();
            string ime = string.IsNullOrWhiteSpace(txtIme.Text) ? null : txtIme.Text.Trim();
            string prezime = string.IsNullOrWhiteSpace(txtPrezime.Text) ? null : txtPrezime.Text.Trim();
            string pasosLike = string.IsNullOrWhiteSpace(txtPasos.Text) ? null : txtPasos.Text.Trim();
            clientsBinding.DataSource = service.Search(ime, prezime, pasosLike);
        }
    }
}
