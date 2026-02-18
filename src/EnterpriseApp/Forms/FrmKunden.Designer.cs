namespace EnterpriseApp.Forms
{
    partial class FrmKunden
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dgvKunden = new System.Windows.Forms.DataGridView();
            this.panelSearch = new System.Windows.Forms.Panel();
            this.btnSuchen = new System.Windows.Forms.Button();
            this.txtSuche = new System.Windows.Forms.TextBox();
            this.lblSuche = new System.Windows.Forms.Label();
            this.panelDetail = new System.Windows.Forms.Panel();
            this.txtBemerkung = new System.Windows.Forms.TextBox();
            this.lblBemerkung = new System.Windows.Forms.Label();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.lblEmail = new System.Windows.Forms.Label();
            this.txtTelefon = new System.Windows.Forms.TextBox();
            this.lblTelefon = new System.Windows.Forms.Label();
            this.cboLand = new System.Windows.Forms.ComboBox();
            this.lblLand = new System.Windows.Forms.Label();
            this.txtOrt = new System.Windows.Forms.TextBox();
            this.lblOrt = new System.Windows.Forms.Label();
            this.txtPLZ = new System.Windows.Forms.TextBox();
            this.lblPLZ = new System.Windows.Forms.Label();
            this.txtStrasse = new System.Windows.Forms.TextBox();
            this.lblStrasse = new System.Windows.Forms.Label();
            this.txtNachname = new System.Windows.Forms.TextBox();
            this.lblNachname = new System.Windows.Forms.Label();
            this.txtVorname = new System.Windows.Forms.TextBox();
            this.lblVorname = new System.Windows.Forms.Label();
            this.cboAnrede = new System.Windows.Forms.ComboBox();
            this.lblAnrede = new System.Windows.Forms.Label();
            this.txtFirma = new System.Windows.Forms.TextBox();
            this.lblFirma = new System.Windows.Forms.Label();
            this.txtKundenNr = new System.Windows.Forms.TextBox();
            this.lblKundenNr = new System.Windows.Forms.Label();
            this.panelButtons = new System.Windows.Forms.Panel();
            this.btnSchliessen = new System.Windows.Forms.Button();
            this.btnLoeschen = new System.Windows.Forms.Button();
            this.btnSpeichern = new System.Windows.Forms.Button();
            this.btnNeu = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvKunden)).BeginInit();
            this.panelSearch.SuspendLayout();
            this.panelDetail.SuspendLayout();
            this.panelButtons.SuspendLayout();
            this.SuspendLayout();
            //
            // splitContainer1
            //
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Panel1.Controls.Add(this.dgvKunden);
            this.splitContainer1.Panel1.Controls.Add(this.panelSearch);
            this.splitContainer1.Panel2.Controls.Add(this.panelDetail);
            this.splitContainer1.Panel2.Controls.Add(this.panelButtons);
            this.splitContainer1.Panel2.Controls.Add(this.lblStatus);
            this.splitContainer1.Size = new System.Drawing.Size(984, 561);
            this.splitContainer1.SplitterDistance = 450;
            this.splitContainer1.TabIndex = 0;
            //
            // dgvKunden
            //
            this.dgvKunden.AllowUserToAddRows = false;
            this.dgvKunden.AllowUserToDeleteRows = false;
            this.dgvKunden.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvKunden.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvKunden.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvKunden.Location = new System.Drawing.Point(0, 40);
            this.dgvKunden.MultiSelect = false;
            this.dgvKunden.Name = "dgvKunden";
            this.dgvKunden.ReadOnly = true;
            this.dgvKunden.RowTemplate.Height = 25;
            this.dgvKunden.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvKunden.Size = new System.Drawing.Size(450, 521);
            this.dgvKunden.TabIndex = 1;
            this.dgvKunden.SelectionChanged += new System.EventHandler(this.dgvKunden_SelectionChanged);
            this.dgvKunden.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvKunden_CellDoubleClick);
            //
            // panelSearch
            //
            this.panelSearch.Controls.Add(this.btnSuchen);
            this.panelSearch.Controls.Add(this.txtSuche);
            this.panelSearch.Controls.Add(this.lblSuche);
            this.panelSearch.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelSearch.Location = new System.Drawing.Point(0, 0);
            this.panelSearch.Name = "panelSearch";
            this.panelSearch.Size = new System.Drawing.Size(450, 40);
            this.panelSearch.TabIndex = 0;
            //
            // btnSuchen
            //
            this.btnSuchen.Location = new System.Drawing.Point(350, 8);
            this.btnSuchen.Name = "btnSuchen";
            this.btnSuchen.Size = new System.Drawing.Size(75, 23);
            this.btnSuchen.TabIndex = 2;
            this.btnSuchen.Text = "Suchen";
            this.btnSuchen.UseVisualStyleBackColor = true;
            this.btnSuchen.Click += new System.EventHandler(this.btnSuchen_Click);
            //
            // txtSuche
            //
            this.txtSuche.Location = new System.Drawing.Point(60, 9);
            this.txtSuche.Name = "txtSuche";
            this.txtSuche.Size = new System.Drawing.Size(280, 23);
            this.txtSuche.TabIndex = 1;
            this.txtSuche.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSuche_KeyPress);
            //
            // lblSuche
            //
            this.lblSuche.AutoSize = true;
            this.lblSuche.Location = new System.Drawing.Point(10, 12);
            this.lblSuche.Name = "lblSuche";
            this.lblSuche.Size = new System.Drawing.Size(44, 15);
            this.lblSuche.TabIndex = 0;
            this.lblSuche.Text = "Suche:";
            //
            // panelDetail
            //
            this.panelDetail.AutoScroll = true;
            this.panelDetail.Controls.Add(this.txtBemerkung);
            this.panelDetail.Controls.Add(this.lblBemerkung);
            this.panelDetail.Controls.Add(this.txtEmail);
            this.panelDetail.Controls.Add(this.lblEmail);
            this.panelDetail.Controls.Add(this.txtTelefon);
            this.panelDetail.Controls.Add(this.lblTelefon);
            this.panelDetail.Controls.Add(this.cboLand);
            this.panelDetail.Controls.Add(this.lblLand);
            this.panelDetail.Controls.Add(this.txtOrt);
            this.panelDetail.Controls.Add(this.lblOrt);
            this.panelDetail.Controls.Add(this.txtPLZ);
            this.panelDetail.Controls.Add(this.lblPLZ);
            this.panelDetail.Controls.Add(this.txtStrasse);
            this.panelDetail.Controls.Add(this.lblStrasse);
            this.panelDetail.Controls.Add(this.txtNachname);
            this.panelDetail.Controls.Add(this.lblNachname);
            this.panelDetail.Controls.Add(this.txtVorname);
            this.panelDetail.Controls.Add(this.lblVorname);
            this.panelDetail.Controls.Add(this.cboAnrede);
            this.panelDetail.Controls.Add(this.lblAnrede);
            this.panelDetail.Controls.Add(this.txtFirma);
            this.panelDetail.Controls.Add(this.lblFirma);
            this.panelDetail.Controls.Add(this.txtKundenNr);
            this.panelDetail.Controls.Add(this.lblKundenNr);
            this.panelDetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelDetail.Location = new System.Drawing.Point(0, 0);
            this.panelDetail.Name = "panelDetail";
            this.panelDetail.Size = new System.Drawing.Size(530, 481);
            this.panelDetail.TabIndex = 0;
            //
            // txtBemerkung
            //
            this.txtBemerkung.Location = new System.Drawing.Point(100, 380);
            this.txtBemerkung.Multiline = true;
            this.txtBemerkung.Name = "txtBemerkung";
            this.txtBemerkung.Size = new System.Drawing.Size(400, 80);
            this.txtBemerkung.TabIndex = 23;
            //
            // lblBemerkung
            //
            this.lblBemerkung.AutoSize = true;
            this.lblBemerkung.Location = new System.Drawing.Point(20, 383);
            this.lblBemerkung.Name = "lblBemerkung";
            this.lblBemerkung.Size = new System.Drawing.Size(69, 15);
            this.lblBemerkung.TabIndex = 22;
            this.lblBemerkung.Text = "Bemerkung:";
            //
            // txtEmail
            //
            this.txtEmail.Location = new System.Drawing.Point(100, 350);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(250, 23);
            this.txtEmail.TabIndex = 21;
            //
            // lblEmail
            //
            this.lblEmail.AutoSize = true;
            this.lblEmail.Location = new System.Drawing.Point(20, 353);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(39, 15);
            this.lblEmail.TabIndex = 20;
            this.lblEmail.Text = "E-Mail:";
            //
            // txtTelefon
            //
            this.txtTelefon.Location = new System.Drawing.Point(100, 320);
            this.txtTelefon.Name = "txtTelefon";
            this.txtTelefon.Size = new System.Drawing.Size(200, 23);
            this.txtTelefon.TabIndex = 19;
            //
            // lblTelefon
            //
            this.lblTelefon.AutoSize = true;
            this.lblTelefon.Location = new System.Drawing.Point(20, 323);
            this.lblTelefon.Name = "lblTelefon";
            this.lblTelefon.Size = new System.Drawing.Size(48, 15);
            this.lblTelefon.TabIndex = 18;
            this.lblTelefon.Text = "Telefon:";
            //
            // cboLand
            //
            this.cboLand.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLand.FormattingEnabled = true;
            this.cboLand.Location = new System.Drawing.Point(100, 290);
            this.cboLand.Name = "cboLand";
            this.cboLand.Size = new System.Drawing.Size(200, 23);
            this.cboLand.TabIndex = 17;
            //
            // lblLand
            //
            this.lblLand.AutoSize = true;
            this.lblLand.Location = new System.Drawing.Point(20, 293);
            this.lblLand.Name = "lblLand";
            this.lblLand.Size = new System.Drawing.Size(35, 15);
            this.lblLand.TabIndex = 16;
            this.lblLand.Text = "Land:";
            //
            // txtOrt
            //
            this.txtOrt.Location = new System.Drawing.Point(100, 260);
            this.txtOrt.Name = "txtOrt";
            this.txtOrt.Size = new System.Drawing.Size(250, 23);
            this.txtOrt.TabIndex = 15;
            //
            // lblOrt
            //
            this.lblOrt.AutoSize = true;
            this.lblOrt.Location = new System.Drawing.Point(20, 263);
            this.lblOrt.Name = "lblOrt";
            this.lblOrt.Size = new System.Drawing.Size(26, 15);
            this.lblOrt.TabIndex = 14;
            this.lblOrt.Text = "Ort:";
            //
            // txtPLZ
            //
            this.txtPLZ.Location = new System.Drawing.Point(100, 230);
            this.txtPLZ.MaxLength = 10;
            this.txtPLZ.Name = "txtPLZ";
            this.txtPLZ.Size = new System.Drawing.Size(80, 23);
            this.txtPLZ.TabIndex = 13;
            //
            // lblPLZ
            //
            this.lblPLZ.AutoSize = true;
            this.lblPLZ.Location = new System.Drawing.Point(20, 233);
            this.lblPLZ.Name = "lblPLZ";
            this.lblPLZ.Size = new System.Drawing.Size(29, 15);
            this.lblPLZ.TabIndex = 12;
            this.lblPLZ.Text = "PLZ:";
            //
            // txtStrasse
            //
            this.txtStrasse.Location = new System.Drawing.Point(100, 200);
            this.txtStrasse.Name = "txtStrasse";
            this.txtStrasse.Size = new System.Drawing.Size(300, 23);
            this.txtStrasse.TabIndex = 11;
            //
            // lblStrasse
            //
            this.lblStrasse.AutoSize = true;
            this.lblStrasse.Location = new System.Drawing.Point(20, 203);
            this.lblStrasse.Name = "lblStrasse";
            this.lblStrasse.Size = new System.Drawing.Size(43, 15);
            this.lblStrasse.TabIndex = 10;
            this.lblStrasse.Text = "Straße:";
            //
            // txtNachname
            //
            this.txtNachname.Location = new System.Drawing.Point(100, 140);
            this.txtNachname.Name = "txtNachname";
            this.txtNachname.Size = new System.Drawing.Size(200, 23);
            this.txtNachname.TabIndex = 9;
            //
            // lblNachname
            //
            this.lblNachname.AutoSize = true;
            this.lblNachname.Location = new System.Drawing.Point(20, 143);
            this.lblNachname.Name = "lblNachname";
            this.lblNachname.Size = new System.Drawing.Size(65, 15);
            this.lblNachname.TabIndex = 8;
            this.lblNachname.Text = "Nachname:";
            //
            // txtVorname
            //
            this.txtVorname.Location = new System.Drawing.Point(100, 110);
            this.txtVorname.Name = "txtVorname";
            this.txtVorname.Size = new System.Drawing.Size(200, 23);
            this.txtVorname.TabIndex = 7;
            //
            // lblVorname
            //
            this.lblVorname.AutoSize = true;
            this.lblVorname.Location = new System.Drawing.Point(20, 113);
            this.lblVorname.Name = "lblVorname";
            this.lblVorname.Size = new System.Drawing.Size(54, 15);
            this.lblVorname.TabIndex = 6;
            this.lblVorname.Text = "Vorname:";
            //
            // cboAnrede
            //
            this.cboAnrede.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAnrede.FormattingEnabled = true;
            this.cboAnrede.Location = new System.Drawing.Point(100, 80);
            this.cboAnrede.Name = "cboAnrede";
            this.cboAnrede.Size = new System.Drawing.Size(120, 23);
            this.cboAnrede.TabIndex = 5;
            //
            // lblAnrede
            //
            this.lblAnrede.AutoSize = true;
            this.lblAnrede.Location = new System.Drawing.Point(20, 83);
            this.lblAnrede.Name = "lblAnrede";
            this.lblAnrede.Size = new System.Drawing.Size(46, 15);
            this.lblAnrede.TabIndex = 4;
            this.lblAnrede.Text = "Anrede:";
            //
            // txtFirma
            //
            this.txtFirma.Location = new System.Drawing.Point(100, 50);
            this.txtFirma.Name = "txtFirma";
            this.txtFirma.Size = new System.Drawing.Size(300, 23);
            this.txtFirma.TabIndex = 3;
            //
            // lblFirma
            //
            this.lblFirma.AutoSize = true;
            this.lblFirma.Location = new System.Drawing.Point(20, 53);
            this.lblFirma.Name = "lblFirma";
            this.lblFirma.Size = new System.Drawing.Size(38, 15);
            this.lblFirma.TabIndex = 2;
            this.lblFirma.Text = "Firma:";
            //
            // txtKundenNr
            //
            this.txtKundenNr.Location = new System.Drawing.Point(100, 20);
            this.txtKundenNr.Name = "txtKundenNr";
            this.txtKundenNr.ReadOnly = true;
            this.txtKundenNr.Size = new System.Drawing.Size(100, 23);
            this.txtKundenNr.TabIndex = 1;
            //
            // lblKundenNr
            //
            this.lblKundenNr.AutoSize = true;
            this.lblKundenNr.Location = new System.Drawing.Point(20, 23);
            this.lblKundenNr.Name = "lblKundenNr";
            this.lblKundenNr.Size = new System.Drawing.Size(67, 15);
            this.lblKundenNr.TabIndex = 0;
            this.lblKundenNr.Text = "Kunden-Nr:";
            //
            // panelButtons
            //
            this.panelButtons.Controls.Add(this.btnSchliessen);
            this.panelButtons.Controls.Add(this.btnLoeschen);
            this.panelButtons.Controls.Add(this.btnSpeichern);
            this.panelButtons.Controls.Add(this.btnNeu);
            this.panelButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelButtons.Location = new System.Drawing.Point(0, 511);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Size = new System.Drawing.Size(530, 50);
            this.panelButtons.TabIndex = 1;
            //
            // btnSchliessen
            //
            this.btnSchliessen.Location = new System.Drawing.Point(330, 12);
            this.btnSchliessen.Name = "btnSchliessen";
            this.btnSchliessen.Size = new System.Drawing.Size(100, 30);
            this.btnSchliessen.TabIndex = 3;
            this.btnSchliessen.Text = "Schließen";
            this.btnSchliessen.UseVisualStyleBackColor = true;
            this.btnSchliessen.Click += new System.EventHandler(this.btnSchliessen_Click);
            //
            // btnLoeschen
            //
            this.btnLoeschen.Location = new System.Drawing.Point(220, 12);
            this.btnLoeschen.Name = "btnLoeschen";
            this.btnLoeschen.Size = new System.Drawing.Size(100, 30);
            this.btnLoeschen.TabIndex = 2;
            this.btnLoeschen.Text = "Löschen";
            this.btnLoeschen.UseVisualStyleBackColor = true;
            this.btnLoeschen.Click += new System.EventHandler(this.btnLoeschen_Click);
            //
            // btnSpeichern
            //
            this.btnSpeichern.Location = new System.Drawing.Point(110, 12);
            this.btnSpeichern.Name = "btnSpeichern";
            this.btnSpeichern.Size = new System.Drawing.Size(100, 30);
            this.btnSpeichern.TabIndex = 1;
            this.btnSpeichern.Text = "Speichern";
            this.btnSpeichern.UseVisualStyleBackColor = true;
            this.btnSpeichern.Click += new System.EventHandler(this.btnSpeichern_Click);
            //
            // btnNeu
            //
            this.btnNeu.Location = new System.Drawing.Point(10, 12);
            this.btnNeu.Name = "btnNeu";
            this.btnNeu.Size = new System.Drawing.Size(90, 30);
            this.btnNeu.TabIndex = 0;
            this.btnNeu.Text = "Neu";
            this.btnNeu.UseVisualStyleBackColor = true;
            this.btnNeu.Click += new System.EventHandler(this.btnNeu_Click);
            //
            // lblStatus
            //
            this.lblStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblStatus.Location = new System.Drawing.Point(0, 481);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(530, 30);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.Text = "Bereit";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // FrmKunden
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 561);
            this.Controls.Add(this.splitContainer1);
            this.Name = "FrmKunden";
            this.Text = "Kundenverwaltung";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmKunden_FormClosing);
            this.Load += new System.EventHandler(this.FrmKunden_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvKunden)).EndInit();
            this.panelSearch.ResumeLayout(false);
            this.panelSearch.PerformLayout();
            this.panelDetail.ResumeLayout(false);
            this.panelDetail.PerformLayout();
            this.panelButtons.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView dgvKunden;
        private System.Windows.Forms.Panel panelSearch;
        private System.Windows.Forms.Button btnSuchen;
        private System.Windows.Forms.TextBox txtSuche;
        private System.Windows.Forms.Label lblSuche;
        private System.Windows.Forms.Panel panelDetail;
        private System.Windows.Forms.TextBox txtKundenNr;
        private System.Windows.Forms.Label lblKundenNr;
        private System.Windows.Forms.TextBox txtFirma;
        private System.Windows.Forms.Label lblFirma;
        private System.Windows.Forms.ComboBox cboAnrede;
        private System.Windows.Forms.Label lblAnrede;
        private System.Windows.Forms.TextBox txtVorname;
        private System.Windows.Forms.Label lblVorname;
        private System.Windows.Forms.TextBox txtNachname;
        private System.Windows.Forms.Label lblNachname;
        private System.Windows.Forms.TextBox txtStrasse;
        private System.Windows.Forms.Label lblStrasse;
        private System.Windows.Forms.TextBox txtPLZ;
        private System.Windows.Forms.Label lblPLZ;
        private System.Windows.Forms.TextBox txtOrt;
        private System.Windows.Forms.Label lblOrt;
        private System.Windows.Forms.ComboBox cboLand;
        private System.Windows.Forms.Label lblLand;
        private System.Windows.Forms.TextBox txtTelefon;
        private System.Windows.Forms.Label lblTelefon;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.Label lblEmail;
        private System.Windows.Forms.TextBox txtBemerkung;
        private System.Windows.Forms.Label lblBemerkung;
        private System.Windows.Forms.Panel panelButtons;
        private System.Windows.Forms.Button btnSchliessen;
        private System.Windows.Forms.Button btnLoeschen;
        private System.Windows.Forms.Button btnSpeichern;
        private System.Windows.Forms.Button btnNeu;
        private System.Windows.Forms.Label lblStatus;
    }
}
