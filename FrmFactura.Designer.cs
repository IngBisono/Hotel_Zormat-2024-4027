namespace Hotel_Zormat
{
    partial class FrmFactura
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
            this.flpFactura = new System.Windows.Forms.FlowLayoutPanel();
            this.lblNumeroNCF = new System.Windows.Forms.Label();
            this.lblSubtotal = new System.Windows.Forms.Label();
            this.lblITBIS = new System.Windows.Forms.Label();
            this.lblPropina = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            this.btnCerrar = new System.Windows.Forms.Button();
            this.flpFactura.SuspendLayout();
            this.SuspendLayout();
            // 
            // flpFactura
            // 
            this.flpFactura.Controls.Add(this.lblNumeroNCF);
            this.flpFactura.Controls.Add(this.lblSubtotal);
            this.flpFactura.Controls.Add(this.lblITBIS);
            this.flpFactura.Controls.Add(this.lblPropina);
            this.flpFactura.Controls.Add(this.lblTotal);
            this.flpFactura.Controls.Add(this.btnCerrar);
            this.flpFactura.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpFactura.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flpFactura.Location = new System.Drawing.Point(0, 0);
            this.flpFactura.Name = "flpFactura";
            this.flpFactura.Size = new System.Drawing.Size(800, 450);
            this.flpFactura.TabIndex = 0;
            // 
            // lblNumeroNCF
            // 
            this.lblNumeroNCF.AutoSize = true;
            this.lblNumeroNCF.Location = new System.Drawing.Point(3, 0);
            this.lblNumeroNCF.Name = "lblNumeroNCF";
            this.lblNumeroNCF.Size = new System.Drawing.Size(35, 13);
            this.lblNumeroNCF.TabIndex = 0;
            this.lblNumeroNCF.Text = "label1";
            // 
            // lblSubtotal
            // 
            this.lblSubtotal.AutoSize = true;
            this.lblSubtotal.Location = new System.Drawing.Point(3, 13);
            this.lblSubtotal.Name = "lblSubtotal";
            this.lblSubtotal.Size = new System.Drawing.Size(35, 13);
            this.lblSubtotal.TabIndex = 1;
            this.lblSubtotal.Text = "label1";
            // 
            // lblITBIS
            // 
            this.lblITBIS.AutoSize = true;
            this.lblITBIS.Location = new System.Drawing.Point(3, 26);
            this.lblITBIS.Name = "lblITBIS";
            this.lblITBIS.Size = new System.Drawing.Size(35, 13);
            this.lblITBIS.TabIndex = 2;
            this.lblITBIS.Text = "label1";
            // 
            // lblPropina
            // 
            this.lblPropina.AutoSize = true;
            this.lblPropina.Location = new System.Drawing.Point(3, 39);
            this.lblPropina.Name = "lblPropina";
            this.lblPropina.Size = new System.Drawing.Size(35, 13);
            this.lblPropina.TabIndex = 3;
            this.lblPropina.Text = "label1";
            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = true;
            this.lblTotal.Location = new System.Drawing.Point(3, 52);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(35, 13);
            this.lblTotal.TabIndex = 4;
            this.lblTotal.Text = "label1";
            // 
            // btnCerrar
            // 
            this.btnCerrar.Location = new System.Drawing.Point(3, 68);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(75, 23);
            this.btnCerrar.TabIndex = 5;
            this.btnCerrar.Text = "button1";
            this.btnCerrar.UseVisualStyleBackColor = true;
            // 
            // FrmFactura
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.flpFactura);
            this.Name = "FrmFactura";
            this.Text = "Form2";
            this.flpFactura.ResumeLayout(false);
            this.flpFactura.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flpFactura;
        private System.Windows.Forms.Label lblNumeroNCF;
        private System.Windows.Forms.Label lblSubtotal;
        private System.Windows.Forms.Label lblITBIS;
        private System.Windows.Forms.Label lblPropina;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Button btnCerrar;
    }
}