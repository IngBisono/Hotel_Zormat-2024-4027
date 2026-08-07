namespace Hotel_Zormat
{
    partial class FrmDashboardHabitaciones
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
            this.components = new System.ComponentModel.Container();
            this.pnlTablero = new System.Windows.Forms.FlowLayoutPanel();
            this.cboFiltroPiso = new System.Windows.Forms.ComboBox();
            this.cboFiltroEstado = new System.Windows.Forms.ComboBox();
            this.btnActualizar = new System.Windows.Forms.Button();
            this.lblLeyenda = new System.Windows.Forms.Label();
            this.menuPrincipal = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.pnlContenedorTablero = new System.Windows.Forms.Panel();
            this.pnlTablero.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlTablero
            // 
            this.pnlTablero.AutoScroll = true;
            this.pnlTablero.Controls.Add(this.cboFiltroPiso);
            this.pnlTablero.Controls.Add(this.cboFiltroEstado);
            this.pnlTablero.Controls.Add(this.btnActualizar);
            this.pnlTablero.Controls.Add(this.lblLeyenda);
            this.pnlTablero.Controls.Add(this.pnlContenedorTablero);
            this.pnlTablero.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlTablero.Location = new System.Drawing.Point(0, 0);
            this.pnlTablero.Name = "pnlTablero";
            this.pnlTablero.Size = new System.Drawing.Size(800, 450);
            this.pnlTablero.TabIndex = 0;
            // 
            // cboFiltroPiso
            // 
            this.cboFiltroPiso.FormattingEnabled = true;
            this.cboFiltroPiso.Location = new System.Drawing.Point(3, 3);
            this.cboFiltroPiso.Name = "cboFiltroPiso";
            this.cboFiltroPiso.Size = new System.Drawing.Size(121, 21);
            this.cboFiltroPiso.TabIndex = 0;
            // 
            // cboFiltroEstado
            // 
            this.cboFiltroEstado.FormattingEnabled = true;
            this.cboFiltroEstado.Location = new System.Drawing.Point(130, 3);
            this.cboFiltroEstado.Name = "cboFiltroEstado";
            this.cboFiltroEstado.Size = new System.Drawing.Size(121, 21);
            this.cboFiltroEstado.TabIndex = 1;
            // 
            // btnActualizar
            // 
            this.btnActualizar.Location = new System.Drawing.Point(257, 3);
            this.btnActualizar.Name = "btnActualizar";
            this.btnActualizar.Size = new System.Drawing.Size(75, 23);
            this.btnActualizar.TabIndex = 2;
            this.btnActualizar.Text = "button1";
            this.btnActualizar.UseVisualStyleBackColor = true;
            // 
            // lblLeyenda
            // 
            this.lblLeyenda.AutoSize = true;
            this.lblLeyenda.Location = new System.Drawing.Point(338, 0);
            this.lblLeyenda.Name = "lblLeyenda";
            this.lblLeyenda.Size = new System.Drawing.Size(35, 13);
            this.lblLeyenda.TabIndex = 3;
            this.lblLeyenda.Text = "label1";
            // 
            // menuPrincipal
            // 
            this.menuPrincipal.Name = "menuPrincipal";
            this.menuPrincipal.Size = new System.Drawing.Size(61, 4);
            // 
            // pnlContenedorTablero
            // 
            this.pnlContenedorTablero.Location = new System.Drawing.Point(379, 3);
            this.pnlContenedorTablero.Name = "pnlContenedorTablero";
            this.pnlContenedorTablero.Size = new System.Drawing.Size(200, 100);
            this.pnlContenedorTablero.TabIndex = 4;
            // 
            // FrmDashboardHabitaciones
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.pnlTablero);
            this.Name = "FrmDashboardHabitaciones";
            this.Text = "Form1";
            this.pnlTablero.ResumeLayout(false);
            this.pnlTablero.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel pnlTablero;
        private System.Windows.Forms.ContextMenuStrip menuPrincipal;
        private System.Windows.Forms.ComboBox cboFiltroPiso;
        private System.Windows.Forms.ComboBox cboFiltroEstado;
        private System.Windows.Forms.Button btnActualizar;
        private System.Windows.Forms.Label lblLeyenda;
        private System.Windows.Forms.Panel pnlContenedorTablero;
    }
}