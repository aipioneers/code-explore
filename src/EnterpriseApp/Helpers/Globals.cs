using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;

namespace EnterpriseApp.Helpers
{
    /// <summary>
    /// Globale Variablen und Zustände
    /// ACHTUNG: Änderungen hier können die ganze Anwendung beeinflussen!
    /// Erstellt: 2015 - Hr. Schmidt
    /// </summary>
    public static class Globals
    {
        // ============================================================
        // Anwendungszustand
        // ============================================================
        public static bool IsInitialized = false;
        public static bool IsDebugMode = true;
        public static bool IsReadOnly = false;

        // ============================================================
        // Benutzerinformationen
        // ============================================================
        public static string CurrentUser = "";
        public static int CurrentUserLevel = 0; // 0=Normal, 1=Admin, 2=SuperAdmin
        public static DateTime LoginTime;
        public static bool IsLoggedIn = false;

        // ============================================================
        // Datenbankpfade
        // ============================================================
        public static string DbPath = "";
        public static string BackupPath = @"C:\EnterpriseApp\Backup\";
        public static string TempPath = @"C:\EnterpriseApp\Temp\";
        public static string LogPath = @"C:\EnterpriseApp\Logs\";

        // ============================================================
        // Aktuelle Auswahl (für Form-übergreifende Kommunikation)
        // ============================================================
        public static int CurrentKundenNr = 0;
        public static int CurrentKundenId = 0;
        public static string CurrentKundenNrStr = "";
        public static int CurrentBestellNr = 0;
        public static string CurrentArtikelNr = "";
        public static int CurrentPosNr = 0;
        public static string AktiveForm = "";
        public static int AnzahlKunden = 0;
        public static int AnzahlProdukte = 0;
        public static int AnzahlBestellungen = 0;

        // Zusätzliche Form-Referenzen (inkonsistente Benennung)
        public static object FrmKundenInstance = null;
        public static object FrmProdukteInstance = null;
        public static object FrmBestellungenInstance = null;

        // ============================================================
        // Referenzen auf geöffnete Formulare
        // WICHTIG: Nicht null-prüfen vergessen!
        // ============================================================
        public static Form MainFormRef = null;
        public static Form KundenFormRef = null;
        public static Form ProdukteFormRef = null;
        public static Form BestellungenFormRef = null;
        public static Form DruckvorschauFormRef = null;

        // ============================================================
        // Listen für Dropdowns (werden beim Start geladen)
        // ============================================================
        public static List<string> Anreden = new List<string> { "Herr", "Frau", "Firma", "Dr.", "Prof." };
        public static List<string> Laender = new List<string> { "Deutschland", "Österreich", "Schweiz", "Andere" };
        public static List<string> Kategorien = new List<string>();
        public static List<string> StatusListe = new List<string> { "Neu", "In Bearbeitung", "Versendet", "Abgeschlossen", "Storniert" };

        // ============================================================
        // Konstanten (die eigentlich Konstanten sein sollten)
        // ============================================================
        public static int MaxRecords = 1000;
        public static int PageSize = 20;
        public static int Timeout = 30;
        public static double MwstSatz = 19.0;

        // ============================================================
        // Farben (legacy style)
        // ============================================================
        public static Color FarbeAktiv = Color.White;
        public static Color FarbeInaktiv = Color.LightGray;
        public static Color FarbeFehler = Color.LightCoral;
        public static Color FarbeWarnung = Color.LightYellow;
        public static Color FarbeOK = Color.LightGreen;

        // ============================================================
        // Zähler und Statistiken
        // ============================================================
        public static int FormOpenCount = 0;
        public static int SaveCount = 0;
        public static int ErrorCount = 0;
        public static DateTime LastSaveTime;
        public static DateTime LastErrorTime;
        public static string LastErrorMessage = "";

        // ============================================================
        // Methoden
        // ============================================================

        public static void Reset()
        {
            CurrentKundenNr = 0;
            CurrentBestellNr = 0;
            CurrentArtikelNr = "";
            CurrentPosNr = 0;
        }

        public static void LogError(string message)
        {
            ErrorCount++;
            LastErrorTime = DateTime.Now;
            LastErrorMessage = message;
            // TODO: In Datei schreiben
        }

        public static void IncrementSaveCount()
        {
            SaveCount++;
            LastSaveTime = DateTime.Now;
        }
    }
}
