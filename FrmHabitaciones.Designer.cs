namespace Hotel_Zormat
{
    partial class FrmHabitaciones
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
            this.flpHabitaciones = new System.Windows.Forms.FlowLayoutPanel();
            this.dgvHabitaciones = new System.Windows.Forms.DataGridView();
            this.txtNumero = new System.Windows.Forms.MaskedTextBox();
            this.numPiso = new System.Windows.Forms.NumericUpDown();
            this.cboTipo = new System.Windows.Forms.ComboBox();
            this.cboEstado = new System.Windows.Forms.ComboBox();
            this.txtTarifaBase = new System.Windows.Forms.NumericUpDown();
            this.btnNuevo = new System.Windows.Forms.Button();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnEliminar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.cboFiltroPiso = new System.Windows.Forms.ComboBox();
            this.cboFiltroEstado = new System.Windows.Forms.ComboBox();
            this.numCapacidad = new System.Windows.Forms.NumericUpDown();
            this.flpHabitaciones.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHabitaciones)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPiso)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTarifaBase)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCapacidad)).BeginInit();
            this.SuspendLayout();
            // 
            // flpHabitaciones
            // 
            this.flpHabitaciones.Controls.Add(this.dgvHabitaciones);
            this.flpHabitaciones.Controls.Add(this.txtNumero);
            this.flpHabitaciones.Controls.Add(this.numPiso);
            this.flpHabitaciones.Controls.Add(this.cboTipo);
            this.flpHabitaciones.Controls.Add(this.cboEstado);
            this.flpHabitaciones.Controls.Add(this.txtTarifaBase);
            this.flpHabitaciones.Controls.Add(this.btnNuevo);
            this.flpHabitaciones.Controls.Add(this.btnGuardar);
            this.flpHabitaciones.Controls.Add(this.btnEliminar);
            this.flpHabitaciones.Controls.Add(this.btnCancelar);
            this.flpHabitaciones.Controls.Add(this.cboFiltroPiso);
            this.flpHabitaciones.Controls.Add(this.cboFiltroEstado);
            this.flpHabitaciones.Controls.Add(this.numCapacidad);
            this.flpHabitaciones.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpHabitaciones.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flpHabitaciones.Location = new System.Drawing.Point(0, 0);
            this.flpHabitaciones.Name = "flpHabitaciones";
            this.flpHabitaciones.Size = new System.Drawing.Size(800, 450);
            this.flpHabitaciones.TabIndex = 0;
            // 
            // dgvHabitaciones
            // 
            this.dgvHabitaciones.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvHabitaciones.Location = new System.Drawing.Point(3, 3);
            this.dgvHabitaciones.Name = "dgvHabitaciones";
            this.dgvHabitaciones.Size = new System.Drawing.Size(240, 150);
            this.dgvHabitaciones.TabIndex = 0;
            // 
            // txtNumero
            // 
            this.txtNumero.Location = new System.Drawing.Point(3, 159);
            this.txtNumero.Name = "txtNumero";
            this.txtNumero.Size = new System.Drawing.Size(100, 20);
            this.txtNumero.TabIndex = 1;
            // 
            // numPiso
            // 
            this.numPiso.Location = new System.Drawing.Point(3, 185);
            this.numPiso.Name = "numPiso";
            this.numPiso.Size = new System.Drawing.Size(120, 20);
            this.numPiso.TabIndex = 2;
            // 
            // cboTipo
            // 
            this.cboTipo.FormattingEnabled = true;
            this.cboTipo.Location = new System.Drawing.Point(3, 211);
            this.cboTipo.Name = "cboTipo";
            this.cboTipo.Size = new System.Drawing.Size(121, 21);
            this.cboTipo.TabIndex = 3;
            // 
            // cboEstado
            // 
            this.cboEstado.FormattingEnabled = true;
            this.cboEstado.Location = new System.Drawing.Point(3, 238);
            this.cboEstado.Name = "cboEstado";
            this.cboEstado.Size = new System.Drawing.Size(121, 21);
            this.cboEstado.TabIndex = 4;
            // 
            // txtTarifaBase
            // 
            this.txtTarifaBase.Location = new System.Drawing.Point(3, 265);
            this.txtTarifaBase.Name = "txtTarifaBase";
            this.txtTarifaBase.Size = new System.Drawing.Size(120, 20);
            this.txtTarifaBase.TabIndex = 5;
            // 
            // btnNuevo
            // 
            this.btnNuevo.Location = new System.Drawing.Point(3, 291);
            this.btnNuevo.Name = "btnNuevo";
            this.btnNuevo.Size = new System.Drawing.Size(75, 23);
            this.btnNuevo.TabIndex = 6;
            this.btnNuevo.Text = "button1";
            this.btnNuevo.UseVisualStyleBackColor = true;
            // 
            // btnGuardar
            // 
            this.btnGuardar.Location = new System.Drawing.Point(3, 320);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(75, 23);
            this.btnGuardar.TabIndex = 7;
            this.btnGuardar.Text = "button1";
            this.btnGuardar.UseVisualStyleBackColor = true;
            // 
            // btnEliminar
            // 
            this.btnEliminar.Location = new System.Drawing.Point(3, 349);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(75, 23);
            this.btnEliminar.TabIndex = 8;
            this.btnEliminar.Text = "button1";
            this.btnEliminar.UseVisualStyleBackColor = true;
            // 
            // btnCancelar
            // 
            this.btnCancelar.Location = new System.Drawing.Point(3, 378);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(75, 23);
            this.btnCancelar.TabIndex = 9;
            this.btnCancelar.Text = "button1";
            this.btnCancelar.UseVisualStyleBackColor = true;
            // 
            // cboFiltroPiso
            // 
            this.cboFiltroPiso.FormattingEnabled = true;
            this.cboFiltroPiso.Location = new System.Drawing.Point(3, 407);
            this.cboFiltroPiso.Name = "cboFiltroPiso";
            this.cboFiltroPiso.Size = new System.Drawing.Size(121, 21);
            this.cboFiltroPiso.TabIndex = 10;
            // 
            // cboFiltroEstado
            // 
            this.cboFiltroEstado.FormattingEnabled = true;
            this.cboFiltroEstado.Location = new System.Drawing.Point(249, 3);
            this.cboFiltroEstado.Name = "cboFiltroEstado";
            this.cboFiltroEstado.Size = new System.Drawing.Size(121, 21);
            this.cboFiltroEstado.TabIndex = 11;
            // 
            // numCapacidad
            // 
            this.numCapacidad.Location = new System.Drawing.Point(249, 30);
            this.numCapacidad.Name = "numCapacidad";
            this.numCapacidad.Size = new System.Drawing.Size(120, 20);
            this.numCapacidad.TabIndex = 12;
            // 
            // FrmHabitaciones
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.flpHabitaciones);
            this.Name = "FrmHabitaciones";
            this.Text = "Form1";
            this.flpHabitaciones.ResumeLayout(false);
            this.flpHabitaciones.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHabitaciones)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPiso)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTarifaBase)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCapacidad)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flpHabitaciones;
        private System.Windows.Forms.DataGridView dgvHabitaciones;
        private System.Windows.Forms.MaskedTextBox txtNumero;
        private System.Windows.Forms.NumericUpDown numPiso;
        private System.Windows.Forms.ComboBox cboTipo;
        private System.Windows.Forms.ComboBox cboEstado;
        private System.Windows.Forms.NumericUpDown txtTarifaBase;
        private System.Windows.Forms.Button btnNuevo;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnEliminar;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.ComboBox cboFiltroPiso;
        private System.Windows.Forms.ComboBox cboFiltroEstado;
        private System.Windows.Forms.NumericUpDown numCapacidad;
    }
}