namespace Hotel_Zormat
{
    partial class FrmBitacora
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dgvBitacora = new System.Windows.Forms.DataGridView();
            this.cboFiltroAccion = new System.Windows.Forms.ComboBox();
            this.dtpFiltroFecha = new System.Windows.Forms.DateTimePicker();
            this.flpFiltrosBitacora = new System.Windows.Forms.FlowLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBitacora)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvBitacora
            // 
            this.dgvBitacora.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvBitacora.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBitacora.Location = new System.Drawing.Point(0, 80);
            this.dgvBitacora.Name = "dgvBitacora";
            this.dgvBitacora.Size = new System.Drawing.Size(788, 358);
            this.dgvBitacora.TabIndex = 0;
            // 
            // cboFiltroAccion
            // 
            this.cboFiltroAccion.FormattingEnabled = true;
            this.cboFiltroAccion.Location = new System.Drawing.Point(102, 11);
            this.cboFiltroAccion.Name = "cboFiltroAccion";
            this.cboFiltroAccion.Size = new System.Drawing.Size(121, 21);
            this.cboFiltroAccion.TabIndex = 1;
            // 
            // dtpFiltroFecha
            // 
            this.dtpFiltroFecha.Location = new System.Drawing.Point(323, 12);
            this.dtpFiltroFecha.Name = "dtpFiltroFecha";
            this.dtpFiltroFecha.Size = new System.Drawing.Size(200, 20);
            this.dtpFiltroFecha.TabIndex = 2;
            // 
            // flpFiltrosBitacora
            // 
            this.flpFiltrosBitacora.Dock = System.Windows.Forms.DockStyle.Top;
            this.flpFiltrosBitacora.Location = new System.Drawing.Point(0, 0);
            this.flpFiltrosBitacora.Name = "flpFiltrosBitacora";
            this.flpFiltrosBitacora.Size = new System.Drawing.Size(800, 60);
            this.flpFiltrosBitacora.TabIndex = 3;
            // 
            // FrmBitacora
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.flpFiltrosBitacora);
            this.Controls.Add(this.dtpFiltroFecha);
            this.Controls.Add(this.cboFiltroAccion);
            this.Controls.Add(this.dgvBitacora);
            this.Name = "FrmBitacora";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.FrmBitacora_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvBitacora)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvBitacora;
        private System.Windows.Forms.ComboBox cboFiltroAccion;
        private System.Windows.Forms.DateTimePicker dtpFiltroFecha;
        private System.Windows.Forms.FlowLayoutPanel flpFiltrosBitacora;
    }
}