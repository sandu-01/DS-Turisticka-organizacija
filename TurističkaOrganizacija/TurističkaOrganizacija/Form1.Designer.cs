namespace TurističkaOrganizacija
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.txtIme = new System.Windows.Forms.TextBox();
            this.txtPrezime = new System.Windows.Forms.TextBox();
            this.txtPasos = new System.Windows.Forms.TextBox();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.txtTelefon = new System.Windows.Forms.TextBox();
            this.dtpRodjenje = new System.Windows.Forms.DateTimePicker();
            this.btnDodaj = new System.Windows.Forms.Button();
            this.btnIzmeni = new System.Windows.Forms.Button();
            this.btnObrisi = new System.Windows.Forms.Button();
            this.btnOsvezi = new System.Windows.Forms.Button();
            this.btnPaketi = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 700);
            this.MinimumSize = new System.Drawing.Size(900, 700);
            // dataGridView
            this.dataGridView1.Location = new System.Drawing.Point(12, 30);
            this.dataGridView1.Size = new System.Drawing.Size(860, 400);
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            // Ime
            this.txtIme.Location = new System.Drawing.Point(12, 460);
            this.txtIme.Width = 140;
            this.txtIme.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            // Prezime
            this.txtPrezime.Location = new System.Drawing.Point(160, 460);
            this.txtPrezime.Width = 140;
            this.txtPrezime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            // Pasos
            this.txtPasos.Location = new System.Drawing.Point(308, 460);
            this.txtPasos.Width = 120;
            this.txtPasos.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            // Email
            this.txtEmail.Location = new System.Drawing.Point(12, 500);
            this.txtEmail.Width = 280;
            this.txtEmail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            // Telefon
            this.txtTelefon.Location = new System.Drawing.Point(300, 500);
            this.txtTelefon.Width = 128;
            this.txtTelefon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            // Datum
            this.dtpRodjenje.Location = new System.Drawing.Point(12, 540);
            this.dtpRodjenje.Width = 200;
            this.dtpRodjenje.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            // Dugmad
            this.btnDodaj.Location = new System.Drawing.Point(12, 580);
            this.btnDodaj.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDodaj.Text = "Dodaj";
            this.btnIzmeni.Location = new System.Drawing.Point(92, 580);
            this.btnIzmeni.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnIzmeni.Text = "Izmeni";
            this.btnObrisi.Location = new System.Drawing.Point(172, 580);
            this.btnObrisi.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnObrisi.Text = "Obriši";
            this.btnOsvezi.Location = new System.Drawing.Point(252, 580);
            this.btnOsvezi.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOsvezi.Text = "Osveži";
            // Paketi
            this.btnPaketi.Location = new System.Drawing.Point(332, 580);
            this.btnPaketi.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPaketi.Text = "Paketi";
            // Agency name label
            var lblAgencyName = new System.Windows.Forms.Label(); 
            lblAgencyName.Text = "Naziv agencije: " + TurističkaOrganizacija.Backend.AppConfig.Instance.AgencyName; 
            lblAgencyName.Location = new System.Drawing.Point(12, 2); 
            lblAgencyName.AutoSize = true; 
            lblAgencyName.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold); 
            lblAgencyName.ForeColor = System.Drawing.Color.DarkBlue;
            lblAgencyName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left))); 
            this.Controls.Add(lblAgencyName);
            
            // Labels
            var lblIme = new System.Windows.Forms.Label(); lblIme.Text = "Ime *"; lblIme.Location = new System.Drawing.Point(12, 442); lblIme.AutoSize = true; lblIme.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left))); this.Controls.Add(lblIme);
            var lblPrezime = new System.Windows.Forms.Label(); lblPrezime.Text = "Prezime *"; lblPrezime.Location = new System.Drawing.Point(160, 442); lblPrezime.AutoSize = true; lblPrezime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left))); this.Controls.Add(lblPrezime);
            var lblPasos = new System.Windows.Forms.Label(); lblPasos.Text = "Pasoš *"; lblPasos.Location = new System.Drawing.Point(308, 442); lblPasos.AutoSize = true; lblPasos.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left))); this.Controls.Add(lblPasos);
            var lblEmail = new System.Windows.Forms.Label(); lblEmail.Text = "Email *"; lblEmail.Location = new System.Drawing.Point(12, 482); lblEmail.AutoSize = true; lblEmail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left))); this.Controls.Add(lblEmail);
            var lblTelefon = new System.Windows.Forms.Label(); lblTelefon.Text = "Telefon *"; lblTelefon.Location = new System.Drawing.Point(300, 482); lblTelefon.AutoSize = true; lblTelefon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left))); this.Controls.Add(lblTelefon);
            var lblDatum = new System.Windows.Forms.Label(); lblDatum.Text = "Datum rođenja"; lblDatum.Location = new System.Drawing.Point(12, 522); lblDatum.AutoSize = true; lblDatum.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left))); this.Controls.Add(lblDatum);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.txtIme);
            this.Controls.Add(this.txtPrezime);
            this.Controls.Add(this.txtPasos);
            this.Controls.Add(this.txtEmail);
            this.Controls.Add(this.txtTelefon);
            this.Controls.Add(this.dtpRodjenje);
            this.Controls.Add(this.btnDodaj);
            this.Controls.Add(this.btnIzmeni);
            this.Controls.Add(this.btnObrisi);
            this.Controls.Add(this.btnOsvezi);
            this.Controls.Add(this.btnPaketi);
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TextBox txtIme;
        private System.Windows.Forms.TextBox txtPrezime;
        private System.Windows.Forms.TextBox txtPasos;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.TextBox txtTelefon;
        private System.Windows.Forms.DateTimePicker dtpRodjenje;
        private System.Windows.Forms.Button btnDodaj;
        private System.Windows.Forms.Button btnIzmeni;
        private System.Windows.Forms.Button btnObrisi;
        private System.Windows.Forms.Button btnOsvezi;
        private System.Windows.Forms.Button btnPaketi;
    }
}

