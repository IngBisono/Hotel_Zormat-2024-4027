namespace Hotel_Zormat
{
    partial class FrmNuevoHuesped
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
            this.flpNuevoHuesped = new System.Windows.Forms.FlowLayoutPanel();
            this.cboTipoDocumento = new System.Windows.Forms.ComboBox();
            this.txtNumeroDocumento = new System.Windows.Forms.MaskedTextBox();
            this.txtNombre = new System.Windows.Forms.MaskedTextBox();
            this.txtApellido = new System.Windows.Forms.MaskedTextBox();
            this.txtTelefono = new System.Windows.Forms.MaskedTextBox();
            this.txtEmail = new System.Windows.Forms.MaskedTextBox();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.flpNuevoHuesped.SuspendLayout();
            this.SuspendLayout();
            //
            // flpNuevoHuesped
            //
            this.flpNuevoHuesped.Controls.Add(this.cboTipoDocumento);
            this.flpNuevoHuesped.Controls.Add(this.txtNumeroDocumento);
            this.flpNuevoHuesped.Controls.Add(this.txtNombre);
            this.flpNuevoHuesped.Controls.Add(this.txtApellido);
            this.flpNuevoHuesped.Controls.Add(this.txtTelefono);
            this.flpNuevoHuesped.Controls.Add(this.txtEmail);
            this.flpNuevoHuesped.Controls.Add(this.btnGuardar);
            this.flpNuevoHuesped.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpNuevoHuesped.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flpNuevoHuesped.Location = new System.Drawing.Point(0, 0);
            this.flpNuevoHuesped.Name = "flpNuevoHuesped";
            this.flpNuevoHuesped.Size = new System.Drawing.Size(480, 480);
            this.flpNuevoHuesped.TabIndex = 0;
            //
            // cboTipoDocumento
            //
            this.cboTipoDocumento.FormattingEnabled = true;
            this.cboTipoDocumento.Location = new System.Drawing.Point(3, 3);
            this.cboTipoDocumento.Name = "cboTipoDocumento";
            this.cboTipoDocumento.Size = new System.Drawing.Size(121, 21);
            this.cboTipoDocumento.TabIndex = 0;
            //
            // txtNumeroDocumento
            //
            this.txtNumeroDocumento.Location = new System.Drawing.Point(3, 30);
            this.txtNumeroDocumento.Name = "txtNumeroDocumento";
            this.txtNumeroDocumento.Size = new System.Drawing.Size(100, 20);
            this.txtNumeroDocumento.TabIndex = 1;
            //
            // txtNombre
            //
            this.txtNombre.Location = new System.Drawing.Point(3, 56);
            this.txtNombre.Name = "txtNombre";
            this.txtNombre.Size = new System.Drawing.Size(100, 20);
            this.txtNombre.TabIndex = 2;
            //
            // txtApellido
            //
            this.txtApellido.Location = new System.Drawing.Point(3, 82);
            this.txtApellido.Name = "txtApellido";
            this.txtApellido.Size = new System.Drawing.Size(100, 20);
            this.txtApellido.TabIndex = 3;
            //
            // txtTelefono
            //
            this.txtTelefono.Location = new System.Drawing.Point(3, 108);
            this.txtTelefono.Name = "txtTelefono";
            this.txtTelefono.Size = new System.Drawing.Size(100, 20);
            this.txtTelefono.TabIndex = 4;
            //
            // txtEmail
            //
            this.txtEmail.Location = new System.Drawing.Point(3, 134);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(100, 20);
            this.txtEmail.TabIndex = 5;
            //
            // btnGuardar
            //
            this.btnGuardar.Location = new System.Drawing.Point(3, 160);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(75, 23);
            this.btnGuardar.TabIndex = 6;
            this.btnGuardar.Text = "button1";
            this.btnGuardar.UseVisualStyleBackColor = true;
            //
            // FrmNuevoHuesped
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(480, 480);
            this.Controls.Add(this.flpNuevoHuesped);
            this.Name = "FrmNuevoHuesped";
            this.Text = "FrmNuevoHuesped";
            this.flpNuevoHuesped.ResumeLayout(false);
            this.flpNuevoHuesped.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flpNuevoHuesped;
        private System.Windows.Forms.ComboBox cboTipoDocumento;
        private System.Windows.Forms.MaskedTextBox txtNumeroDocumento;
        private System.Windows.Forms.MaskedTextBox txtNombre;
        private System.Windows.Forms.MaskedTextBox txtApellido;
        private System.Windows.Forms.MaskedTextBox txtTelefono;
        private System.Windows.Forms.MaskedTextBox txtEmail;
        private System.Windows.Forms.Button btnGuardar;
    }
}
