namespace EnterpriseApp.Forms
{
    partial class FrmProdukte
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dgvProdukte = new System.Windows.Forms.DataGridView();
            this.panelFilter = new System.Windows.Forms.Panel();
            this.chkNurAktive = new System.Windows.Forms.CheckBox();
            this.cboKategorie = new System.Windows.Forms.ComboBox();
            this.lblKategorie = new System.Windows.Forms.Label();
            this.btnSuchen = new System.Windows.Forms.Button();
            this.txtSuche = new System.Windows.Forms.TextBox();
            this.lblSuche = new System.Windows.Forms.Label();
            this.panelDetail = new System.Windows.Forms.Panel();
            this.numLagerbestand = new System.Windows.Forms.NumericUpDown();
            this.lblLagerbestand = new System.Windows.Forms.Label();
            this.numMindestbestand = new System.Windows.Forms.NumericUpDown();
            this.lblMindestbestand = new System.Windows.Forms.Label();
            this.chkAktiv = new System.Windows.Forms.CheckBox();
            this.txtBeschreibung = new System.Windows.Forms.TextBox();
            this.lblBeschreibung = new System.Windows.Forms.Label();
            this.numEKPreis = new System.Windows.Forms.NumericUpDown();
            this.lblEKPreis = new System.Windows.Forms.Label();
            this.numVKPreis = new System.Windows.Forms.NumericUpDown();
            this.lblVKPreis = new System.Windows.Forms.Label();
            this.txtEinheit = new System.Windows.Forms.TextBox();
            this.lblEinheit = new System.Windows.Forms.Label();
            this.cboProduktKategorie = new System.Windows.Forms.ComboBox();
            this.lblProduktKategorie = new System.Windows.Forms.Label();
            this.txtProduktName = new System.Windows.Forms.TextBox();
            this.lblProduktName = new System.Windows.Forms.Label();
            this.txtArtikelnummer = new System.Windows.Forms.TextBox();
            this.lblArtikelnummer = new System.Windows.Forms.Label();
            this.panelButtons = new System.Windows.Forms.Panel();
            this.btnSchliessen = new System.Windows.Forms.Button();
            this.btnLoeschen = new System.Windows.Forms.Button();
            this.btnSpeichern = new System.Windows.Forms.Button();
            this.btnNeu = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProdukte)).BeginInit();
            this.panelFilter.SuspendLayout();
            this.panelDetail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numLagerbestand)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMindestbestand)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numEKPreis)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numVKPreis)).BeginInit();
            this.panelButtons.SuspendLayout();
            this.SuspendLayout();
            //
            // splitContainer1
            //
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Panel1.Controls.Add(this.dgvProdukte);
            this.splitContainer1.Panel1.Controls.Add(this.panelFilter);
            this.splitContainer1.Panel2.Controls.Add(this.panelDetail);
            this.splitContainer1.Panel2.Controls.Add(this.panelButtons);
            this.splitContainer1.Panel2.Controls.Add(this.lblStatus);
            this.splitContainer1.Size = new System.Drawing.Size(1084, 611);
            this.splitContainer1.SplitterDistance = 500;
            this.splitContainer1.TabIndex = 0;
            //
            // dgvProdukte
            //
            this.dgvProdukte.AllowUserToAddRows = false;
            this.dgvProdukte.AllowUserToDeleteRows = false;
            this.dgvProdukte.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvProdukte.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvProdukte.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvProdukte.Location = new System.Drawing.Point(0, 70);
            this.dgvProdukte.MultiSelect = false;
            this.dgvProdukte.Name = "dgvProdukte";
            this.dgvProdukte.ReadOnly = true;
            this.dgvProdukte.RowTemplate.Height = 25;
            this.dgvProdukte.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvProdukte.Size = new System.Drawing.Size(500, 541);
            this.dgvProdukte.TabIndex = 1;
            this.dgvProdukte.SelectionChanged += new System.EventHandler(this.dgvProdukte_SelectionChanged);
            //
            // panelFilter
            //
            this.panelFilter.Controls.Add(this.chkNurAktive);
            this.panelFilter.Controls.Add(this.cboKategorie);
            this.panelFilter.Controls.Add(this.lblKategorie);
            this.panelFilter.Controls.Add(this.btnSuchen);
            this.panelFilter.Controls.Add(this.txtSuche);
            this.panelFilter.Controls.Add(this.lblSuche);
            this.panelFilter.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelFilter.Location = new System.Drawing.Point(0, 0);
            this.panelFilter.Name = "panelFilter";
            this.panelFilter.Size = new System.Drawing.Size(500, 70);
            this.panelFilter.TabIndex = 0;
            //
            // chkNurAktive
            //
            this.chkNurAktive.AutoSize = true;
            this.chkNurAktive.Checked = true;
            this.chkNurAktive.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkNurAktive.Location = new System.Drawing.Point(300, 42);
            this.chkNurAktive.Name = "chkNurAktive";
            this.chkNurAktive.Size = new System.Drawing.Size(85, 19);
            this.chkNurAktive.TabIndex = 5;
            this.chkNurAktive.Text = "Nur Aktive";
            this.chkNurAktive.UseVisualStyleBackColor = true;
            this.chkNurAktive.CheckedChanged += new System.EventHandler(this.chkNurAktive_CheckedChanged);
            //
            // cboKategorie
            //
            this.cboKategorie.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboKategorie.FormattingEnabled = true;
            this.cboKategorie.Location = new System.Drawing.Point(80, 40);
            this.cboKategorie.Name = "cboKategorie";
            this.cboKategorie.Size = new System.Drawing.Size(200, 23);
            this.cboKategorie.TabIndex = 4;
            this.cboKategorie.SelectedIndexChanged += new System.EventHandler(this.cboKategorie_SelectedIndexChanged);
            //
            // lblKategorie
            //
            this.lblKategorie.AutoSize = true;
            this.lblKategorie.Location = new System.Drawing.Point(10, 43);
            this.lblKategorie.Name = "lblKategorie";
            this.lblKategorie.Size = new System.Drawing.Size(58, 15);
            this.lblKategorie.TabIndex = 3;
            this.lblKategorie.Text = "Kategorie:";
            //
            // btnSuchen
            //
            this.btnSuchen.Location = new System.Drawing.Point(400, 8);
            this.btnSuchen.Name = "btnSuchen";
            this.btnSuchen.Size = new System.Drawing.Size(75, 23);
            this.btnSuchen.TabIndex = 2;
            this.btnSuchen.Text = "Suchen";
            this.btnSuchen.UseVisualStyleBackColor = true;
            this.btnSuchen.Click += new System.EventHandler(this.btnSuchen_Click);
            //
            // txtSuche
            //
            this.txtSuche.Location = new System.Drawing.Point(80, 9);
            this.txtSuche.Name = "txtSuche";
            this.txtSuche.Size = new System.Drawing.Size(310, 23);
            this.txtSuche.TabIndex = 1;
            this.txtSuche.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSuche_KeyPress);
            //
            // lblSuche
            //
            this.lblSuche.AutoSize = true;
            this.lblSuche.Location = new System.Drawing.Point(10, 12);
            this.lblSuche.Name = "lblSuche";
            this.lblSuche.Size = new System.Drawing.Size(44, 15);
            this.lblSuche.TabIndex = 0;
            this.lblSuche.Text = "Suche:";
            //
            // panelDetail
            //
            this.panelDetail.AutoScroll = true;
            this.panelDetail.Controls.Add(this.numLagerbestand);
            this.panelDetail.Controls.Add(this.lblLagerbestand);
            this.panelDetail.Controls.Add(this.numMindestbestand);
            this.panelDetail.Controls.Add(this.lblMindestbestand);
            this.panelDetail.Controls.Add(this.chkAktiv);
            this.panelDetail.Controls.Add(this.txtBeschreibung);
            this.panelDetail.Controls.Add(this.lblBeschreibung);
            this.panelDetail.Controls.Add(this.numEKPreis);
            this.panelDetail.Controls.Add(this.lblEKPreis);
            this.panelDetail.Controls.Add(this.numVKPreis);
            this.panelDetail.Controls.Add(this.lblVKPreis);
            this.panelDetail.Controls.Add(this.txtEinheit);
            this.panelDetail.Controls.Add(this.lblEinheit);
            this.panelDetail.Controls.Add(this.cboProduktKategorie);
            this.panelDetail.Controls.Add(this.lblProduktKategorie);
            this.panelDetail.Controls.Add(this.txtProduktName);
            this.panelDetail.Controls.Add(this.lblProduktName);
            this.panelDetail.Controls.Add(this.txtArtikelnummer);
            this.panelDetail.Controls.Add(this.lblArtikelnummer);
            this.panelDetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelDetail.Location = new System.Drawing.Point(0, 0);
            this.panelDetail.Name = "panelDetail";
            this.panelDetail.Size = new System.Drawing.Size(580, 531);
            this.panelDetail.TabIndex = 0;
            //
            // numLagerbestand
            //
            this.numLagerbestand.Location = new System.Drawing.Point(120, 230);
            this.numLagerbestand.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            this.numLagerbestand.Name = "numLagerbestand";
            this.numLagerbestand.Size = new System.Drawing.Size(100, 23);
            this.numLagerbestand.TabIndex = 17;
            //
            // lblLagerbestand
            //
            this.lblLagerbestand.AutoSize = true;
            this.lblLagerbestand.Location = new System.Drawing.Point(20, 232);
            this.lblLagerbestand.Name = "lblLagerbestand";
            this.lblLagerbestand.Size = new System.Drawing.Size(79, 15);
            this.lblLagerbestand.TabIndex = 16;
            this.lblLagerbestand.Text = "Lagerbestand:";
            //
            // numMindestbestand
            //
            this.numMindestbestand.Location = new System.Drawing.Point(120, 260);
            this.numMindestbestand.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            this.numMindestbestand.Name = "numMindestbestand";
            this.numMindestbestand.Size = new System.Drawing.Size(100, 23);
            this.numMindestbestand.TabIndex = 19;
            //
            // lblMindestbestand
            //
            this.lblMindestbestand.AutoSize = true;
            this.lblMindestbestand.Location = new System.Drawing.Point(20, 262);
            this.lblMindestbestand.Name = "lblMindestbestand";
            this.lblMindestbestand.Size = new System.Drawing.Size(94, 15);
            this.lblMindestbestand.TabIndex = 18;
            this.lblMindestbestand.Text = "Mindestbestand:";
            //
            // chkAktiv
            //
            this.chkAktiv.AutoSize = true;
            this.chkAktiv.Checked = true;
            this.chkAktiv.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAktiv.Location = new System.Drawing.Point(120, 295);
            this.chkAktiv.Name = "chkAktiv";
            this.chkAktiv.Size = new System.Drawing.Size(53, 19);
            this.chkAktiv.TabIndex = 20;
            this.chkAktiv.Text = "Aktiv";
            this.chkAktiv.UseVisualStyleBackColor = true;
            //
            // txtBeschreibung
            //
            this.txtBeschreibung.Location = new System.Drawing.Point(120, 320);
            this.txtBeschreibung.Multiline = true;
            this.txtBeschreibung.Name = "txtBeschreibung";
            this.txtBeschreibung.Size = new System.Drawing.Size(400, 100);
            this.txtBeschreibung.TabIndex = 22;
            //
            // lblBeschreibung
            //
            this.lblBeschreibung.AutoSize = true;
            this.lblBeschreibung.Location = new System.Drawing.Point(20, 323);
            this.lblBeschreibung.Name = "lblBeschreibung";
            this.lblBeschreibung.Size = new System.Drawing.Size(78, 15);
            this.lblBeschreibung.TabIndex = 21;
            this.lblBeschreibung.Text = "Beschreibung:";
            //
            // numEKPreis
            //
            this.numEKPreis.DecimalPlaces = 2;
            this.numEKPreis.Location = new System.Drawing.Point(120, 170);
            this.numEKPreis.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
            this.numEKPreis.Name = "numEKPreis";
            this.numEKPreis.Size = new System.Drawing.Size(120, 23);
            this.numEKPreis.TabIndex = 13;
            //
            // lblEKPreis
            //
            this.lblEKPreis.AutoSize = true;
            this.lblEKPreis.Location = new System.Drawing.Point(20, 172);
            this.lblEKPreis.Name = "lblEKPreis";
            this.lblEKPreis.Size = new System.Drawing.Size(53, 15);
            this.lblEKPreis.TabIndex = 12;
            this.lblEKPreis.Text = "EK-Preis:";
            //
            // numVKPreis
            //
            this.numVKPreis.DecimalPlaces = 2;
            this.numVKPreis.Location = new System.Drawing.Point(120, 200);
            this.numVKPreis.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
            this.numVKPreis.Name = "numVKPreis";
            this.numVKPreis.Size = new System.Drawing.Size(120, 23);
            this.numVKPreis.TabIndex = 15;
            //
            // lblVKPreis
            //
            this.lblVKPreis.AutoSize = true;
            this.lblVKPreis.Location = new System.Drawing.Point(20, 202);
            this.lblVKPreis.Name = "lblVKPreis";
            this.lblVKPreis.Size = new System.Drawing.Size(52, 15);
            this.lblVKPreis.TabIndex = 14;
            this.lblVKPreis.Text = "VK-Preis:";
            //
            // txtEinheit
            //
            this.txtEinheit.Location = new System.Drawing.Point(120, 140);
            this.txtEinheit.Name = "txtEinheit";
            this.txtEinheit.Size = new System.Drawing.Size(80, 23);
            this.txtEinheit.TabIndex = 11;
            //
            // lblEinheit
            //
            this.lblEinheit.AutoSize = true;
            this.lblEinheit.Location = new System.Drawing.Point(20, 143);
            this.lblEinheit.Name = "lblEinheit";
            this.lblEinheit.Size = new System.Drawing.Size(45, 15);
            this.lblEinheit.TabIndex = 10;
            this.lblEinheit.Text = "Einheit:";
            //
            // cboProduktKategorie
            //
            this.cboProduktKategorie.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboProduktKategorie.FormattingEnabled = true;
            this.cboProduktKategorie.Location = new System.Drawing.Point(120, 110);
            this.cboProduktKategorie.Name = "cboProduktKategorie";
            this.cboProduktKategorie.Size = new System.Drawing.Size(200, 23);
            this.cboProduktKategorie.TabIndex = 9;
            //
            // lblProduktKategorie
            //
            this.lblProduktKategorie.AutoSize = true;
            this.lblProduktKategorie.Location = new System.Drawing.Point(20, 113);
            this.lblProduktKategorie.Name = "lblProduktKategorie";
            this.lblProduktKategorie.Size = new System.Drawing.Size(58, 15);
            this.lblProduktKategorie.TabIndex = 8;
            this.lblProduktKategorie.Text = "Kategorie:";
            //
            // txtProduktName
            //
            this.txtProduktName.Location = new System.Drawing.Point(120, 50);
            this.txtProduktName.Name = "txtProduktName";
            this.txtProduktName.Size = new System.Drawing.Size(350, 23);
            this.txtProduktName.TabIndex = 3;
            //
            // lblProduktName
            //
            this.lblProduktName.AutoSize = true;
            this.lblProduktName.Location = new System.Drawing.Point(20, 53);
            this.lblProduktName.Name = "lblProduktName";
            this.lblProduktName.Size = new System.Drawing.Size(82, 15);
            this.lblProduktName.TabIndex = 2;
            this.lblProduktName.Text = "Produktname:";
            //
            // txtArtikelnummer
            //
            this.txtArtikelnummer.Location = new System.Drawing.Point(120, 20);
            this.txtArtikelnummer.Name = "txtArtikelnummer";
            this.txtArtikelnummer.ReadOnly = true;
            this.txtArtikelnummer.Size = new System.Drawing.Size(120, 23);
            this.txtArtikelnummer.TabIndex = 1;
            //
            // lblArtikelnummer
            //
            this.lblArtikelnummer.AutoSize = true;
            this.lblArtikelnummer.Location = new System.Drawing.Point(20, 23);
            this.lblArtikelnummer.Name = "lblArtikelnummer";
            this.lblArtikelnummer.Size = new System.Drawing.Size(83, 15);
            this.lblArtikelnummer.TabIndex = 0;
            this.lblArtikelnummer.Text = "Artikelnummer:";
            //
            // panelButtons
            //
            this.panelButtons.Controls.Add(this.btnSchliessen);
            this.panelButtons.Controls.Add(this.btnLoeschen);
            this.panelButtons.Controls.Add(this.btnSpeichern);
            this.panelButtons.Controls.Add(this.btnNeu);
            this.panelButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelButtons.Location = new System.Drawing.Point(0, 561);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Size = new System.Drawing.Size(580, 50);
            this.panelButtons.TabIndex = 1;
            //
            // btnSchliessen
            //
            this.btnSchliessen.Location = new System.Drawing.Point(330, 12);
            this.btnSchliessen.Name = "btnSchliessen";
            this.btnSchliessen.Size = new System.Drawing.Size(100, 30);
            this.btnSchliessen.TabIndex = 3;
            this.btnSchliessen.Text = "Schließen";
            this.btnSchliessen.UseVisualStyleBackColor = true;
            this.btnSchliessen.Click += new System.EventHandler(this.btnSchliessen_Click);
            //
            // btnLoeschen
            //
            this.btnLoeschen.Location = new System.Drawing.Point(220, 12);
            this.btnLoeschen.Name = "btnLoeschen";
            this.btnLoeschen.Size = new System.Drawing.Size(100, 30);
            this.btnLoeschen.TabIndex = 2;
            this.btnLoeschen.Text = "Löschen";
            this.btnLoeschen.UseVisualStyleBackColor = true;
            this.btnLoeschen.Click += new System.EventHandler(this.btnLoeschen_Click);
            //
            // btnSpeichern
            //
            this.btnSpeichern.Location = new System.Drawing.Point(110, 12);
            this.btnSpeichern.Name = "btnSpeichern";
            this.btnSpeichern.Size = new System.Drawing.Size(100, 30);
            this.btnSpeichern.TabIndex = 1;
            this.btnSpeichern.Text = "Speichern";
            this.btnSpeichern.UseVisualStyleBackColor = true;
            this.btnSpeichern.Click += new System.EventHandler(this.btnSpeichern_Click);
            //
            // btnNeu
            //
            this.btnNeu.Location = new System.Drawing.Point(10, 12);
            this.btnNeu.Name = "btnNeu";
            this.btnNeu.Size = new System.Drawing.Size(90, 30);
            this.btnNeu.TabIndex = 0;
            this.btnNeu.Text = "Neu";
            this.btnNeu.UseVisualStyleBackColor = true;
            this.btnNeu.Click += new System.EventHandler(this.btnNeu_Click);
            //
            // lblStatus
            //
            this.lblStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblStatus.Location = new System.Drawing.Point(0, 531);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(580, 30);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.Text = "Bereit";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // FrmProdukte
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1084, 611);
            this.Controls.Add(this.splitContainer1);
            this.Name = "FrmProdukte";
            this.Text = "Produktverwaltung";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmProdukte_FormClosing);
            this.Load += new System.EventHandler(this.FrmProdukte_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvProdukte)).EndInit();
            this.panelFilter.ResumeLayout(false);
            this.panelFilter.PerformLayout();
            this.panelDetail.ResumeLayout(false);
            this.panelDetail.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numLagerbestand)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMindestbestand)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numEKPreis)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numVKPreis)).EndInit();
            this.panelButtons.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView dgvProdukte;
        private System.Windows.Forms.Panel panelFilter;
        private System.Windows.Forms.CheckBox chkNurAktive;
        private System.Windows.Forms.ComboBox cboKategorie;
        private System.Windows.Forms.Label lblKategorie;
        private System.Windows.Forms.Button btnSuchen;
        private System.Windows.Forms.TextBox txtSuche;
        private System.Windows.Forms.Label lblSuche;
        private System.Windows.Forms.Panel panelDetail;
        private System.Windows.Forms.NumericUpDown numLagerbestand;
        private System.Windows.Forms.Label lblLagerbestand;
        private System.Windows.Forms.NumericUpDown numMindestbestand;
        private System.Windows.Forms.Label lblMindestbestand;
        private System.Windows.Forms.CheckBox chkAktiv;
        private System.Windows.Forms.TextBox txtBeschreibung;
        private System.Windows.Forms.Label lblBeschreibung;
        private System.Windows.Forms.NumericUpDown numEKPreis;
        private System.Windows.Forms.Label lblEKPreis;
        private System.Windows.Forms.NumericUpDown numVKPreis;
        private System.Windows.Forms.Label lblVKPreis;
        private System.Windows.Forms.TextBox txtEinheit;
        private System.Windows.Forms.Label lblEinheit;
        private System.Windows.Forms.ComboBox cboProduktKategorie;
        private System.Windows.Forms.Label lblProduktKategorie;
        private System.Windows.Forms.TextBox txtProduktName;
        private System.Windows.Forms.Label lblProduktName;
        private System.Windows.Forms.TextBox txtArtikelnummer;
        private System.Windows.Forms.Label lblArtikelnummer;
        private System.Windows.Forms.Panel panelButtons;
        private System.Windows.Forms.Button btnSchliessen;
        private System.Windows.Forms.Button btnLoeschen;
        private System.Windows.Forms.Button btnSpeichern;
        private System.Windows.Forms.Button btnNeu;
        private System.Windows.Forms.Label lblStatus;
    }
}
