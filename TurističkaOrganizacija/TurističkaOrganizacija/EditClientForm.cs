using System;
using System.Linq;
using System.Windows.Forms;
using TurističkaOrganizacija.Domain;
using TurističkaOrganizacija.Application;
using TurističkaOrganizacija.Application.Commands;
using TurističkaOrganizacija.Infrastructure.Repositories.SqlClient;
using TurističkaOrganizacija.GUI; // opcionalno za TextBoxPlaceholder

namespace TurističkaOrganizacija
{
    public partial class EditClientForm : Form
    {
        private readonly Client _original;
        private readonly ClientService _service;
        private readonly CommandInvoker _invoker;

        private TextBox txtIme = new TextBox();
        private TextBox txtPrezime = new TextBox();
        private TextBox txtEmail = new TextBox();
        private TextBox txtTelefon = new TextBox();
        private TextBox txtPasos = new TextBox();
        private DateTimePicker dtpRodjenje = new DateTimePicker { Format = DateTimePickerFormat.Short };

        private Button btnSave = new Button { Text = "Sačuvaj" };
        private Button btnCancel = new Button { Text = "Otkaži", DialogResult = DialogResult.Cancel };

        public EditClientForm(Client original, ClientService service, CommandInvoker invoker)
        {
            if (original == null) throw new ArgumentNullException(nameof(original));
            _original = original; _service = service; _invoker = invoker ?? new CommandInvoker();

            Text = "Izmena klijenta";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false; MinimizeBox = false; ShowInTaskbar = false;
            AcceptButton = btnSave; CancelButton = btnCancel;

            var grid = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, Padding = new Padding(12), AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink };
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            AddRow(grid, "Ime:", txtIme, 0);
            AddRow(grid, "Prezime:", txtPrezime, 1);
            AddRow(grid, "Email:", txtEmail, 2);
            AddRow(grid, "Telefon:", txtTelefon, 3);
            AddRow(grid, "Pasoš (9 cifara):", txtPasos, 4);
            AddRow(grid, "Datum rođenja:", dtpRodjenje, 5);

            var pnl = new FlowLayoutPanel { FlowDirection = FlowDirection.RightToLeft, Dock = DockStyle.Fill, AutoSize = true };
            pnl.Controls.Add(btnSave); pnl.Controls.Add(btnCancel);
            grid.Controls.Add(pnl, 0, 6); grid.SetColumnSpan(pnl, 2);
            Controls.Add(grid);

            // Prefill
            txtIme.Text = _original.FirstName;
            txtPrezime.Text = _original.LastName;
            txtEmail.Text = _original.Email;
            txtTelefon.Text = _original.Phone;
            txtPasos.Text = _original.PassportNumber.ToString();
            dtpRodjenje.Value = _original.DateOfBirth == default(DateTime) ? DateTime.Today : _original.DateOfBirth;

            try
            {
                TextBoxPlaceholder.SetPlaceholder(txtIme, "Ime");
                TextBoxPlaceholder.SetPlaceholder(txtPrezime, "Prezime");
                TextBoxPlaceholder.SetPlaceholder(txtEmail, "Email");
                TextBoxPlaceholder.SetPlaceholder(txtTelefon, "Telefon");
                TextBoxPlaceholder.SetPlaceholder(txtPasos, "Pasoš (9 cifara)");
            }
            catch { }

            btnSave.Click += (s, e) => Save();
        }

        private static void AddRow(TableLayoutPanel grid, string label, Control input, int row)
        {
            grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            var lbl = new Label { Text = label, AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(0, 6, 6, 0) };
            input.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            grid.Controls.Add(lbl, 0, row);
            grid.Controls.Add(input, 1, row);
        }

        private void Save()
        {
            if (string.IsNullOrWhiteSpace(txtIme.Text) || string.IsNullOrWhiteSpace(txtPrezime.Text))
            { MessageBox.Show("Ime i prezime su obavezni."); return; }
            var email = txtEmail.Text?.Trim();
            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
            { MessageBox.Show("Unesite ispravan email."); return; }
            var tel = txtTelefon.Text?.Trim();
            if (string.IsNullOrWhiteSpace(tel))
            { MessageBox.Show("Telefon je obavezan."); return; }
            int pasos;
            if (!int.TryParse(txtPasos.Text?.Trim(), out pasos) || pasos < 100000000 || pasos > 999999999)
            { MessageBox.Show("Pasoš mora biti 9-cifren broj."); return; }

            var updated = new Client
            {
                Id = _original.Id,
                FirstName = txtIme.Text.Trim(),
                LastName = txtPrezime.Text.Trim(),
                PassportNumber = pasos,
                DateOfBirth = dtpRodjenje.Value.Date,
                Email = email,
                Phone = tel
            };

            try
            {
                // Validacija: uniq pasoš, izuzmi trenutni Id
                var repo = new ClientRepositorySql();
                IValidationRule chain = new RequiredFieldsRule();
                chain.SetNext(new EmailFormatRule());
                chain.SetNext(new UniquePassportRule(repo, updated.Id));
                chain.Validate(updated);

                var cmd = new UpdateClientCommand(_original, updated, pasos.ToString(), _service);
                _invoker.ExecuteCommand(cmd);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}