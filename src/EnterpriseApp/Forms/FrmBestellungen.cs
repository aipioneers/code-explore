using System;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;
using EnterpriseApp.Helpers;

namespace EnterpriseApp.Forms
{
    // Bestellungsverwaltung - Legacy Form
    // WARNUNG: Absichtlich schlecht strukturiert für Schulungszwecke
    public partial class FrmBestellungen : Form
    {
        // Anti-Pattern: Viele primitive Felder statt Objekten
        public int aktuelleBestellungId = 0;
        public string aktuelleBestellNr = "";
        public int aktuellerKundeId = 0;
        public bool istNeuanlage = false;
        public bool hatAenderungen = false;
        public DataTable bestellungenTabelle = null;
        public DataTable positionenTabelle = null;
        private decimal gesamtSumme = 0;
        private int anzahlPositionen = 0;
        private string letzterStatus = "";
        private bool _loadingData = false;

        // Magic Numbers
        private const int MAX_POSITIONEN = 99;
        private const decimal MAX_RABATT = 50;
        private const decimal MWST_SATZ = 19;

        public FrmBestellungen()
        {
            InitializeComponent();
        }

        // God Method - Form Load
        private void FrmBestellungen_Load(object sender, EventArgs e)
        {
            _loadingData = true;

            // Status ComboBox - hardcoded
            cboStatus.Items.Clear();
            cboStatus.Items.Add("(Alle)");
            cboStatus.Items.Add("Neu");
            cboStatus.Items.Add("In Bearbeitung");
            cboStatus.Items.Add("Versendet");
            cboStatus.Items.Add("Abgeschlossen");
            cboStatus.Items.Add("Storniert");
            cboStatus.SelectedIndex = 0;

            cboBestellStatus.Items.Clear();
            cboBestellStatus.Items.Add("Neu");
            cboBestellStatus.Items.Add("In Bearbeitung");
            cboBestellStatus.Items.Add("Versendet");
            cboBestellStatus.Items.Add("Abgeschlossen");
            cboBestellStatus.Items.Add("Storniert");
            cboBestellStatus.SelectedIndex = 0;

            // Kunden laden für Auswahl
            LadeKundenListe();

            // Bestellungen laden
            LadeBestellungenListe();

            // Grid für Positionen konfigurieren
            KonfigurierePositionenGrid();

            LeereAlleFelder();

            _loadingData = false;

            lblStatus.Text = "Bestellverwaltung geladen - " + DateTime.Now.ToString("HH:mm:ss");
            Globals.AktiveForm = "Bestellungen";
        }

        // Kunden laden - direkte SQL
        private void LadeKundenListe()
        {
            cboKunde.Items.Clear();
            cboKunde.Items.Add("");

            try
            {
                string connStr = "Data Source=" + Globals.DbPath + ";Version=3;";
                SQLiteConnection conn = new SQLiteConnection(connStr);
                conn.Open();

                string sql = "SELECT Id, KundenNr, Firma, Vorname, Nachname FROM Kunden ORDER BY Nachname, Vorname";
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                SQLiteDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string anzeige = reader["KundenNr"].ToString() + " - ";
                    if (reader["Firma"] != DBNull.Value && reader["Firma"].ToString() != "")
                    {
                        anzeige += reader["Firma"].ToString();
                    }
                    else
                    {
                        anzeige += reader["Vorname"].ToString() + " " + reader["Nachname"].ToString();
                    }
                    cboKunde.Items.Add(new KundeItem(Convert.ToInt32(reader["Id"]), anzeige));
                }

                reader.Close();
                conn.Close();
            }
            catch { }
        }

        // Bestellungen laden - lange Methode mit vielen Verantwortlichkeiten
        private void LadeBestellungenListe()
        {
            try
            {
                string connStr = "Data Source=" + Globals.DbPath + ";Version=3;";
                SQLiteConnection conn = new SQLiteConnection(connStr);
                conn.Open();

                string sql = @"SELECT b.Id, b.BestellNr, b.BestellDatum,
                    k.KundenNr, k.Firma, k.Vorname, k.Nachname,
                    b.Status, b.GesamtSumme
                    FROM Bestellungen b
                    LEFT JOIN Kunden k ON b.KundeId = k.Id
                    WHERE 1=1";

                // Filter Status
                if (cboStatus.SelectedIndex > 0)
                {
                    sql += " AND b.Status = '" + cboStatus.SelectedItem.ToString() + "'";
                }

                // Filter Datum
                if (dtpVon.Checked)
                {
                    sql += " AND b.BestellDatum >= '" + dtpVon.Value.ToString("yyyy-MM-dd") + "'";
                }
                if (dtpBis.Checked)
                {
                    sql += " AND b.BestellDatum <= '" + dtpBis.Value.ToString("yyyy-MM-dd") + " 23:59:59'";
                }

                sql += " ORDER BY b.BestellDatum DESC, b.BestellNr DESC";

                SQLiteDataAdapter adapter = new SQLiteDataAdapter(sql, conn);
                bestellungenTabelle = new DataTable();
                adapter.Fill(bestellungenTabelle);
                conn.Close();

                // Kundenname zusammenbauen - inline
                bestellungenTabelle.Columns.Add("KundeName", typeof(string));
                foreach (DataRow row in bestellungenTabelle.Rows)
                {
                    string name = "";
                    if (row["Firma"] != DBNull.Value && row["Firma"].ToString() != "")
                    {
                        name = row["Firma"].ToString();
                    }
                    else
                    {
                        name = row["Vorname"].ToString() + " " + row["Nachname"].ToString();
                    }
                    row["KundeName"] = row["KundenNr"].ToString() + " - " + name;
                }

                dgvBestellungen.DataSource = null;
                dgvBestellungen.DataSource = bestellungenTabelle;

                // Spalten konfigurieren - Magic Strings
                if (dgvBestellungen.Columns.Count > 0)
                {
                    dgvBestellungen.Columns["Id"].Visible = false;
                    dgvBestellungen.Columns["KundenNr"].Visible = false;
                    dgvBestellungen.Columns["Firma"].Visible = false;
                    dgvBestellungen.Columns["Vorname"].Visible = false;
                    dgvBestellungen.Columns["Nachname"].Visible = false;
                    dgvBestellungen.Columns["BestellNr"].HeaderText = "Bestell-Nr";
                    dgvBestellungen.Columns["BestellNr"].Width = 90;
                    dgvBestellungen.Columns["BestellDatum"].HeaderText = "Datum";
                    dgvBestellungen.Columns["BestellDatum"].Width = 80;
                    dgvBestellungen.Columns["BestellDatum"].DefaultCellStyle.Format = "dd.MM.yyyy";
                    dgvBestellungen.Columns["KundeName"].HeaderText = "Kunde";
                    dgvBestellungen.Columns["KundeName"].Width = 180;
                    dgvBestellungen.Columns["Status"].HeaderText = "Status";
                    dgvBestellungen.Columns["Status"].Width = 100;
                    dgvBestellungen.Columns["GesamtSumme"].HeaderText = "Summe";
                    dgvBestellungen.Columns["GesamtSumme"].Width = 90;
                    dgvBestellungen.Columns["GesamtSumme"].DefaultCellStyle.Format = "N2";
                    dgvBestellungen.Columns["GesamtSumme"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }

                lblStatus.Text = bestellungenTabelle.Rows.Count.ToString() + " Bestellungen geladen";
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Fehler: " + ex.Message;
            }
        }

        // Positionen Grid konfigurieren
        private void KonfigurierePositionenGrid()
        {
            dgvPositionen.Columns.Clear();
            dgvPositionen.Columns.Add("Id", "Id");
            dgvPositionen.Columns.Add("ProduktId", "ProduktId");
            dgvPositionen.Columns.Add("ArtikelNr", "Artikel-Nr");
            dgvPositionen.Columns.Add("Bezeichnung", "Bezeichnung");
            dgvPositionen.Columns.Add("Menge", "Menge");
            dgvPositionen.Columns.Add("Einheit", "Einheit");
            dgvPositionen.Columns.Add("Einzelpreis", "Einzelpreis");
            dgvPositionen.Columns.Add("Rabatt", "Rabatt %");
            dgvPositionen.Columns.Add("Positionssumme", "Summe");

            dgvPositionen.Columns["Id"].Visible = false;
            dgvPositionen.Columns["ProduktId"].Visible = false;
            dgvPositionen.Columns["ArtikelNr"].Width = 80;
            dgvPositionen.Columns["Bezeichnung"].Width = 200;
            dgvPositionen.Columns["Menge"].Width = 60;
            dgvPositionen.Columns["Einheit"].Width = 50;
            dgvPositionen.Columns["Einzelpreis"].Width = 80;
            dgvPositionen.Columns["Einzelpreis"].DefaultCellStyle.Format = "N2";
            dgvPositionen.Columns["Rabatt"].Width = 60;
            dgvPositionen.Columns["Positionssumme"].Width = 90;
            dgvPositionen.Columns["Positionssumme"].DefaultCellStyle.Format = "N2";
        }

        // Filter Button
        private void btnFilter_Click(object sender, EventArgs e)
        {
            LadeBestellungenListe();
        }

        // Selection Changed - lädt Bestellungsdaten
        private void dgvBestellungen_SelectionChanged(object sender, EventArgs e)
        {
            if (_loadingData) return;

            if (dgvBestellungen.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dgvBestellungen.SelectedRows[0];
                if (row.Cells["Id"].Value != null && row.Cells["Id"].Value != DBNull.Value)
                {
                    int id = Convert.ToInt32(row.Cells["Id"].Value);
                    LadeBestellungById(id);
                }
            }
        }

        // Bestellung laden - lange Methode
        private void LadeBestellungById(int bestellungId)
        {
            try
            {
                string connStr = "Data Source=" + Globals.DbPath + ";Version=3;";
                SQLiteConnection conn = new SQLiteConnection(connStr);
                conn.Open();

                // Bestellung laden
                string sql = "SELECT * FROM Bestellungen WHERE Id = " + bestellungId.ToString();
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                SQLiteDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    aktuelleBestellungId = bestellungId;
                    aktuelleBestellNr = reader["BestellNr"].ToString();
                    istNeuanlage = false;

                    txtBestellNr.Text = aktuelleBestellNr;
                    dtpBestellDatum.Value = Convert.ToDateTime(reader["BestellDatum"]);

                    int kundeId = Convert.ToInt32(reader["KundeId"]);
                    aktuellerKundeId = kundeId;

                    // Kunde in ComboBox finden - umständlich
                    for (int i = 1; i < cboKunde.Items.Count; i++)
                    {
                        if (cboKunde.Items[i] is KundeItem ki && ki.Id == kundeId)
                        {
                            cboKunde.SelectedIndex = i;
                            break;
                        }
                    }

                    // Status setzen
                    string status = reader["Status"].ToString();
                    letzterStatus = status;
                    int statusIndex = cboBestellStatus.Items.IndexOf(status);
                    if (statusIndex >= 0) cboBestellStatus.SelectedIndex = statusIndex;

                    txtBemerkung.Text = reader["Bemerkung"] != DBNull.Value ? reader["Bemerkung"].ToString() : "";
                    txtLieferadresse.Text = reader["Lieferadresse"] != DBNull.Value ? reader["Lieferadresse"].ToString() : "";

                    gesamtSumme = reader["GesamtSumme"] != DBNull.Value ? Convert.ToDecimal(reader["GesamtSumme"]) : 0;
                }
                reader.Close();

                // Positionen laden
                LadePositionen(bestellungId);

                conn.Close();

                hatAenderungen = false;
                lblStatus.Text = "Bestellung " + aktuelleBestellNr + " geladen";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler: " + ex.Message);
            }
        }

        // Positionen laden
        private void LadePositionen(int bestellungId)
        {
            dgvPositionen.Rows.Clear();
            gesamtSumme = 0;
            anzahlPositionen = 0;

            try
            {
                string connStr = "Data Source=" + Globals.DbPath + ";Version=3;";
                SQLiteConnection conn = new SQLiteConnection(connStr);
                conn.Open();

                string sql = @"SELECT bp.Id, bp.ProduktId, p.Artikelnummer, p.Produktname,
                    bp.Menge, bp.Einheit, bp.Einzelpreis, bp.Rabatt, bp.Positionssumme
                    FROM BestellPositionen bp
                    LEFT JOIN Produkte p ON bp.ProduktId = p.Id
                    WHERE bp.BestellungId = " + bestellungId.ToString() +
                    " ORDER BY bp.Position";

                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                SQLiteDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    int rowIndex = dgvPositionen.Rows.Add();
                    dgvPositionen.Rows[rowIndex].Cells["Id"].Value = reader["Id"];
                    dgvPositionen.Rows[rowIndex].Cells["ProduktId"].Value = reader["ProduktId"];
                    dgvPositionen.Rows[rowIndex].Cells["ArtikelNr"].Value = reader["Artikelnummer"];
                    dgvPositionen.Rows[rowIndex].Cells["Bezeichnung"].Value = reader["Produktname"];
                    dgvPositionen.Rows[rowIndex].Cells["Menge"].Value = reader["Menge"];
                    dgvPositionen.Rows[rowIndex].Cells["Einheit"].Value = reader["Einheit"];
                    dgvPositionen.Rows[rowIndex].Cells["Einzelpreis"].Value = reader["Einzelpreis"];
                    dgvPositionen.Rows[rowIndex].Cells["Rabatt"].Value = reader["Rabatt"];
                    dgvPositionen.Rows[rowIndex].Cells["Positionssumme"].Value = reader["Positionssumme"];

                    gesamtSumme += Convert.ToDecimal(reader["Positionssumme"]);
                    anzahlPositionen++;
                }

                reader.Close();
                conn.Close();

                BerechneGesamtsumme();
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Fehler Positionen: " + ex.Message;
            }
        }

        // Gesamtsumme berechnen
        private void BerechneGesamtsumme()
        {
            gesamtSumme = 0;
            anzahlPositionen = 0;

            foreach (DataGridViewRow row in dgvPositionen.Rows)
            {
                if (row.Cells["Positionssumme"].Value != null)
                {
                    decimal posSum = 0;
                    if (decimal.TryParse(row.Cells["Positionssumme"].Value.ToString(), out posSum))
                    {
                        gesamtSumme += posSum;
                        anzahlPositionen++;
                    }
                }
            }

            decimal netto = gesamtSumme;
            decimal mwst = netto * (MWST_SATZ / 100);
            decimal brutto = netto + mwst;

            txtNetto.Text = netto.ToString("N2");
            txtMwst.Text = mwst.ToString("N2");
            txtBrutto.Text = brutto.ToString("N2");
            lblAnzahlPos.Text = anzahlPositionen.ToString() + " Positionen";
        }

        // Neu Button
        private void btnNeu_Click(object sender, EventArgs e)
        {
            LeereAlleFelder();

            // Bestellnummer generieren
            string neueBestellNr = GeneriereBestellnummer();
            txtBestellNr.Text = neueBestellNr;
            aktuelleBestellNr = neueBestellNr;

            dtpBestellDatum.Value = DateTime.Now;
            cboBestellStatus.SelectedIndex = 0; // "Neu"

            istNeuanlage = true;
            aktuelleBestellungId = 0;

            cboKunde.Focus();
            lblStatus.Text = "Neue Bestellung anlegen...";
        }

        // Bestellnummer generieren
        private string GeneriereBestellnummer()
        {
            string neueNr = "";

            try
            {
                string connStr = "Data Source=" + Globals.DbPath + ";Version=3;";
                SQLiteConnection conn = new SQLiteConnection(connStr);
                conn.Open();

                // Format: B-YYYYMMDD-XXX
                string prefix = "B-" + DateTime.Now.ToString("yyyyMMdd") + "-";
                string sql = "SELECT MAX(CAST(SUBSTR(BestellNr, " + (prefix.Length + 1).ToString() + ") AS INTEGER)) FROM Bestellungen WHERE BestellNr LIKE '" + prefix + "%'";
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                object result = cmd.ExecuteScalar();

                int letzteNr = 0;
                if (result != null && result != DBNull.Value)
                {
                    letzteNr = Convert.ToInt32(result);
                }

                neueNr = prefix + (letzteNr + 1).ToString().PadLeft(3, '0');
                conn.Close();
            }
            catch
            {
                neueNr = "B-" + DateTime.Now.ToString("yyyyMMddHHmmss");
            }

            return neueNr;
        }

        // Felder leeren
        private void LeereAlleFelder()
        {
            txtBestellNr.Text = "";
            dtpBestellDatum.Value = DateTime.Now;
            cboKunde.SelectedIndex = 0;
            cboBestellStatus.SelectedIndex = 0;
            txtBemerkung.Text = "";
            txtLieferadresse.Text = "";
            dgvPositionen.Rows.Clear();
            txtNetto.Text = "0,00";
            txtMwst.Text = "0,00";
            txtBrutto.Text = "0,00";
            lblAnzahlPos.Text = "0 Positionen";

            aktuelleBestellungId = 0;
            aktuellerKundeId = 0;
            gesamtSumme = 0;
            anzahlPositionen = 0;
            hatAenderungen = false;
        }

        // Position hinzufügen
        private void btnPositionHinzu_Click(object sender, EventArgs e)
        {
            if (anzahlPositionen >= MAX_POSITIONEN)
            {
                MessageBox.Show("Maximale Anzahl Positionen (" + MAX_POSITIONEN + ") erreicht!");
                return;
            }

            // Dialog öffnen für Produktauswahl
            using (FrmProduktAuswahl dlg = new FrmProduktAuswahl())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    int produktId = dlg.AusgewaehltesProduktId;
                    string artikelNr = dlg.AusgewaehlteArtikelNr;
                    string bezeichnung = dlg.AusgewaehlteBezeichnung;
                    string einheit = dlg.AusgewaehlteEinheit;
                    decimal preis = dlg.AusgewaehlterPreis;

                    // Neue Zeile hinzufügen
                    int rowIndex = dgvPositionen.Rows.Add();
                    dgvPositionen.Rows[rowIndex].Cells["Id"].Value = 0; // Neue Position
                    dgvPositionen.Rows[rowIndex].Cells["ProduktId"].Value = produktId;
                    dgvPositionen.Rows[rowIndex].Cells["ArtikelNr"].Value = artikelNr;
                    dgvPositionen.Rows[rowIndex].Cells["Bezeichnung"].Value = bezeichnung;
                    dgvPositionen.Rows[rowIndex].Cells["Menge"].Value = 1;
                    dgvPositionen.Rows[rowIndex].Cells["Einheit"].Value = einheit;
                    dgvPositionen.Rows[rowIndex].Cells["Einzelpreis"].Value = preis;
                    dgvPositionen.Rows[rowIndex].Cells["Rabatt"].Value = 0;
                    dgvPositionen.Rows[rowIndex].Cells["Positionssumme"].Value = preis;

                    BerechneGesamtsumme();
                    hatAenderungen = true;

                    lblStatus.Text = "Position hinzugefügt: " + artikelNr;
                }
            }
        }

        // Position entfernen
        private void btnPositionEntfernen_Click(object sender, EventArgs e)
        {
            if (dgvPositionen.SelectedRows.Count > 0)
            {
                DialogResult res = MessageBox.Show("Position wirklich entfernen?", "Bestätigen",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (res == DialogResult.Yes)
                {
                    dgvPositionen.Rows.Remove(dgvPositionen.SelectedRows[0]);
                    BerechneGesamtsumme();
                    hatAenderungen = true;
                }
            }
            else
            {
                MessageBox.Show("Bitte wählen Sie eine Position aus!");
            }
        }

        // Position bearbeiten - wenn Menge oder Rabatt geändert
        private void dgvPositionen_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (_loadingData) return;
            if (e.RowIndex < 0) return;

            if (e.ColumnIndex == dgvPositionen.Columns["Menge"].Index ||
                e.ColumnIndex == dgvPositionen.Columns["Rabatt"].Index ||
                e.ColumnIndex == dgvPositionen.Columns["Einzelpreis"].Index)
            {
                DataGridViewRow row = dgvPositionen.Rows[e.RowIndex];

                decimal menge = 0;
                decimal einzelpreis = 0;
                decimal rabatt = 0;

                decimal.TryParse(row.Cells["Menge"].Value?.ToString(), out menge);
                decimal.TryParse(row.Cells["Einzelpreis"].Value?.ToString(), out einzelpreis);
                decimal.TryParse(row.Cells["Rabatt"].Value?.ToString(), out rabatt);

                // Rabatt validieren
                if (rabatt > MAX_RABATT)
                {
                    rabatt = MAX_RABATT;
                    row.Cells["Rabatt"].Value = rabatt;
                    MessageBox.Show("Maximaler Rabatt ist " + MAX_RABATT + "%!");
                }
                if (rabatt < 0) rabatt = 0;

                decimal positionssumme = menge * einzelpreis * (1 - rabatt / 100);
                row.Cells["Positionssumme"].Value = Math.Round(positionssumme, 2);

                BerechneGesamtsumme();
                hatAenderungen = true;
            }
        }

        // SPEICHERN - Monster-Methode
        private void btnSpeichern_Click(object sender, EventArgs e)
        {
            // Validierung
            if (cboKunde.SelectedIndex <= 0)
            {
                MessageBox.Show("Bitte wählen Sie einen Kunden aus!", "Validierung",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboKunde.Focus();
                return;
            }

            if (dgvPositionen.Rows.Count == 0)
            {
                MessageBox.Show("Bitte fügen Sie mindestens eine Position hinzu!", "Validierung",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            KundeItem kunde = cboKunde.SelectedItem as KundeItem;
            if (kunde == null)
            {
                MessageBox.Show("Ungültiger Kunde!", "Fehler");
                return;
            }

            try
            {
                string connStr = "Data Source=" + Globals.DbPath + ";Version=3;";
                SQLiteConnection conn = new SQLiteConnection(connStr);
                conn.Open();
                SQLiteTransaction trans = conn.BeginTransaction();

                try
                {
                    string sql = "";
                    decimal brutto = decimal.Parse(txtBrutto.Text);

                    if (istNeuanlage)
                    {
                        // INSERT Bestellung
                        sql = "INSERT INTO Bestellungen (BestellNr, KundeId, BestellDatum, Status, Bemerkung, Lieferadresse, GesamtSumme, ErstelltAm, ErstelltVon) VALUES (" +
                            "'" + txtBestellNr.Text + "', " +
                            kunde.Id.ToString() + ", " +
                            "'" + dtpBestellDatum.Value.ToString("yyyy-MM-dd") + "', " +
                            "'" + cboBestellStatus.SelectedItem.ToString() + "', " +
                            "'" + txtBemerkung.Text.Replace("'", "''") + "', " +
                            "'" + txtLieferadresse.Text.Replace("'", "''") + "', " +
                            brutto.ToString(System.Globalization.CultureInfo.InvariantCulture) + ", " +
                            "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', " +
                            "'" + Globals.CurrentUser + "')";

                        SQLiteCommand cmd = new SQLiteCommand(sql, conn, trans);
                        cmd.ExecuteNonQuery();

                        cmd = new SQLiteCommand("SELECT last_insert_rowid()", conn, trans);
                        aktuelleBestellungId = Convert.ToInt32(cmd.ExecuteScalar());

                        istNeuanlage = false;

                        // Audit Log
                        try
                        {
                            string auditSql = "INSERT INTO AuditLog (Tabelle, Aktion, DatensatzId, Benutzer, Zeitstempel, Details) VALUES (" +
                                "'Bestellungen', 'INSERT', " + aktuelleBestellungId + ", '" + Globals.CurrentUser + "', '" +
                                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', 'Neue Bestellung: " + txtBestellNr.Text + "')";
                            SQLiteCommand auditCmd = new SQLiteCommand(auditSql, conn, trans);
                            auditCmd.ExecuteNonQuery();
                        }
                        catch { }
                    }
                    else
                    {
                        // UPDATE Bestellung
                        sql = "UPDATE Bestellungen SET " +
                            "KundeId = " + kunde.Id.ToString() + ", " +
                            "BestellDatum = '" + dtpBestellDatum.Value.ToString("yyyy-MM-dd") + "', " +
                            "Status = '" + cboBestellStatus.SelectedItem.ToString() + "', " +
                            "Bemerkung = '" + txtBemerkung.Text.Replace("'", "''") + "', " +
                            "Lieferadresse = '" + txtLieferadresse.Text.Replace("'", "''") + "', " +
                            "GesamtSumme = " + brutto.ToString(System.Globalization.CultureInfo.InvariantCulture) + ", " +
                            "GeaendertAm = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', " +
                            "GeaendertVon = '" + Globals.CurrentUser + "' " +
                            "WHERE Id = " + aktuelleBestellungId.ToString();

                        SQLiteCommand cmd = new SQLiteCommand(sql, conn, trans);
                        cmd.ExecuteNonQuery();

                        // Alte Positionen löschen
                        sql = "DELETE FROM BestellPositionen WHERE BestellungId = " + aktuelleBestellungId.ToString();
                        cmd = new SQLiteCommand(sql, conn, trans);
                        cmd.ExecuteNonQuery();
                    }

                    // Positionen speichern
                    int posNr = 1;
                    foreach (DataGridViewRow row in dgvPositionen.Rows)
                    {
                        if (row.Cells["ProduktId"].Value == null) continue;

                        int produktId = Convert.ToInt32(row.Cells["ProduktId"].Value);
                        decimal menge = Convert.ToDecimal(row.Cells["Menge"].Value);
                        string einheit = row.Cells["Einheit"].Value?.ToString() ?? "";
                        decimal einzelpreis = Convert.ToDecimal(row.Cells["Einzelpreis"].Value);
                        decimal rabatt = Convert.ToDecimal(row.Cells["Rabatt"].Value ?? 0);
                        decimal posSum = Convert.ToDecimal(row.Cells["Positionssumme"].Value);

                        sql = "INSERT INTO BestellPositionen (BestellungId, Position, ProduktId, Menge, Einheit, Einzelpreis, Rabatt, Positionssumme) VALUES (" +
                            aktuelleBestellungId.ToString() + ", " +
                            posNr.ToString() + ", " +
                            produktId.ToString() + ", " +
                            menge.ToString(System.Globalization.CultureInfo.InvariantCulture) + ", " +
                            "'" + einheit + "', " +
                            einzelpreis.ToString(System.Globalization.CultureInfo.InvariantCulture) + ", " +
                            rabatt.ToString(System.Globalization.CultureInfo.InvariantCulture) + ", " +
                            posSum.ToString(System.Globalization.CultureInfo.InvariantCulture) + ")";

                        SQLiteCommand posCmd = new SQLiteCommand(sql, conn, trans);
                        posCmd.ExecuteNonQuery();
                        posNr++;
                    }

                    trans.Commit();
                    conn.Close();

                    hatAenderungen = false;
                    LadeBestellungenListe();

                    MessageBox.Show("Bestellung gespeichert!", "Erfolg",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    lblStatus.Text = "Gespeichert um " + DateTime.Now.ToString("HH:mm:ss");
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    conn.Close();
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Speichern: " + ex.Message, "Fehler",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // LÖSCHEN
        private void btnLoeschen_Click(object sender, EventArgs e)
        {
            if (aktuelleBestellungId == 0)
            {
                MessageBox.Show("Bitte wählen Sie eine Bestellung aus!");
                return;
            }

            // Stornierte Bestellungen können nicht gelöscht werden
            if (cboBestellStatus.SelectedItem?.ToString() == "Abgeschlossen")
            {
                MessageBox.Show("Abgeschlossene Bestellungen können nicht gelöscht werden.\nVerwenden Sie 'Stornieren'.");
                return;
            }

            DialogResult result = MessageBox.Show(
                "Möchten Sie die Bestellung \"" + txtBestellNr.Text + "\" wirklich löschen?\n\nAlle Positionen werden ebenfalls gelöscht!",
                "Löschen bestätigen",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (result == DialogResult.Yes)
            {
                try
                {
                    string connStr = "Data Source=" + Globals.DbPath + ";Version=3;";
                    SQLiteConnection conn = new SQLiteConnection(connStr);
                    conn.Open();

                    // Positionen löschen
                    string sql = "DELETE FROM BestellPositionen WHERE BestellungId = " + aktuelleBestellungId.ToString();
                    SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                    cmd.ExecuteNonQuery();

                    // Bestellung löschen
                    sql = "DELETE FROM Bestellungen WHERE Id = " + aktuelleBestellungId.ToString();
                    cmd = new SQLiteCommand(sql, conn);
                    cmd.ExecuteNonQuery();

                    // Audit Log
                    try
                    {
                        string auditSql = "INSERT INTO AuditLog (Tabelle, Aktion, DatensatzId, Benutzer, Zeitstempel, Details) VALUES (" +
                            "'Bestellungen', 'DELETE', " + aktuelleBestellungId + ", '" + Globals.CurrentUser + "', '" +
                            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', 'Bestellung gelöscht: " + txtBestellNr.Text + "')";
                        SQLiteCommand auditCmd = new SQLiteCommand(auditSql, conn);
                        auditCmd.ExecuteNonQuery();
                    }
                    catch { }

                    conn.Close();

                    MessageBox.Show("Bestellung gelöscht!", "Erfolg");

                    LeereAlleFelder();
                    LadeBestellungenListe();
                    lblStatus.Text = "Bestellung gelöscht";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fehler: " + ex.Message);
                }
            }
        }

        // Drucken
        private void btnDrucken_Click(object sender, EventArgs e)
        {
            if (aktuelleBestellungId == 0)
            {
                MessageBox.Show("Bitte wählen Sie eine Bestellung aus!");
                return;
            }

            // Druckvorschau öffnen
            FrmDruckvorschau dlg = new FrmDruckvorschau();
            dlg.DokumentTyp = "Bestellung";
            dlg.DokumentId = aktuelleBestellungId;
            dlg.ShowDialog();
        }

        private void btnSchliessen_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FrmBestellungen_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (hatAenderungen)
            {
                DialogResult result = MessageBox.Show(
                    "Es gibt ungespeicherte Änderungen. Trotzdem schließen?",
                    "Warnung",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }

            Globals.AktiveForm = "";

            if (this.MdiParent != null && this.MdiParent is MainForm)
            {
                ((MainForm)this.MdiParent).frmBestellungen = null;
            }
        }

        // Kunde geändert - Lieferadresse übernehmen
        private void cboKunde_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loadingData) return;
            if (cboKunde.SelectedIndex <= 0) return;

            KundeItem kunde = cboKunde.SelectedItem as KundeItem;
            if (kunde == null) return;

            aktuellerKundeId = kunde.Id;

            // Lieferadresse laden
            try
            {
                string connStr = "Data Source=" + Globals.DbPath + ";Version=3;";
                SQLiteConnection conn = new SQLiteConnection(connStr);
                conn.Open();

                string sql = "SELECT Strasse, PLZ, Ort, Land FROM Kunden WHERE Id = " + kunde.Id.ToString();
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                SQLiteDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    string adresse = reader["Strasse"].ToString();
                    adresse += "\n" + reader["PLZ"].ToString() + " " + reader["Ort"].ToString();
                    if (reader["Land"] != DBNull.Value && reader["Land"].ToString() != "")
                    {
                        adresse += "\n" + reader["Land"].ToString();
                    }
                    txtLieferadresse.Text = adresse;
                }

                reader.Close();
                conn.Close();
            }
            catch { }

            hatAenderungen = true;
        }

        // Hilfklasse für Kunden ComboBox
        private class KundeItem
        {
            public int Id { get; set; }
            public string Anzeige { get; set; }

            public KundeItem(int id, string anzeige)
            {
                Id = id;
                Anzeige = anzeige;
            }

            public override string ToString()
            {
                return Anzeige;
            }
        }
    }
}
