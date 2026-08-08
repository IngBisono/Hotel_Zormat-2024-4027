namespace Hotel_Zormat
{
    partial class FrmBuscarHuesped
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
            this.flpBuscarHuesped = new System.Windows.Forms.FlowLayoutPanel();
            this.txtBuscar = new System.Windows.Forms.MaskedTextBox();
            this.btnBuscar = new System.Windows.Forms.Button();
            this.dgvHuespedes = new System.Windows.Forms.DataGridView();
            this.btnSeleccionar = new System.Windows.Forms.Button();
            this.flpBuscarHuesped.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHuespedes)).BeginInit();
            this.SuspendLayout();
            //
            // flpBuscarHuesped
            //
            this.flpBuscarHuesped.Controls.Add(this.txtBuscar);
            this.flpBuscarHuesped.Controls.Add(this.btnBuscar);
            this.flpBuscarHuesped.Controls.Add(this.dgvHuespedes);
            this.flpBuscarHuesped.Controls.Add(this.btnSeleccionar);
            this.flpBuscarHuesped.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpBuscarHuesped.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flpBuscarHuesped.Location = new System.Drawing.Point(0, 0);
            this.flpBuscarHuesped.Name = "flpBuscarHuesped";
            this.flpBuscarHuesped.Size = new System.Drawing.Size(640, 480);
            this.flpBuscarHuesped.TabIndex = 0;
            //
            // txtBuscar
            //
            this.txtBuscar.Location = new System.Drawing.Point(3, 3);
            this.txtBuscar.Name = "txtBuscar";
            this.txtBuscar.Size = new System.Drawing.Size(200, 20);
            this.txtBuscar.TabIndex = 0;
            //
            // btnBuscar
            //
            this.btnBuscar.Location = new System.Drawing.Point(3, 29);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(75, 23);
            this.btnBuscar.TabIndex = 1;
            this.btnBuscar.Text = "button1";
            this.btnBuscar.UseVisualStyleBackColor = true;
            //
            // dgvHuespedes
            //
            this.dgvHuespedes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvHuespedes.Location = new System.Drawing.Point(3, 58);
            this.dgvHuespedes.Name = "dgvHuespedes";
            this.dgvHuespedes.Size = new System.Drawing.Size(240, 150);
            this.dgvHuespedes.TabIndex = 2;
            //
            // btnSeleccionar
            //
            this.btnSeleccionar.Location = new System.Drawing.Point(3, 214);
            this.btnSeleccionar.Name = "btnSeleccionar";
            this.btnSeleccionar.Size = new System.Drawing.Size(75, 23);
            this.btnSeleccionar.TabIndex = 3;
            this.btnSeleccionar.Text = "button1";
            this.btnSeleccionar.UseVisualStyleBackColor = true;
            //
            // FrmBuscarHuesped
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(640, 480);
            this.Controls.Add(this.flpBuscarHuesped);
            this.Name = "FrmBuscarHuesped";
            this.Text = "FrmBuscarHuesped";
            this.flpBuscarHuesped.ResumeLayout(false);
            this.flpBuscarHuesped.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHuespedes)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flpBuscarHuesped;
        private System.Windows.Forms.MaskedTextBox txtBuscar;
        private System.Windows.Forms.Button btnBuscar;
        private System.Windows.Forms.DataGridView dgvHuespedes;
        private System.Windows.Forms.Button btnSeleccionar;
    }
}
