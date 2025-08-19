using System;
using System.Linq;
using System.Windows.Forms;
using TurističkaOrganizacija.Application;
using TurističkaOrganizacija.Domain;
using TurističkaOrganizacija.Infrastructure.Repositories.SqlClient;

namespace TurističkaOrganizacija
{
    public class PackagesForm : Form
    {
        private DataGridView grid = new DataGridView();
        private TextBox txtNaziv = new TextBox();
        private NumericUpDown numCena = new NumericUpDown();
        private ComboBox cmbVrsta = new ComboBox();
        private TextBox txtDestinacija = new TextBox();
        private TextBox txtPrevoz = new TextBox();
        private TextBox txtSmestaj = new TextBox();
        private Button btnDodaj = new Button();
        private Button btnIzmeni = new Button();
        private Button btnObrisi = new Button();

        private PackageService service = new PackageService(new PackageRepositorySql());

        public PackagesForm()
        {
            this.Text = "Paketi";
            this.Width = 1000;
            this.Height = 650;

            grid.Location = new System.Drawing.Point(12, 12);
            grid.Size = new System.Drawing.Size(960, 420);
            grid.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

            int y = 450;
            txtNaziv.Location = new System.Drawing.Point(12, y); txtNaziv.Width = 180; txtNaziv.Name = "Naziv";
            numCena.Location = new System.Drawing.Point(200, y); numCena.Width = 100; numCena.Maximum = 1000000000; numCena.DecimalPlaces = 2; numCena.Name = "Cena";
            cmbVrsta.Location = new System.Drawing.Point(310, y); cmbVrsta.Width = 120; cmbVrsta.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbVrsta.Items.AddRange(new object[] { "more", "planina", "ekskurzija", "krstarenje" });
            txtDestinacija.Location = new System.Drawing.Point(440, y); txtDestinacija.Width = 150;
            txtPrevoz.Location = new System.Drawing.Point(600, y); txtPrevoz.Width = 120;
            txtSmestaj.Location = new System.Drawing.Point(730, y); txtSmestaj.Width = 150;

            btnDodaj.Text = "Dodaj"; btnDodaj.Location = new System.Drawing.Point(12, 500);
            btnIzmeni.Text = "Izmeni"; btnIzmeni.Location = new System.Drawing.Point(92, 500);
            btnObrisi.Text = "Obriši"; btnObrisi.Location = new System.Drawing.Point(172, 500);

            this.Controls.Add(grid);
            this.Controls.Add(txtNaziv);
            this.Controls.Add(numCena);
            this.Controls.Add(cmbVrsta);
            this.Controls.Add(txtDestinacija);
            this.Controls.Add(txtPrevoz);
            this.Controls.Add(txtSmestaj);
            this.Controls.Add(btnDodaj);
            this.Controls.Add(btnIzmeni);
            this.Controls.Add(btnObrisi);

            LoadData();
            WireEvents();
        }

        private void LoadData()
        {
            grid.DataSource = service.GetAll(null).ToList();
        }

        private void WireEvents()
        {
            btnDodaj.Click += (s, e) => AddPackage();
            btnIzmeni.Click += (s, e) => UpdatePackage();
            btnObrisi.Click += (s, e) => DeletePackage();
        }

        private void AddPackage()
        {
            var p = BuildPackageFromInputs();
            if (p == null) return;
            new PackageRepositorySql().Add(p);
            LoadData();
        }

        private void UpdatePackage()
        {
            if (grid.CurrentRow == null) return;
            var selected = grid.CurrentRow.DataBoundItem as TravelPackage;
            if (selected == null) return;
            var p = BuildPackageFromInputs();
            if (p == null) return;
            p.Id = selected.Id;
            new PackageRepositorySql().Update(p);
            LoadData();
        }

        private void DeletePackage()
        {
            if (grid.CurrentRow == null) return;
            var selected = grid.CurrentRow.DataBoundItem as TravelPackage;
            if (selected == null) return;
            new PackageRepositorySql().Delete(selected.Id);
            LoadData();
        }

        private TravelPackage BuildPackageFromInputs()
        {
            if (string.IsNullOrWhiteSpace(txtNaziv.Text) || cmbVrsta.SelectedItem == null)
            {
                MessageBox.Show("Naziv i vrsta su obavezni.");
                return null;
            }
            string vrsta = cmbVrsta.SelectedItem.ToString();
            TravelPackage p;
            switch (vrsta)
            {
                case "more": p = new SeaPackage(); break;
                case "planina": p = new MountainPackage(); break;
                case "ekskurzija": p = new ExcursionPackage(); break;
                case "krstarenje": p = new CruisePackage(); break;
                default: p = new SeaPackage(); break;
            }
            p.Name = txtNaziv.Text.Trim();
            p.Price = numCena.Value;
            p.Destination = txtDestinacija.Text.Trim();
            p.TransportType = txtPrevoz.Text.Trim();
            p.AccommodationType = txtSmestaj.Text.Trim();
            return p;
        }
    }
}


