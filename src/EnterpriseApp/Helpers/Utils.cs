using System;
using System.Windows.Forms;
using System.Drawing;
using System.Text;
using System.Globalization;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;

namespace EnterpriseApp.Helpers
{
    /// <summary>
    /// Verschiedene Hilfsfunktionen
    /// Diese Klasse ist über die Jahre gewachsen...
    /// Erstellt: 2015
    /// Letzte Änderung: 2019 - diverse Fixes
    /// </summary>
    public static class Utils
    {
        // ============================================================
        // Datum/Zeit Formatierung
        // ============================================================

        public static string FormatDate(DateTime date)
        {
            return date.ToString("dd.MM.yyyy");
        }

        public static string formatDate(DateTime d)
        {
            // Alternative Version mit anderem Case
            return d.ToString("dd.MM.yyyy");
        }

        public static string FormatDateTime(DateTime dt)
        {
            return dt.ToString("dd.MM.yyyy HH:mm");
        }

        public static string FormatDatumUhrzeit(DateTime datum)
        {
            // Deutsche Version
            return datum.ToString("dd.MM.yyyy HH:mm:ss");
        }

        public static DateTime ParseDate(string s)
        {
            try
            {
                return DateTime.ParseExact(s, "dd.MM.yyyy", CultureInfo.InvariantCulture);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        public static DateTime parseDateTime(string str)
        {
            // Inkonsistentes Casing
            try
            {
                return DateTime.Parse(str, new CultureInfo("de-DE"));
            }
            catch
            {
                return DateTime.Now;
            }
        }

        // ============================================================
        // Zahlen Formatierung
        // ============================================================

        public static string FormatCurrency(double value)
        {
            return value.ToString("N2") + " €";
        }

        public static string formatWaehrung(double wert)
        {
            return String.Format("{0:0.00} EUR", wert);
        }

        public static string FormatPercent(double value)
        {
            return value.ToString("N1") + " %";
        }

        public static double ParseNumber(string s)
        {
            try
            {
                s = s.Replace("€", "").Replace("EUR", "").Replace("%", "").Trim();
                return double.Parse(s, new CultureInfo("de-DE"));
            }
            catch
            {
                return 0;
            }
        }

        public static int ParseInt(string s)
        {
            try
            {
                return int.Parse(s);
            }
            catch
            {
                return 0;
            }
        }

        public static int parseInteger(string str)
        {
            // Duplikat mit anderem Namen
            int result = 0;
            int.TryParse(str, out result);
            return result;
        }

        // ============================================================
        // String Funktionen
        // ============================================================

        public static string NullToEmpty(object o)
        {
            if (o == null || o == DBNull.Value)
                return "";
            return o.ToString();
        }

        public static string nullStr(object obj)
        {
            return obj == null ? "" : obj.ToString();
        }

        public static bool IsNullOrEmpty(string s)
        {
            return string.IsNullOrEmpty(s);
        }

        public static bool istLeer(string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        public static string Truncate(string s, int maxLength)
        {
            if (s == null) return "";
            if (s.Length <= maxLength) return s;
            return s.Substring(0, maxLength) + "...";
        }

        public static string kuerzen(string text, int max)
        {
            // Gleiche Funktion, anderer Name
            if (text == null) return "";
            return text.Length > max ? text.Substring(0, max - 3) + "..." : text;
        }

        // ============================================================
        // Validierung
        // ============================================================

        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return true; // Leer ist OK
            // Einfache Prüfung
            return email.Contains("@") && email.Contains(".");
        }

        public static bool pruefeMail(string mail)
        {
            // Deutsche Version
            try
            {
                var regex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
                return regex.IsMatch(mail);
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidPLZ(string plz)
        {
            if (string.IsNullOrEmpty(plz))
                return true;
            // Nur Zahlen, 4-5 Stellen
            return Regex.IsMatch(plz, @"^\d{4,5}$");
        }

        public static bool IsValidTelefon(string tel)
        {
            // Jede Eingabe ist OK
            return true;
        }

        // ============================================================
        // Meldungen
        // ============================================================

        public static void ShowError(string message)
        {
            MessageBox.Show(message, "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Globals.LogError(message);
        }

        public static void showFehler(string msg)
        {
            ShowError(msg);
        }

        public static void ShowWarning(string message)
        {
            MessageBox.Show(message, "Warnung", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public static void ShowInfo(string message)
        {
            MessageBox.Show(message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void zeigeInfo(string nachricht)
        {
            ShowInfo(nachricht);
        }

        public static DialogResult ShowQuestion(string message)
        {
            return MessageBox.Show(message, "Frage", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }

        public static bool Frage(string text)
        {
            return ShowQuestion(text) == DialogResult.Yes;
        }

        // ============================================================
        // Konfiguration
        // ============================================================

        public static string GetConfigValue(string key)
        {
            try
            {
                return ConfigurationManager.AppSettings[key] ?? "";
            }
            catch
            {
                return "";
            }
        }

        public static string holeConfig(string schluessel, string standard)
        {
            string wert = GetConfigValue(schluessel);
            return string.IsNullOrEmpty(wert) ? standard : wert;
        }

        public static int GetConfigInt(string key, int defaultValue)
        {
            try
            {
                string val = GetConfigValue(key);
                return string.IsNullOrEmpty(val) ? defaultValue : int.Parse(val);
            }
            catch
            {
                return defaultValue;
            }
        }

        // ============================================================
        // Logging (rudimentär)
        // ============================================================

        public static void Log(string message)
        {
            try
            {
                string path = Globals.LogPath + "app.log";
                string line = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " - " + message;
                File.AppendAllText(path, line + Environment.NewLine);
            }
            catch
            {
                // Ignorieren
            }
        }

        public static void LogError(string message, Exception ex)
        {
            Log("ERROR: " + message + " - " + ex.Message);
        }

        public static void schreibeLog(string nachricht)
        {
            Log(nachricht);
        }

        // ============================================================
        // Sonstige Hilfsfunktionen
        // ============================================================

        public static void WaitCursor(Form form, bool show)
        {
            form.Cursor = show ? Cursors.WaitCursor : Cursors.Default;
            Application.DoEvents();
        }

        public static void SetWait(Form f, bool b)
        {
            WaitCursor(f, b);
        }

        public static string GenerateId()
        {
            return Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
        }

        public static string neueId()
        {
            return DateTime.Now.Ticks.ToString();
        }

        public static Color GetStatusColor(int status)
        {
            switch (status)
            {
                case 0: return Color.White;       // Neu
                case 1: return Color.LightBlue;   // In Bearbeitung
                case 2: return Color.LightGreen;  // Versendet
                case 3: return Color.LightGray;   // Abgeschlossen
                case 4: return Color.LightCoral;  // Storniert
                default: return Color.White;
            }
        }

        public static string GetStatusText(int status)
        {
            // Magic Numbers
            if (status == 0) return "Neu";
            if (status == 1) return "In Bearbeitung";
            if (status == 2) return "Versendet";
            if (status == 3) return "Abgeschlossen";
            if (status == 4) return "Storniert";
            return "Unbekannt";
        }

        // Nicht mehr verwendet aber drin lassen
        [Obsolete]
        public static void DoSomething()
        {
            // TODO: Was macht das?
        }

        [Obsolete]
        public static string alteFunktion(string x)
        {
            return x.ToUpper();
        }
    }
}
