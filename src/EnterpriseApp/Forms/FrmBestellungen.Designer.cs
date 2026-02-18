namespace EnterpriseApp.Forms
{
    partial class FrmBestellungen
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
            this.dgvBestellungen = new System.Windows.Forms.DataGridView();
            this.panelFilter = new System.Windows.Forms.Panel();
            this.dtpBis = new System.Windows.Forms.DateTimePicker();
            this.lblBis = new System.Windows.Forms.Label();
            this.dtpVon = new System.Windows.Forms.DateTimePicker();
            this.lblVon = new System.Windows.Forms.Label();
            this.cboStatus = new System.Windows.Forms.ComboBox();
            this.lblFilterStatus = new System.Windows.Forms.Label();
            this.btnSuchen = new System.Windows.Forms.Button();
            this.txtSuche = new System.Windows.Forms.TextBox();
            this.lblSuche = new System.Windows.Forms.Label();
            this.panelRight = new System.Windows.Forms.Panel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabKopfdaten = new System.Windows.Forms.TabPage();
            this.txtBemerkung = new System.Windows.Forms.TextBox();
            this.lblBemerkung = new System.Windows.Forms.Label();
            this.cboBestellStatus = new System.Windows.Forms.ComboBox();
            this.lblBestellStatus = new System.Windows.Forms.Label();
            this.txtLieferadresse = new System.Windows.Forms.TextBox();
            this.lblLieferadresse = new System.Windows.Forms.Label();
            this.dtpLieferdatum = new System.Windows.Forms.DateTimePicker();
            this.lblLieferdatum = new System.Windows.Forms.Label();
            this.dtpBestelldatum = new System.Windows.Forms.DateTimePicker();
            this.lblBestelldatum = new System.Windows.Forms.Label();
            this.btnKundeWaehlen = new System.Windows.Forms.Button();
            this.txtKunde = new System.Windows.Forms.TextBox();
            this.lblKunde = new System.Windows.Forms.Label();
            this.txtBestellnummer = new System.Windows.Forms.TextBox();
            this.lblBestellnummer = new System.Windows.Forms.Label();
            this.tabPositionen = new System.Windows.Forms.TabPage();
            this.dgvPositionen = new System.Windows.Forms.DataGridView();
            this.panelPositionButtons = new System.Windows.Forms.Panel();
            this.lblGesamtsumme = new System.Windows.Forms.Label();
            this.btnPositionLoeschen = new System.Windows.Forms.Button();
            this.btnPositionHinzufuegen = new System.Windows.Forms.Button();
            this.panelButtons = new System.Windows.Forms.Panel();
            this.btnDrucken = new System.Windows.Forms.Button();
            this.btnSchliessen = new System.Windows.Forms.Button();
            this.btnLoeschen = new System.Windows.Forms.Button();
            this.btnSpeichern = new System.Windows.Forms.Button();
            this.btnNeu = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBestellungen)).BeginInit();
            this.panelFilter.SuspendLayout();
            this.panelRight.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabKopfdaten.SuspendLayout();
            this.tabPositionen.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPositionen)).BeginInit();
            this.panelPositionButtons.SuspendLayout();
            this.panelButtons.SuspendLayout();
            this.SuspendLayout();
            //
            // splitContainer1
            //
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Panel1.Controls.Add(this.dgvBestellungen);
            this.splitContainer1.Panel1.Controls.Add(this.panelFilter);
            this.splitContainer1.Panel2.Controls.Add(this.panelRight);
            this.splitContainer1.Size = new System.Drawing.Size(1184, 661);
            this.splitContainer1.SplitterDistance = 450;
            this.splitContainer1.TabIndex = 0;
            //
            // dgvBestellungen
            //
            this.dgvBestellungen.AllowUserToAddRows = false;
            this.dgvBestellungen.AllowUserToDeleteRows = false;
            this.dgvBestellungen.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvBestellungen.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBestellungen.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvBestellungen.Location = new System.Drawing.Point(0, 100);
            this.dgvBestellungen.MultiSelect = false;
            this.dgvBestellungen.Name = "dgvBestellungen";
            this.dgvBestellungen.ReadOnly = true;
            this.dgvBestellungen.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvBestellungen.Size = new System.Drawing.Size(450, 561);
            this.dgvBestellungen.TabIndex = 1;
            this.dgvBestellungen.SelectionChanged += new System.EventHandler(this.dgvBestellungen_SelectionChanged);
            //
            // panelFilter
            //
            this.panelFilter.Controls.Add(this.dtpBis);
            this.panelFilter.Controls.Add(this.lblBis);
            this.panelFilter.Controls.Add(this.dtpVon);
            this.panelFilter.Controls.Add(this.lblVon);
            this.panelFilter.Controls.Add(this.cboStatus);
            this.panelFilter.Controls.Add(this.lblFilterStatus);
            this.panelFilter.Controls.Add(this.btnSuchen);
            this.panelFilter.Controls.Add(this.txtSuche);
            this.panelFilter.Controls.Add(this.lblSuche);
            this.panelFilter.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelFilter.Location = new System.Drawing.Point(0, 0);
            this.panelFilter.Name = "panelFilter";
            this.panelFilter.Size = new System.Drawing.Size(450, 100);
            this.panelFilter.TabIndex = 0;
            //
            // dtpBis
            //
            this.dtpBis.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpBis.Location = new System.Drawing.Point(280, 68);
            this.dtpBis.Name = "dtpBis";
            this.dtpBis.Size = new System.Drawing.Size(100, 23);
            this.dtpBis.TabIndex = 8;
            //
            // lblBis
            //
            this.lblBis.AutoSize = true;
            this.lblBis.Location = new System.Drawing.Point(250, 72);
            this.lblBis.Name = "lblBis";
            this.lblBis.Size = new System.Drawing.Size(23, 15);
            this.lblBis.TabIndex = 7;
            this.lblBis.Text = "bis";
            //
            // dtpVon
            //
            this.dtpVon.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpVon.Location = new System.Drawing.Point(80, 68);
            this.dtpVon.Name = "dtpVon";
            this.dtpVon.Size = new System.Drawing.Size(100, 23);
            this.dtpVon.TabIndex = 6;
            //
            // lblVon
            //
            this.lblVon.AutoSize = true;
            this.lblVon.Location = new System.Drawing.Point(10, 72);
            this.lblVon.Name = "lblVon";
            this.lblVon.Size = new System.Drawing.Size(56, 15);
            this.lblVon.TabIndex = 5;
            this.lblVon.Text = "Zeitraum:";
            //
            // cboStatus
            //
            this.cboStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboStatus.FormattingEnabled = true;
            this.cboStatus.Location = new System.Drawing.Point(80, 38);
            this.cboStatus.Name = "cboStatus";
            this.cboStatus.Size = new System.Drawing.Size(150, 23);
            this.cboStatus.TabIndex = 4;
            this.cboStatus.SelectedIndexChanged += new System.EventHandler(this.cboStatus_SelectedIndexChanged);
            //
            // lblFilterStatus
            //
            this.lblFilterStatus.AutoSize = true;
            this.lblFilterStatus.Location = new System.Drawing.Point(10, 41);
            this.lblFilterStatus.Name = "lblFilterStatus";
            this.lblFilterStatus.Size = new System.Drawing.Size(42, 15);
            this.lblFilterStatus.TabIndex = 3;
            this.lblFilterStatus.Text = "Status:";
            //
            // btnSuchen
            //
            this.btnSuchen.Location = new System.Drawing.Point(365, 8);
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
            this.txtSuche.Size = new System.Drawing.Size(275, 23);
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
            // panelRight
            //
            this.panelRight.Controls.Add(this.tabControl1);
            this.panelRight.Controls.Add(this.panelButtons);
            this.panelRight.Controls.Add(this.lblStatus);
            this.panelRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelRight.Location = new System.Drawing.Point(0, 0);
            this.panelRight.Name = "panelRight";
            this.panelRight.Size = new System.Drawing.Size(730, 661);
            this.panelRight.TabIndex = 0;
            //
            // tabControl1
            //
            this.tabControl1.Controls.Add(this.tabKopfdaten);
            this.tabControl1.Controls.Add(this.tabPositionen);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(730, 581);
            this.tabControl1.TabIndex = 0;
            //
            // tabKopfdaten
            //
            this.tabKopfdaten.Controls.Add(this.txtBemerkung);
            this.tabKopfdaten.Controls.Add(this.lblBemerkung);
            this.tabKopfdaten.Controls.Add(this.cboBestellStatus);
            this.tabKopfdaten.Controls.Add(this.lblBestellStatus);
            this.tabKopfdaten.Controls.Add(this.txtLieferadresse);
            this.tabKopfdaten.Controls.Add(this.lblLieferadresse);
            this.tabKopfdaten.Controls.Add(this.dtpLieferdatum);
            this.tabKopfdaten.Controls.Add(this.lblLieferdatum);
            this.tabKopfdaten.Controls.Add(this.dtpBestelldatum);
            this.tabKopfdaten.Controls.Add(this.lblBestelldatum);
            this.tabKopfdaten.Controls.Add(this.btnKundeWaehlen);
            this.tabKopfdaten.Controls.Add(this.txtKunde);
            this.tabKopfdaten.Controls.Add(this.lblKunde);
            this.tabKopfdaten.Controls.Add(this.txtBestellnummer);
            this.tabKopfdaten.Controls.Add(this.lblBestellnummer);
            this.tabKopfdaten.Location = new System.Drawing.Point(4, 24);
            this.tabKopfdaten.Name = "tabKopfdaten";
            this.tabKopfdaten.Padding = new System.Windows.Forms.Padding(3);
            this.tabKopfdaten.Size = new System.Drawing.Size(722, 553);
            this.tabKopfdaten.TabIndex = 0;
            this.tabKopfdaten.Text = "Kopfdaten";
            this.tabKopfdaten.UseVisualStyleBackColor = true;
            //
            // txtBemerkung
            //
            this.txtBemerkung.Location = new System.Drawing.Point(120, 310);
            this.txtBemerkung.Multiline = true;
            this.txtBemerkung.Name = "txtBemerkung";
            this.txtBemerkung.Size = new System.Drawing.Size(500, 100);
            this.txtBemerkung.TabIndex = 14;
            //
            // lblBemerkung
            //
            this.lblBemerkung.AutoSize = true;
            this.lblBemerkung.Location = new System.Drawing.Point(20, 313);
            this.lblBemerkung.Name = "lblBemerkung";
            this.lblBemerkung.Size = new System.Drawing.Size(69, 15);
            this.lblBemerkung.TabIndex = 13;
            this.lblBemerkung.Text = "Bemerkung:";
            //
            // cboBestellStatus
            //
            this.cboBestellStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBestellStatus.FormattingEnabled = true;
            this.cboBestellStatus.Location = new System.Drawing.Point(120, 140);
            this.cboBestellStatus.Name = "cboBestellStatus";
            this.cboBestellStatus.Size = new System.Drawing.Size(150, 23);
            this.cboBestellStatus.TabIndex = 12;
            //
            // lblBestellStatus
            //
            this.lblBestellStatus.AutoSize = true;
            this.lblBestellStatus.Location = new System.Drawing.Point(20, 143);
            this.lblBestellStatus.Name = "lblBestellStatus";
            this.lblBestellStatus.Size = new System.Drawing.Size(42, 15);
            this.lblBestellStatus.TabIndex = 11;
            this.lblBestellStatus.Text = "Status:";
            //
            // txtLieferadresse
            //
            this.txtLieferadresse.Location = new System.Drawing.Point(120, 180);
            this.txtLieferadresse.Multiline = true;
            this.txtLieferadresse.Name = "txtLieferadresse";
            this.txtLieferadresse.Size = new System.Drawing.Size(400, 120);
            this.txtLieferadresse.TabIndex = 10;
            //
            // lblLieferadresse
            //
            this.lblLieferadresse.AutoSize = true;
            this.lblLieferadresse.Location = new System.Drawing.Point(20, 183);
            this.lblLieferadresse.Name = "lblLieferadresse";
            this.lblLieferadresse.Size = new System.Drawing.Size(75, 15);
            this.lblLieferadresse.TabIndex = 9;
            this.lblLieferadresse.Text = "Lieferadresse:";
            //
            // dtpLieferdatum
            //
            this.dtpLieferdatum.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpLieferdatum.Location = new System.Drawing.Point(380, 110);
            this.dtpLieferdatum.Name = "dtpLieferdatum";
            this.dtpLieferdatum.Size = new System.Drawing.Size(120, 23);
            this.dtpLieferdatum.TabIndex = 8;
            //
            // lblLieferdatum
            //
            this.lblLieferdatum.AutoSize = true;
            this.lblLieferdatum.Location = new System.Drawing.Point(290, 113);
            this.lblLieferdatum.Name = "lblLieferdatum";
            this.lblLieferdatum.Size = new System.Drawing.Size(71, 15);
            this.lblLieferdatum.TabIndex = 7;
            this.lblLieferdatum.Text = "Lieferdatum:";
            //
            // dtpBestelldatum
            //
            this.dtpBestelldatum.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpBestelldatum.Location = new System.Drawing.Point(120, 110);
            this.dtpBestelldatum.Name = "dtpBestelldatum";
            this.dtpBestelldatum.Size = new System.Drawing.Size(120, 23);
            this.dtpBestelldatum.TabIndex = 6;
            //
            // lblBestelldatum
            //
            this.lblBestelldatum.AutoSize = true;
            this.lblBestelldatum.Location = new System.Drawing.Point(20, 113);
            this.lblBestelldatum.Name = "lblBestelldatum";
            this.lblBestelldatum.Size = new System.Drawing.Size(77, 15);
            this.lblBestelldatum.TabIndex = 5;
            this.lblBestelldatum.Text = "Bestelldatum:";
            //
            // btnKundeWaehlen
            //
            this.btnKundeWaehlen.Location = new System.Drawing.Point(530, 50);
            this.btnKundeWaehlen.Name = "btnKundeWaehlen";
            this.btnKundeWaehlen.Size = new System.Drawing.Size(30, 23);
            this.btnKundeWaehlen.TabIndex = 4;
            this.btnKundeWaehlen.Text = "...";
            this.btnKundeWaehlen.UseVisualStyleBackColor = true;
            this.btnKundeWaehlen.Click += new System.EventHandler(this.btnKundeWaehlen_Click);
            //
            // txtKunde
            //
            this.txtKunde.Location = new System.Drawing.Point(120, 50);
            this.txtKunde.Name = "txtKunde";
            this.txtKunde.ReadOnly = true;
            this.txtKunde.Size = new System.Drawing.Size(400, 23);
            this.txtKunde.TabIndex = 3;
            //
            // lblKunde
            //
            this.lblKunde.AutoSize = true;
            this.lblKunde.Location = new System.Drawing.Point(20, 53);
            this.lblKunde.Name = "lblKunde";
            this.lblKunde.Size = new System.Drawing.Size(43, 15);
            this.lblKunde.TabIndex = 2;
            this.lblKunde.Text = "Kunde:";
            //
            // txtBestellnummer
            //
            this.txtBestellnummer.Location = new System.Drawing.Point(120, 20);
            this.txtBestellnummer.Name = "txtBestellnummer";
            this.txtBestellnummer.ReadOnly = true;
            this.txtBestellnummer.Size = new System.Drawing.Size(150, 23);
            this.txtBestellnummer.TabIndex = 1;
            //
            // lblBestellnummer
            //
            this.lblBestellnummer.AutoSize = true;
            this.lblBestellnummer.Location = new System.Drawing.Point(20, 23);
            this.lblBestellnummer.Name = "lblBestellnummer";
            this.lblBestellnummer.Size = new System.Drawing.Size(88, 15);
            this.lblBestellnummer.TabIndex = 0;
            this.lblBestellnummer.Text = "Bestellnummer:";
            //
            // tabPositionen
            //
            this.tabPositionen.Controls.Add(this.dgvPositionen);
            this.tabPositionen.Controls.Add(this.panelPositionButtons);
            this.tabPositionen.Location = new System.Drawing.Point(4, 24);
            this.tabPositionen.Name = "tabPositionen";
            this.tabPositionen.Padding = new System.Windows.Forms.Padding(3);
            this.tabPositionen.Size = new System.Drawing.Size(722, 553);
            this.tabPositionen.TabIndex = 1;
            this.tabPositionen.Text = "Positionen";
            this.tabPositionen.UseVisualStyleBackColor = true;
            //
            // dgvPositionen
            //
            this.dgvPositionen.AllowUserToAddRows = false;
            this.dgvPositionen.AllowUserToDeleteRows = false;
            this.dgvPositionen.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvPositionen.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPositionen.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvPositionen.Location = new System.Drawing.Point(3, 3);
            this.dgvPositionen.Name = "dgvPositionen";
            this.dgvPositionen.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPositionen.Size = new System.Drawing.Size(716, 497);
            this.dgvPositionen.TabIndex = 0;
            //
            // panelPositionButtons
            //
            this.panelPositionButtons.Controls.Add(this.lblGesamtsumme);
            this.panelPositionButtons.Controls.Add(this.btnPositionLoeschen);
            this.panelPositionButtons.Controls.Add(this.btnPositionHinzufuegen);
            this.panelPositionButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelPositionButtons.Location = new System.Drawing.Point(3, 500);
            this.panelPositionButtons.Name = "panelPositionButtons";
            this.panelPositionButtons.Size = new System.Drawing.Size(716, 50);
            this.panelPositionButtons.TabIndex = 1;
            //
            // lblGesamtsumme
            //
            this.lblGesamtsumme.AutoSize = true;
            this.lblGesamtsumme.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblGesamtsumme.Location = new System.Drawing.Point(450, 14);
            this.lblGesamtsumme.Name = "lblGesamtsumme";
            this.lblGesamtsumme.Size = new System.Drawing.Size(149, 21);
            this.lblGesamtsumme.TabIndex = 2;
            this.lblGesamtsumme.Text = "Gesamt: 0,00 EUR";
            //
            // btnPositionLoeschen
            //
            this.btnPositionLoeschen.Location = new System.Drawing.Point(170, 10);
            this.btnPositionLoeschen.Name = "btnPositionLoeschen";
            this.btnPositionLoeschen.Size = new System.Drawing.Size(150, 30);
            this.btnPositionLoeschen.TabIndex = 1;
            this.btnPositionLoeschen.Text = "Position entfernen";
            this.btnPositionLoeschen.UseVisualStyleBackColor = true;
            this.btnPositionLoeschen.Click += new System.EventHandler(this.btnPositionLoeschen_Click);
            //
            // btnPositionHinzufuegen
            //
            this.btnPositionHinzufuegen.Location = new System.Drawing.Point(10, 10);
            this.btnPositionHinzufuegen.Name = "btnPositionHinzufuegen";
            this.btnPositionHinzufuegen.Size = new System.Drawing.Size(150, 30);
            this.btnPositionHinzufuegen.TabIndex = 0;
            this.btnPositionHinzufuegen.Text = "Position hinzufügen";
            this.btnPositionHinzufuegen.UseVisualStyleBackColor = true;
            this.btnPositionHinzufuegen.Click += new System.EventHandler(this.btnPositionHinzufuegen_Click);
            //
            // panelButtons
            //
            this.panelButtons.Controls.Add(this.btnDrucken);
            this.panelButtons.Controls.Add(this.btnSchliessen);
            this.panelButtons.Controls.Add(this.btnLoeschen);
            this.panelButtons.Controls.Add(this.btnSpeichern);
            this.panelButtons.Controls.Add(this.btnNeu);
            this.panelButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelButtons.Location = new System.Drawing.Point(0, 611);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Size = new System.Drawing.Size(730, 50);
            this.panelButtons.TabIndex = 1;
            //
            // btnDrucken
            //
            this.btnDrucken.Location = new System.Drawing.Point(440, 12);
            this.btnDrucken.Name = "btnDrucken";
            this.btnDrucken.Size = new System.Drawing.Size(100, 30);
            this.btnDrucken.TabIndex = 4;
            this.btnDrucken.Text = "Drucken";
            this.btnDrucken.UseVisualStyleBackColor = true;
            this.btnDrucken.Click += new System.EventHandler(this.btnDrucken_Click);
            //
            // btnSchliessen
            //
            this.btnSchliessen.Location = new System.Drawing.Point(550, 12);
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
            this.lblStatus.Location = new System.Drawing.Point(0, 581);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(730, 30);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.Text = "Bereit";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // FrmBestellungen
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 661);
            this.Controls.Add(this.splitContainer1);
            this.Name = "FrmBestellungen";
            this.Text = "Bestellverwaltung";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmBestellungen_FormClosing);
            this.Load += new System.EventHandler(this.FrmBestellungen_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvBestellungen)).EndInit();
            this.panelFilter.ResumeLayout(false);
            this.panelFilter.PerformLayout();
            this.panelRight.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabKopfdaten.ResumeLayout(false);
            this.tabKopfdaten.PerformLayout();
            this.tabPositionen.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPositionen)).EndInit();
            this.panelPositionButtons.ResumeLayout(false);
            this.panelPositionButtons.PerformLayout();
            this.panelButtons.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView dgvBestellungen;
        private System.Windows.Forms.Panel panelFilter;
        private System.Windows.Forms.DateTimePicker dtpBis;
        private System.Windows.Forms.Label lblBis;
        private System.Windows.Forms.DateTimePicker dtpVon;
        private System.Windows.Forms.Label lblVon;
        private System.Windows.Forms.ComboBox cboStatus;
        private System.Windows.Forms.Label lblFilterStatus;
        private System.Windows.Forms.Button btnSuchen;
        private System.Windows.Forms.TextBox txtSuche;
        private System.Windows.Forms.Label lblSuche;
        private System.Windows.Forms.Panel panelRight;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabKopfdaten;
        private System.Windows.Forms.TextBox txtBemerkung;
        private System.Windows.Forms.Label lblBemerkung;
        private System.Windows.Forms.ComboBox cboBestellStatus;
        private System.Windows.Forms.Label lblBestellStatus;
        private System.Windows.Forms.TextBox txtLieferadresse;
        private System.Windows.Forms.Label lblLieferadresse;
        private System.Windows.Forms.DateTimePicker dtpLieferdatum;
        private System.Windows.Forms.Label lblLieferdatum;
        private System.Windows.Forms.DateTimePicker dtpBestelldatum;
        private System.Windows.Forms.Label lblBestelldatum;
        private System.Windows.Forms.Button btnKundeWaehlen;
        private System.Windows.Forms.TextBox txtKunde;
        private System.Windows.Forms.Label lblKunde;
        private System.Windows.Forms.TextBox txtBestellnummer;
        private System.Windows.Forms.Label lblBestellnummer;
        private System.Windows.Forms.TabPage tabPositionen;
        private System.Windows.Forms.DataGridView dgvPositionen;
        private System.Windows.Forms.Panel panelPositionButtons;
        private System.Windows.Forms.Label lblGesamtsumme;
        private System.Windows.Forms.Button btnPositionLoeschen;
        private System.Windows.Forms.Button btnPositionHinzufuegen;
        private System.Windows.Forms.Panel panelButtons;
        private System.Windows.Forms.Button btnDrucken;
        private System.Windows.Forms.Button btnSchliessen;
        private System.Windows.Forms.Button btnLoeschen;
        private System.Windows.Forms.Button btnSpeichern;
        private System.Windows.Forms.Button btnNeu;
        private System.Windows.Forms.Label lblStatus;
    }
}
