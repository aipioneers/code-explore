using System;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;

namespace EnterpriseApp.Helpers
{
    /// <summary>
    /// Datenbankzugriffs-Helfer Klasse
    /// ACHTUNG: Diese Klasse enthält alle Datenbankoperationen
    /// Erstellt von: Hr. Müller, 2016
    /// Letzte Änderung: Fr. Schmidt, 2019 - "Optimierungen"
    /// </summary>
    public static class DbHelper
    {
        // Statische Variablen für Connection Management
        private static SQLiteConnection _connection = null;
        private static bool _isConnected = false;
        private static string _lastError = "";
        private static int _queryCount = 0;
        private static DateTime _lastQueryTime;

        // Connection String - sollte eigentlich aus Config kommen
        // TODO: Aus App.config lesen (Ticket #1234)
        private static string ConnectionString
        {
            get
            {
                // Hartcodierter Pfad als Fallback
                string path = Globals.DbPath;
                if (string.IsNullOrEmpty(path))
                {
                    path = @"C:\EnterpriseApp\legacy.db";
                }
                return "Data Source=" + path + ";Version=3;";
            }
        }

        /// <summary>
        /// Initialisiert die Datenbank und erstellt Tabellen falls nicht vorhanden
        /// Diese Methode ist sehr lang aber das ist OK weil sie nur einmal läuft
        /// </summary>
        public static void InitializeDatabase()
        {
            try
            {
                // Verzeichnis erstellen falls nicht vorhanden
                string dir = Path.GetDirectoryName(Globals.DbPath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                // Datenbank erstellen falls nicht vorhanden
                if (!File.Exists(Globals.DbPath))
                {
                    SQLiteConnection.CreateFile(Globals.DbPath);
                }

                // Tabellen erstellen
                using (var conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();

                    // Kunden Tabelle
                    string sqlKunden = @"
                        CREATE TABLE IF NOT EXISTS Kunden (
                            KundenNr INTEGER PRIMARY KEY AUTOINCREMENT,
                            Firma TEXT,
                            Anrede TEXT,
                            Vorname TEXT,
                            Nachname TEXT,
                            Strasse TEXT,
                            PLZ TEXT,
                            Ort TEXT,
                            Land TEXT DEFAULT 'Deutschland',
                            Telefon TEXT,
                            Email TEXT,
                            Bemerkung TEXT,
                            Erstellt TEXT,
                            Geaendert TEXT,
                            Aktiv INTEGER DEFAULT 1
                        )";
                    ExecuteNonQueryInternal(conn, sqlKunden);

                    // Produkte Tabelle
                    string sqlProdukte = @"
                        CREATE TABLE IF NOT EXISTS Produkte (
                            ArtikelNr TEXT PRIMARY KEY,
                            Bezeichnung TEXT NOT NULL,
                            Kategorie TEXT,
                            Beschreibung TEXT,
                            Preis REAL DEFAULT 0,
                            Lagerbestand INTEGER DEFAULT 0,
                            Mindestbestand INTEGER DEFAULT 0,
                            Lieferant TEXT,
                            Erstellt TEXT,
                            Geaendert TEXT,
                            Aktiv INTEGER DEFAULT 1
                        )";
                    ExecuteNonQueryInternal(conn, sqlProdukte);

                    // Bestellungen Tabelle
                    string sqlBestellungen = @"
                        CREATE TABLE IF NOT EXISTS Bestellungen (
                            BestellNr INTEGER PRIMARY KEY AUTOINCREMENT,
                            KundenNr INTEGER NOT NULL,
                            BestellDatum TEXT,
                            LieferDatum TEXT,
                            Status INTEGER DEFAULT 0,
                            Gesamtsumme REAL DEFAULT 0,
                            Rabatt REAL DEFAULT 0,
                            Bemerkung TEXT,
                            Erstellt TEXT,
                            Geaendert TEXT
                        )";
                    ExecuteNonQueryInternal(conn, sqlBestellungen);

                    // Bestellpositionen Tabelle
                    string sqlPositionen = @"
                        CREATE TABLE IF NOT EXISTS Bestellpositionen (
                            PosNr INTEGER PRIMARY KEY AUTOINCREMENT,
                            BestellNr INTEGER NOT NULL,
                            ArtikelNr TEXT NOT NULL,
                            Menge INTEGER DEFAULT 1,
                            Einzelpreis REAL DEFAULT 0,
                            Rabatt REAL DEFAULT 0,
                            Gesamt REAL DEFAULT 0
                        )";
                    ExecuteNonQueryInternal(conn, sqlPositionen);

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                _lastError = ex.Message;
                // Fehler ignorieren und hoffen dass es trotzdem geht
            }
        }

        /// <summary>
        /// Gibt eine Datenbankverbindung zurück
        /// WICHTIG: Connection muss vom Aufrufer geschlossen werden!
        /// </summary>
        public static SQLiteConnection GetConnection()
        {
            try
            {
                var conn = new SQLiteConnection(ConnectionString);
                conn.Open();
                _isConnected = true;
                return conn;
            }
            catch (Exception)
            {
                _isConnected = false;
                return null;
            }
        }

        /// <summary>
        /// Führt eine SELECT Query aus und gibt DataTable zurück
        /// </summary>
        public static DataTable ExecuteQuery(string sql)
        {
            DataTable dt = new DataTable();
            try
            {
                using (var conn = GetConnection())
                {
                    if (conn != null)
                    {
                        using (var cmd = new SQLiteCommand(sql, conn))
                        {
                            using (var adapter = new SQLiteDataAdapter(cmd))
                            {
                                adapter.Fill(dt);
                            }
                        }
                        conn.Close();
                    }
                }
                _queryCount++;
                _lastQueryTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                _lastError = ex.Message;
                // Leere Tabelle zurückgeben bei Fehler
            }
            return dt;
        }

        /// <summary>
        /// Führt eine SELECT Query mit Parametern aus
        /// Parameter werden als Dictionary übergeben
        /// </summary>
        public static DataTable ExecuteQuery(string sql, Dictionary<string, object> parameters)
        {
            DataTable dt = new DataTable();
            try
            {
                using (var conn = GetConnection())
                {
                    if (conn != null)
                    {
                        using (var cmd = new SQLiteCommand(sql, conn))
                        {
                            if (parameters != null)
                            {
                                foreach (var param in parameters)
                                {
                                    cmd.Parameters.AddWithValue(param.Key, param.Value);
                                }
                            }
                            using (var adapter = new SQLiteDataAdapter(cmd))
                            {
                                adapter.Fill(dt);
                            }
                        }
                        conn.Close();
                    }
                }
                _queryCount++;
            }
            catch (Exception)
            {
                // Fehler verschlucken
            }
            return dt;
        }

        /// <summary>
        /// Führt INSERT/UPDATE/DELETE aus
        /// </summary>
        public static int ExecuteNonQuery(string sql)
        {
            int result = 0;
            try
            {
                using (var conn = GetConnection())
                {
                    if (conn != null)
                    {
                        result = ExecuteNonQueryInternal(conn, sql);
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                _lastError = ex.Message;
                MessageBox.Show("Datenbankfehler: " + ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Führt INSERT/UPDATE/DELETE mit Parametern aus
        /// </summary>
        public static int ExecuteNonQuery(string sql, Dictionary<string, object> parameters)
        {
            int result = 0;
            try
            {
                using (var conn = GetConnection())
                {
                    if (conn != null)
                    {
                        using (var cmd = new SQLiteCommand(sql, conn))
                        {
                            if (parameters != null)
                            {
                                foreach (var param in parameters)
                                {
                                    cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                                }
                            }
                            result = cmd.ExecuteNonQuery();
                        }
                        conn.Close();
                    }
                }
                _queryCount++;
            }
            catch (Exception)
            {
                // Ignorieren
            }
            return result;
        }

        /// <summary>
        /// Interne Methode für NonQuery
        /// </summary>
        private static int ExecuteNonQueryInternal(SQLiteConnection conn, string sql)
        {
            using (var cmd = new SQLiteCommand(sql, conn))
            {
                return cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Gibt einen einzelnen Wert zurück (für COUNT, MAX, etc.)
        /// </summary>
        public static object ExecuteScalar(string sql)
        {
            object result = null;
            try
            {
                using (var conn = GetConnection())
                {
                    if (conn != null)
                    {
                        using (var cmd = new SQLiteCommand(sql, conn))
                        {
                            result = cmd.ExecuteScalar();
                        }
                        conn.Close();
                    }
                }
            }
            catch
            {
                // Null zurückgeben
            }
            return result;
        }

        /// <summary>
        /// Gibt die letzte eingefügte ID zurück
        /// </summary>
        public static long GetLastInsertId()
        {
            try
            {
                object result = ExecuteScalar("SELECT last_insert_rowid()");
                if (result != null)
                {
                    return Convert.ToInt64(result);
                }
            }
            catch { }
            return 0;
        }

        /// <summary>
        /// Prüft ob ein Datensatz existiert
        /// </summary>
        public static bool RecordExists(string table, string column, object value)
        {
            try
            {
                string sql = "SELECT COUNT(*) FROM " + table + " WHERE " + column + " = @val";
                var parameters = new Dictionary<string, object> { { "@val", value } };
                DataTable dt = ExecuteQuery(sql, parameters);
                if (dt.Rows.Count > 0)
                {
                    return Convert.ToInt32(dt.Rows[0][0]) > 0;
                }
            }
            catch { }
            return false;
        }

        // ============================================================
        // Kunden spezifische Methoden (sollten in eigenem Service sein)
        // ============================================================

        public static DataTable GetAlleKunden()
        {
            return ExecuteQuery("SELECT * FROM Kunden WHERE Aktiv = 1 ORDER BY Nachname, Vorname");
        }

        public static DataTable GetKunde(int kundenNr)
        {
            string sql = "SELECT * FROM Kunden WHERE KundenNr = " + kundenNr;
            return ExecuteQuery(sql);
        }

        public static bool SaveKunde(int kundenNr, string firma, string anrede, string vorname,
            string nachname, string strasse, string plz, string ort, string land,
            string telefon, string email, string bemerkung)
        {
            try
            {
                string now = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
                string sql;

                if (kundenNr == 0)
                {
                    // Neuer Kunde
                    sql = "INSERT INTO Kunden (Firma, Anrede, Vorname, Nachname, Strasse, PLZ, Ort, Land, Telefon, Email, Bemerkung, Erstellt, Geaendert, Aktiv) " +
                          "VALUES (@firma, @anrede, @vorname, @nachname, @strasse, @plz, @ort, @land, @telefon, @email, @bemerkung, @erstellt, @geaendert, 1)";
                }
                else
                {
                    // Bestehender Kunde
                    sql = "UPDATE Kunden SET Firma=@firma, Anrede=@anrede, Vorname=@vorname, Nachname=@nachname, " +
                          "Strasse=@strasse, PLZ=@plz, Ort=@ort, Land=@land, Telefon=@telefon, Email=@email, " +
                          "Bemerkung=@bemerkung, Geaendert=@geaendert WHERE KundenNr=" + kundenNr;
                }

                var parameters = new Dictionary<string, object>
                {
                    { "@firma", firma },
                    { "@anrede", anrede },
                    { "@vorname", vorname },
                    { "@nachname", nachname },
                    { "@strasse", strasse },
                    { "@plz", plz },
                    { "@ort", ort },
                    { "@land", land },
                    { "@telefon", telefon },
                    { "@email", email },
                    { "@bemerkung", bemerkung },
                    { "@erstellt", now },
                    { "@geaendert", now }
                };

                ExecuteNonQuery(sql, parameters);
                return true;
            }
            catch
            {
                return false;
            }
        }

        // ============================================================
        // Produkte spezifische Methoden
        // ============================================================

        public static DataTable GetAlleProdukte()
        {
            return ExecuteQuery("SELECT * FROM Produkte WHERE Aktiv = 1 ORDER BY Bezeichnung");
        }

        public static DataTable GetProdukt(string artikelNr)
        {
            string sql = "SELECT * FROM Produkte WHERE ArtikelNr = '" + artikelNr + "'";
            return ExecuteQuery(sql);
        }

        public static DataTable GetKategorien()
        {
            return ExecuteQuery("SELECT DISTINCT Kategorie FROM Produkte WHERE Kategorie IS NOT NULL AND Kategorie != '' ORDER BY Kategorie");
        }

        // ============================================================
        // Bestellungen spezifische Methoden
        // ============================================================

        public static DataTable GetAlleBestellungen()
        {
            string sql = @"SELECT b.*, k.Firma, k.Vorname, k.Nachname
                          FROM Bestellungen b
                          LEFT JOIN Kunden k ON b.KundenNr = k.KundenNr
                          ORDER BY b.BestellDatum DESC";
            return ExecuteQuery(sql);
        }

        public static DataTable GetBestellung(int bestellNr)
        {
            return ExecuteQuery("SELECT * FROM Bestellungen WHERE BestellNr = " + bestellNr);
        }

        public static DataTable GetBestellpositionen(int bestellNr)
        {
            string sql = @"SELECT bp.*, p.Bezeichnung
                          FROM Bestellpositionen bp
                          LEFT JOIN Produkte p ON bp.ArtikelNr = p.ArtikelNr
                          WHERE bp.BestellNr = " + bestellNr;
            return ExecuteQuery(sql);
        }

        // ============================================================
        // Statistik Methoden für Dashboard
        // ============================================================

        public static int GetAnzahlKunden()
        {
            try
            {
                object result = ExecuteScalar("SELECT COUNT(*) FROM Kunden WHERE Aktiv = 1");
                return result != null ? Convert.ToInt32(result) : 0;
            }
            catch { return 0; }
        }

        public static int GetAnzahlProdukte()
        {
            try
            {
                object result = ExecuteScalar("SELECT COUNT(*) FROM Produkte WHERE Aktiv = 1");
                return result != null ? Convert.ToInt32(result) : 0;
            }
            catch { return 0; }
        }

        public static int GetAnzahlBestellungen()
        {
            try
            {
                object result = ExecuteScalar("SELECT COUNT(*) FROM Bestellungen");
                return result != null ? Convert.ToInt32(result) : 0;
            }
            catch { return 0; }
        }

        public static double GetUmsatzGesamt()
        {
            try
            {
                object result = ExecuteScalar("SELECT SUM(Gesamtsumme) FROM Bestellungen WHERE Status >= 2");
                return result != null && result != DBNull.Value ? Convert.ToDouble(result) : 0;
            }
            catch { return 0; }
        }

        // ============================================================
        // Alte/ungenutzte Methoden (nicht löschen - könnten noch gebraucht werden)
        // ============================================================

        [Obsolete("Nicht mehr verwenden - siehe GetAlleKunden")]
        public static DataTable LoadCustomers()
        {
            return GetAlleKunden();
        }

        public static void BackupDatabase()
        {
            // TODO: Implementieren
            // Ticket #5678 - offen seit 2017
        }

        public static void OptimizeDatabase()
        {
            try
            {
                ExecuteNonQuery("VACUUM");
            }
            catch { }
        }

        /// <summary>
        /// Fügt Testdaten ein falls Datenbank leer ist
        /// </summary>
        public static void InsertSampleData()
        {
            try
            {
                // Prüfen ob schon Daten vorhanden
                if (GetAnzahlKunden() > 0) return;

                string now = DateTime.Now.ToString("dd.MM.yyyy HH:mm");

                // 20 Kunden einfügen
                string[] firmen = { "Müller GmbH", "Schmidt AG", "Weber KG", "Fischer OHG", "Meyer & Co" };
                string[] vornamen = { "Hans", "Peter", "Klaus", "Thomas", "Andreas", "Michael", "Stefan", "Frank", "Martin", "Jürgen" };
                string[] nachnamen = { "Müller", "Schmidt", "Weber", "Fischer", "Meyer", "Wagner", "Becker", "Schulz", "Hoffmann", "Koch" };
                string[] orte = { "Berlin", "Hamburg", "München", "Köln", "Frankfurt", "Stuttgart", "Düsseldorf", "Leipzig", "Dresden", "Hannover" };

                Random rnd = new Random(42); // Fixed seed für reproduzierbare Daten

                for (int i = 1; i <= 20; i++)
                {
                    string firma = i <= 5 ? firmen[i - 1] : "";
                    string anrede = firma != "" ? "Firma" : (i % 3 == 0 ? "Frau" : "Herr");
                    string vorname = vornamen[rnd.Next(vornamen.Length)];
                    string nachname = nachnamen[rnd.Next(nachnamen.Length)];
                    string ort = orte[rnd.Next(orte.Length)];
                    string plz = (10000 + rnd.Next(90000)).ToString();

                    string sql = $"INSERT INTO Kunden (Firma, Anrede, Vorname, Nachname, Strasse, PLZ, Ort, Land, Telefon, Email, Erstellt, Geaendert, Aktiv) " +
                                 $"VALUES ('{firma}', '{anrede}', '{vorname}', '{nachname}', 'Musterstraße {i}', '{plz}', '{ort}', 'Deutschland', " +
                                 $"'0{rnd.Next(100, 999)}/{rnd.Next(1000000, 9999999)}', '{vorname.ToLower()}.{nachname.ToLower()}@example.de', '{now}', '{now}', 1)";
                    ExecuteNonQuery(sql);
                }

                // 50 Produkte einfügen
                string[] kategorien = { "Elektronik", "Bürobedarf", "Möbel", "Software", "Dienstleistung" };
                string[] produktPrefixe = { "Premium", "Standard", "Basic", "Professional", "Enterprise" };
                string[] produktNamen = { "Monitor", "Tastatur", "Maus", "Drucker", "Scanner", "Laptop", "PC", "Server", "Stuhl", "Schreibtisch" };

                for (int i = 1; i <= 50; i++)
                {
                    string artikelNr = $"{(char)('A' + (i % 5))}-{i:D3}";
                    string kategorie = kategorien[(i - 1) % kategorien.Length];
                    string prefix = produktPrefixe[rnd.Next(produktPrefixe.Length)];
                    string name = produktNamen[rnd.Next(produktNamen.Length)];
                    string bezeichnung = $"{prefix} {name} {i}";
                    double preis = Math.Round(rnd.Next(50, 2000) + rnd.NextDouble(), 2);
                    int lagerbestand = rnd.Next(0, 100);
                    int mindestbestand = rnd.Next(5, 20);

                    string sql = $"INSERT INTO Produkte (ArtikelNr, Bezeichnung, Kategorie, Beschreibung, Preis, Lagerbestand, Mindestbestand, Lieferant, Erstellt, Geaendert, Aktiv) " +
                                 $"VALUES ('{artikelNr}', '{bezeichnung}', '{kategorie}', 'Beschreibung für {bezeichnung}', {preis.ToString(System.Globalization.CultureInfo.InvariantCulture)}, {lagerbestand}, {mindestbestand}, 'Lieferant {(i % 5) + 1}', '{now}', '{now}', 1)";
                    ExecuteNonQuery(sql);
                }

                // 30 Bestellungen mit Positionen einfügen
                string[] statusWerte = { "0", "1", "2", "3" };

                for (int i = 1; i <= 30; i++)
                {
                    int kundenNr = rnd.Next(1, 21);
                    int status = rnd.Next(0, 4);
                    DateTime bestellDatum = DateTime.Now.AddDays(-rnd.Next(1, 90));
                    DateTime lieferDatum = bestellDatum.AddDays(rnd.Next(3, 14));
                    double gesamtsumme = 0;
                    double rabatt = rnd.Next(0, 3) == 0 ? rnd.Next(5, 15) : 0;

                    string sqlBest = $"INSERT INTO Bestellungen (KundenNr, BestellDatum, LieferDatum, Status, Gesamtsumme, Rabatt, Bemerkung, Erstellt, Geaendert) " +
                                     $"VALUES ({kundenNr}, '{bestellDatum:dd.MM.yyyy}', '{lieferDatum:dd.MM.yyyy}', {status}, 0, {rabatt.ToString(System.Globalization.CultureInfo.InvariantCulture)}, '', '{now}', '{now}')";
                    ExecuteNonQuery(sqlBest);
                    long bestellNr = GetLastInsertId();

                    // 1-5 Positionen pro Bestellung
                    int anzahlPositionen = rnd.Next(1, 6);
                    for (int p = 1; p <= anzahlPositionen; p++)
                    {
                        string artikelNr = $"{(char)('A' + rnd.Next(0, 5))}-{rnd.Next(1, 51):D3}";
                        int menge = rnd.Next(1, 10);
                        double einzelpreis = Math.Round(rnd.Next(50, 500) + rnd.NextDouble(), 2);
                        double posRabatt = rnd.Next(0, 5) == 0 ? rnd.Next(5, 10) : 0;
                        double gesamt = Math.Round(menge * einzelpreis * (1 - posRabatt / 100), 2);
                        gesamtsumme += gesamt;

                        string sqlPos = $"INSERT INTO Bestellpositionen (BestellNr, ArtikelNr, Menge, Einzelpreis, Rabatt, Gesamt) " +
                                        $"VALUES ({bestellNr}, '{artikelNr}', {menge}, {einzelpreis.ToString(System.Globalization.CultureInfo.InvariantCulture)}, {posRabatt.ToString(System.Globalization.CultureInfo.InvariantCulture)}, {gesamt.ToString(System.Globalization.CultureInfo.InvariantCulture)})";
                        ExecuteNonQuery(sqlPos);
                    }

                    // Gesamtsumme aktualisieren (mit Rabatt)
                    gesamtsumme = Math.Round(gesamtsumme * (1 - rabatt / 100), 2);
                    ExecuteNonQuery($"UPDATE Bestellungen SET Gesamtsumme = {gesamtsumme.ToString(System.Globalization.CultureInfo.InvariantCulture)} WHERE BestellNr = {bestellNr}");
                }
            }
            catch (Exception ex)
            {
                // Fehler beim Einfügen von Testdaten ignorieren
                _lastError = "Testdaten: " + ex.Message;
            }
        }

        // Getter für Status
        public static bool IsConnected { get { return _isConnected; } }
        public static string LastError { get { return _lastError; } }
        public static int QueryCount { get { return _queryCount; } }
    }
}
