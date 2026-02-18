using System;
using System.Windows.Forms;
using EnterpriseApp.Forms;
using EnterpriseApp.Helpers;

namespace EnterpriseApp
{
    // Programm Einstiegspunkt - Legacy Style ohne async
    // TODO: Sollte irgendwann mal modernisiert werden
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// ACHTUNG: Nicht ändern ohne Rücksprache mit Hr. Müller!
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Legacy Application Configuration
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Globale Variablen initialisieren - muss vor allem anderen passieren!
            Globals.IsInitialized = false;
            Globals.CurrentUser = "admin"; // Hardcoded für jetzt
            Globals.DbPath = AppDomain.CurrentDomain.BaseDirectory + "Resources\\legacy.db";

            try
            {
                // Datenbank prüfen und ggf. erstellen
                DbHelper.InitializeDatabase();
                DbHelper.InsertSampleData();
                Globals.IsInitialized = true;
            }
            catch (Exception ex)
            {
                // Einfach ignorieren und hoffen dass es klappt
                MessageBox.Show("Datenbankfehler: " + ex.Message, "Fehler",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            // Hauptformular starten
            Application.Run(new MainForm());
        }
    }
}
