using System;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;
using EnterpriseApp.Helpers;

namespace EnterpriseApp.Forms
{
    // Produktauswahl Dialog - Legacy Style
    public partial class FrmProduktAuswahl : Form
    {
        // Anti-Pattern: Public fields
        public int AusgewaehltesProduktId = 0;
        public string AusgewaehlteArtikelNr = "";
        public string AusgewaehlteBezeichnung = "";
        public string AusgewaehlteEinheit = "";
        public decimal AusgewaehlterPreis = 0;

        private DataTable dtProdukte = null;

        public FrmProduktAuswahl()
        {
            InitializeComponent();
        }

        private void FrmProduktAuswahl_Load(object sender, EventArgs e)
        {
            LadeProdukte();
        }

        private void LadeProdukte()
        {
            try
            {
                string connStr = "Data Source=" + Globals.DbPath + ";Version=3;";
                SQLiteConnection conn = new SQLiteConnection(connStr);
                conn.Open();

                string sql = "SELECT Id, Artikelnummer, Produktname, Einheit, VKPreis, Lagerbestand FROM Produkte WHERE Aktiv = 1 ORDER BY Produktname";
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(sql, conn);
                dtProdukte = new DataTable();
                adapter.Fill(dtProdukte);
                conn.Close();

                dgvProdukte.DataSource = dtProdukte;

                if (dgvProdukte.Columns.Count > 0)
                {
                    dgvProdukte.Columns["Id"].Visible = false;
                    dgvProdukte.Columns["Artikelnummer"].HeaderText = "Art.-Nr.";
                    dgvProdukte.Columns["Artikelnummer"].Width = 80;
                    dgvProdukte.Columns["Produktname"].HeaderText = "Bezeichnung";
                    dgvProdukte.Columns["Produktname"].Width = 200;
                    dgvProdukte.Columns["Einheit"].HeaderText = "Einheit";
                    dgvProdukte.Columns["Einheit"].Width = 60;
                    dgvProdukte.Columns["VKPreis"].HeaderText = "Preis";
                    dgvProdukte.Columns["VKPreis"].Width = 80;
                    dgvProdukte.Columns["VKPreis"].DefaultCellStyle.Format = "N2";
                    dgvProdukte.Columns["Lagerbestand"].HeaderText = "Bestand";
                    dgvProdukte.Columns["Lagerbestand"].Width = 60;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler: " + ex.Message);
            }
        }

        private void btnSuchen_Click(object sender, EventArgs e)
        {
            string suchtext = txtSuche.Text.Trim();

            try
            {
                string connStr = "Data Source=" + Globals.DbPath + ";Version=3;";
                SQLiteConnection conn = new SQLiteConnection(connStr);
                conn.Open();

                string sql = "SELECT Id, Artikelnummer, Produktname, Einheit, VKPreis, Lagerbestand FROM Produkte WHERE Aktiv = 1";

                if (suchtext != "")
                {
                    sql += " AND (Artikelnummer LIKE '%" + suchtext + "%' OR Produktname LIKE '%" + suchtext + "%')";
                }

                sql += " ORDER BY Produktname";

                SQLiteDataAdapter adapter = new SQLiteDataAdapter(sql, conn);
                dtProdukte = new DataTable();
                adapter.Fill(dtProdukte);
                conn.Close();

                dgvProdukte.DataSource = dtProdukte;
            }
            catch { }
        }

        private void txtSuche_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                btnSuchen_Click(sender, e);
            }
        }

        private void dgvProdukte_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                UebernehmeAuswahl();
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            UebernehmeAuswahl();
        }

        private void UebernehmeAuswahl()
        {
            if (dgvProdukte.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dgvProdukte.SelectedRows[0];

                AusgewaehltesProduktId = Convert.ToInt32(row.Cells["Id"].Value);
                AusgewaehlteArtikelNr = row.Cells["Artikelnummer"].Value.ToString();
                AusgewaehlteBezeichnung = row.Cells["Produktname"].Value.ToString();
                AusgewaehlteEinheit = row.Cells["Einheit"].Value?.ToString() ?? "Stk";
                AusgewaehlterPreis = Convert.ToDecimal(row.Cells["VKPreis"].Value ?? 0);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Bitte wählen Sie ein Produkt aus!");
            }
        }

        private void btnAbbrechen_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void InitializeComponent()
        {
            this.dgvProdukte = new System.Windows.Forms.DataGridView();
            this.panelTop = new System.Windows.Forms.Panel();
            this.btnSuchen = new System.Windows.Forms.Button();
            this.txtSuche = new System.Windows.Forms.TextBox();
            this.lblSuche = new System.Windows.Forms.Label();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.btnAbbrechen = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProdukte)).BeginInit();
            this.panelTop.SuspendLayout();
            this.panelBottom.SuspendLayout();
            this.SuspendLayout();
            // dgvProdukte
            this.dgvProdukte.AllowUserToAddRows = false;
            this.dgvProdukte.AllowUserToDeleteRows = false;
            this.dgvProdukte.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvProdukte.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvProdukte.Location = new System.Drawing.Point(0, 40);
            this.dgvProdukte.MultiSelect = false;
            this.dgvProdukte.Name = "dgvProdukte";
            this.dgvProdukte.ReadOnly = true;
            this.dgvProdukte.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvProdukte.Size = new System.Drawing.Size(584, 311);
            this.dgvProdukte.TabIndex = 0;
            this.dgvProdukte.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvProdukte_CellDoubleClick);
            // panelTop
            this.panelTop.Controls.Add(this.btnSuchen);
            this.panelTop.Controls.Add(this.txtSuche);
            this.panelTop.Controls.Add(this.lblSuche);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Size = new System.Drawing.Size(584, 40);
            // lblSuche
            this.lblSuche.AutoSize = true;
            this.lblSuche.Location = new System.Drawing.Point(10, 12);
            this.lblSuche.Text = "Suche:";
            // txtSuche
            this.txtSuche.Location = new System.Drawing.Point(60, 9);
            this.txtSuche.Size = new System.Drawing.Size(400, 23);
            this.txtSuche.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSuche_KeyPress);
            // btnSuchen
            this.btnSuchen.Location = new System.Drawing.Point(470, 8);
            this.btnSuchen.Size = new System.Drawing.Size(75, 23);
            this.btnSuchen.Text = "Suchen";
            this.btnSuchen.Click += new System.EventHandler(this.btnSuchen_Click);
            // panelBottom
            this.panelBottom.Controls.Add(this.btnAbbrechen);
            this.panelBottom.Controls.Add(this.btnOK);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 351);
            this.panelBottom.Size = new System.Drawing.Size(584, 50);
            // btnOK
            this.btnOK.Location = new System.Drawing.Point(350, 10);
            this.btnOK.Size = new System.Drawing.Size(100, 30);
            this.btnOK.Text = "Übernehmen";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // btnAbbrechen
            this.btnAbbrechen.Location = new System.Drawing.Point(460, 10);
            this.btnAbbrechen.Size = new System.Drawing.Size(100, 30);
            this.btnAbbrechen.Text = "Abbrechen";
            this.btnAbbrechen.Click += new System.EventHandler(this.btnAbbrechen_Click);
            // FrmProduktAuswahl
            this.ClientSize = new System.Drawing.Size(584, 401);
            this.Controls.Add(this.dgvProdukte);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.panelBottom);
            this.Name = "FrmProduktAuswahl";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Produkt auswählen";
            this.Load += new System.EventHandler(this.FrmProduktAuswahl_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvProdukte)).EndInit();
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.panelBottom.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.DataGridView dgvProdukte;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Button btnSuchen;
        private System.Windows.Forms.TextBox txtSuche;
        private System.Windows.Forms.Label lblSuche;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Button btnAbbrechen;
        private System.Windows.Forms.Button btnOK;
    }
}
