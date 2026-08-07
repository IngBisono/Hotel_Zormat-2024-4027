namespace Hotel_Zormat
{
    partial class FrmReservas
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
            this.flpReservas = new System.Windows.Forms.FlowLayoutPanel();
            this.cboHuesped = new System.Windows.Forms.ComboBox();
            this.cboHabitacion = new System.Windows.Forms.ComboBox();
            this.dtpCheckIn = new System.Windows.Forms.DateTimePicker();
            this.dtpCheckOut = new System.Windows.Forms.DateTimePicker();
            this.cboTemporada = new System.Windows.Forms.ComboBox();
            this.lblNochesCalculadas = new System.Windows.Forms.Label();
            this.lblMontoCalculado = new System.Windows.Forms.Label();
            this.cboEstadoReserva = new System.Windows.Forms.ComboBox();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.dgvReservasProximas = new System.Windows.Forms.DataGridView();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.flpReservas.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReservasProximas)).BeginInit();
            this.SuspendLayout();
            // 
            // flpReservas
            // 
            this.flpReservas.AutoScroll = true;
            this.flpReservas.Controls.Add(this.cboHuesped);
            this.flpReservas.Controls.Add(this.cboHabitacion);
            this.flpReservas.Controls.Add(this.dtpCheckIn);
            this.flpReservas.Controls.Add(this.dtpCheckOut);
            this.flpReservas.Controls.Add(this.cboTemporada);
            this.flpReservas.Controls.Add(this.lblNochesCalculadas);
            this.flpReservas.Controls.Add(this.lblMontoCalculado);
            this.flpReservas.Controls.Add(this.cboEstadoReserva);
            this.flpReservas.Controls.Add(this.btnGuardar);
            this.flpReservas.Controls.Add(this.dgvReservasProximas);
            this.flpReservas.Controls.Add(this.tableLayoutPanel1);
            this.flpReservas.Controls.Add(this.panel1);
            this.flpReservas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpReservas.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flpReservas.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.flpReservas.Location = new System.Drawing.Point(0, 0);
            this.flpReservas.Name = "flpReservas";
            this.flpReservas.Size = new System.Drawing.Size(800, 450);
            this.flpReservas.TabIndex = 0;
            // 
            // cboHuesped
            // 
            this.cboHuesped.FormattingEnabled = true;
            this.cboHuesped.Location = new System.Drawing.Point(3, 3);
            this.cboHuesped.Name = "cboHuesped";
            this.cboHuesped.Size = new System.Drawing.Size(121, 21);
            this.cboHuesped.TabIndex = 0;
            // 
            // cboHabitacion
            // 
            this.cboHabitacion.FormattingEnabled = true;
            this.cboHabitacion.Location = new System.Drawing.Point(3, 30);
            this.cboHabitacion.Name = "cboHabitacion";
            this.cboHabitacion.Size = new System.Drawing.Size(121, 21);
            this.cboHabitacion.TabIndex = 1;
            // 
            // dtpCheckIn
            // 
            this.dtpCheckIn.Location = new System.Drawing.Point(3, 57);
            this.dtpCheckIn.Name = "dtpCheckIn";
            this.dtpCheckIn.Size = new System.Drawing.Size(200, 20);
            this.dtpCheckIn.TabIndex = 2;
            // 
            // dtpCheckOut
            // 
            this.dtpCheckOut.Location = new System.Drawing.Point(3, 83);
            this.dtpCheckOut.Name = "dtpCheckOut";
            this.dtpCheckOut.Size = new System.Drawing.Size(200, 20);
            this.dtpCheckOut.TabIndex = 3;
            // 
            // cboTemporada
            // 
            this.cboTemporada.FormattingEnabled = true;
            this.cboTemporada.Location = new System.Drawing.Point(3, 109);
            this.cboTemporada.Name = "cboTemporada";
            this.cboTemporada.Size = new System.Drawing.Size(121, 21);
            this.cboTemporada.TabIndex = 4;
            // 
            // lblNochesCalculadas
            // 
            this.lblNochesCalculadas.AutoSize = true;
            this.lblNochesCalculadas.Location = new System.Drawing.Point(3, 133);
            this.lblNochesCalculadas.Name = "lblNochesCalculadas";
            this.lblNochesCalculadas.Size = new System.Drawing.Size(35, 13);
            this.lblNochesCalculadas.TabIndex = 5;
            this.lblNochesCalculadas.Text = "label1";
            // 
            // lblMontoCalculado
            // 
            this.lblMontoCalculado.AutoSize = true;
            this.lblMontoCalculado.Location = new System.Drawing.Point(3, 146);
            this.lblMontoCalculado.Name = "lblMontoCalculado";
            this.lblMontoCalculado.Size = new System.Drawing.Size(35, 13);
            this.lblMontoCalculado.TabIndex = 6;
            this.lblMontoCalculado.Text = "label1";
            // 
            // cboEstadoReserva
            // 
            this.cboEstadoReserva.FormattingEnabled = true;
            this.cboEstadoReserva.Location = new System.Drawing.Point(3, 162);
            this.cboEstadoReserva.Name = "cboEstadoReserva";
            this.cboEstadoReserva.Size = new System.Drawing.Size(121, 21);
            this.cboEstadoReserva.TabIndex = 7;
            // 
            // btnGuardar
            // 
            this.btnGuardar.Location = new System.Drawing.Point(3, 189);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(75, 23);
            this.btnGuardar.TabIndex = 8;
            this.btnGuardar.Text = "button1";
            this.btnGuardar.UseVisualStyleBackColor = true;
            // 
            // dgvReservasProximas
            // 
            this.dgvReservasProximas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvReservasProximas.Location = new System.Drawing.Point(3, 218);
            this.dgvReservasProximas.Name = "dgvReservasProximas";
            this.dgvReservasProximas.Size = new System.Drawing.Size(240, 150);
            this.dgvReservasProximas.TabIndex = 9;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Location = new System.Drawing.Point(249, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(200, 100);
            this.tableLayoutPanel1.TabIndex = 10;
            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(249, 109);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(200, 100);
            this.panel1.TabIndex = 11;
            // 
            // FrmReservas
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.flpReservas);
            this.Name = "FrmReservas";
            this.Text = "Form1";
            this.flpReservas.ResumeLayout(false);
            this.flpReservas.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReservasProximas)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flpReservas;
        private System.Windows.Forms.ComboBox cboHuesped;
        private System.Windows.Forms.ComboBox cboHabitacion;
        private System.Windows.Forms.DateTimePicker dtpCheckIn;
        private System.Windows.Forms.DateTimePicker dtpCheckOut;
        private System.Windows.Forms.ComboBox cboTemporada;
        private System.Windows.Forms.Label lblNochesCalculadas;
        private System.Windows.Forms.Label lblMontoCalculado;
        private System.Windows.Forms.ComboBox cboEstadoReserva;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.DataGridView dgvReservasProximas;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
    }
}