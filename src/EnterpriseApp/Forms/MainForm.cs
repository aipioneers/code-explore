using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EnterpriseApp.Helpers;

namespace EnterpriseApp.Forms
{
    /// <summary>
    /// Hauptformular der Anwendung - MDI Container
    /// ACHTUNG: Diese Klasse ist über die Jahre sehr groß geworden!
    /// TODO: Sollte irgendwann mal aufgeteilt werden (Ticket #9999)
    /// Erstellt: 2015 - Hr. Schmidt
    /// Letzte Änderung: 2019 - diverse Entwickler
    /// </summary>
    public partial class MainForm : Form
    {
        // ============================================================
        // Private Variablen - viele davon sollten eigentlich woanders sein
        // ============================================================
        private bool _isLoading = false;
        private bool _hasChanges = false;
        private int _refreshCount = 0;
        private DateTime _lastRefresh;
        private string _currentModule = "";
        private int _errorCount = 0;
        private List<Form> _openForms = new List<Form>();
        private DataTable _dtKunden = null;
        private DataTable _dtProdukte = null;
        private DataTable _dtBestellungen = null;
        private int _anzKunden = 0;
        private int _anzProdukte = 0;
        private int _anzBestellungen = 0;
        private double _umsatzGesamt = 0;
        private bool _dashboardVisible = true;
        private Color _originalBackColor;
        private Font _originalFont;
        private int _formWidth = 1024;
        private int _formHeight = 700;
        private const int MIN_WIDTH = 800;
        private const int MIN_HEIGHT = 600;
        private const int DASHBOARD_WIDTH = 600;
        private const int DASHBOARD_HEIGHT = 350;

        // Statische Variablen (Anti-Pattern: sollten in Globals sein)
        private static MainForm _instance = null;
        private static int _instanceCount = 0;

        /// <summary>
        /// Konstruktor
        /// </summary>
        public MainForm()
        {
            InitializeComponent();

            // Singleton-artiges Verhalten (schlecht implementiert)
            _instance = this;
            _instanceCount++;

            // Globale Referenz setzen
            Globals.MainFormRef = this;

            // Standard-Werte setzen
            _isLoading = true;
            _lastRefresh = DateTime.Now;
            _originalBackColor = this.BackColor;

            // Form-Eigenschaften
            this.Text = "EnterpriseApp - Verwaltungssoftware v1.0";
            this.MinimumSize = new Size(MIN_WIDTH, MIN_HEIGHT);
        }

        /// <summary>
        /// Form Load Event - Hier passiert viel zu viel!
        /// </summary>
        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                _isLoading = true;
                this.Cursor = Cursors.WaitCursor;

                // Status aktualisieren
                SetStatus("Anwendung wird geladen...");

                // Benutzer-Info aktualisieren
                tsslUser.Text = "Benutzer: " + Globals.CurrentUser;

                // Datum/Zeit aktualisieren
                UpdateDateTime();

                // Dashboard positionieren
                CenterDashboard();

                // Dashboard-Daten laden
                LoadDashboardData();

                // Kategorien laden für spätere Verwendung
                LoadKategorien();

                // Menü-Status aktualisieren
                UpdateMenuState();

                // Toolbar-Status aktualisieren
                UpdateToolbarState();

                // Letzte Einstellungen laden
                LoadUserSettings();

                // Willkommensnachricht
                lblWillkommen.Text = "Willkommen, " + Globals.CurrentUser + "!";

                SetStatus("Bereit");
            }
            catch (Exception ex)
            {
                // Fehler beim Laden - aber trotzdem weitermachen
                _errorCount++;
                Globals.LogError(ex.Message);
                SetStatus("Fehler beim Laden: " + ex.Message);
            }
            finally
            {
                _isLoading = false;
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Dashboard-Daten aus der Datenbank laden
        /// Diese Methode ist viel zu lang!
        /// </summary>
        private void LoadDashboardData()
        {
            try
            {
                SetStatus("Dashboard wird aktualisiert...");
                Application.DoEvents();

                // Kunden zählen
                _anzKunden = DbHelper.GetAnzahlKunden();
                lblAnzahlKunden.Text = _anzKunden.ToString();

                // Je nach Anzahl Farbe ändern
                if (_anzKunden == 0)
                {
                    lblAnzahlKunden.ForeColor = Color.Red;
                }
                else if (_anzKunden < 10)
                {
                    lblAnzahlKunden.ForeColor = Color.Orange;
                }
                else
                {
                    lblAnzahlKunden.ForeColor = Color.DodgerBlue;
                }

                // Produkte zählen
                _anzProdukte = DbHelper.GetAnzahlProdukte();
                lblAnzahlProdukte.Text = _anzProdukte.ToString();

                // Je nach Anzahl Farbe ändern
                if (_anzProdukte == 0)
                {
                    lblAnzahlProdukte.ForeColor = Color.Red;
                }
                else if (_anzProdukte < 20)
                {
                    lblAnzahlProdukte.ForeColor = Color.Orange;
                }
                else
                {
                    lblAnzahlProdukte.ForeColor = Color.Purple;
                }

                // Bestellungen zählen
                _anzBestellungen = DbHelper.GetAnzahlBestellungen();
                lblAnzahlBestellungen.Text = _anzBestellungen.ToString();

                // Je nach Anzahl Farbe ändern
                if (_anzBestellungen == 0)
                {
                    lblAnzahlBestellungen.ForeColor = Color.Gray;
                }
                else if (_anzBestellungen < 10)
                {
                    lblAnzahlBestellungen.ForeColor = Color.Orange;
                }
                else
                {
                    lblAnzahlBestellungen.ForeColor = Color.DarkOrange;
                }

                // Umsatz berechnen
                _umsatzGesamt = DbHelper.GetUmsatzGesamt();
                lblUmsatz.Text = Utils.FormatCurrency(_umsatzGesamt);

                // Umsatz-Farbe
                if (_umsatzGesamt == 0)
                {
                    lblUmsatz.ForeColor = Color.Gray;
                }
                else if (_umsatzGesamt < 1000)
                {
                    lblUmsatz.ForeColor = Color.Orange;
                }
                else if (_umsatzGesamt < 10000)
                {
                    lblUmsatz.ForeColor = Color.DarkGreen;
                }
                else
                {
                    lblUmsatz.ForeColor = Color.Green;
                }

                _lastRefresh = DateTime.Now;
                _refreshCount++;

                SetStatus("Dashboard aktualisiert (" + _refreshCount + " mal)");
            }
            catch (Exception ex)
            {
                _errorCount++;
                lblAnzahlKunden.Text = "?";
                lblAnzahlProdukte.Text = "?";
                lblAnzahlBestellungen.Text = "?";
                lblUmsatz.Text = "?";
                Globals.LogError("Dashboard: " + ex.Message);
            }
        }

        /// <summary>
        /// Kategorien für Produkte laden
        /// </summary>
        private void LoadKategorien()
        {
            try
            {
                Globals.Kategorien.Clear();
                DataTable dt = DbHelper.GetKategorien();
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        string kat = row["Kategorie"].ToString();
                        if (!string.IsNullOrEmpty(kat))
                        {
                            Globals.Kategorien.Add(kat);
                        }
                    }
                }

                // Standard-Kategorien hinzufügen falls leer
                if (Globals.Kategorien.Count == 0)
                {
                    Globals.Kategorien.Add("Elektronik");
                    Globals.Kategorien.Add("Bürobedarf");
                    Globals.Kategorien.Add("Möbel");
                    Globals.Kategorien.Add("Software");
                    Globals.Kategorien.Add("Dienstleistung");
                }
            }
            catch
            {
                // Ignorieren
            }
        }

        /// <summary>
        /// Benutzereinstellungen laden (aktuell nicht implementiert)
        /// </summary>
        private void LoadUserSettings()
        {
            // TODO: Aus Registry oder Datei laden
            // Ticket #4567 - offen seit 2017
            try
            {
                string maxRecords = Utils.GetConfigValue("MaxRecords");
                if (!string.IsNullOrEmpty(maxRecords))
                {
                    Globals.MaxRecords = int.Parse(maxRecords);
                }
            }
            catch { }
        }

        /// <summary>
        /// Dashboard zentrieren
        /// </summary>
        private void CenterDashboard()
        {
            if (panelDashboard != null)
            {
                int x = (this.ClientSize.Width - panelDashboard.Width) / 2;
                int y = (this.ClientSize.Height - panelDashboard.Height) / 2;

                // Offset für Menü und Toolbar
                y = y + 50;

                if (x < 0) x = 10;
                if (y < 60) y = 60;

                panelDashboard.Location = new Point(x, y);
            }
        }

        /// <summary>
        /// Menü-Status aktualisieren
        /// </summary>
        private void UpdateMenuState()
        {
            // Speichern nur aktiv wenn Änderungen vorhanden
            speichernToolStripMenuItem.Enabled = _hasChanges;

            // Drucken nur aktiv wenn Daten vorhanden
            druckenToolStripMenuItem.Enabled = (_anzKunden > 0 || _anzProdukte > 0 || _anzBestellungen > 0);
        }

        /// <summary>
        /// Toolbar-Status aktualisieren
        /// </summary>
        private void UpdateToolbarState()
        {
            tsbSpeichern.Enabled = _hasChanges;
        }

        /// <summary>
        /// Status in Statusleiste anzeigen
        /// </summary>
        private void SetStatus(string message)
        {
            tsslStatus.Text = message;
            Application.DoEvents();
        }

        /// <summary>
        /// Datum und Uhrzeit aktualisieren
        /// </summary>
        private void UpdateDateTime()
        {
            tsslDateTime.Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
        }

        // ============================================================
        // MENÜ EVENT HANDLER - Datei Menü
        // ============================================================

        private void neuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Neuer Datensatz - je nach aktivem Formular
            try
            {
                Form activeChild = this.ActiveMdiChild;
                if (activeChild != null)
                {
                    if (activeChild is FrmKunden)
                    {
                        ((FrmKunden)activeChild).NeuerKunde();
                    }
                    else if (activeChild is FrmProdukte)
                    {
                        ((FrmProdukte)activeChild).NeuesProdukt();
                    }
                    else if (activeChild is FrmBestellungen)
                    {
                        ((FrmBestellungen)activeChild).NeueBestellung();
                    }
                }
                else
                {
                    // Kein aktives Formular - Bestellungen öffnen
                    OpenBestellungenForm(true);
                }
            }
            catch (Exception ex)
            {
                Utils.ShowError("Fehler bei Neu: " + ex.Message);
            }
        }

        private void öffnenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Öffnen Dialog - nicht wirklich implementiert
            MessageBox.Show("Diese Funktion ist noch nicht implementiert.\n\nBitte verwenden Sie die Stammdaten-Menüs.",
                "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void speichernToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Speichern - delegiert an aktives Formular
            try
            {
                Form activeChild = this.ActiveMdiChild;
                if (activeChild != null)
                {
                    if (activeChild is FrmKunden)
                    {
                        ((FrmKunden)activeChild).Speichern();
                    }
                    else if (activeChild is FrmProdukte)
                    {
                        ((FrmProdukte)activeChild).Speichern();
                    }
                    else if (activeChild is FrmBestellungen)
                    {
                        ((FrmBestellungen)activeChild).Speichern();
                    }
                }
                _hasChanges = false;
                UpdateMenuState();
                UpdateToolbarState();
                Globals.IncrementSaveCount();
            }
            catch (Exception ex)
            {
                Utils.ShowError("Fehler beim Speichern: " + ex.Message);
            }
        }

        private void druckenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Drucken - öffnet Druckvorschau
            OpenDruckvorschauForm(1); // 1 = Standardbericht
        }

        private void druckvorschauToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Druckvorschau öffnen
            OpenDruckvorschauForm(0);
        }

        private void beendenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Anwendung beenden
            this.Close();
        }

        // ============================================================
        // MENÜ EVENT HANDLER - Stammdaten Menü
        // ============================================================

        private void kundenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenKundenForm();
        }

        private void produkteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenProdukteForm();
        }

        // ============================================================
        // MENÜ EVENT HANDLER - Bestellungen Menü
        // ============================================================

        private void neueBestellungToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenBestellungenForm(true);
        }

        private void bestellübersichtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenBestellungenForm(false);
        }

        // ============================================================
        // MENÜ EVENT HANDLER - Berichte Menü
        // ============================================================

        private void kundenlisteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenDruckvorschauForm(1);
        }

        private void produktlisteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenDruckvorschauForm(2);
        }

        private void bestellungenListeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenDruckvorschauForm(3);
        }

        private void umsatzberichtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenDruckvorschauForm(4);
        }

        // ============================================================
        // MENÜ EVENT HANDLER - Extras Menü
        // ============================================================

        private void einstellungenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Einstellungen sind noch nicht implementiert.\n\n" +
                "Aktuelle Konfiguration:\n" +
                "- Datenbankpfad: " + Globals.DbPath + "\n" +
                "- Benutzer: " + Globals.CurrentUser + "\n" +
                "- Max. Datensätze: " + Globals.MaxRecords,
                "Einstellungen", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void datenbankToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                SetStatus("Datenbank wird optimiert...");
                this.Cursor = Cursors.WaitCursor;
                Application.DoEvents();

                DbHelper.OptimizeDatabase();

                MessageBox.Show("Datenbank wurde optimiert.", "Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Utils.ShowError("Fehler bei Datenbankoptimierung: " + ex.Message);
            }
            finally
            {
                this.Cursor = Cursors.Default;
                SetStatus("Bereit");
            }
        }

        private void testdatenGenerierenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Möchten Sie Testdaten generieren?\n\nVorhandene Daten werden NICHT gelöscht.",
                "Testdaten", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    SetStatus("Testdaten werden generiert...");
                    this.Cursor = Cursors.WaitCursor;
                    Application.DoEvents();

                    DbHelper.InsertSampleData();
                    LoadDashboardData();

                    MessageBox.Show("Testdaten wurden generiert.", "Information",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    Utils.ShowError("Fehler bei Testdaten: " + ex.Message);
                }
                finally
                {
                    this.Cursor = Cursors.Default;
                    SetStatus("Bereit");
                }
            }
        }

        // ============================================================
        // MENÜ EVENT HANDLER - Hilfe Menü
        // ============================================================

        private void infoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string info = "EnterpriseApp - Verwaltungssoftware\n\n" +
                "Version: 1.0.0.0\n" +
                "Copyright © 2015-2019 Musterfirma GmbH\n\n" +
                "Entwickelt von: Hr. Schmidt, Fr. Weber, Hr. Müller\n\n" +
                "Statistik:\n" +
                "- Formulare geöffnet: " + Globals.FormOpenCount + "\n" +
                "- Speichervorgänge: " + Globals.SaveCount + "\n" +
                "- Fehler: " + _errorCount + "\n" +
                "- Dashboard-Aktualisierungen: " + _refreshCount + "\n\n" +
                "Datenbank: " + Globals.DbPath;

            MessageBox.Show(info, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ============================================================
        // TOOLBAR EVENT HANDLER
        // ============================================================

        private void tsbNeu_Click(object sender, EventArgs e)
        {
            neuToolStripMenuItem_Click(sender, e);
        }

        private void tsbSpeichern_Click(object sender, EventArgs e)
        {
            speichernToolStripMenuItem_Click(sender, e);
        }

        private void tsbKunden_Click(object sender, EventArgs e)
        {
            OpenKundenForm();
        }

        private void tsbProdukte_Click(object sender, EventArgs e)
        {
            OpenProdukteForm();
        }

        private void tsbBestellungen_Click(object sender, EventArgs e)
        {
            OpenBestellungenForm(false);
        }

        private void tsbDrucken_Click(object sender, EventArgs e)
        {
            druckenToolStripMenuItem_Click(sender, e);
        }

        // ============================================================
        // FORMULAR-ÖFFNEN METHODEN
        // ============================================================

        /// <summary>
        /// Öffnet das Kunden-Formular
        /// </summary>
        private void OpenKundenForm()
        {
            try
            {
                SetStatus("Kundenverwaltung wird geöffnet...");
                this.Cursor = Cursors.WaitCursor;

                // Prüfen ob schon offen
                if (Globals.KundenFormRef != null && !((Form)Globals.KundenFormRef).IsDisposed)
                {
                    ((Form)Globals.KundenFormRef).Activate();
                    ((Form)Globals.KundenFormRef).BringToFront();
                }
                else
                {
                    // Neues Formular erstellen
                    FrmKunden frm = new FrmKunden();
                    frm.MdiParent = this;
                    frm.WindowState = FormWindowState.Maximized;
                    frm.Show();

                    Globals.KundenFormRef = frm;
                    Globals.FormOpenCount++;
                    _openForms.Add(frm);

                    // Dashboard ausblenden wenn MDI-Child offen
                    panelDashboard.Visible = false;
                    _dashboardVisible = false;
                }

                _currentModule = "Kunden";
                SetStatus("Kundenverwaltung");
            }
            catch (Exception ex)
            {
                Utils.ShowError("Fehler beim Öffnen der Kundenverwaltung: " + ex.Message);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Öffnet das Produkte-Formular
        /// </summary>
        private void OpenProdukteForm()
        {
            try
            {
                SetStatus("Produktverwaltung wird geöffnet...");
                this.Cursor = Cursors.WaitCursor;

                // Prüfen ob schon offen
                if (Globals.ProdukteFormRef != null && !((Form)Globals.ProdukteFormRef).IsDisposed)
                {
                    ((Form)Globals.ProdukteFormRef).Activate();
                    ((Form)Globals.ProdukteFormRef).BringToFront();
                }
                else
                {
                    // Neues Formular erstellen
                    FrmProdukte frm = new FrmProdukte();
                    frm.MdiParent = this;
                    frm.WindowState = FormWindowState.Maximized;
                    frm.Show();

                    Globals.ProdukteFormRef = frm;
                    Globals.FormOpenCount++;
                    _openForms.Add(frm);

                    // Dashboard ausblenden
                    panelDashboard.Visible = false;
                    _dashboardVisible = false;
                }

                _currentModule = "Produkte";
                SetStatus("Produktverwaltung");
            }
            catch (Exception ex)
            {
                Utils.ShowError("Fehler beim Öffnen der Produktverwaltung: " + ex.Message);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Öffnet das Bestellungen-Formular
        /// </summary>
        private void OpenBestellungenForm(bool neueBestellung)
        {
            try
            {
                SetStatus("Bestellverwaltung wird geöffnet...");
                this.Cursor = Cursors.WaitCursor;

                // Prüfen ob schon offen
                if (Globals.BestellungenFormRef != null && !((Form)Globals.BestellungenFormRef).IsDisposed)
                {
                    ((Form)Globals.BestellungenFormRef).Activate();
                    ((Form)Globals.BestellungenFormRef).BringToFront();

                    if (neueBestellung)
                    {
                        ((FrmBestellungen)Globals.BestellungenFormRef).NeueBestellung();
                    }
                }
                else
                {
                    // Neues Formular erstellen
                    FrmBestellungen frm = new FrmBestellungen();
                    frm.MdiParent = this;
                    frm.WindowState = FormWindowState.Maximized;
                    frm.Show();

                    Globals.BestellungenFormRef = frm;
                    Globals.FormOpenCount++;
                    _openForms.Add(frm);

                    if (neueBestellung)
                    {
                        frm.NeueBestellung();
                    }

                    // Dashboard ausblenden
                    panelDashboard.Visible = false;
                    _dashboardVisible = false;
                }

                _currentModule = "Bestellungen";
                SetStatus("Bestellverwaltung");
            }
            catch (Exception ex)
            {
                Utils.ShowError("Fehler beim Öffnen der Bestellverwaltung: " + ex.Message);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Öffnet die Druckvorschau
        /// </summary>
        private void OpenDruckvorschauForm(int reportType)
        {
            try
            {
                SetStatus("Druckvorschau wird geöffnet...");
                this.Cursor = Cursors.WaitCursor;

                // Immer neues Formular erstellen
                FrmDruckvorschau frm = new FrmDruckvorschau(reportType);
                frm.MdiParent = this;
                frm.WindowState = FormWindowState.Maximized;
                frm.Show();

                Globals.DruckvorschauFormRef = frm;
                Globals.FormOpenCount++;
                _openForms.Add(frm);

                // Dashboard ausblenden
                panelDashboard.Visible = false;
                _dashboardVisible = false;

                _currentModule = "Druckvorschau";
                SetStatus("Druckvorschau");
            }
            catch (Exception ex)
            {
                Utils.ShowError("Fehler beim Öffnen der Druckvorschau: " + ex.Message);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        // ============================================================
        // TIMER EVENT HANDLER
        // ============================================================

        private void timerClock_Tick(object sender, EventArgs e)
        {
            UpdateDateTime();
        }

        private void timerRefresh_Tick(object sender, EventArgs e)
        {
            // Dashboard alle 30 Sekunden aktualisieren wenn sichtbar
            if (_dashboardVisible && panelDashboard.Visible)
            {
                LoadDashboardData();
            }
        }

        // ============================================================
        // FORM EVENT HANDLER
        // ============================================================

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (!_isLoading)
            {
                CenterDashboard();
                _formWidth = this.Width;
                _formHeight = this.Height;
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Prüfen ob Änderungen vorhanden
            if (_hasChanges)
            {
                DialogResult result = MessageBox.Show(
                    "Es gibt ungespeicherte Änderungen.\n\nMöchten Sie die Anwendung trotzdem beenden?",
                    "Beenden", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }

            // Alle offenen Formulare schließen
            try
            {
                foreach (Form frm in _openForms.ToList())
                {
                    if (frm != null && !frm.IsDisposed)
                    {
                        frm.Close();
                    }
                }
                _openForms.Clear();
            }
            catch { }

            // Globale Referenzen aufräumen
            Globals.MainFormRef = null;
            Globals.KundenFormRef = null;
            Globals.ProdukteFormRef = null;
            Globals.BestellungenFormRef = null;
            Globals.DruckvorschauFormRef = null;

            // Log schreiben
            Utils.Log("Anwendung beendet");
        }

        // ============================================================
        // ÖFFENTLICHE METHODEN
        // ============================================================

        /// <summary>
        /// Markiert dass Änderungen vorhanden sind
        /// </summary>
        public void MarkAsChanged()
        {
            _hasChanges = true;
            UpdateMenuState();
            UpdateToolbarState();
        }

        /// <summary>
        /// Dashboard aktualisieren (von außen aufrufbar)
        /// </summary>
        public void RefreshDashboard()
        {
            LoadDashboardData();
        }

        /// <summary>
        /// Dashboard ein-/ausblenden
        /// </summary>
        public void ShowDashboard(bool show)
        {
            panelDashboard.Visible = show;
            _dashboardVisible = show;

            if (show)
            {
                CenterDashboard();
                LoadDashboardData();
            }
        }

        /// <summary>
        /// Prüft ob MDI-Children offen sind
        /// </summary>
        public bool HasOpenForms()
        {
            return _openForms.Count > 0 && _openForms.Any(f => f != null && !f.IsDisposed);
        }

        /// <summary>
        /// Formular aus der Liste entfernen (wird von Child-Forms aufgerufen)
        /// </summary>
        public void RemoveForm(Form frm)
        {
            if (_openForms.Contains(frm))
            {
                _openForms.Remove(frm);
            }

            // Dashboard wieder anzeigen wenn keine Formulare mehr offen
            if (!HasOpenForms())
            {
                ShowDashboard(true);
                _currentModule = "";
                SetStatus("Bereit");
            }
        }

        // ============================================================
        // NICHT VERWENDETE METHODEN (aber nicht löschen!)
        // ============================================================

        [Obsolete("Nicht mehr verwendet - siehe LoadDashboardData")]
        private void LoadStatistik()
        {
            // Alte Implementierung
            _dtKunden = DbHelper.GetAlleKunden();
            _dtProdukte = DbHelper.GetAlleProdukte();
            _dtBestellungen = DbHelper.GetAlleBestellungen();
        }

        [Obsolete]
        private void CalculateOldWay()
        {
            // Alte Berechnungsmethode
            int total = 0;
            if (_dtKunden != null) total += _dtKunden.Rows.Count;
            if (_dtProdukte != null) total += _dtProdukte.Rows.Count;
            // etc.
        }

        private void LoadLegacyData()
        {
            // TODO: Entfernen wenn niemand mehr verwendet
            // War für Import aus altem System
        }

        private void ExportData()
        {
            // Nicht implementiert
            // Ticket #7890
        }

        private void ImportData()
        {
            // Nicht implementiert
            // Ticket #7891
        }

        // Getter für Singleton
        public static MainForm Instance
        {
            get { return _instance; }
        }

        // Weitere Properties
        public string CurrentModule { get { return _currentModule; } }
        public int ErrorCount { get { return _errorCount; } }
        public bool HasChanges { get { return _hasChanges; } set { _hasChanges = value; } }
    }
}
