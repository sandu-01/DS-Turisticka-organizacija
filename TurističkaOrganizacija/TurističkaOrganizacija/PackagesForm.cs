using System;
using System.Linq;
using System.Windows.Forms;
using TurističkaOrganizacija.Application;
using TurističkaOrganizacija.Domain;
using TurističkaOrganizacija.Infrastructure.Repositories.SqlClient;
using TurističkaOrganizacija.Application.TemplateMethod;

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
            this.Width = 1400;
            this.Height = 650;
            this.MinimumSize = new System.Drawing.Size(1000, 600);

            grid.Location = new System.Drawing.Point(12, 12);
            grid.Size = new System.Drawing.Size(1360, 380);
            grid.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

            int y = 420;
            int labelY = 400;
            
            // Labels
            var lblNaziv = new Label { Text = "Naziv *", Location = new System.Drawing.Point(12, labelY), AutoSize = true, Anchor = AnchorStyles.Bottom | AnchorStyles.Left };
            var lblCena = new Label { Text = "Cena *", Location = new System.Drawing.Point(200, labelY), AutoSize = true, Anchor = AnchorStyles.Bottom | AnchorStyles.Left };
            var lblVrsta = new Label { Text = "Vrsta *", Location = new System.Drawing.Point(310, labelY), AutoSize = true, Anchor = AnchorStyles.Bottom | AnchorStyles.Left };
            var lblDestinacija = new Label { Text = "Destinacija", Location = new System.Drawing.Point(440, labelY), AutoSize = true, Anchor = AnchorStyles.Bottom | AnchorStyles.Left };
            var lblPrevoz = new Label { Text = "Prevoz", Location = new System.Drawing.Point(600, labelY), AutoSize = true, Anchor = AnchorStyles.Bottom | AnchorStyles.Left };
            var lblSmestaj = new Label { Text = "Smeštaj", Location = new System.Drawing.Point(730, labelY), AutoSize = true, Anchor = AnchorStyles.Bottom | AnchorStyles.Left };
            
            // Input controls
            txtNaziv.Location = new System.Drawing.Point(12, y); txtNaziv.Width = 180; txtNaziv.Name = "Naziv"; txtNaziv.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            numCena.Location = new System.Drawing.Point(200, y); numCena.Width = 100; numCena.Maximum = 1000000000; numCena.DecimalPlaces = 2; numCena.Name = "Cena"; numCena.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            cmbVrsta.Location = new System.Drawing.Point(310, y); cmbVrsta.Width = 120; cmbVrsta.DropDownStyle = ComboBoxStyle.DropDownList; cmbVrsta.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            cmbVrsta.Items.AddRange(new object[] { "more", "planina", "ekskurzija", "krstarenje" });
            txtDestinacija.Location = new System.Drawing.Point(440, y); txtDestinacija.Width = 150; txtDestinacija.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            txtPrevoz.Location = new System.Drawing.Point(600, y); txtPrevoz.Width = 120; txtPrevoz.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            txtSmestaj.Location = new System.Drawing.Point(730, y); txtSmestaj.Width = 150; txtSmestaj.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;

            btnDodaj.Text = "Dodaj"; btnDodaj.Location = new System.Drawing.Point(12, 470); btnDodaj.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnIzmeni.Text = "Izmeni"; btnIzmeni.Location = new System.Drawing.Point(92, 470); btnIzmeni.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnObrisi.Text = "Obriši"; btnObrisi.Location = new System.Drawing.Point(172, 470); btnObrisi.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;

            this.Controls.Add(grid);
            this.Controls.Add(lblNaziv);
            this.Controls.Add(lblCena);
            this.Controls.Add(lblVrsta);
            this.Controls.Add(lblDestinacija);
            this.Controls.Add(lblPrevoz);
            this.Controls.Add(lblSmestaj);
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

            TurističkaOrganizacija.Application.EventBus.PackagesChanged += () =>
            {
                try { LoadData(); } catch { }
            };
        }

        private void AddPackage()
        {
            var p = BuildPackageFromInputs();
            if (p == null) return;

            var processor = PackageProcessorFactory.CreateProcessor(p.Type);
            processor.ProcessPackage(p);
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
            var processor = PackageProcessorFactory.CreateProcessor(p.Type);
            processor.ProcessPackage(p);
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


