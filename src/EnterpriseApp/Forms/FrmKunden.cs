using System;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;
using EnterpriseApp.Helpers;

namespace EnterpriseApp.Forms
{
    public partial class FrmKunden : Form
    {
        // Anti-Pattern: Primitive Obsession - using primitives instead of proper types
        private int aktuelleKundenId = 0;
        private bool isNeu = false;
        private bool isDirty = false;
        private string letzteSuche = "";
        private int anzahlKunden = 0;
        private DataTable dtKunden;

        // Anti-Pattern: Magic numbers as class fields
        private int maxKundenProSeite = 100;
        private int zeitoutInSekunden = 30;

        public FrmKunden()
        {
            InitializeComponent();
        }

        // Anti-Pattern: God Method - Form_Load does way too much
        private void FrmKunden_Load(object sender, EventArgs e)
        {
            // Anti-Pattern: Direct UI manipulation mixed with data loading
            this.Cursor = Cursors.WaitCursor;
            lblStatus.Text = "Lade Kundendaten...";
            lblStatus.ForeColor = Color.Blue;

            try
            {
                // Anti-Pattern: Hardcoded values in ComboBox
                cboAnrede.Items.Clear();
                cboAnrede.Items.Add("");
                cboAnrede.Items.Add("Herr");
                cboAnrede.Items.Add("Frau");
                cboAnrede.Items.Add("Firma");
                cboAnrede.Items.Add("Dr.");
                cboAnrede.Items.Add("Prof.");
                cboAnrede.Items.Add("Prof. Dr.");
                cboAnrede.SelectedIndex = 0;

                // Anti-Pattern: More hardcoded values
                cboLand.Items.Clear();
                cboLand.Items.Add("");
                cboLand.Items.Add("Deutschland");
                cboLand.Items.Add("Österreich");
                cboLand.Items.Add("Schweiz");
                cboLand.Items.Add("Frankreich");
                cboLand.Items.Add("Italien");
                cboLand.Items.Add("Niederlande");
                cboLand.Items.Add("Belgien");
                cboLand.Items.Add("Polen");
                cboLand.Items.Add("Tschechien");
                cboLand.Items.Add("Dänemark");
                cboLand.Items.Add("Schweden");
                cboLand.Items.Add("Norwegen");
                cboLand.Items.Add("Finnland");
                cboLand.Items.Add("Spanien");
                cboLand.Items.Add("Portugal");
                cboLand.Items.Add("Großbritannien");
                cboLand.Items.Add("Irland");
                cboLand.Items.Add("Luxemburg");
                cboLand.Items.Add("USA");
                cboLand.Items.Add("Kanada");
                cboLand.SelectedIndex = 0;

                // Anti-Pattern: Direct SQL in Form
                string connStr = "Data Source=" + Globals.DbPath + ";Version=3;";
                using (SQLiteConnection conn = new SQLiteConnection(connStr))
                {
                    conn.Open();
                    string sql = "SELECT Id, KundenNr, Firma, Anrede, Vorname, Nachname, Strasse, PLZ, Ort, Land, Telefon, Email FROM Kunden ORDER BY Nachname, Vorname";
                    SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                    SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
                    dtKunden = new DataTable();
                    adapter.Fill(dtKunden);

                    dgvKunden.DataSource = dtKunden;

                    // Anti-Pattern: Magic strings for column configuration
                    if (dgvKunden.Columns.Contains("Id"))
                        dgvKunden.Columns["Id"].Visible = false;
                    if (dgvKunden.Columns.Contains("KundenNr"))
                    {
                        dgvKunden.Columns["KundenNr"].HeaderText = "Kunden-Nr";
                        dgvKunden.Columns["KundenNr"].Width = 80;
                    }
                    if (dgvKunden.Columns.Contains("Firma"))
                    {
                        dgvKunden.Columns["Firma"].HeaderText = "Firma";
                        dgvKunden.Columns["Firma"].Width = 120;
                    }
                    if (dgvKunden.Columns.Contains("Anrede"))
                    {
                        dgvKunden.Columns["Anrede"].Visible = false;
                    }
                    if (dgvKunden.Columns.Contains("Vorname"))
                    {
                        dgvKunden.Columns["Vorname"].HeaderText = "Vorname";
                        dgvKunden.Columns["Vorname"].Width = 100;
                    }
                    if (dgvKunden.Columns.Contains("Nachname"))
                    {
                        dgvKunden.Columns["Nachname"].HeaderText = "Nachname";
                        dgvKunden.Columns["Nachname"].Width = 100;
                    }
                    if (dgvKunden.Columns.Contains("Strasse"))
                    {
                        dgvKunden.Columns["Strasse"].Visible = false;
                    }
                    if (dgvKunden.Columns.Contains("PLZ"))
                    {
                        dgvKunden.Columns["PLZ"].Visible = false;
                    }
                    if (dgvKunden.Columns.Contains("Ort"))
                    {
                        dgvKunden.Columns["Ort"].HeaderText = "Ort";
                        dgvKunden.Columns["Ort"].Width = 100;
                    }
                    if (dgvKunden.Columns.Contains("Land"))
                    {
                        dgvKunden.Columns["Land"].Visible = false;
                    }
                    if (dgvKunden.Columns.Contains("Telefon"))
                    {
                        dgvKunden.Columns["Telefon"].HeaderText = "Telefon";
                        dgvKunden.Columns["Telefon"].Width = 100;
                    }
                    if (dgvKunden.Columns.Contains("Email"))
                    {
                        dgvKunden.Columns["Email"].HeaderText = "E-Mail";
                        dgvKunden.Columns["Email"].Width = 150;
                    }

                    anzahlKunden = dtKunden.Rows.Count;
                    conn.Close();
                }

                // Anti-Pattern: Update global state
                Globals.AnzahlKunden = anzahlKunden;

                ClearForm();

                lblStatus.Text = "Bereit - " + anzahlKunden.ToString() + " Kunden geladen";
                lblStatus.ForeColor = Color.Black;
            }
            catch (Exception ex)
            {
                // Anti-Pattern: Poor error handling
                lblStatus.Text = "Fehler: " + ex.Message;
                lblStatus.ForeColor = Color.Red;
                MessageBox.Show("Fehler beim Laden der Kundendaten:\n" + ex.Message, "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        // Anti-Pattern: Long method with too many responsibilities
        private void btnSpeichern_Click(object sender, EventArgs e)
        {
            // Anti-Pattern: Inline validation with magic numbers
            if (txtNachname.Text.Trim().Length < 2)
            {
                MessageBox.Show("Bitte geben Sie einen Nachnamen ein (mindestens 2 Zeichen).", "Validierung", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNachname.Focus();
                return;
            }

            if (txtVorname.Text.Trim().Length < 2)
            {
                MessageBox.Show("Bitte geben Sie einen Vornamen ein (mindestens 2 Zeichen).", "Validierung", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtVorname.Focus();
                return;
            }

            // Anti-Pattern: Copy-paste email validation
            if (txtEmail.Text.Trim().Length > 0)
            {
                if (!txtEmail.Text.Contains("@"))
                {
                    MessageBox.Show("Bitte geben Sie eine gültige E-Mail-Adresse ein.", "Validierung", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtEmail.Focus();
                    return;
                }
                if (!txtEmail.Text.Contains("."))
                {
                    MessageBox.Show("Bitte geben Sie eine gültige E-Mail-Adresse ein.", "Validierung", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtEmail.Focus();
                    return;
                }
                if (txtEmail.Text.IndexOf("@") > txtEmail.Text.LastIndexOf("."))
                {
                    // do nothing, valid
                }
                else
                {
                    if (txtEmail.Text.LastIndexOf(".") - txtEmail.Text.IndexOf("@") < 2)
                    {
                        MessageBox.Show("Bitte geben Sie eine gültige E-Mail-Adresse ein.", "Validierung", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtEmail.Focus();
                        return;
                    }
                }
            }

            // Anti-Pattern: PLZ validation with deep nesting
            if (txtPLZ.Text.Trim().Length > 0)
            {
                if (cboLand.SelectedItem != null)
                {
                    if (cboLand.SelectedItem.ToString() == "Deutschland")
                    {
                        if (txtPLZ.Text.Trim().Length != 5)
                        {
                            MessageBox.Show("Deutsche PLZ muss 5 Stellen haben.", "Validierung", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txtPLZ.Focus();
                            return;
                        }
                        else
                        {
                            foreach (char c in txtPLZ.Text.Trim())
                            {
                                if (!char.IsDigit(c))
                                {
                                    MessageBox.Show("Deutsche PLZ darf nur Ziffern enthalten.", "Validierung", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    txtPLZ.Focus();
                                    return;
                                }
                            }
                        }
                    }
                    else if (cboLand.SelectedItem.ToString() == "Österreich")
                    {
                        if (txtPLZ.Text.Trim().Length != 4)
                        {
                            MessageBox.Show("Österreichische PLZ muss 4 Stellen haben.", "Validierung", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txtPLZ.Focus();
                            return;
                        }
                        else
                        {
                            foreach (char c in txtPLZ.Text.Trim())
                            {
                                if (!char.IsDigit(c))
                                {
                                    MessageBox.Show("Österreichische PLZ darf nur Ziffern enthalten.", "Validierung", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    txtPLZ.Focus();
                                    return;
                                }
                            }
                        }
                    }
                    else if (cboLand.SelectedItem.ToString() == "Schweiz")
                    {
                        if (txtPLZ.Text.Trim().Length != 4)
                        {
                            MessageBox.Show("Schweizer PLZ muss 4 Stellen haben.", "Validierung", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txtPLZ.Focus();
                            return;
                        }
                        else
                        {
                            foreach (char c in txtPLZ.Text.Trim())
                            {
                                if (!char.IsDigit(c))
                                {
                                    MessageBox.Show("Schweizer PLZ darf nur Ziffern enthalten.", "Validierung", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    txtPLZ.Focus();
                                    return;
                                }
                            }
                        }
                    }
                }
            }

            // Anti-Pattern: Telefon validation copy-pasted
            if (txtTelefon.Text.Trim().Length > 0)
            {
                string tel = txtTelefon.Text.Trim();
                bool valid = true;
                foreach (char c in tel)
                {
                    if (!char.IsDigit(c) && c != '+' && c != '-' && c != ' ' && c != '/' && c != '(' && c != ')')
                    {
                        valid = false;
                        break;
                    }
                }
                if (!valid)
                {
                    MessageBox.Show("Telefonnummer enthält ungültige Zeichen.", "Validierung", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtTelefon.Focus();
                    return;
                }
            }

            this.Cursor = Cursors.WaitCursor;
            lblStatus.Text = "Speichere...";
            lblStatus.ForeColor = Color.Blue;

            try
            {
                string connStr = "Data Source=" + Globals.DbPath + ";Version=3;";
                using (SQLiteConnection conn = new SQLiteConnection(connStr))
                {
                    conn.Open();

                    if (isNeu)
                    {
                        // Anti-Pattern: Generate customer number inline
                        string neueKundenNr = "";
                        SQLiteCommand cmdMax = new SQLiteCommand("SELECT MAX(CAST(SUBSTR(KundenNr, 2) AS INTEGER)) FROM Kunden WHERE KundenNr LIKE 'K%'", conn);
                        object result = cmdMax.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            int maxNr = Convert.ToInt32(result);
                            neueKundenNr = "K" + (maxNr + 1).ToString("D5");
                        }
                        else
                        {
                            neueKundenNr = "K00001";
                        }

                        // Anti-Pattern: SQL string concatenation (SQL Injection risk for demo)
                        string sql = "INSERT INTO Kunden (KundenNr, Firma, Anrede, Vorname, Nachname, Strasse, PLZ, Ort, Land, Telefon, Email, Bemerkung, ErstelltAm, ErstelltVon) VALUES (";
                        sql += "'" + neueKundenNr + "', ";
                        sql += "'" + txtFirma.Text.Replace("'", "''") + "', ";
                        sql += "'" + (cboAnrede.SelectedItem != null ? cboAnrede.SelectedItem.ToString().Replace("'", "''") : "") + "', ";
                        sql += "'" + txtVorname.Text.Replace("'", "''") + "', ";
                        sql += "'" + txtNachname.Text.Replace("'", "''") + "', ";
                        sql += "'" + txtStrasse.Text.Replace("'", "''") + "', ";
                        sql += "'" + txtPLZ.Text.Replace("'", "''") + "', ";
                        sql += "'" + txtOrt.Text.Replace("'", "''") + "', ";
                        sql += "'" + (cboLand.SelectedItem != null ? cboLand.SelectedItem.ToString().Replace("'", "''") : "") + "', ";
                        sql += "'" + txtTelefon.Text.Replace("'", "''") + "', ";
                        sql += "'" + txtEmail.Text.Replace("'", "''") + "', ";
                        sql += "'" + txtBemerkung.Text.Replace("'", "''") + "', ";
                        sql += "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', ";
                        sql += "'" + Globals.CurrentUser + "')";

                        SQLiteCommand cmdInsert = new SQLiteCommand(sql, conn);
                        cmdInsert.ExecuteNonQuery();

                        // Anti-Pattern: Get last ID with separate query
                        SQLiteCommand cmdId = new SQLiteCommand("SELECT last_insert_rowid()", conn);
                        aktuelleKundenId = Convert.ToInt32(cmdId.ExecuteScalar());

                        txtKundenNr.Text = neueKundenNr;
                        isNeu = false;

                        // Anti-Pattern: Logging mixed with business logic
                        string logSql = "INSERT INTO AuditLog (Tabelle, AktionTyp, DatensatzId, Beschreibung, BenutzerName, Zeitstempel) VALUES ('Kunden', 'INSERT', " + aktuelleKundenId + ", 'Neuer Kunde angelegt: " + neueKundenNr + "', '" + Globals.CurrentUser + "', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                        try
                        {
                            SQLiteCommand cmdLog = new SQLiteCommand(logSql, conn);
                            cmdLog.ExecuteNonQuery();
                        }
                        catch
                        {
                            // Anti-Pattern: Empty catch - swallow logging errors
                        }

                        anzahlKunden++;
                        Globals.AnzahlKunden = anzahlKunden;

                        lblStatus.Text = "Kunde " + neueKundenNr + " wurde angelegt";
                        lblStatus.ForeColor = Color.Green;
                    }
                    else
                    {
                        // Anti-Pattern: Update with string concatenation (copy-paste from insert)
                        string sql = "UPDATE Kunden SET ";
                        sql += "Firma = '" + txtFirma.Text.Replace("'", "''") + "', ";
                        sql += "Anrede = '" + (cboAnrede.SelectedItem != null ? cboAnrede.SelectedItem.ToString().Replace("'", "''") : "") + "', ";
                        sql += "Vorname = '" + txtVorname.Text.Replace("'", "''") + "', ";
                        sql += "Nachname = '" + txtNachname.Text.Replace("'", "''") + "', ";
                        sql += "Strasse = '" + txtStrasse.Text.Replace("'", "''") + "', ";
                        sql += "PLZ = '" + txtPLZ.Text.Replace("'", "''") + "', ";
                        sql += "Ort = '" + txtOrt.Text.Replace("'", "''") + "', ";
                        sql += "Land = '" + (cboLand.SelectedItem != null ? cboLand.SelectedItem.ToString().Replace("'", "''") : "") + "', ";
                        sql += "Telefon = '" + txtTelefon.Text.Replace("'", "''") + "', ";
                        sql += "Email = '" + txtEmail.Text.Replace("'", "''") + "', ";
                        sql += "Bemerkung = '" + txtBemerkung.Text.Replace("'", "''") + "', ";
                        sql += "GeaendertAm = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', ";
                        sql += "GeaendertVon = '" + Globals.CurrentUser + "' ";
                        sql += "WHERE Id = " + aktuelleKundenId;

                        SQLiteCommand cmdUpdate = new SQLiteCommand(sql, conn);
                        cmdUpdate.ExecuteNonQuery();

                        // Anti-Pattern: Copy-paste logging
                        string logSql = "INSERT INTO AuditLog (Tabelle, AktionTyp, DatensatzId, Beschreibung, BenutzerName, Zeitstempel) VALUES ('Kunden', 'UPDATE', " + aktuelleKundenId + ", 'Kunde geändert: " + txtKundenNr.Text + "', '" + Globals.CurrentUser + "', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                        try
                        {
                            SQLiteCommand cmdLog = new SQLiteCommand(logSql, conn);
                            cmdLog.ExecuteNonQuery();
                        }
                        catch
                        {
                            // Anti-Pattern: Empty catch
                        }

                        lblStatus.Text = "Änderungen gespeichert für " + txtKundenNr.Text;
                        lblStatus.ForeColor = Color.Green;
                    }

                    conn.Close();
                }

                isDirty = false;

                // Anti-Pattern: Reload entire grid after save
                ReloadGrid();
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Fehler beim Speichern: " + ex.Message;
                lblStatus.ForeColor = Color.Red;
                MessageBox.Show("Fehler beim Speichern:\n" + ex.Message, "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        // Anti-Pattern: Method does too much, mixed concerns
        private void btnLoeschen_Click(object sender, EventArgs e)
        {
            if (aktuelleKundenId == 0)
            {
                MessageBox.Show("Bitte wählen Sie einen Kunden aus.", "Hinweis", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Anti-Pattern: Check for orders inline
            try
            {
                string connStr = "Data Source=" + Globals.DbPath + ";Version=3;";
                using (SQLiteConnection conn = new SQLiteConnection(connStr))
                {
                    conn.Open();

                    // Anti-Pattern: Magic string SQL
                    string sqlCheck = "SELECT COUNT(*) FROM Bestellungen WHERE KundeId = " + aktuelleKundenId;
                    SQLiteCommand cmdCheck = new SQLiteCommand(sqlCheck, conn);
                    int anzahlBestellungen = Convert.ToInt32(cmdCheck.ExecuteScalar());

                    if (anzahlBestellungen > 0)
                    {
                        MessageBox.Show("Dieser Kunde hat " + anzahlBestellungen + " Bestellung(en) und kann nicht gelöscht werden.\n\nBitte löschen Sie zuerst die Bestellungen.", "Löschen nicht möglich", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        conn.Close();
                        return;
                    }

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler bei der Prüfung:\n" + ex.Message, "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult result = MessageBox.Show("Möchten Sie den Kunden \"" + txtVorname.Text + " " + txtNachname.Text + "\" wirklich löschen?", "Löschen bestätigen", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                this.Cursor = Cursors.WaitCursor;
                lblStatus.Text = "Lösche...";
                lblStatus.ForeColor = Color.Blue;

                try
                {
                    string connStr = "Data Source=" + Globals.DbPath + ";Version=3;";
                    using (SQLiteConnection conn = new SQLiteConnection(connStr))
                    {
                        conn.Open();

                        // Anti-Pattern: Copy-paste logging before delete
                        string logSql = "INSERT INTO AuditLog (Tabelle, AktionTyp, DatensatzId, Beschreibung, BenutzerName, Zeitstempel) VALUES ('Kunden', 'DELETE', " + aktuelleKundenId + ", 'Kunde gelöscht: " + txtKundenNr.Text + " - " + txtVorname.Text + " " + txtNachname.Text + "', '" + Globals.CurrentUser + "', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                        try
                        {
                            SQLiteCommand cmdLog = new SQLiteCommand(logSql, conn);
                            cmdLog.ExecuteNonQuery();
                        }
                        catch
                        {
                            // Anti-Pattern: Empty catch
                        }

                        string sql = "DELETE FROM Kunden WHERE Id = " + aktuelleKundenId;
                        SQLiteCommand cmdDelete = new SQLiteCommand(sql, conn);
                        cmdDelete.ExecuteNonQuery();

                        conn.Close();
                    }

                    anzahlKunden--;
                    Globals.AnzahlKunden = anzahlKunden;

                    ClearForm();
                    ReloadGrid();

                    lblStatus.Text = "Kunde gelöscht - " + anzahlKunden + " Kunden verbleibend";
                    lblStatus.ForeColor = Color.Green;
                }
                catch (Exception ex)
                {
                    lblStatus.Text = "Fehler beim Löschen: " + ex.Message;
                    lblStatus.ForeColor = Color.Red;
                    MessageBox.Show("Fehler beim Löschen:\n" + ex.Message, "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    this.Cursor = Cursors.Default;
                }
            }
        }

        private void btnNeu_Click(object sender, EventArgs e)
        {
            // Anti-Pattern: Check dirty without proper dialog handling
            if (isDirty)
            {
                DialogResult res = MessageBox.Show("Es gibt ungespeicherte Änderungen. Verwerfen?", "Warnung", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (res == DialogResult.No)
                {
                    return;
                }
            }

            ClearForm();
            isNeu = true;
            aktuelleKundenId = 0;
            txtKundenNr.Text = "(wird automatisch vergeben)";
            txtVorname.Focus();

            lblStatus.Text = "Neuer Kunde - Bitte Daten eingeben";
            lblStatus.ForeColor = Color.Blue;

            // Anti-Pattern: Deselect grid
            dgvKunden.ClearSelection();
        }

        private void btnSchliessen_Click(object sender, EventArgs e)
        {
            // Anti-Pattern: Duplicate dirty check
            if (isDirty)
            {
                DialogResult res = MessageBox.Show("Es gibt ungespeicherte Änderungen. Trotzdem schließen?", "Warnung", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (res == DialogResult.No)
                {
                    return;
                }
            }

            this.Close();
        }

        // Anti-Pattern: Long method for search
        private void btnSuchen_Click(object sender, EventArgs e)
        {
            string suchtext = txtSuche.Text.Trim();
            letzteSuche = suchtext;

            this.Cursor = Cursors.WaitCursor;
            lblStatus.Text = "Suche...";
            lblStatus.ForeColor = Color.Blue;

            try
            {
                string connStr = "Data Source=" + Globals.DbPath + ";Version=3;";
                using (SQLiteConnection conn = new SQLiteConnection(connStr))
                {
                    conn.Open();

                    string sql = "";
                    if (string.IsNullOrEmpty(suchtext))
                    {
                        sql = "SELECT Id, KundenNr, Firma, Anrede, Vorname, Nachname, Strasse, PLZ, Ort, Land, Telefon, Email FROM Kunden ORDER BY Nachname, Vorname";
                    }
                    else
                    {
                        // Anti-Pattern: SQL string concatenation for search
                        sql = "SELECT Id, KundenNr, Firma, Anrede, Vorname, Nachname, Strasse, PLZ, Ort, Land, Telefon, Email FROM Kunden WHERE ";
                        sql += "KundenNr LIKE '%" + suchtext.Replace("'", "''") + "%' OR ";
                        sql += "Firma LIKE '%" + suchtext.Replace("'", "''") + "%' OR ";
                        sql += "Vorname LIKE '%" + suchtext.Replace("'", "''") + "%' OR ";
                        sql += "Nachname LIKE '%" + suchtext.Replace("'", "''") + "%' OR ";
                        sql += "Ort LIKE '%" + suchtext.Replace("'", "''") + "%' OR ";
                        sql += "Email LIKE '%" + suchtext.Replace("'", "''") + "%' OR ";
                        sql += "Telefon LIKE '%" + suchtext.Replace("'", "''") + "%' ";
                        sql += "ORDER BY Nachname, Vorname";
                    }

                    SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                    SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
                    dtKunden = new DataTable();
                    adapter.Fill(dtKunden);

                    dgvKunden.DataSource = dtKunden;

                    // Anti-Pattern: Copy-paste column configuration from Load
                    if (dgvKunden.Columns.Contains("Id"))
                        dgvKunden.Columns["Id"].Visible = false;
                    if (dgvKunden.Columns.Contains("KundenNr"))
                    {
                        dgvKunden.Columns["KundenNr"].HeaderText = "Kunden-Nr";
                        dgvKunden.Columns["KundenNr"].Width = 80;
                    }
                    if (dgvKunden.Columns.Contains("Firma"))
                    {
                        dgvKunden.Columns["Firma"].HeaderText = "Firma";
                        dgvKunden.Columns["Firma"].Width = 120;
                    }
                    if (dgvKunden.Columns.Contains("Anrede"))
                    {
                        dgvKunden.Columns["Anrede"].Visible = false;
                    }
                    if (dgvKunden.Columns.Contains("Vorname"))
                    {
                        dgvKunden.Columns["Vorname"].HeaderText = "Vorname";
                        dgvKunden.Columns["Vorname"].Width = 100;
                    }
                    if (dgvKunden.Columns.Contains("Nachname"))
                    {
                        dgvKunden.Columns["Nachname"].HeaderText = "Nachname";
                        dgvKunden.Columns["Nachname"].Width = 100;
                    }
                    if (dgvKunden.Columns.Contains("Strasse"))
                    {
                        dgvKunden.Columns["Strasse"].Visible = false;
                    }
                    if (dgvKunden.Columns.Contains("PLZ"))
                    {
                        dgvKunden.Columns["PLZ"].Visible = false;
                    }
                    if (dgvKunden.Columns.Contains("Ort"))
                    {
                        dgvKunden.Columns["Ort"].HeaderText = "Ort";
                        dgvKunden.Columns["Ort"].Width = 100;
                    }
                    if (dgvKunden.Columns.Contains("Land"))
                    {
                        dgvKunden.Columns["Land"].Visible = false;
                    }
                    if (dgvKunden.Columns.Contains("Telefon"))
                    {
                        dgvKunden.Columns["Telefon"].HeaderText = "Telefon";
                        dgvKunden.Columns["Telefon"].Width = 100;
                    }
                    if (dgvKunden.Columns.Contains("Email"))
                    {
                        dgvKunden.Columns["Email"].HeaderText = "E-Mail";
                        dgvKunden.Columns["Email"].Width = 150;
                    }

                    int gefunden = dtKunden.Rows.Count;

                    conn.Close();

                    if (string.IsNullOrEmpty(suchtext))
                    {
                        lblStatus.Text = "Alle " + gefunden + " Kunden angezeigt";
                    }
                    else
                    {
                        lblStatus.Text = gefunden + " Kunden gefunden für \"" + suchtext + "\"";
                    }
                    lblStatus.ForeColor = Color.Black;
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Suchfehler: " + ex.Message;
                lblStatus.ForeColor = Color.Red;
                MessageBox.Show("Fehler bei der Suche:\n" + ex.Message, "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void txtSuche_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnSuchen_Click(sender, e);
                e.Handled = true;
            }
        }

        // Anti-Pattern: Selection changed with direct SQL
        private void dgvKunden_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvKunden.SelectedRows.Count > 0)
            {
                // Anti-Pattern: Check dirty inline
                if (isDirty)
                {
                    // Don't warn on every selection, just load
                }

                DataGridViewRow row = dgvKunden.SelectedRows[0];
                if (row.Cells["Id"].Value != null && row.Cells["Id"].Value != DBNull.Value)
                {
                    int id = Convert.ToInt32(row.Cells["Id"].Value);
                    LoadKunde(id);
                }
            }
        }

        private void dgvKunden_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Anti-Pattern: Set global for other forms
                Globals.CurrentKundenNr = txtKundenNr.Text;
                Globals.CurrentKundenId = aktuelleKundenId;

                // Anti-Pattern: Could open order form here
                lblStatus.Text = "Kunde " + txtKundenNr.Text + " ausgewählt";
                lblStatus.ForeColor = Color.Blue;
            }
        }

        // Anti-Pattern: Load single customer with raw SQL
        private void LoadKunde(int kundenId)
        {
            try
            {
                string connStr = "Data Source=" + Globals.DbPath + ";Version=3;";
                using (SQLiteConnection conn = new SQLiteConnection(connStr))
                {
                    conn.Open();

                    string sql = "SELECT * FROM Kunden WHERE Id = " + kundenId;
                    SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                    SQLiteDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        aktuelleKundenId = kundenId;
                        isNeu = false;

                        txtKundenNr.Text = reader["KundenNr"] != DBNull.Value ? reader["KundenNr"].ToString() : "";
                        txtFirma.Text = reader["Firma"] != DBNull.Value ? reader["Firma"].ToString() : "";

                        // Anti-Pattern: ComboBox selection by iteration
                        string anrede = reader["Anrede"] != DBNull.Value ? reader["Anrede"].ToString() : "";
                        cboAnrede.SelectedIndex = 0;
                        for (int i = 0; i < cboAnrede.Items.Count; i++)
                        {
                            if (cboAnrede.Items[i].ToString() == anrede)
                            {
                                cboAnrede.SelectedIndex = i;
                                break;
                            }
                        }

                        txtVorname.Text = reader["Vorname"] != DBNull.Value ? reader["Vorname"].ToString() : "";
                        txtNachname.Text = reader["Nachname"] != DBNull.Value ? reader["Nachname"].ToString() : "";
                        txtStrasse.Text = reader["Strasse"] != DBNull.Value ? reader["Strasse"].ToString() : "";
                        txtPLZ.Text = reader["PLZ"] != DBNull.Value ? reader["PLZ"].ToString() : "";
                        txtOrt.Text = reader["Ort"] != DBNull.Value ? reader["Ort"].ToString() : "";

                        // Anti-Pattern: Copy-paste ComboBox selection
                        string land = reader["Land"] != DBNull.Value ? reader["Land"].ToString() : "";
                        cboLand.SelectedIndex = 0;
                        for (int i = 0; i < cboLand.Items.Count; i++)
                        {
                            if (cboLand.Items[i].ToString() == land)
                            {
                                cboLand.SelectedIndex = i;
                                break;
                            }
                        }

                        txtTelefon.Text = reader["Telefon"] != DBNull.Value ? reader["Telefon"].ToString() : "";
                        txtEmail.Text = reader["Email"] != DBNull.Value ? reader["Email"].ToString() : "";
                        txtBemerkung.Text = reader["Bemerkung"] != DBNull.Value ? reader["Bemerkung"].ToString() : "";

                        isDirty = false;

                        // Anti-Pattern: Update global state
                        Globals.CurrentKundenNr = txtKundenNr.Text;
                        Globals.CurrentKundenId = aktuelleKundenId;
                    }

                    reader.Close();
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Laden des Kunden:\n" + ex.Message, "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Anti-Pattern: Duplicate reload logic
        private void ReloadGrid()
        {
            try
            {
                string connStr = "Data Source=" + Globals.DbPath + ";Version=3;";
                using (SQLiteConnection conn = new SQLiteConnection(connStr))
                {
                    conn.Open();

                    string sql = "";
                    if (string.IsNullOrEmpty(letzteSuche))
                    {
                        sql = "SELECT Id, KundenNr, Firma, Anrede, Vorname, Nachname, Strasse, PLZ, Ort, Land, Telefon, Email FROM Kunden ORDER BY Nachname, Vorname";
                    }
                    else
                    {
                        // Anti-Pattern: Copy-paste search SQL
                        sql = "SELECT Id, KundenNr, Firma, Anrede, Vorname, Nachname, Strasse, PLZ, Ort, Land, Telefon, Email FROM Kunden WHERE ";
                        sql += "KundenNr LIKE '%" + letzteSuche.Replace("'", "''") + "%' OR ";
                        sql += "Firma LIKE '%" + letzteSuche.Replace("'", "''") + "%' OR ";
                        sql += "Vorname LIKE '%" + letzteSuche.Replace("'", "''") + "%' OR ";
                        sql += "Nachname LIKE '%" + letzteSuche.Replace("'", "''") + "%' OR ";
                        sql += "Ort LIKE '%" + letzteSuche.Replace("'", "''") + "%' OR ";
                        sql += "Email LIKE '%" + letzteSuche.Replace("'", "''") + "%' OR ";
                        sql += "Telefon LIKE '%" + letzteSuche.Replace("'", "''") + "%' ";
                        sql += "ORDER BY Nachname, Vorname";
                    }

                    SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                    SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
                    dtKunden = new DataTable();
                    adapter.Fill(dtKunden);

                    dgvKunden.DataSource = dtKunden;

                    // Anti-Pattern: Copy-paste column configuration AGAIN
                    if (dgvKunden.Columns.Contains("Id"))
                        dgvKunden.Columns["Id"].Visible = false;
                    if (dgvKunden.Columns.Contains("Anrede"))
                        dgvKunden.Columns["Anrede"].Visible = false;
                    if (dgvKunden.Columns.Contains("Strasse"))
                        dgvKunden.Columns["Strasse"].Visible = false;
                    if (dgvKunden.Columns.Contains("PLZ"))
                        dgvKunden.Columns["PLZ"].Visible = false;
                    if (dgvKunden.Columns.Contains("Land"))
                        dgvKunden.Columns["Land"].Visible = false;

                    anzahlKunden = dtKunden.Rows.Count;

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                // Anti-Pattern: Swallow exception
                lblStatus.Text = "Fehler beim Aktualisieren";
                lblStatus.ForeColor = Color.Red;
            }
        }

        private void ClearForm()
        {
            aktuelleKundenId = 0;
            isNeu = false;
            isDirty = false;

            txtKundenNr.Text = "";
            txtFirma.Text = "";
            cboAnrede.SelectedIndex = 0;
            txtVorname.Text = "";
            txtNachname.Text = "";
            txtStrasse.Text = "";
            txtPLZ.Text = "";
            txtOrt.Text = "";
            cboLand.SelectedIndex = 0;
            txtTelefon.Text = "";
            txtEmail.Text = "";
            txtBemerkung.Text = "";
        }

        private void FrmKunden_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Anti-Pattern: Duplicate dirty check
            if (isDirty)
            {
                DialogResult res = MessageBox.Show("Es gibt ungespeicherte Änderungen. Trotzdem schließen?", "Warnung", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (res == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }

            // Anti-Pattern: Update global reference
            Globals.FrmKundenInstance = null;
        }

        // Anti-Pattern: Public method to access private state
        public int GetAktuelleKundenId()
        {
            return aktuelleKundenId;
        }

        // Anti-Pattern: Public method exposing internal details
        public string GetAktuelleKundenNr()
        {
            return txtKundenNr.Text;
        }

        // Anti-Pattern: Method to set external state
        public void SetKundeFromExternal(int kundenId)
        {
            LoadKunde(kundenId);

            // Anti-Pattern: Find and select in grid manually
            for (int i = 0; i < dgvKunden.Rows.Count; i++)
            {
                if (dgvKunden.Rows[i].Cells["Id"].Value != null)
                {
                    if (Convert.ToInt32(dgvKunden.Rows[i].Cells["Id"].Value) == kundenId)
                    {
                        dgvKunden.Rows[i].Selected = true;
                        dgvKunden.FirstDisplayedScrollingRowIndex = i;
                        break;
                    }
                }
            }
        }

        // Anti-Pattern: Expose DataTable publicly
        public DataTable GetKundenData()
        {
            return dtKunden;
        }

        // Anti-Pattern: Utility method that belongs elsewhere
        public string FormatKundenAnzeige(int kundenId)
        {
            try
            {
                string connStr = "Data Source=" + Globals.DbPath + ";Version=3;";
                using (SQLiteConnection conn = new SQLiteConnection(connStr))
                {
                    conn.Open();
                    string sql = "SELECT KundenNr, Vorname, Nachname, Firma FROM Kunden WHERE Id = " + kundenId;
                    SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                    SQLiteDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        string result = reader["KundenNr"].ToString() + " - ";
                        if (reader["Firma"] != DBNull.Value && !string.IsNullOrEmpty(reader["Firma"].ToString()))
                        {
                            result += reader["Firma"].ToString();
                        }
                        else
                        {
                            result += reader["Vorname"].ToString() + " " + reader["Nachname"].ToString();
                        }
                        reader.Close();
                        conn.Close();
                        return result;
                    }

                    reader.Close();
                    conn.Close();
                }
            }
            catch
            {
                // Anti-Pattern: Empty catch
            }

            return "";
        }
    }
}
