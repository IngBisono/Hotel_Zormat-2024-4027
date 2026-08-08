namespace Hotel_Zormat
{
    partial class FrmNuevaReserva
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
            this.flpNuevaReserva = new System.Windows.Forms.FlowLayoutPanel();
            this.txtHuesped = new System.Windows.Forms.TextBox();
            this.btnBuscarHuesped = new System.Windows.Forms.Button();
            this.cboHabitacion = new System.Windows.Forms.ComboBox();
            this.dtpCheckIn = new System.Windows.Forms.DateTimePicker();
            this.dtpCheckOut = new System.Windows.Forms.DateTimePicker();
            this.cboTemporada = new System.Windows.Forms.ComboBox();
            this.lblNochesCalculadas = new System.Windows.Forms.Label();
            this.lblMontoCalculado = new System.Windows.Forms.Label();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.flpNuevaReserva.SuspendLayout();
            this.SuspendLayout();
            //
            // flpNuevaReserva
            //
            this.flpNuevaReserva.Controls.Add(this.txtHuesped);
            this.flpNuevaReserva.Controls.Add(this.btnBuscarHuesped);
            this.flpNuevaReserva.Controls.Add(this.cboHabitacion);
            this.flpNuevaReserva.Controls.Add(this.dtpCheckIn);
            this.flpNuevaReserva.Controls.Add(this.dtpCheckOut);
            this.flpNuevaReserva.Controls.Add(this.cboTemporada);
            this.flpNuevaReserva.Controls.Add(this.lblNochesCalculadas);
            this.flpNuevaReserva.Controls.Add(this.lblMontoCalculado);
            this.flpNuevaReserva.Controls.Add(this.btnGuardar);
            this.flpNuevaReserva.Controls.Add(this.panel1);
            this.flpNuevaReserva.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpNuevaReserva.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flpNuevaReserva.Location = new System.Drawing.Point(0, 0);
            this.flpNuevaReserva.Name = "flpNuevaReserva";
            this.flpNuevaReserva.Size = new System.Drawing.Size(760, 560);
            this.flpNuevaReserva.TabIndex = 0;
            //
            // txtHuesped
            //
            this.txtHuesped.Location = new System.Drawing.Point(3, 3);
            this.txtHuesped.Name = "txtHuesped";
            this.txtHuesped.Size = new System.Drawing.Size(200, 20);
            this.txtHuesped.TabIndex = 0;
            //
            // btnBuscarHuesped
            //
            this.btnBuscarHuesped.Location = new System.Drawing.Point(3, 29);
            this.btnBuscarHuesped.Name = "btnBuscarHuesped";
            this.btnBuscarHuesped.Size = new System.Drawing.Size(75, 23);
            this.btnBuscarHuesped.TabIndex = 1;
            this.btnBuscarHuesped.Text = "button1";
            this.btnBuscarHuesped.UseVisualStyleBackColor = true;
            //
            // cboHabitacion
            //
            this.cboHabitacion.FormattingEnabled = true;
            this.cboHabitacion.Location = new System.Drawing.Point(3, 58);
            this.cboHabitacion.Name = "cboHabitacion";
            this.cboHabitacion.Size = new System.Drawing.Size(121, 21);
            this.cboHabitacion.TabIndex = 2;
            //
            // dtpCheckIn
            //
            this.dtpCheckIn.Location = new System.Drawing.Point(3, 85);
            this.dtpCheckIn.Name = "dtpCheckIn";
            this.dtpCheckIn.Size = new System.Drawing.Size(200, 20);
            this.dtpCheckIn.TabIndex = 3;
            //
            // dtpCheckOut
            //
            this.dtpCheckOut.Location = new System.Drawing.Point(3, 111);
            this.dtpCheckOut.Name = "dtpCheckOut";
            this.dtpCheckOut.Size = new System.Drawing.Size(200, 20);
            this.dtpCheckOut.TabIndex = 4;
            //
            // cboTemporada
            //
            this.cboTemporada.FormattingEnabled = true;
            this.cboTemporada.Location = new System.Drawing.Point(3, 137);
            this.cboTemporada.Name = "cboTemporada";
            this.cboTemporada.Size = new System.Drawing.Size(121, 21);
            this.cboTemporada.TabIndex = 5;
            //
            // lblNochesCalculadas
            //
            this.lblNochesCalculadas.AutoSize = true;
            this.lblNochesCalculadas.Location = new System.Drawing.Point(3, 161);
            this.lblNochesCalculadas.Name = "lblNochesCalculadas";
            this.lblNochesCalculadas.Size = new System.Drawing.Size(35, 13);
            this.lblNochesCalculadas.TabIndex = 6;
            this.lblNochesCalculadas.Text = "label1";
            //
            // lblMontoCalculado
            //
            this.lblMontoCalculado.AutoSize = true;
            this.lblMontoCalculado.Location = new System.Drawing.Point(3, 174);
            this.lblMontoCalculado.Name = "lblMontoCalculado";
            this.lblMontoCalculado.Size = new System.Drawing.Size(35, 13);
            this.lblMontoCalculado.TabIndex = 7;
            this.lblMontoCalculado.Text = "label1";
            //
            // btnGuardar
            //
            this.btnGuardar.Location = new System.Drawing.Point(3, 190);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(75, 23);
            this.btnGuardar.TabIndex = 8;
            this.btnGuardar.Text = "button1";
            this.btnGuardar.UseVisualStyleBackColor = true;
            //
            // panel1
            //
            this.panel1.Location = new System.Drawing.Point(3, 219);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(200, 100);
            this.panel1.TabIndex = 9;
            //
            // FrmNuevaReserva
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(760, 560);
            this.Controls.Add(this.flpNuevaReserva);
            this.Name = "FrmNuevaReserva";
            this.Text = "FrmNuevaReserva";
            this.flpNuevaReserva.ResumeLayout(false);
            this.flpNuevaReserva.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flpNuevaReserva;
        private System.Windows.Forms.TextBox txtHuesped;
        private System.Windows.Forms.Button btnBuscarHuesped;
        private System.Windows.Forms.ComboBox cboHabitacion;
        private System.Windows.Forms.DateTimePicker dtpCheckIn;
        private System.Windows.Forms.DateTimePicker dtpCheckOut;
        private System.Windows.Forms.ComboBox cboTemporada;
        private System.Windows.Forms.Label lblNochesCalculadas;
        private System.Windows.Forms.Label lblMontoCalculado;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Panel panel1;
    }
}
