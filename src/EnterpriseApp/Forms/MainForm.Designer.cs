namespace EnterpriseApp.Forms
{
    partial class MainForm
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.dateiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.neuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.öffnenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.speichernToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.druckenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.druckvorschauToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.beendenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bearbeitenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rückgängigToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wiederholenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.ausschneidenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.kopierenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.einfügenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stammdatenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.kundenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.produkteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bestellungenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.neueBestellungToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bestellübersichtToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.berichteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.kundenlisteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.produktlisteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bestellungenListeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.umsatzberichtToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extrasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.einstellungenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.datenbankToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testdatenGenerierenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hilfeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.infoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsbNeu = new System.Windows.Forms.ToolStripButton();
            this.tsbSpeichern = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbKunden = new System.Windows.Forms.ToolStripButton();
            this.tsbProdukte = new System.Windows.Forms.ToolStripButton();
            this.tsbBestellungen = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbDrucken = new System.Windows.Forms.ToolStripButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tsslStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslUser = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslDateTime = new System.Windows.Forms.ToolStripStatusLabel();
            this.panelDashboard = new System.Windows.Forms.Panel();
            this.lblUmsatz = new System.Windows.Forms.Label();
            this.lblAnzahlBestellungen = new System.Windows.Forms.Label();
            this.lblAnzahlProdukte = new System.Windows.Forms.Label();
            this.lblAnzahlKunden = new System.Windows.Forms.Label();
            this.lblTitelUmsatz = new System.Windows.Forms.Label();
            this.lblTitelBestellungen = new System.Windows.Forms.Label();
            this.lblTitelProdukte = new System.Windows.Forms.Label();
            this.lblTitelKunden = new System.Windows.Forms.Label();
            this.lblWillkommen = new System.Windows.Forms.Label();
            this.timerClock = new System.Windows.Forms.Timer(this.components);
            this.timerRefresh = new System.Windows.Forms.Timer(this.components);
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.panelDashboard.SuspendLayout();
            this.SuspendLayout();
            //
            // menuStrip1
            //
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dateiToolStripMenuItem,
            this.bearbeitenToolStripMenuItem,
            this.stammdatenToolStripMenuItem,
            this.bestellungenToolStripMenuItem,
            this.berichteToolStripMenuItem,
            this.extrasToolStripMenuItem,
            this.hilfeToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1024, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            //
            // dateiToolStripMenuItem
            //
            this.dateiToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.neuToolStripMenuItem,
            this.öffnenToolStripMenuItem,
            this.speichernToolStripMenuItem,
            this.toolStripSeparator1,
            this.druckenToolStripMenuItem,
            this.druckvorschauToolStripMenuItem,
            this.toolStripSeparator2,
            this.beendenToolStripMenuItem});
            this.dateiToolStripMenuItem.Name = "dateiToolStripMenuItem";
            this.dateiToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.dateiToolStripMenuItem.Text = "&Datei";
            //
            // neuToolStripMenuItem
            //
            this.neuToolStripMenuItem.Name = "neuToolStripMenuItem";
            this.neuToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.neuToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.neuToolStripMenuItem.Text = "&Neu";
            this.neuToolStripMenuItem.Click += new System.EventHandler(this.neuToolStripMenuItem_Click);
            //
            // öffnenToolStripMenuItem
            //
            this.öffnenToolStripMenuItem.Name = "öffnenToolStripMenuItem";
            this.öffnenToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.öffnenToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.öffnenToolStripMenuItem.Text = "Ö&ffnen";
            this.öffnenToolStripMenuItem.Click += new System.EventHandler(this.öffnenToolStripMenuItem_Click);
            //
            // speichernToolStripMenuItem
            //
            this.speichernToolStripMenuItem.Name = "speichernToolStripMenuItem";
            this.speichernToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.speichernToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.speichernToolStripMenuItem.Text = "&Speichern";
            this.speichernToolStripMenuItem.Click += new System.EventHandler(this.speichernToolStripMenuItem_Click);
            //
            // toolStripSeparator1
            //
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(177, 6);
            //
            // druckenToolStripMenuItem
            //
            this.druckenToolStripMenuItem.Name = "druckenToolStripMenuItem";
            this.druckenToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.druckenToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.druckenToolStripMenuItem.Text = "&Drucken";
            this.druckenToolStripMenuItem.Click += new System.EventHandler(this.druckenToolStripMenuItem_Click);
            //
            // druckvorschauToolStripMenuItem
            //
            this.druckvorschauToolStripMenuItem.Name = "druckvorschauToolStripMenuItem";
            this.druckvorschauToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.druckvorschauToolStripMenuItem.Text = "Druck&vorschau";
            this.druckvorschauToolStripMenuItem.Click += new System.EventHandler(this.druckvorschauToolStripMenuItem_Click);
            //
            // toolStripSeparator2
            //
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(177, 6);
            //
            // beendenToolStripMenuItem
            //
            this.beendenToolStripMenuItem.Name = "beendenToolStripMenuItem";
            this.beendenToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.beendenToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.beendenToolStripMenuItem.Text = "&Beenden";
            this.beendenToolStripMenuItem.Click += new System.EventHandler(this.beendenToolStripMenuItem_Click);
            //
            // bearbeitenToolStripMenuItem
            //
            this.bearbeitenToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rückgängigToolStripMenuItem,
            this.wiederholenToolStripMenuItem,
            this.toolStripSeparator3,
            this.ausschneidenToolStripMenuItem,
            this.kopierenToolStripMenuItem,
            this.einfügenToolStripMenuItem});
            this.bearbeitenToolStripMenuItem.Name = "bearbeitenToolStripMenuItem";
            this.bearbeitenToolStripMenuItem.Size = new System.Drawing.Size(75, 20);
            this.bearbeitenToolStripMenuItem.Text = "&Bearbeiten";
            //
            // rückgängigToolStripMenuItem
            //
            this.rückgängigToolStripMenuItem.Name = "rückgängigToolStripMenuItem";
            this.rückgängigToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.rückgängigToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.rückgängigToolStripMenuItem.Text = "&Rückgängig";
            //
            // wiederholenToolStripMenuItem
            //
            this.wiederholenToolStripMenuItem.Name = "wiederholenToolStripMenuItem";
            this.wiederholenToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this.wiederholenToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.wiederholenToolStripMenuItem.Text = "&Wiederholen";
            //
            // toolStripSeparator3
            //
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(177, 6);
            //
            // ausschneidenToolStripMenuItem
            //
            this.ausschneidenToolStripMenuItem.Name = "ausschneidenToolStripMenuItem";
            this.ausschneidenToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.ausschneidenToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.ausschneidenToolStripMenuItem.Text = "Auss&chneiden";
            //
            // kopierenToolStripMenuItem
            //
            this.kopierenToolStripMenuItem.Name = "kopierenToolStripMenuItem";
            this.kopierenToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.kopierenToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.kopierenToolStripMenuItem.Text = "&Kopieren";
            //
            // einfügenToolStripMenuItem
            //
            this.einfügenToolStripMenuItem.Name = "einfügenToolStripMenuItem";
            this.einfügenToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.einfügenToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.einfügenToolStripMenuItem.Text = "E&infügen";
            //
            // stammdatenToolStripMenuItem
            //
            this.stammdatenToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.kundenToolStripMenuItem,
            this.produkteToolStripMenuItem});
            this.stammdatenToolStripMenuItem.Name = "stammdatenToolStripMenuItem";
            this.stammdatenToolStripMenuItem.Size = new System.Drawing.Size(83, 20);
            this.stammdatenToolStripMenuItem.Text = "&Stammdaten";
            //
            // kundenToolStripMenuItem
            //
            this.kundenToolStripMenuItem.Name = "kundenToolStripMenuItem";
            this.kundenToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.kundenToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.kundenToolStripMenuItem.Text = "&Kunden";
            this.kundenToolStripMenuItem.Click += new System.EventHandler(this.kundenToolStripMenuItem_Click);
            //
            // produkteToolStripMenuItem
            //
            this.produkteToolStripMenuItem.Name = "produkteToolStripMenuItem";
            this.produkteToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F6;
            this.produkteToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.produkteToolStripMenuItem.Text = "&Produkte";
            this.produkteToolStripMenuItem.Click += new System.EventHandler(this.produkteToolStripMenuItem_Click);
            //
            // bestellungenToolStripMenuItem
            //
            this.bestellungenToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.neueBestellungToolStripMenuItem,
            this.bestellübersichtToolStripMenuItem});
            this.bestellungenToolStripMenuItem.Name = "bestellungenToolStripMenuItem";
            this.bestellungenToolStripMenuItem.Size = new System.Drawing.Size(87, 20);
            this.bestellungenToolStripMenuItem.Text = "B&estellungen";
            //
            // neueBestellungToolStripMenuItem
            //
            this.neueBestellungToolStripMenuItem.Name = "neueBestellungToolStripMenuItem";
            this.neueBestellungToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F7;
            this.neueBestellungToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.neueBestellungToolStripMenuItem.Text = "&Neue Bestellung";
            this.neueBestellungToolStripMenuItem.Click += new System.EventHandler(this.neueBestellungToolStripMenuItem_Click);
            //
            // bestellübersichtToolStripMenuItem
            //
            this.bestellübersichtToolStripMenuItem.Name = "bestellübersichtToolStripMenuItem";
            this.bestellübersichtToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F8;
            this.bestellübersichtToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.bestellübersichtToolStripMenuItem.Text = "Bestellü&bersicht";
            this.bestellübersichtToolStripMenuItem.Click += new System.EventHandler(this.bestellübersichtToolStripMenuItem_Click);
            //
            // berichteToolStripMenuItem
            //
            this.berichteToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.kundenlisteToolStripMenuItem,
            this.produktlisteToolStripMenuItem,
            this.bestellungenListeToolStripMenuItem,
            this.umsatzberichtToolStripMenuItem});
            this.berichteToolStripMenuItem.Name = "berichteToolStripMenuItem";
            this.berichteToolStripMenuItem.Size = new System.Drawing.Size(63, 20);
            this.berichteToolStripMenuItem.Text = "Be&richte";
            //
            // kundenlisteToolStripMenuItem
            //
            this.kundenlisteToolStripMenuItem.Name = "kundenlisteToolStripMenuItem";
            this.kundenlisteToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.kundenlisteToolStripMenuItem.Text = "Kundenliste";
            this.kundenlisteToolStripMenuItem.Click += new System.EventHandler(this.kundenlisteToolStripMenuItem_Click);
            //
            // produktlisteToolStripMenuItem
            //
            this.produktlisteToolStripMenuItem.Name = "produktlisteToolStripMenuItem";
            this.produktlisteToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.produktlisteToolStripMenuItem.Text = "Produktliste";
            this.produktlisteToolStripMenuItem.Click += new System.EventHandler(this.produktlisteToolStripMenuItem_Click);
            //
            // bestellungenListeToolStripMenuItem
            //
            this.bestellungenListeToolStripMenuItem.Name = "bestellungenListeToolStripMenuItem";
            this.bestellungenListeToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.bestellungenListeToolStripMenuItem.Text = "Bestellungenliste";
            this.bestellungenListeToolStripMenuItem.Click += new System.EventHandler(this.bestellungenListeToolStripMenuItem_Click);
            //
            // umsatzberichtToolStripMenuItem
            //
            this.umsatzberichtToolStripMenuItem.Name = "umsatzberichtToolStripMenuItem";
            this.umsatzberichtToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.umsatzberichtToolStripMenuItem.Text = "Umsatzbericht";
            this.umsatzberichtToolStripMenuItem.Click += new System.EventHandler(this.umsatzberichtToolStripMenuItem_Click);
            //
            // extrasToolStripMenuItem
            //
            this.extrasToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.einstellungenToolStripMenuItem,
            this.datenbankToolStripMenuItem,
            this.testdatenGenerierenToolStripMenuItem});
            this.extrasToolStripMenuItem.Name = "extrasToolStripMenuItem";
            this.extrasToolStripMenuItem.Size = new System.Drawing.Size(50, 20);
            this.extrasToolStripMenuItem.Text = "E&xtras";
            //
            // einstellungenToolStripMenuItem
            //
            this.einstellungenToolStripMenuItem.Name = "einstellungenToolStripMenuItem";
            this.einstellungenToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.einstellungenToolStripMenuItem.Text = "&Einstellungen";
            this.einstellungenToolStripMenuItem.Click += new System.EventHandler(this.einstellungenToolStripMenuItem_Click);
            //
            // datenbankToolStripMenuItem
            //
            this.datenbankToolStripMenuItem.Name = "datenbankToolStripMenuItem";
            this.datenbankToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.datenbankToolStripMenuItem.Text = "&Datenbank optimieren";
            this.datenbankToolStripMenuItem.Click += new System.EventHandler(this.datenbankToolStripMenuItem_Click);
            //
            // testdatenGenerierenToolStripMenuItem
            //
            this.testdatenGenerierenToolStripMenuItem.Name = "testdatenGenerierenToolStripMenuItem";
            this.testdatenGenerierenToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T)));
            this.testdatenGenerierenToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.testdatenGenerierenToolStripMenuItem.Text = "&Testdaten generieren";
            this.testdatenGenerierenToolStripMenuItem.Click += new System.EventHandler(this.testdatenGenerierenToolStripMenuItem_Click);
            //
            // hilfeToolStripMenuItem
            //
            this.hilfeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.infoToolStripMenuItem});
            this.hilfeToolStripMenuItem.Name = "hilfeToolStripMenuItem";
            this.hilfeToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.hilfeToolStripMenuItem.Text = "&Hilfe";
            //
            // infoToolStripMenuItem
            //
            this.infoToolStripMenuItem.Name = "infoToolStripMenuItem";
            this.infoToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F1;
            this.infoToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.infoToolStripMenuItem.Text = "&Info";
            this.infoToolStripMenuItem.Click += new System.EventHandler(this.infoToolStripMenuItem_Click);
            //
            // toolStrip1
            //
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbNeu,
            this.tsbSpeichern,
            this.toolStripSeparator4,
            this.tsbKunden,
            this.tsbProdukte,
            this.tsbBestellungen,
            this.toolStripSeparator5,
            this.tsbDrucken});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1024, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            //
            // tsbNeu
            //
            this.tsbNeu.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbNeu.Name = "tsbNeu";
            this.tsbNeu.Size = new System.Drawing.Size(31, 22);
            this.tsbNeu.Text = "Neu";
            this.tsbNeu.Click += new System.EventHandler(this.tsbNeu_Click);
            //
            // tsbSpeichern
            //
            this.tsbSpeichern.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbSpeichern.Name = "tsbSpeichern";
            this.tsbSpeichern.Size = new System.Drawing.Size(63, 22);
            this.tsbSpeichern.Text = "Speichern";
            this.tsbSpeichern.Click += new System.EventHandler(this.tsbSpeichern_Click);
            //
            // toolStripSeparator4
            //
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            //
            // tsbKunden
            //
            this.tsbKunden.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbKunden.Name = "tsbKunden";
            this.tsbKunden.Size = new System.Drawing.Size(51, 22);
            this.tsbKunden.Text = "Kunden";
            this.tsbKunden.Click += new System.EventHandler(this.tsbKunden_Click);
            //
            // tsbProdukte
            //
            this.tsbProdukte.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbProdukte.Name = "tsbProdukte";
            this.tsbProdukte.Size = new System.Drawing.Size(57, 22);
            this.tsbProdukte.Text = "Produkte";
            this.tsbProdukte.Click += new System.EventHandler(this.tsbProdukte_Click);
            //
            // tsbBestellungen
            //
            this.tsbBestellungen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbBestellungen.Name = "tsbBestellungen";
            this.tsbBestellungen.Size = new System.Drawing.Size(79, 22);
            this.tsbBestellungen.Text = "Bestellungen";
            this.tsbBestellungen.Click += new System.EventHandler(this.tsbBestellungen_Click);
            //
            // toolStripSeparator5
            //
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 25);
            //
            // tsbDrucken
            //
            this.tsbDrucken.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbDrucken.Name = "tsbDrucken";
            this.tsbDrucken.Size = new System.Drawing.Size(54, 22);
            this.tsbDrucken.Text = "Drucken";
            this.tsbDrucken.Click += new System.EventHandler(this.tsbDrucken_Click);
            //
            // statusStrip1
            //
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslStatus,
            this.tsslUser,
            this.tsslDateTime});
            this.statusStrip1.Location = new System.Drawing.Point(0, 678);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1024, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            //
            // tsslStatus
            //
            this.tsslStatus.Name = "tsslStatus";
            this.tsslStatus.Size = new System.Drawing.Size(809, 17);
            this.tsslStatus.Spring = true;
            this.tsslStatus.Text = "Bereit";
            this.tsslStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // tsslUser
            //
            this.tsslUser.Name = "tsslUser";
            this.tsslUser.Size = new System.Drawing.Size(100, 17);
            this.tsslUser.Text = "Benutzer: admin";
            //
            // tsslDateTime
            //
            this.tsslDateTime.Name = "tsslDateTime";
            this.tsslDateTime.Size = new System.Drawing.Size(100, 17);
            this.tsslDateTime.Text = "01.01.2024 12:00";
            //
            // panelDashboard
            //
            this.panelDashboard.BackColor = System.Drawing.Color.White;
            this.panelDashboard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelDashboard.Controls.Add(this.lblUmsatz);
            this.panelDashboard.Controls.Add(this.lblAnzahlBestellungen);
            this.panelDashboard.Controls.Add(this.lblAnzahlProdukte);
            this.panelDashboard.Controls.Add(this.lblAnzahlKunden);
            this.panelDashboard.Controls.Add(this.lblTitelUmsatz);
            this.panelDashboard.Controls.Add(this.lblTitelBestellungen);
            this.panelDashboard.Controls.Add(this.lblTitelProdukte);
            this.panelDashboard.Controls.Add(this.lblTitelKunden);
            this.panelDashboard.Controls.Add(this.lblWillkommen);
            this.panelDashboard.Location = new System.Drawing.Point(200, 150);
            this.panelDashboard.Name = "panelDashboard";
            this.panelDashboard.Size = new System.Drawing.Size(600, 350);
            this.panelDashboard.TabIndex = 3;
            //
            // lblUmsatz
            //
            this.lblUmsatz.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.lblUmsatz.ForeColor = System.Drawing.Color.Green;
            this.lblUmsatz.Location = new System.Drawing.Point(320, 230);
            this.lblUmsatz.Name = "lblUmsatz";
            this.lblUmsatz.Size = new System.Drawing.Size(250, 50);
            this.lblUmsatz.TabIndex = 8;
            this.lblUmsatz.Text = "0,00 €";
            this.lblUmsatz.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // lblAnzahlBestellungen
            //
            this.lblAnzahlBestellungen.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.lblAnzahlBestellungen.ForeColor = System.Drawing.Color.DarkOrange;
            this.lblAnzahlBestellungen.Location = new System.Drawing.Point(320, 130);
            this.lblAnzahlBestellungen.Name = "lblAnzahlBestellungen";
            this.lblAnzahlBestellungen.Size = new System.Drawing.Size(250, 50);
            this.lblAnzahlBestellungen.TabIndex = 7;
            this.lblAnzahlBestellungen.Text = "0";
            this.lblAnzahlBestellungen.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // lblAnzahlProdukte
            //
            this.lblAnzahlProdukte.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.lblAnzahlProdukte.ForeColor = System.Drawing.Color.Purple;
            this.lblAnzahlProdukte.Location = new System.Drawing.Point(30, 230);
            this.lblAnzahlProdukte.Name = "lblAnzahlProdukte";
            this.lblAnzahlProdukte.Size = new System.Drawing.Size(250, 50);
            this.lblAnzahlProdukte.TabIndex = 6;
            this.lblAnzahlProdukte.Text = "0";
            this.lblAnzahlProdukte.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // lblAnzahlKunden
            //
            this.lblAnzahlKunden.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.lblAnzahlKunden.ForeColor = System.Drawing.Color.DodgerBlue;
            this.lblAnzahlKunden.Location = new System.Drawing.Point(30, 130);
            this.lblAnzahlKunden.Name = "lblAnzahlKunden";
            this.lblAnzahlKunden.Size = new System.Drawing.Size(250, 50);
            this.lblAnzahlKunden.TabIndex = 5;
            this.lblAnzahlKunden.Text = "0";
            this.lblAnzahlKunden.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // lblTitelUmsatz
            //
            this.lblTitelUmsatz.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblTitelUmsatz.Location = new System.Drawing.Point(320, 200);
            this.lblTitelUmsatz.Name = "lblTitelUmsatz";
            this.lblTitelUmsatz.Size = new System.Drawing.Size(250, 30);
            this.lblTitelUmsatz.TabIndex = 4;
            this.lblTitelUmsatz.Text = "Umsatz (Gesamt)";
            this.lblTitelUmsatz.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // lblTitelBestellungen
            //
            this.lblTitelBestellungen.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblTitelBestellungen.Location = new System.Drawing.Point(320, 100);
            this.lblTitelBestellungen.Name = "lblTitelBestellungen";
            this.lblTitelBestellungen.Size = new System.Drawing.Size(250, 30);
            this.lblTitelBestellungen.TabIndex = 3;
            this.lblTitelBestellungen.Text = "Bestellungen";
            this.lblTitelBestellungen.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // lblTitelProdukte
            //
            this.lblTitelProdukte.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblTitelProdukte.Location = new System.Drawing.Point(30, 200);
            this.lblTitelProdukte.Name = "lblTitelProdukte";
            this.lblTitelProdukte.Size = new System.Drawing.Size(250, 30);
            this.lblTitelProdukte.TabIndex = 2;
            this.lblTitelProdukte.Text = "Produkte";
            this.lblTitelProdukte.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // lblTitelKunden
            //
            this.lblTitelKunden.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblTitelKunden.Location = new System.Drawing.Point(30, 100);
            this.lblTitelKunden.Name = "lblTitelKunden";
            this.lblTitelKunden.Size = new System.Drawing.Size(250, 30);
            this.lblTitelKunden.TabIndex = 1;
            this.lblTitelKunden.Text = "Kunden";
            this.lblTitelKunden.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // lblWillkommen
            //
            this.lblWillkommen.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblWillkommen.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblWillkommen.Location = new System.Drawing.Point(0, 0);
            this.lblWillkommen.Name = "lblWillkommen";
            this.lblWillkommen.Size = new System.Drawing.Size(598, 60);
            this.lblWillkommen.TabIndex = 0;
            this.lblWillkommen.Text = "Willkommen bei EnterpriseApp";
            this.lblWillkommen.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // timerClock
            //
            this.timerClock.Enabled = true;
            this.timerClock.Interval = 1000;
            this.timerClock.Tick += new System.EventHandler(this.timerClock_Tick);
            //
            // timerRefresh
            //
            this.timerRefresh.Enabled = true;
            this.timerRefresh.Interval = 30000;
            this.timerRefresh.Tick += new System.EventHandler(this.timerRefresh_Tick);
            //
            // MainForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1024, 700);
            this.Controls.Add(this.panelDashboard);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "EnterpriseApp - Verwaltungssoftware";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.panelDashboard.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem dateiToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem neuToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem öffnenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem speichernToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem druckenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem druckvorschauToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem beendenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bearbeitenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rückgängigToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem wiederholenToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem ausschneidenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem kopierenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem einfügenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stammdatenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem kundenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem produkteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bestellungenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem neueBestellungToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bestellübersichtToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem berichteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem kundenlisteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem produktlisteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bestellungenListeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem umsatzberichtToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem extrasToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem einstellungenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem datenbankToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testdatenGenerierenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hilfeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem infoToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsbNeu;
        private System.Windows.Forms.ToolStripButton tsbSpeichern;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton tsbKunden;
        private System.Windows.Forms.ToolStripButton tsbProdukte;
        private System.Windows.Forms.ToolStripButton tsbBestellungen;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripButton tsbDrucken;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel tsslStatus;
        private System.Windows.Forms.ToolStripStatusLabel tsslUser;
        private System.Windows.Forms.ToolStripStatusLabel tsslDateTime;
        private System.Windows.Forms.Panel panelDashboard;
        private System.Windows.Forms.Label lblWillkommen;
        private System.Windows.Forms.Label lblTitelKunden;
        private System.Windows.Forms.Label lblTitelProdukte;
        private System.Windows.Forms.Label lblTitelBestellungen;
        private System.Windows.Forms.Label lblTitelUmsatz;
        private System.Windows.Forms.Label lblAnzahlKunden;
        private System.Windows.Forms.Label lblAnzahlProdukte;
        private System.Windows.Forms.Label lblAnzahlBestellungen;
        private System.Windows.Forms.Label lblUmsatz;
        private System.Windows.Forms.Timer timerClock;
        private System.Windows.Forms.Timer timerRefresh;
    }
}
