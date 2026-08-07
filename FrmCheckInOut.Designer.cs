namespace Hotel_Zormat
{
    partial class FrmCheckInOut
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
            this.flpCheckInOut = new System.Windows.Forms.FlowLayoutPanel();
            this.dgvReservasConfirmadas = new System.Windows.Forms.DataGridView();
            this.btnCheckIn = new System.Windows.Forms.Button();
            this.dgvEstadiasActivas = new System.Windows.Forms.DataGridView();
            this.btnCheckOut = new System.Windows.Forms.Button();
            this.lblEstadoHabitacionActual = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.flpCheckInOut.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReservasConfirmadas)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEstadiasActivas)).BeginInit();
            this.SuspendLayout();
            // 
            // flpCheckInOut
            // 
            this.flpCheckInOut.Controls.Add(this.dgvReservasConfirmadas);
            this.flpCheckInOut.Controls.Add(this.btnCheckIn);
            this.flpCheckInOut.Controls.Add(this.dgvEstadiasActivas);
            this.flpCheckInOut.Controls.Add(this.btnCheckOut);
            this.flpCheckInOut.Controls.Add(this.lblEstadoHabitacionActual);
            this.flpCheckInOut.Controls.Add(this.tableLayoutPanel1);
            this.flpCheckInOut.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpCheckInOut.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flpCheckInOut.Location = new System.Drawing.Point(0, 0);
            this.flpCheckInOut.Name = "flpCheckInOut";
            this.flpCheckInOut.Size = new System.Drawing.Size(800, 450);
            this.flpCheckInOut.TabIndex = 0;
            // 
            // dgvReservasConfirmadas
            // 
            this.dgvReservasConfirmadas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvReservasConfirmadas.Location = new System.Drawing.Point(3, 3);
            this.dgvReservasConfirmadas.Name = "dgvReservasConfirmadas";
            this.dgvReservasConfirmadas.Size = new System.Drawing.Size(240, 150);
            this.dgvReservasConfirmadas.TabIndex = 0;
            // 
            // btnCheckIn
            // 
            this.btnCheckIn.Location = new System.Drawing.Point(3, 159);
            this.btnCheckIn.Name = "btnCheckIn";
            this.btnCheckIn.Size = new System.Drawing.Size(75, 23);
            this.btnCheckIn.TabIndex = 1;
            this.btnCheckIn.Text = "button1";
            this.btnCheckIn.UseVisualStyleBackColor = true;
            // 
            // dgvEstadiasActivas
            // 
            this.dgvEstadiasActivas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvEstadiasActivas.Location = new System.Drawing.Point(3, 188);
            this.dgvEstadiasActivas.Name = "dgvEstadiasActivas";
            this.dgvEstadiasActivas.Size = new System.Drawing.Size(240, 150);
            this.dgvEstadiasActivas.TabIndex = 2;
            // 
            // btnCheckOut
            // 
            this.btnCheckOut.Location = new System.Drawing.Point(3, 344);
            this.btnCheckOut.Name = "btnCheckOut";
            this.btnCheckOut.Size = new System.Drawing.Size(75, 23);
            this.btnCheckOut.TabIndex = 3;
            this.btnCheckOut.Text = "button1";
            this.btnCheckOut.UseVisualStyleBackColor = true;
            // 
            // lblEstadoHabitacionActual
            // 
            this.lblEstadoHabitacionActual.AutoSize = true;
            this.lblEstadoHabitacionActual.Location = new System.Drawing.Point(3, 370);
            this.lblEstadoHabitacionActual.Name = "lblEstadoHabitacionActual";
            this.lblEstadoHabitacionActual.Size = new System.Drawing.Size(35, 13);
            this.lblEstadoHabitacionActual.TabIndex = 4;
            this.lblEstadoHabitacionActual.Text = "label1";
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
            this.tableLayoutPanel1.TabIndex = 5;
            // 
            // FrmCheckInOut
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.flpCheckInOut);
            this.Name = "FrmCheckInOut";
            this.Text = "Form1";
            this.flpCheckInOut.ResumeLayout(false);
            this.flpCheckInOut.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReservasConfirmadas)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEstadiasActivas)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flpCheckInOut;
        private System.Windows.Forms.DataGridView dgvReservasConfirmadas;
        private System.Windows.Forms.Button btnCheckIn;
        private System.Windows.Forms.DataGridView dgvEstadiasActivas;
        private System.Windows.Forms.Button btnCheckOut;
        private System.Windows.Forms.Label lblEstadoHabitacionActual;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}