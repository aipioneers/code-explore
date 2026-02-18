using System;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;
using EnterpriseApp.Helpers;

namespace EnterpriseApp.Forms
{
    // Produktverwaltung - Legacy Form
    // WARNUNG: Absichtlich schlecht für Schulungszwecke!
    public partial class FrmProdukte : Form
    {
        // Anti-Pattern: Public fields statt Properties
        public int aktuelleProduktId = 0;
        public bool istNeuanlage = false;
        public bool hatAenderungen = false;
        public DataTable produkteTabelle = null;
        public string aktuelleKategorie = "";
        private int _magicCounter = 0;
        private bool _flag = false;

        public FrmProdukte()
        {
            InitializeComponent();
        }

        // Form Load - God Method
        private void FrmProdukte_Load(object sender, EventArgs e)
        {
            // Kategorien laden - hardcoded Fallback
            LadeKategorien();

            // Filter Kategorie befüllen - Copy-Paste
            cboKategorie.Items.Clear();
            cboKategorie.Items.Add("(Alle)");
            try
            {
                string connStr = "Data Source=" + Globals.DbPath + ";Version=3;";
                SQLiteConnection conn = new SQLiteConnection(connStr);
                conn.Open();
                string sql = "SELECT DISTINCT Kategorie FROM Produkte WHERE Kategorie IS NOT NULL AND Kategorie != '' ORDER BY Kategorie";
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    cboKategorie.Items.Add(reader["Kategorie"].ToString());
                }
                reader.Close();
                conn.Close();
            }
            catch { }
            cboKategorie.SelectedIndex = 0;

            // Produkte laden
            LadeProdukteListe();

            // Felder leeren
            LeereAlleFelder();

            lblStatus.Text = "Produktverwaltung geladen - " + DateTime.Now.ToString();
            Globals.AktiveForm = "Produkte";
        }

        // Kategorien laden für ComboBox - direkte SQL
        private void LadeKategorien()
        {
            cboProduktKategorie.Items.Clear();
            cboProduktKategorie.Items.Add("");

            // Hardcoded Kategorien als Fallback
            cboProduktKategorie.Items.Add("Elektronik");
            cboProduktKategorie.Items.Add("Bürobedarf");
            cboProduktKategorie.Items.Add("Möbel");
            cboProduktKategorie.Items.Add("Software");
            cboProduktKategorie.Items.Add("Hardware");
            cboProduktKategorie.Items.Add("Verbrauchsmaterial");
            cboProduktKategorie.Items.Add("Dienstleistungen");
            cboProduktKategorie.Items.Add("Sonstiges");

            cboProduktKategorie.SelectedIndex = 0;
        }

        // Produkte laden - direkte SQL, keine Abstraktion
        private void LadeProdukteListe()
        {
            try
            {
                string connStr = "Data Source=" + Globals.DbPath + ";Version=3;";
                SQLiteConnection conn = new SQLiteConnection(connStr);
                conn.Open();

                // SQL zusammenbauen - Magic Strings
                string sql = "SELECT Id, Artikelnummer, Produktname, Kategorie, Einheit, EKPreis, VKPreis, Lagerbestand, Mindestbestand, Aktiv FROM Produkte";

                // Filter hinzufügen
                string whereClause = "";

                if (chkNurAktive.Checked)
                {
                    whereClause = " WHERE Aktiv = 1";
                }

                if (cboKategorie.SelectedIndex > 0)
                {
                    string kat = cboKategorie.SelectedItem.ToString();
                    if (whereClause == "")
                        whereClause = " WHERE Kategorie = '" + kat + "'";
                    else
                        whereClause += " AND Kategorie = '" + kat + "'";
                }

                sql += whereClause + " ORDER BY Kategorie, Produktname";

                SQLiteDataAdapter adapter = new SQLiteDataAdapter(sql, conn);
                produkteTabelle = new DataTable();
                adapter.Fill(produkteTabelle);
                conn.Close();

                dgvProdukte.DataSource = null;
                dgvProdukte.DataSource = produkteTabelle;

                // Spalten konfigurieren - Magic Numbers
                if (dgvProdukte.Columns.Count > 0)
                {
                    dgvProdukte.Columns["Id"].Visible = false;
                    dgvProdukte.Columns["Artikelnummer"].HeaderText = "Art.-Nr.";
                    dgvProdukte.Columns["Artikelnummer"].Width = 80;
                    dgvProdukte.Columns["Produktname"].HeaderText = "Produkt";
                    dgvProdukte.Columns["Produktname"].Width = 150;
                    dgvProdukte.Columns["Kategorie"].HeaderText = "Kategorie";
                    dgvProdukte.Columns["Kategorie"].Width = 100;
                    dgvProdukte.Columns["Einheit"].HeaderText = "Einheit";
                    dgvProdukte.Columns["Einheit"].Width = 50;
                    dgvProdukte.Columns["EKPreis"].HeaderText = "EK";
                    dgvProdukte.Columns["EKPreis"].Width = 70;
                    dgvProdukte.Columns["EKPreis"].DefaultCellStyle.Format = "N2";
                    dgvProdukte.Columns["VKPreis"].HeaderText = "VK";
                    dgvProdukte.Columns["VKPreis"].Width = 70;
                    dgvProdukte.Columns["VKPreis"].DefaultCellStyle.Format = "N2";
                    dgvProdukte.Columns["Lagerbestand"].HeaderText = "Lager";
                    dgvProdukte.Columns["Lagerbestand"].Width = 60;
                    dgvProdukte.Columns["Mindestbestand"].Visible = false;
                    dgvProdukte.Columns["Aktiv"].Visible = false;
                }

                lblStatus.Text = produkteTabelle.Rows.Count.ToString() + " Produkte geladen";
            }
            catch (Exception ex)
            {
                // Fehler ignorieren - Anti-Pattern
                lblStatus.Text = "Fehler: " + ex.Message;
            }
        }

        // Suchen Button - SQL Injection möglich
        private void btnSuchen_Click(object sender, EventArgs e)
        {
            string suchtext = txtSuche.Text.Trim();

            try
            {
                string connStr = "Data Source=" + Globals.DbPath + ";Version=3;";
                SQLiteConnection conn = new SQLiteConnection(connStr);
                conn.Open();

                string sql = "SELECT Id, Artikelnummer, Produktname, Kategorie, Einheit, EKPreis, VKPreis, Lagerbestand, Mindestbestand, Aktiv FROM Produkte WHERE 1=1";

                if (suchtext != "")
                {
                    // SQL Injection anfällig!
                    sql += " AND (Artikelnummer LIKE '%" + suchtext + "%' OR Produktname LIKE '%" + suchtext + "%' OR Beschreibung LIKE '%" + suchtext + "%')";
                }

                if (chkNurAktive.Checked)
                {
                    sql += " AND Aktiv = 1";
                }

                if (cboKategorie.SelectedIndex > 0)
                {
                    sql += " AND Kategorie = '" + cboKategorie.SelectedItem.ToString() + "'";
                }

                sql += " ORDER BY Kategorie, Produktname";

                SQLiteDataAdapter adapter = new SQLiteDataAdapter(sql, conn);
                produkteTabelle = new DataTable();
                adapter.Fill(produkteTabelle);
                conn.Close();

                dgvProdukte.DataSource = null;
                dgvProdukte.DataSource = produkteTabelle;

                // Copy-Paste Spalten-Konfiguration von oben
                if (dgvProdukte.Columns.Count > 0)
                {
                    dgvProdukte.Columns["Id"].Visible = false;
                    dgvProdukte.Columns["Artikelnummer"].HeaderText = "Art.-Nr.";
                    dgvProdukte.Columns["Artikelnummer"].Width = 80;
                    dgvProdukte.Columns["Produktname"].HeaderText = "Produkt";
                    dgvProdukte.Columns["Produktname"].Width = 150;
                    dgvProdukte.Columns["Kategorie"].HeaderText = "Kategorie";
                    dgvProdukte.Columns["Kategorie"].Width = 100;
                    dgvProdukte.Columns["Einheit"].HeaderText = "Einheit";
                    dgvProdukte.Columns["Einheit"].Width = 50;
                    dgvProdukte.Columns["EKPreis"].HeaderText = "EK";
                    dgvProdukte.Columns["EKPreis"].Width = 70;
                    dgvProdukte.Columns["EKPreis"].DefaultCellStyle.Format = "N2";
                    dgvProdukte.Columns["VKPreis"].HeaderText = "VK";
                    dgvProdukte.Columns["VKPreis"].Width = 70;
                    dgvProdukte.Columns["VKPreis"].DefaultCellStyle.Format = "N2";
                    dgvProdukte.Columns["Lagerbestand"].HeaderText = "Lager";
                    dgvProdukte.Columns["Lagerbestand"].Width = 60;
                    dgvProdukte.Columns["Mindestbestand"].Visible = false;
                    dgvProdukte.Columns["Aktiv"].Visible = false;
                }

                lblStatus.Text = produkteTabelle.Rows.Count.ToString() + " Produkte gefunden";
            }
            catch
            {
                // Leerer Catch
            }
        }

        private void txtSuche_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13) // Magic number
            {
                btnSuchen_Click(sender, e);
            }
        }

        private void cboKategorie_SelectedIndexChanged(object sender, EventArgs e)
        {
            LadeProdukteListe();
        }

        private void chkNurAktive_CheckedChanged(object sender, EventArgs e)
        {
            LadeProdukteListe();
        }

        // Selection Changed - lädt Produktdaten
        private void dgvProdukte_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvProdukte.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dgvProdukte.SelectedRows[0];
                if (row.Cells["Id"].Value != null && row.Cells["Id"].Value != DBNull.Value)
                {
                    int id = Convert.ToInt32(row.Cells["Id"].Value);
                    LadeProduktById(id);
                }
            }
        }

        // Produkt laden - lange Methode
        private void LadeProduktById(int produktId)
        {
            try
            {
                string connStr = "Data Source=" + Globals.DbPath + ";Version=3;";
                SQLiteConnection conn = new SQLiteConnection(connStr);
                conn.Open();

                string sql = "SELECT * FROM Produkte WHERE Id = " + produktId.ToString();
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                SQLiteDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    aktuelleProduktId = produktId;
                    istNeuanlage = false;

                    txtArtikelnummer.Text = reader["Artikelnummer"].ToString();
                    txtProduktName.Text = reader["Produktname"].ToString();

                    // Kategorie setzen - umständlich
                    string kat = reader["Kategorie"].ToString();
                    int katIndex = cboProduktKategorie.Items.IndexOf(kat);
                    if (katIndex >= 0) cboProduktKategorie.SelectedIndex = katIndex;
                    else cboProduktKategorie.SelectedIndex = 0;

                    txtEinheit.Text = reader["Einheit"].ToString();

                    if (reader["EKPreis"] != DBNull.Value)
                        numEKPreis.Value = Convert.ToDecimal(reader["EKPreis"]);
                    else
                        numEKPreis.Value = 0;

                    if (reader["VKPreis"] != DBNull.Value)
                        numVKPreis.Value = Convert.ToDecimal(reader["VKPreis"]);
                    else
                        numVKPreis.Value = 0;

                    if (reader["Lagerbestand"] != DBNull.Value)
                        numLagerbestand.Value = Convert.ToDecimal(reader["Lagerbestand"]);
                    else
                        numLagerbestand.Value = 0;

                    if (reader["Mindestbestand"] != DBNull.Value)
                        numMindestbestand.Value = Convert.ToDecimal(reader["Mindestbestand"]);
                    else
                        numMindestbestand.Value = 0;

                    chkAktiv.Checked = reader["Aktiv"] != DBNull.Value && Convert.ToInt32(reader["Aktiv"]) == 1;
                    txtBeschreibung.Text = reader["Beschreibung"].ToString();

                    hatAenderungen = false;
                    lblStatus.Text = "Produkt geladen: " + txtArtikelnummer.Text;
                }

                reader.Close();
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler: " + ex.Message);
            }
        }

        // Neu Button
        private void btnNeu_Click(object sender, EventArgs e)
        {
            LeereAlleFelder();

            // Artikelnummer generieren - direkt hier
            string neueArtNr = GeneriereArtikelnummer();
            txtArtikelnummer.Text = neueArtNr;

            istNeuanlage = true;
            aktuelleProduktId = 0;
            chkAktiv.Checked = true;

            txtProduktName.Focus();
            lblStatus.Text = "Neues Produkt anlegen...";
        }

        // Artikelnummer generieren - Copy-Paste Logik
        private string GeneriereArtikelnummer()
        {
            string neueNr = "";

            try
            {
                string connStr = "Data Source=" + Globals.DbPath + ";Version=3;";
                SQLiteConnection conn = new SQLiteConnection(connStr);
                conn.Open();

                string sql = "SELECT MAX(CAST(SUBSTR(Artikelnummer, 2) AS INTEGER)) FROM Produkte WHERE Artikelnummer LIKE 'P%'";
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                object result = cmd.ExecuteScalar();

                int letzteNr = 0;
                if (result != null && result != DBNull.Value)
                {
                    letzteNr = Convert.ToInt32(result);
                }

                neueNr = "P" + (letzteNr + 1).ToString().PadLeft(5, '0');
                conn.Close();
            }
            catch
            {
                neueNr = "P" + DateTime.Now.Ticks.ToString().Substring(10, 5);
            }

            return neueNr;
        }

        // Felder leeren
        private void LeereAlleFelder()
        {
            txtArtikelnummer.Text = "";
            txtProduktName.Text = "";
            cboProduktKategorie.SelectedIndex = 0;
            txtEinheit.Text = "Stk";
            numEKPreis.Value = 0;
            numVKPreis.Value = 0;
            numLagerbestand.Value = 0;
            numMindestbestand.Value = 0;
            chkAktiv.Checked = true;
            txtBeschreibung.Text = "";

            aktuelleProduktId = 0;
            hatAenderungen = false;
        }

        // SPEICHERN - Monster-Methode!
        private void btnSpeichern_Click(object sender, EventArgs e)
        {
            // Validierung inline
            if (txtProduktName.Text.Trim() == "")
            {
                MessageBox.Show("Bitte geben Sie einen Produktnamen ein!",
                    "Validierung", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtProduktName.Focus();
                return;
            }

            if (txtProduktName.Text.Trim().Length < 3)
            {
                MessageBox.Show("Der Produktname muss mindestens 3 Zeichen haben!",
                    "Validierung", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtProduktName.Focus();
                return;
            }

            // Preis Validierung - Magic Numbers
            if (numVKPreis.Value < numEKPreis.Value)
            {
                DialogResult res = MessageBox.Show(
                    "Der VK-Preis ist niedriger als der EK-Preis!\n\nTrotzdem speichern?",
                    "Warnung", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (res == DialogResult.No)
                {
                    numVKPreis.Focus();
                    return;
                }
            }

            // Mindestbestand Warnung
            if (numLagerbestand.Value < numMindestbestand.Value)
            {
                MessageBox.Show("Hinweis: Der Lagerbestand liegt unter dem Mindestbestand!",
                    "Hinweis", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            try
            {
                string connStr = "Data Source=" + Globals.DbPath + ";Version=3;";
                SQLiteConnection conn = new SQLiteConnection(connStr);
                conn.Open();

                string sql = "";

                if (istNeuanlage)
                {
                    // INSERT - direkt zusammengebaut
                    sql = "INSERT INTO Produkte (Artikelnummer, Produktname, Kategorie, Einheit, EKPreis, VKPreis, Lagerbestand, Mindestbestand, Aktiv, Beschreibung, ErstelltAm, ErstelltVon) VALUES (" +
                        "'" + txtArtikelnummer.Text.Replace("'", "''") + "', " +
                        "'" + txtProduktName.Text.Replace("'", "''") + "', " +
                        "'" + (cboProduktKategorie.SelectedItem != null ? cboProduktKategorie.SelectedItem.ToString().Replace("'", "''") : "") + "', " +
                        "'" + txtEinheit.Text.Replace("'", "''") + "', " +
                        numEKPreis.Value.ToString(System.Globalization.CultureInfo.InvariantCulture) + ", " +
                        numVKPreis.Value.ToString(System.Globalization.CultureInfo.InvariantCulture) + ", " +
                        numLagerbestand.Value.ToString() + ", " +
                        numMindestbestand.Value.ToString() + ", " +
                        (chkAktiv.Checked ? "1" : "0") + ", " +
                        "'" + txtBeschreibung.Text.Replace("'", "''") + "', " +
                        "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', " +
                        "'" + Globals.CurrentUser + "')";

                    SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                    cmd.ExecuteNonQuery();

                    cmd = new SQLiteCommand("SELECT last_insert_rowid()", conn);
                    aktuelleProduktId = Convert.ToInt32(cmd.ExecuteScalar());

                    istNeuanlage = false;

                    // Audit Log - Copy-Paste
                    try
                    {
                        string auditSql = "INSERT INTO AuditLog (Tabelle, Aktion, DatensatzId, Benutzer, Zeitstempel, Details) VALUES (" +
                            "'Produkte', 'INSERT', " + aktuelleProduktId.ToString() + ", '" + Globals.CurrentUser + "', '" +
                            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', 'Neues Produkt: " + txtArtikelnummer.Text + "')";
                        SQLiteCommand auditCmd = new SQLiteCommand(auditSql, conn);
                        auditCmd.ExecuteNonQuery();
                    }
                    catch { }

                    MessageBox.Show("Produkt angelegt!", "Erfolg",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // UPDATE - ebenfalls direkt
                    sql = "UPDATE Produkte SET " +
                        "Produktname = '" + txtProduktName.Text.Replace("'", "''") + "', " +
                        "Kategorie = '" + (cboProduktKategorie.SelectedItem != null ? cboProduktKategorie.SelectedItem.ToString().Replace("'", "''") : "") + "', " +
                        "Einheit = '" + txtEinheit.Text.Replace("'", "''") + "', " +
                        "EKPreis = " + numEKPreis.Value.ToString(System.Globalization.CultureInfo.InvariantCulture) + ", " +
                        "VKPreis = " + numVKPreis.Value.ToString(System.Globalization.CultureInfo.InvariantCulture) + ", " +
                        "Lagerbestand = " + numLagerbestand.Value.ToString() + ", " +
                        "Mindestbestand = " + numMindestbestand.Value.ToString() + ", " +
                        "Aktiv = " + (chkAktiv.Checked ? "1" : "0") + ", " +
                        "Beschreibung = '" + txtBeschreibung.Text.Replace("'", "''") + "', " +
                        "GeaendertAm = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', " +
                        "GeaendertVon = '" + Globals.CurrentUser + "' " +
                        "WHERE Id = " + aktuelleProduktId.ToString();

                    SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                    cmd.ExecuteNonQuery();

                    // Audit Log - Copy-Paste von oben
                    try
                    {
                        string auditSql = "INSERT INTO AuditLog (Tabelle, Aktion, DatensatzId, Benutzer, Zeitstempel, Details) VALUES (" +
                            "'Produkte', 'UPDATE', " + aktuelleProduktId.ToString() + ", '" + Globals.CurrentUser + "', '" +
                            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', 'Produkt aktualisiert: " + txtArtikelnummer.Text + "')";
                        SQLiteCommand auditCmd = new SQLiteCommand(auditSql, conn);
                        auditCmd.ExecuteNonQuery();
                    }
                    catch { }

                    MessageBox.Show("Änderungen gespeichert!", "Erfolg",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                conn.Close();
                hatAenderungen = false;
                LadeProdukteListe();
                lblStatus.Text = "Gespeichert um " + DateTime.Now.ToString("HH:mm:ss");

                // MainForm aktualisieren - Feature Envy
                if (this.MdiParent != null && this.MdiParent is MainForm)
                {
                    ((MainForm)this.MdiParent).AktualisiereStatistik();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Speichern: " + ex.Message,
                    "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // LÖSCHEN
        private void btnLoeschen_Click(object sender, EventArgs e)
        {
            if (aktuelleProduktId == 0)
            {
                MessageBox.Show("Bitte wählen Sie ein Produkt aus!",
                    "Hinweis", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Prüfen ob Produkt in Bestellungen verwendet wird
            try
            {
                string connStr = "Data Source=" + Globals.DbPath + ";Version=3;";
                SQLiteConnection conn = new SQLiteConnection(connStr);
                conn.Open();

                string sql = "SELECT COUNT(*) FROM BestellPositionen WHERE ProduktId = " + aktuelleProduktId.ToString();
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                int anzahl = Convert.ToInt32(cmd.ExecuteScalar());

                if (anzahl > 0)
                {
                    MessageBox.Show("Das Produkt ist in " + anzahl.ToString() + " Bestellposition(en) verwendet und kann nicht gelöscht werden!\n\n" +
                        "Setzen Sie das Produkt auf 'Inaktiv' statt es zu löschen.",
                        "Löschen nicht möglich", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    conn.Close();
                    return;
                }

                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler bei der Prüfung: " + ex.Message);
                return;
            }

            DialogResult result = MessageBox.Show(
                "Möchten Sie das Produkt \"" + txtArtikelnummer.Text + " - " + txtProduktName.Text + "\" wirklich löschen?",
                "Löschen bestätigen",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2);

            if (result == DialogResult.Yes)
            {
                try
                {
                    string connStr = "Data Source=" + Globals.DbPath + ";Version=3;";
                    SQLiteConnection conn = new SQLiteConnection(connStr);
                    conn.Open();

                    string sql = "DELETE FROM Produkte WHERE Id = " + aktuelleProduktId.ToString();
                    SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                    cmd.ExecuteNonQuery();

                    // Audit Log - Copy-Paste
                    try
                    {
                        string auditSql = "INSERT INTO AuditLog (Tabelle, Aktion, DatensatzId, Benutzer, Zeitstempel, Details) VALUES (" +
                            "'Produkte', 'DELETE', " + aktuelleProduktId.ToString() + ", '" + Globals.CurrentUser + "', '" +
                            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', 'Produkt gelöscht: " + txtArtikelnummer.Text + "')";
                        SQLiteCommand auditCmd = new SQLiteCommand(auditSql, conn);
                        auditCmd.ExecuteNonQuery();
                    }
                    catch { }

                    conn.Close();

                    MessageBox.Show("Produkt gelöscht!", "Erfolg",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LeereAlleFelder();
                    LadeProdukteListe();
                    lblStatus.Text = "Produkt gelöscht";

                    // MainForm aktualisieren - Feature Envy
                    if (this.MdiParent != null && this.MdiParent is MainForm)
                    {
                        ((MainForm)this.MdiParent).AktualisiereStatistik();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fehler beim Löschen: " + ex.Message,
                        "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnSchliessen_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FrmProdukte_FormClosing(object sender, FormClosingEventArgs e)
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

            // Reference aus MainForm entfernen
            if (this.MdiParent != null && this.MdiParent is MainForm)
            {
                ((MainForm)this.MdiParent).frmProdukte = null;
            }
        }

        // Hilfsmethode - Feature Envy, gehört woanders hin
        public decimal BerechneGewinnspanne(decimal ekPreis, decimal vkPreis)
        {
            if (ekPreis == 0) return 0;
            return ((vkPreis - ekPreis) / ekPreis) * 100;
        }

        // Öffentliche Methode für externe Aufrufe
        public void LadeProduktExtern(int produktId)
        {
            LadeProduktById(produktId);
        }

        // Duplizierte Logik
        public string FormatPreis(decimal preis)
        {
            return preis.ToString("N2") + " €";
        }

        // Noch eine duplizierte Methode
        private string formatPreis(decimal p)
        {
            return p.ToString("0.00") + " EUR";
        }
    }
}
