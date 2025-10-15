using System;
using System.Linq;
using System.Windows.Forms;
using TurističkaOrganizacija.Application;
using TurističkaOrganizacija.Domain;
using TurističkaOrganizacija.Infrastructure.Repositories.SqlClient;
using TurističkaOrganizacija.Application.TemplateMethod;
using System.Drawing;
using TurističkaOrganizacija.GUI;

namespace TurističkaOrganizacija
{
    public class PackagesForm : Form
    {
        private DataGridView grid = new DataGridView();
        private Button btnSea = new Button();
        private Button btnMountain = new Button();
        private Button btnExcursion = new Button();
        private Button btnCruise = new Button();
        private TextBox txtNaziv = new TextBox();
        private NumericUpDown numCena = new NumericUpDown();
        private ComboBox cmbVrsta = new ComboBox();
        private TextBox txtDestinacija = new TextBox();
        private TextBox txtPrevoz = new TextBox();
        private TextBox txtSmestaj = new TextBox();
        private TextBox txtAktivnosti = new TextBox();
        private TextBox txtVodic = new TextBox();
        private NumericUpDown numTrajanje = new NumericUpDown();
        private TextBox txtBrod = new TextBox();
        private TextBox txtRuta = new TextBox();
        private DateTimePicker dtPolazak = new DateTimePicker();
        private TextBox txtKabina = new TextBox();
        private Button btnDodaj = new Button();
        private Button btnIzmeni = new Button();
        private Button btnObrisi = new Button();

        private PackageService service = new PackageService(new PackageRepositorySql());
        private PackageType currentPackageType = PackageType.Sea;

        public PackagesForm()
        {
            this.Text = "Paketi";
            this.Width = 1400;
            this.Height = 650;
            this.MinimumSize = new System.Drawing.Size(1000, 600);

            // Filter buttons row at top
            btnSea.Text = "More"; btnSea.Location = new System.Drawing.Point(12, 12); btnSea.Size = new System.Drawing.Size(80, 28); btnSea.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            btnMountain.Text = "Planina"; btnMountain.Location = new System.Drawing.Point(102, 12); btnMountain.Size = new System.Drawing.Size(80, 28); btnMountain.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            btnExcursion.Text = "Ekskurzija"; btnExcursion.Location = new System.Drawing.Point(192, 12); btnExcursion.Size = new System.Drawing.Size(90, 28); btnExcursion.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            btnCruise.Text = "Krstarenje"; btnCruise.Location = new System.Drawing.Point(292, 12); btnCruise.Size = new System.Drawing.Size(90, 28); btnCruise.Anchor = AnchorStyles.Top | AnchorStyles.Left;

            grid.Location = new System.Drawing.Point(12, 50);
            grid.Size = new System.Drawing.Size(1360, 340);
            grid.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

            int y = 420;
            int labelY = 400;
            
            // Labels
            var lblNaziv = new Label { Text = "Naziv *", Location = new System.Drawing.Point(12, labelY), AutoSize = true, Anchor = AnchorStyles.Bottom | AnchorStyles.Left };
            var lblCena = new Label { Text = "Cena *", Location = new System.Drawing.Point(200, labelY), AutoSize = true, Anchor = AnchorStyles.Bottom | AnchorStyles.Left };
            var lblVrsta = new Label { Text = "Vrsta *", Location = new System.Drawing.Point(310, labelY), AutoSize = true, Anchor = AnchorStyles.Bottom | AnchorStyles.Left };
            var lblDestinacija = new Label { Text = "Destinacija", Location = new System.Drawing.Point(440, labelY), AutoSize = true, Anchor = AnchorStyles.Bottom | AnchorStyles.Left, Tag = "Destinacija" };
            var lblPrevoz = new Label { Text = "Prevoz", Location = new System.Drawing.Point(600, labelY), AutoSize = true, Anchor = AnchorStyles.Bottom | AnchorStyles.Left, Tag = "Prevoz" };
            var lblSmestaj = new Label { Text = "Smeštaj", Location = new System.Drawing.Point(730, labelY), AutoSize = true, Anchor = AnchorStyles.Bottom | AnchorStyles.Left, Tag = "Smestaj" };
            var lblAktivnosti = new Label { Text = "Aktivnosti (zarez)", Location = new System.Drawing.Point(900, labelY), AutoSize = true, Anchor = AnchorStyles.Bottom | AnchorStyles.Left, Tag = "Aktivnosti" };
            var lblVodic = new Label { Text = "Vodič", Location = new System.Drawing.Point(12, labelY + 60), AutoSize = true, Anchor = AnchorStyles.Bottom | AnchorStyles.Left, Tag = "Vodic" };
            var lblTrajanje = new Label { Text = "Trajanje (dana)", Location = new System.Drawing.Point(200, labelY + 60), AutoSize = true, Anchor = AnchorStyles.Bottom | AnchorStyles.Left, Tag = "Trajanje" };
            var lblBrod = new Label { Text = "Brod", Location = new System.Drawing.Point(310, labelY + 60), AutoSize = true, Anchor = AnchorStyles.Bottom | AnchorStyles.Left, Tag = "Brod" };
            var lblRuta = new Label { Text = "Ruta", Location = new System.Drawing.Point(440, labelY + 60), AutoSize = true, Anchor = AnchorStyles.Bottom | AnchorStyles.Left, Tag = "Ruta" };
            var lblPolazak = new Label { Text = "Polazak", Location = new System.Drawing.Point(600, labelY + 60), AutoSize = true, Anchor = AnchorStyles.Bottom | AnchorStyles.Left, Tag = "Polazak" };
            var lblKabina = new Label { Text = "Kabina", Location = new System.Drawing.Point(730, labelY + 60), AutoSize = true, Anchor = AnchorStyles.Bottom | AnchorStyles.Left, Tag = "Kabina" };
            
            // Input controls
            txtNaziv.Location = new System.Drawing.Point(12, y); txtNaziv.Width = 180; txtNaziv.Name = "Naziv"; txtNaziv.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            numCena.Location = new System.Drawing.Point(200, y); numCena.Width = 100; numCena.Maximum = 1000000000; numCena.DecimalPlaces = 2; numCena.Name = "Cena"; numCena.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            cmbVrsta.Location = new System.Drawing.Point(310, y); cmbVrsta.Width = 120; cmbVrsta.DropDownStyle = ComboBoxStyle.DropDownList; cmbVrsta.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            cmbVrsta.Items.AddRange(new object[] { "more", "planina", "ekskurzija", "krstarenje" });
            cmbVrsta.SelectedItem = "more";
            txtDestinacija.Location = new System.Drawing.Point(440, y); txtDestinacija.Width = 150; txtDestinacija.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            txtPrevoz.Location = new System.Drawing.Point(600, y); txtPrevoz.Width = 120; txtPrevoz.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            txtSmestaj.Location = new System.Drawing.Point(730, y); txtSmestaj.Width = 150; txtSmestaj.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            txtAktivnosti.Location = new System.Drawing.Point(900, y); txtAktivnosti.Width = 200; txtAktivnosti.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            txtVodic.Location = new System.Drawing.Point(12, y + 60); txtVodic.Width = 180; txtVodic.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            numTrajanje.Location = new System.Drawing.Point(200, y + 60); numTrajanje.Width = 100; numTrajanje.Maximum = 1000; numTrajanje.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            txtBrod.Location = new System.Drawing.Point(310, y + 60); txtBrod.Width = 120; txtBrod.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            txtRuta.Location = new System.Drawing.Point(440, y + 60); txtRuta.Width = 150; txtRuta.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            dtPolazak.Location = new System.Drawing.Point(600, y + 60); dtPolazak.Width = 120; dtPolazak.Anchor = AnchorStyles.Bottom | AnchorStyles.Left; dtPolazak.Format = DateTimePickerFormat.Short;
            txtKabina.Location = new System.Drawing.Point(730, y + 60); txtKabina.Width = 150; txtKabina.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;

            btnDodaj.Text = "Dodaj"; btnDodaj.Location = new System.Drawing.Point(12, 470); btnDodaj.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnIzmeni.Text = "Izmeni"; btnIzmeni.Location = new System.Drawing.Point(92, 470); btnIzmeni.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnObrisi.Text = "Obriši"; btnObrisi.Location = new System.Drawing.Point(172, 470); btnObrisi.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;

            this.Controls.Add(btnSea);
            this.Controls.Add(btnMountain);
            this.Controls.Add(btnExcursion);
            this.Controls.Add(btnCruise);
            this.Controls.Add(grid);
            this.Controls.Add(lblNaziv);
            this.Controls.Add(lblCena);
            this.Controls.Add(lblVrsta);
            this.Controls.Add(lblDestinacija);
            this.Controls.Add(lblPrevoz);
            this.Controls.Add(lblSmestaj);
            this.Controls.Add(lblAktivnosti);
            this.Controls.Add(txtNaziv);
            this.Controls.Add(numCena);
            this.Controls.Add(cmbVrsta);
            this.Controls.Add(txtDestinacija);
            this.Controls.Add(txtPrevoz);
            this.Controls.Add(txtSmestaj);
            this.Controls.Add(txtAktivnosti);
            this.Controls.Add(lblVodic);
            this.Controls.Add(lblTrajanje);
            this.Controls.Add(lblBrod);
            this.Controls.Add(lblRuta);
            this.Controls.Add(lblPolazak);
            this.Controls.Add(lblKabina);
            this.Controls.Add(txtVodic);
            this.Controls.Add(numTrajanje);
            this.Controls.Add(txtBrod);
            this.Controls.Add(txtRuta);
            this.Controls.Add(dtPolazak);
            this.Controls.Add(txtKabina);
            this.Controls.Add(btnDodaj);
            this.Controls.Add(btnIzmeni);
            this.Controls.Add(btnObrisi);

            LoadData();
            WireEvents();


            UiKitPackagesForm.PackagesDesign(txtDestinacija, txtPrevoz, txtSmestaj, txtNaziv);
            // Grid changes its size according to content
            UiKit.WireAutoSizeGrid(grid, this, maxW: 0.9, maxH: 0.6);
            //form bc color
            UiKit.StyleForm(this, ColorTranslator.FromHtml("#b8aa5c"), 0.96);
            //cells bc color
            UiKitPackagesForm.ApplyCategoryColors(
            grid,
            baseCellBackColor: ColorTranslator.FromHtml("#1f2937"),
            baseFontColor: Color.White,
            baseGridLineColor: Color.Black,
            keepSelectionHighlight: false,
            categoryColumnName: "Type");

            UiKit.StyleButtons(ColorTranslator.FromHtml("#e0c00d"), 96, btnDodaj, btnIzmeni, btnObrisi);
            UiKit.StyleButtons(ColorTranslator.FromHtml("#e0c00d"), 80, btnSea, btnMountain, btnExcursion, btnCruise);

            UpdateInputVisibility();
        }

        private void LoadData()
        {
            LoadPackagesByType(currentPackageType);
        }

        private void WireEvents()
        {
            btnDodaj.Click += (s, e) => AddPackage();
            btnIzmeni.Click += (s, e) => UpdatePackage();
            btnObrisi.Click += (s, e) => DeletePackage();
            cmbVrsta.SelectedIndexChanged += (s, e) => UpdateInputVisibility();
            btnSea.Click += (s, e) => { currentPackageType = PackageType.Sea; LoadPackagesByType(currentPackageType); };
            btnMountain.Click += (s, e) => { currentPackageType = PackageType.Mountain; LoadPackagesByType(currentPackageType); };
            btnExcursion.Click += (s, e) => { currentPackageType = PackageType.Excursion; LoadPackagesByType(currentPackageType); };
            btnCruise.Click += (s, e) => { currentPackageType = PackageType.Cruise; LoadPackagesByType(currentPackageType); };

            TurističkaOrganizacija.Application.EventBus.PackagesChanged += () =>
            {
                try { LoadData(); } catch { }
            };
        }

        private void UpdateInputVisibility()
        {
            string vrsta = cmbVrsta.SelectedItem as string;
            bool isSea = vrsta == "more";
            bool isMountain = vrsta == "planina";
            bool isExcursion = vrsta == "ekskurzija";
            bool isCruise = vrsta == "krstarenje";

            txtDestinacija.Visible = isSea || isMountain || isExcursion;
            txtPrevoz.Visible = isSea || isMountain || isExcursion;
            txtSmestaj.Visible = isSea || isMountain;
            txtAktivnosti.Visible = isMountain;
            txtVodic.Visible = isExcursion;
            numTrajanje.Visible = isExcursion;
            txtBrod.Visible = isCruise;
            txtRuta.Visible = isCruise;
            dtPolazak.Visible = isCruise;
            txtKabina.Visible = isCruise;

            foreach (Control c in this.Controls)
            {
                if (c is Label lbl && lbl.Tag is string tag)
                {
                    if (tag == "Destinacija") lbl.Visible = txtDestinacija.Visible;
                    else if (tag == "Prevoz") lbl.Visible = txtPrevoz.Visible;
                    else if (tag == "Smestaj") lbl.Visible = txtSmestaj.Visible;
                    else if (tag == "Aktivnosti") lbl.Visible = txtAktivnosti.Visible;
                    else if (tag == "Vodic") lbl.Visible = txtVodic.Visible;
                    else if (tag == "Trajanje") lbl.Visible = numTrajanje.Visible;
                    else if (tag == "Brod") lbl.Visible = txtBrod.Visible;
                    else if (tag == "Ruta") lbl.Visible = txtRuta.Visible;
                    else if (tag == "Polazak") lbl.Visible = dtPolazak.Visible;
                    else if (tag == "Kabina") lbl.Visible = txtKabina.Visible;
                }
            }

            // Lay out visible fields in a single row from left to right
            int labelY = 400; // match constructor positions
            int inputY = 420;
            int x = 440; // start after Vrsta
            int spacing = 16;

            void Place(Label lbl, Control input)
            {
                if (lbl.Visible && input.Visible)
                {
                    lbl.Location = new System.Drawing.Point(x, labelY);
                    input.Location = new System.Drawing.Point(x, inputY);
                    int w = input.Width;
                    if (w <= 0) w = 120;
                    x += w + spacing;
                }
            }

            Label FindLabel(string tagName)
            {
                foreach (Control c in this.Controls)
                {
                    if (c is Label l && l.Tag is string t && t == tagName) return l;
                }
                return null;
            }

            // Order per type
            if (isSea)
            {
                Place(FindLabel("Destinacija"), txtDestinacija);
                Place(FindLabel("Prevoz"), txtPrevoz);
                Place(FindLabel("Smestaj"), txtSmestaj);
            }
            else if (isMountain)
            {
                Place(FindLabel("Destinacija"), txtDestinacija);
                Place(FindLabel("Prevoz"), txtPrevoz);
                Place(FindLabel("Smestaj"), txtSmestaj);
                Place(FindLabel("Aktivnosti"), txtAktivnosti);
            }
            else if (isExcursion)
            {
                Place(FindLabel("Destinacija"), txtDestinacija);
                Place(FindLabel("Prevoz"), txtPrevoz);
                Place(FindLabel("Vodic"), txtVodic);
                Place(FindLabel("Trajanje"), numTrajanje);
            }
            else if (isCruise)
            {
                Place(FindLabel("Brod"), txtBrod);
                Place(FindLabel("Ruta"), txtRuta);
                Place(FindLabel("Polazak"), dtPolazak);
                Place(FindLabel("Kabina"), txtKabina);
            }
        }

        private void LoadPackagesByType(PackageType type)
        {
            var list = service.GetAll(type).ToList();

            grid.AutoGenerateColumns = false;
            grid.Columns.Clear();

            // Keep a hidden Type column so existing styling by category still works
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(TravelPackage.Type), HeaderText = "Vrsta", Visible = false });

            // Common columns
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(TravelPackage.Name), HeaderText = "Naziv" });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(TravelPackage.Price), HeaderText = "Cena" });

            if (type == PackageType.Sea)
            {
                grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(TravelPackage.Destination), HeaderText = "Destinacija" });
                grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(TravelPackage.TransportType), HeaderText = "Prevoz" });
                grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(TravelPackage.AccommodationType), HeaderText = "Smeštaj" });
            }
            else if (type == PackageType.Mountain)
            {
                grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(TravelPackage.Destination), HeaderText = "Destinacija" });
                grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(TravelPackage.TransportType), HeaderText = "Prevoz" });
                grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(TravelPackage.AccommodationType), HeaderText = "Smeštaj" });
                
                grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Aktivnosti", HeaderText = "Aktivnosti" });
            }
            else if (type == PackageType.Excursion)
            {
                grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(TravelPackage.Destination), HeaderText = "Destinacija" });
                grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(TravelPackage.TransportType), HeaderText = "Prevoz" });
                grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(TravelPackage.GuideName), HeaderText = "Vodič" });
                grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(TravelPackage.DurationDays), HeaderText = "Trajanje (dana)" });
            }
            else if (type == PackageType.Cruise)
            {
                grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(TravelPackage.ShipName), HeaderText = "Brod" });
                grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(TravelPackage.Route), HeaderText = "Ruta" });
                grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(TravelPackage.DepartureDate), HeaderText = "Polazak" });
                grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(TravelPackage.CabinType), HeaderText = "Kabina" });
            }

            grid.DataSource = list;

            
            grid.CellFormatting -= Grid_CellFormatting;
            grid.CellFormatting += Grid_CellFormatting;
        }

        private void Grid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (grid.Columns[e.ColumnIndex].Name != "Aktivnosti") return;
            var item = grid.Rows[e.RowIndex].DataBoundItem as MountainPackage;
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

        private void AddPackage()
        {
            var p = BuildPackageFromInputs();
            if (p == null) return;

            var processor = PackageProcessorFactory.CreateProcessor(p.Type);
            processor.ProcessPackage(p);
            service.Add(p);
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
            service.Update(p);
        }

        private void DeletePackage()
        {
            if (grid.CurrentRow == null) return;
            var selected = grid.CurrentRow.DataBoundItem as TravelPackage;
            if (selected == null) return;
            service.Delete(selected.Id);
        }

        private TravelPackage BuildPackageFromInputs()
        {
            if (string.IsNullOrWhiteSpace(txtNaziv.Text) || cmbVrsta.SelectedItem == null)
            {
                MessageBox.Show("Naziv i vrsta su obavezni.");
                return null;
            }
            string vrsta = cmbVrsta.SelectedItem.ToString();
            PackageType type = PackageType.Sea;
            if (vrsta == "more") type = PackageType.Sea;
            else if (vrsta == "planina") type = PackageType.Mountain;
            else if (vrsta == "ekskurzija") type = PackageType.Excursion;
            else if (vrsta == "krstarenje") type = PackageType.Cruise;

            var factory = PackageFactory.GetFactory(type);
            var p = factory.CreatePackage(
                txtNaziv.Text.Trim(),
                numCena.Value,
                txtDestinacija.Text.Trim(),
                txtPrevoz.Text.Trim(),
                txtSmestaj.Text.Trim());
            if (type == PackageType.Mountain && p is MountainPackage mp)
            {
                var raw = (txtAktivnosti.Text ?? string.Empty).Trim();
                mp.Activities = raw.Length == 0 ? new System.Collections.Generic.List<string>() : raw.Split(',').Select(a => a.Trim()).Where(a => a.Length > 0).ToList();
            }
            else if (type == PackageType.Excursion)
            {
                p.GuideName = (txtVodic.Text ?? string.Empty).Trim();
                p.DurationDays = (int)numTrajanje.Value;
            }
            else if (type == PackageType.Cruise)
            {
                p.ShipName = (txtBrod.Text ?? string.Empty).Trim();
                p.Route = (txtRuta.Text ?? string.Empty).Trim();
                p.DepartureDate = dtPolazak.Value.Date;
                p.CabinType = (txtKabina.Text ?? string.Empty).Trim();
            }
            return p;
        }
    }
}


