namespace EnterpriseApp.Forms
{
    partial class FrmDruckvorschau
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
            this.panelToolbar = new System.Windows.Forms.Panel();
            this.cboZoom = new System.Windows.Forms.ComboBox();
            this.lblZoom = new System.Windows.Forms.Label();
            this.btnExportPDF = new System.Windows.Forms.Button();
            this.btnDrucken = new System.Windows.Forms.Button();
            this.btnSchliessen = new System.Windows.Forms.Button();
            this.panelPreview = new System.Windows.Forms.Panel();
            this.rtbVorschau = new System.Windows.Forms.RichTextBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.panelToolbar.SuspendLayout();
            this.panelPreview.SuspendLayout();
            this.SuspendLayout();
            //
            // panelToolbar
            //
            this.panelToolbar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelToolbar.Controls.Add(this.cboZoom);
            this.panelToolbar.Controls.Add(this.lblZoom);
            this.panelToolbar.Controls.Add(this.btnExportPDF);
            this.panelToolbar.Controls.Add(this.btnDrucken);
            this.panelToolbar.Controls.Add(this.btnSchliessen);
            this.panelToolbar.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelToolbar.Location = new System.Drawing.Point(0, 0);
            this.panelToolbar.Name = "panelToolbar";
            this.panelToolbar.Size = new System.Drawing.Size(784, 50);
            this.panelToolbar.TabIndex = 0;
            //
            // cboZoom
            //
            this.cboZoom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboZoom.FormattingEnabled = true;
            this.cboZoom.Items.AddRange(new object[] {
            "50%",
            "75%",
            "100%",
            "125%",
            "150%",
            "200%"});
            this.cboZoom.Location = new System.Drawing.Point(390, 13);
            this.cboZoom.Name = "cboZoom";
            this.cboZoom.Size = new System.Drawing.Size(80, 23);
            this.cboZoom.TabIndex = 4;
            this.cboZoom.SelectedIndexChanged += new System.EventHandler(this.cboZoom_SelectedIndexChanged);
            //
            // lblZoom
            //
            this.lblZoom.AutoSize = true;
            this.lblZoom.Location = new System.Drawing.Point(345, 16);
            this.lblZoom.Name = "lblZoom";
            this.lblZoom.Size = new System.Drawing.Size(42, 15);
            this.lblZoom.TabIndex = 3;
            this.lblZoom.Text = "Zoom:";
            //
            // btnExportPDF
            //
            this.btnExportPDF.Location = new System.Drawing.Point(230, 10);
            this.btnExportPDF.Name = "btnExportPDF";
            this.btnExportPDF.Size = new System.Drawing.Size(100, 30);
            this.btnExportPDF.TabIndex = 2;
            this.btnExportPDF.Text = "Export PDF";
            this.btnExportPDF.UseVisualStyleBackColor = true;
            this.btnExportPDF.Click += new System.EventHandler(this.btnExportPDF_Click);
            //
            // btnDrucken
            //
            this.btnDrucken.Location = new System.Drawing.Point(120, 10);
            this.btnDrucken.Name = "btnDrucken";
            this.btnDrucken.Size = new System.Drawing.Size(100, 30);
            this.btnDrucken.TabIndex = 1;
            this.btnDrucken.Text = "Drucken";
            this.btnDrucken.UseVisualStyleBackColor = true;
            this.btnDrucken.Click += new System.EventHandler(this.btnDrucken_Click);
            //
            // btnSchliessen
            //
            this.btnSchliessen.Location = new System.Drawing.Point(10, 10);
            this.btnSchliessen.Name = "btnSchliessen";
            this.btnSchliessen.Size = new System.Drawing.Size(100, 30);
            this.btnSchliessen.TabIndex = 0;
            this.btnSchliessen.Text = "Schließen";
            this.btnSchliessen.UseVisualStyleBackColor = true;
            this.btnSchliessen.Click += new System.EventHandler(this.btnSchliessen_Click);
            //
            // panelPreview
            //
            this.panelPreview.AutoScroll = true;
            this.panelPreview.BackColor = System.Drawing.Color.Gray;
            this.panelPreview.Controls.Add(this.rtbVorschau);
            this.panelPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelPreview.Location = new System.Drawing.Point(0, 50);
            this.panelPreview.Name = "panelPreview";
            this.panelPreview.Padding = new System.Windows.Forms.Padding(20);
            this.panelPreview.Size = new System.Drawing.Size(784, 491);
            this.panelPreview.TabIndex = 1;
            //
            // rtbVorschau
            //
            this.rtbVorschau.BackColor = System.Drawing.Color.White;
            this.rtbVorschau.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbVorschau.Font = new System.Drawing.Font("Consolas", 10F);
            this.rtbVorschau.Location = new System.Drawing.Point(20, 20);
            this.rtbVorschau.Name = "rtbVorschau";
            this.rtbVorschau.ReadOnly = true;
            this.rtbVorschau.Size = new System.Drawing.Size(744, 451);
            this.rtbVorschau.TabIndex = 0;
            this.rtbVorschau.Text = "";
            //
            // lblStatus
            //
            this.lblStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblStatus.Location = new System.Drawing.Point(0, 541);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(784, 20);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.Text = "Bereit";
            //
            // FrmDruckvorschau
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.panelPreview);
            this.Controls.Add(this.panelToolbar);
            this.Controls.Add(this.lblStatus);
            this.Name = "FrmDruckvorschau";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Druckvorschau";
            this.Load += new System.EventHandler(this.FrmDruckvorschau_Load);
            this.panelToolbar.ResumeLayout(false);
            this.panelToolbar.PerformLayout();
            this.panelPreview.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel panelToolbar;
        private System.Windows.Forms.ComboBox cboZoom;
        private System.Windows.Forms.Label lblZoom;
        private System.Windows.Forms.Button btnExportPDF;
        private System.Windows.Forms.Button btnDrucken;
        private System.Windows.Forms.Button btnSchliessen;
        private System.Windows.Forms.Panel panelPreview;
        private System.Windows.Forms.RichTextBox rtbVorschau;
        private System.Windows.Forms.Label lblStatus;
    }
}
