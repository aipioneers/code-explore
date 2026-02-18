using System;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using System.Windows.Forms;
using EnterpriseApp.Helpers;

namespace EnterpriseApp.Forms
{
    // Druckvorschau - Legacy Style mit vielen Anti-Patterns
    public partial class FrmDruckvorschau : Form
    {
        // Anti-Pattern: Public fields
        public string DokumentTyp = "";
        public int DokumentId = 0;

        private string druckInhalt = "";
        private int aktuelleSeite = 1;
        private int gesamtSeiten = 1;
        private Font schriftNormal;
        private Font schriftFett;
        private Font schriftKlein;

        public FrmDruckvorschau()
        {
            InitializeComponent();
            schriftNormal = new Font("Arial", 10);
            schriftFett = new Font("Arial", 10, FontStyle.Bold);
            schriftKlein = new Font("Arial", 8);
        }

        private void FrmDruckvorschau_Load(object sender, EventArgs e)
        {
            this.Text = "Druckvorschau - " + DokumentTyp;
            GeneriereVorschau();
        }

        // Anti-Pattern: Monster-Methode mit verschiedenen Dokumenttypen
        private void GeneriereVorschau()
        {
            StringBuilder sb = new StringBuilder();

            try
            {
                string connStr = "Data Source=" + Globals.DbPath + ";Version=3;";
                SQLiteConnection conn = new SQLiteConnection(connStr);
                conn.Open();

                if (DokumentTyp == "Bestellung")
                {
                    // Bestellung laden
                    string sql = @"SELECT b.*, k.KundenNr, k.Firma, k.Anrede, k.Vorname, k.Nachname,
                        k.Strasse, k.PLZ, k.Ort, k.Land
                        FROM Bestellungen b
                        LEFT JOIN Kunden k ON b.KundeId = k.Id
                        WHERE b.Id = " + DokumentId.ToString();

                    SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                    SQLiteDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        // Header
                        sb.AppendLine("═══════════════════════════════════════════════════════════════");
                        sb.AppendLine("                        BESTELLUNG                              ");
                        sb.AppendLine("═══════════════════════════════════════════════════════════════");
                        sb.AppendLine();
                        sb.AppendLine("Bestellnummer:  " + reader["BestellNr"].ToString());
                        sb.AppendLine("Bestelldatum:   " + Convert.ToDateTime(reader["BestellDatum"]).ToString("dd.MM.yyyy"));
                        sb.AppendLine("Status:         " + reader["Status"].ToString());
                        sb.AppendLine();
                        sb.AppendLine("───────────────────────────────────────────────────────────────");
                        sb.AppendLine("KUNDE");
                        sb.AppendLine("───────────────────────────────────────────────────────────────");
                        sb.AppendLine(reader["KundenNr"].ToString());

                        string kundeName = "";
                        if (reader["Firma"] != DBNull.Value && reader["Firma"].ToString() != "")
                        {
                            kundeName = reader["Firma"].ToString();
                        }
                        else
                        {
                            kundeName = reader["Anrede"].ToString() + " " + reader["Vorname"].ToString() + " " + reader["Nachname"].ToString();
                        }
                        sb.AppendLine(kundeName);
                        sb.AppendLine(reader["Strasse"].ToString());
                        sb.AppendLine(reader["PLZ"].ToString() + " " + reader["Ort"].ToString());
                        if (reader["Land"] != DBNull.Value && reader["Land"].ToString() != "")
                        {
                            sb.AppendLine(reader["Land"].ToString());
                        }
                        sb.AppendLine();

                        if (reader["Lieferadresse"] != DBNull.Value && reader["Lieferadresse"].ToString() != "")
                        {
                            sb.AppendLine("───────────────────────────────────────────────────────────────");
                            sb.AppendLine("LIEFERADRESSE");
                            sb.AppendLine("───────────────────────────────────────────────────────────────");
                            sb.AppendLine(reader["Lieferadresse"].ToString());
                            sb.AppendLine();
                        }
                    }
                    reader.Close();

                    // Positionen laden
                    sql = @"SELECT bp.Position, p.Artikelnummer, p.Produktname, bp.Menge, bp.Einheit,
                        bp.Einzelpreis, bp.Rabatt, bp.Positionssumme
                        FROM BestellPositionen bp
                        LEFT JOIN Produkte p ON bp.ProduktId = p.Id
                        WHERE bp.BestellungId = " + DokumentId.ToString() + " ORDER BY bp.Position";

                    cmd = new SQLiteCommand(sql, conn);
                    reader = cmd.ExecuteReader();

                    sb.AppendLine("───────────────────────────────────────────────────────────────");
                    sb.AppendLine("POSITIONEN");
                    sb.AppendLine("───────────────────────────────────────────────────────────────");
                    sb.AppendLine(String.Format("{0,-5} {1,-12} {2,-25} {3,8} {4,6} {5,10} {6,6} {7,12}",
                        "Pos", "Art.-Nr.", "Bezeichnung", "Menge", "Einh.", "Preis", "Rab.%", "Summe"));
                    sb.AppendLine("───────────────────────────────────────────────────────────────");

                    decimal gesamtNetto = 0;
                    while (reader.Read())
                    {
                        decimal posSum = Convert.ToDecimal(reader["Positionssumme"]);
                        gesamtNetto += posSum;

                        string bezeichnung = reader["Produktname"].ToString();
                        if (bezeichnung.Length > 25) bezeichnung = bezeichnung.Substring(0, 22) + "...";

                        sb.AppendLine(String.Format("{0,-5} {1,-12} {2,-25} {3,8:N2} {4,6} {5,10:N2} {6,6:N1} {7,12:N2}",
                            reader["Position"],
                            reader["Artikelnummer"],
                            bezeichnung,
                            reader["Menge"],
                            reader["Einheit"],
                            reader["Einzelpreis"],
                            reader["Rabatt"],
                            posSum));
                    }
                    reader.Close();

                    sb.AppendLine("───────────────────────────────────────────────────────────────");

                    decimal mwst = gesamtNetto * 0.19m;
                    decimal brutto = gesamtNetto + mwst;

                    sb.AppendLine(String.Format("{0,55} {1,12:N2}", "Netto:", gesamtNetto) + " EUR");
                    sb.AppendLine(String.Format("{0,55} {1,12:N2}", "MwSt. 19%:", mwst) + " EUR");
                    sb.AppendLine("═══════════════════════════════════════════════════════════════");
                    sb.AppendLine(String.Format("{0,55} {1,12:N2}", "GESAMT:", brutto) + " EUR");
                    sb.AppendLine("═══════════════════════════════════════════════════════════════");
                }
                else if (DokumentTyp == "Kunde")
                {
                    // Kundendatenblatt
                    string sql = "SELECT * FROM Kunden WHERE Id = " + DokumentId.ToString();
                    SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                    SQLiteDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        sb.AppendLine("═══════════════════════════════════════════════════════════════");
                        sb.AppendLine("                      KUNDENDATENBLATT                          ");
                        sb.AppendLine("═══════════════════════════════════════════════════════════════");
                        sb.AppendLine();
                        sb.AppendLine("Kundennummer:   " + reader["KundenNr"].ToString());
                        sb.AppendLine();
                        sb.AppendLine("───────────────────────────────────────────────────────────────");
                        sb.AppendLine("STAMMDATEN");
                        sb.AppendLine("───────────────────────────────────────────────────────────────");

                        if (reader["Firma"] != DBNull.Value && reader["Firma"].ToString() != "")
                        {
                            sb.AppendLine("Firma:          " + reader["Firma"].ToString());
                        }
                        sb.AppendLine("Anrede:         " + reader["Anrede"].ToString());
                        sb.AppendLine("Vorname:        " + reader["Vorname"].ToString());
                        sb.AppendLine("Nachname:       " + reader["Nachname"].ToString());
                        sb.AppendLine();
                        sb.AppendLine("───────────────────────────────────────────────────────────────");
                        sb.AppendLine("ADRESSE");
                        sb.AppendLine("───────────────────────────────────────────────────────────────");
                        sb.AppendLine("Straße:         " + reader["Strasse"].ToString());
                        sb.AppendLine("PLZ/Ort:        " + reader["PLZ"].ToString() + " " + reader["Ort"].ToString());
                        sb.AppendLine("Land:           " + reader["Land"].ToString());
                        sb.AppendLine();
                        sb.AppendLine("───────────────────────────────────────────────────────────────");
                        sb.AppendLine("KONTAKT");
                        sb.AppendLine("───────────────────────────────────────────────────────────────");
                        sb.AppendLine("Telefon:        " + reader["Telefon"].ToString());
                        sb.AppendLine("E-Mail:         " + reader["Email"].ToString());

                        if (reader["Bemerkung"] != DBNull.Value && reader["Bemerkung"].ToString() != "")
                        {
                            sb.AppendLine();
                            sb.AppendLine("───────────────────────────────────────────────────────────────");
                            sb.AppendLine("BEMERKUNG");
                            sb.AppendLine("───────────────────────────────────────────────────────────────");
                            sb.AppendLine(reader["Bemerkung"].ToString());
                        }
                    }
                    reader.Close();
                }
                else if (DokumentTyp == "Produkt")
                {
                    // Produktdatenblatt
                    string sql = "SELECT * FROM Produkte WHERE Id = " + DokumentId.ToString();
                    SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                    SQLiteDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        sb.AppendLine("═══════════════════════════════════════════════════════════════");
                        sb.AppendLine("                     PRODUKTDATENBLATT                          ");
                        sb.AppendLine("═══════════════════════════════════════════════════════════════");
                        sb.AppendLine();
                        sb.AppendLine("Artikelnummer:  " + reader["Artikelnummer"].ToString());
                        sb.AppendLine("Bezeichnung:    " + reader["Produktname"].ToString());
                        sb.AppendLine("Kategorie:      " + reader["Kategorie"].ToString());
                        sb.AppendLine();
                        sb.AppendLine("───────────────────────────────────────────────────────────────");
                        sb.AppendLine("PREISE");
                        sb.AppendLine("───────────────────────────────────────────────────────────────");
                        sb.AppendLine("Einheit:        " + reader["Einheit"].ToString());
                        sb.AppendLine("EK-Preis:       " + Convert.ToDecimal(reader["EKPreis"]).ToString("N2") + " EUR");
                        sb.AppendLine("VK-Preis:       " + Convert.ToDecimal(reader["VKPreis"]).ToString("N2") + " EUR");
                        sb.AppendLine();
                        sb.AppendLine("───────────────────────────────────────────────────────────────");
                        sb.AppendLine("LAGER");
                        sb.AppendLine("───────────────────────────────────────────────────────────────");
                        sb.AppendLine("Lagerbestand:   " + reader["Lagerbestand"].ToString());
                        sb.AppendLine("Mindestbestand: " + reader["Mindestbestand"].ToString());
                        sb.AppendLine("Status:         " + (Convert.ToInt32(reader["Aktiv"]) == 1 ? "Aktiv" : "Inaktiv"));

                        if (reader["Beschreibung"] != DBNull.Value && reader["Beschreibung"].ToString() != "")
                        {
                            sb.AppendLine();
                            sb.AppendLine("───────────────────────────────────────────────────────────────");
                            sb.AppendLine("BESCHREIBUNG");
                            sb.AppendLine("───────────────────────────────────────────────────────────────");
                            sb.AppendLine(reader["Beschreibung"].ToString());
                        }
                    }
                    reader.Close();
                }

                conn.Close();

                // Footer
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine("═══════════════════════════════════════════════════════════════");
                sb.AppendLine("Erstellt am: " + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"));
                sb.AppendLine("Benutzer:    " + Globals.CurrentUser);
                sb.AppendLine("═══════════════════════════════════════════════════════════════");

                druckInhalt = sb.ToString();
                txtVorschau.Text = druckInhalt;
            }
            catch (Exception ex)
            {
                txtVorschau.Text = "Fehler beim Generieren der Vorschau:\n" + ex.Message;
            }
        }

        private void btnDrucken_Click(object sender, EventArgs e)
        {
            try
            {
                PrintDocument doc = new PrintDocument();
                doc.PrintPage += Doc_PrintPage;

                PrintDialog dlg = new PrintDialog();
                dlg.Document = doc;

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    doc.Print();
                    MessageBox.Show("Druckauftrag gesendet!", "Drucken", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Druckfehler: " + ex.Message, "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Doc_PrintPage(object sender, PrintPageEventArgs e)
        {
            e.Graphics.DrawString(druckInhalt, schriftNormal, Brushes.Black, 50, 50);
            e.HasMorePages = false;
        }

        private void btnSchliessen_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
