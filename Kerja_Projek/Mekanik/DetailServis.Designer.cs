namespace Kerja_Projek
{
    partial class DetailServis
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtkeluhan = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.lblkendaraan = new System.Windows.Forms.Label();
            this.lblnopol = new System.Windows.Forms.Label();
            this.lblnowo = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.txttemuan = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.dgvjasadikerjakan = new System.Windows.Forms.DataGridView();
            this.cmbjasa = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.dgvsparepartdiminta = new System.Windows.Forms.DataGridView();
            this.cmbSparepart = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.button2 = new System.Windows.Forms.Button();
            this.cmbstatus = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.btnsimpan = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvjasadikerjakan)).BeginInit();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvsparepartdiminta)).BeginInit();
            this.panel5.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.txtkeluhan);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.lblkendaraan);
            this.panel1.Controls.Add(this.lblnopol);
            this.panel1.Controls.Add(this.lblnowo);
            this.panel1.Location = new System.Drawing.Point(24, 23);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(937, 100);
            this.panel1.TabIndex = 0;
            // 
            // txtkeluhan
            // 
            this.txtkeluhan.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtkeluhan.Enabled = false;
            this.txtkeluhan.Location = new System.Drawing.Point(-1, 59);
            this.txtkeluhan.Multiline = true;
            this.txtkeluhan.Name = "txtkeluhan";
            this.txtkeluhan.Size = new System.Drawing.Size(937, 40);
            this.txtkeluhan.TabIndex = 4;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(3, 38);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(73, 18);
            this.label8.TabIndex = 7;
            this.label8.Text = "Keluhan : ";
            // 
            // lblkendaraan
            // 
            this.lblkendaraan.AutoSize = true;
            this.lblkendaraan.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblkendaraan.Location = new System.Drawing.Point(412, 10);
            this.lblkendaraan.Name = "lblkendaraan";
            this.lblkendaraan.Size = new System.Drawing.Size(13, 18);
            this.lblkendaraan.TabIndex = 3;
            this.lblkendaraan.Text = "-";
            this.lblkendaraan.Click += new System.EventHandler(this.lblkendaraan_Click);
            // 
            // lblnopol
            // 
            this.lblnopol.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblnopol.AutoSize = true;
            this.lblnopol.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblnopol.Location = new System.Drawing.Point(762, 10);
            this.lblnopol.Name = "lblnopol";
            this.lblnopol.Size = new System.Drawing.Size(13, 18);
            this.lblnopol.TabIndex = 5;
            this.lblnopol.Text = "-";
            this.lblnopol.Click += new System.EventHandler(this.lblnopol_Click);
            // 
            // lblnowo
            // 
            this.lblnowo.AutoSize = true;
            this.lblnowo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblnowo.Location = new System.Drawing.Point(12, 10);
            this.lblnowo.Name = "lblnowo";
            this.lblnowo.Size = new System.Drawing.Size(13, 18);
            this.lblnowo.TabIndex = 3;
            this.lblnowo.Text = "-";
            this.lblnowo.Click += new System.EventHandler(this.lblnowo_Click);
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.txttemuan);
            this.panel2.Controls.Add(this.label16);
            this.panel2.Location = new System.Drawing.Point(24, 129);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(937, 100);
            this.panel2.TabIndex = 8;
            this.panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.panel2_Paint_1);
            // 
            // txttemuan
            // 
            this.txttemuan.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txttemuan.Location = new System.Drawing.Point(-1, 31);
            this.txttemuan.Multiline = true;
            this.txttemuan.Name = "txttemuan";
            this.txttemuan.Size = new System.Drawing.Size(938, 68);
            this.txttemuan.TabIndex = 3;
            this.txttemuan.TextChanged += new System.EventHandler(this.txttemuan_TextChanged);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(3, 10);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(74, 18);
            this.label16.TabIndex = 2;
            this.label16.Text = "Temuan : ";
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.dgvjasadikerjakan);
            this.panel3.Controls.Add(this.cmbjasa);
            this.panel3.Controls.Add(this.label9);
            this.panel3.Location = new System.Drawing.Point(24, 236);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(350, 232);
            this.panel3.TabIndex = 9;
            // 
            // dgvjasadikerjakan
            // 
            this.dgvjasadikerjakan.AllowUserToAddRows = false;
            this.dgvjasadikerjakan.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvjasadikerjakan.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvjasadikerjakan.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvjasadikerjakan.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvjasadikerjakan.ColumnHeadersVisible = false;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvjasadikerjakan.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvjasadikerjakan.Location = new System.Drawing.Point(0, 39);
            this.dgvjasadikerjakan.Name = "dgvjasadikerjakan";
            this.dgvjasadikerjakan.ReadOnly = true;
            this.dgvjasadikerjakan.RowHeadersVisible = false;
            this.dgvjasadikerjakan.RowHeadersWidth = 51;
            this.dgvjasadikerjakan.RowTemplate.Height = 24;
            this.dgvjasadikerjakan.Size = new System.Drawing.Size(350, 193);
            this.dgvjasadikerjakan.TabIndex = 17;
            this.dgvjasadikerjakan.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvjasadikerjakan_CellContentClick);
            this.dgvjasadikerjakan.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvjasadikerjakan_CellDoubleClick);
            // 
            // cmbjasa
            // 
            this.cmbjasa.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbjasa.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbjasa.FormattingEnabled = true;
            this.cmbjasa.Location = new System.Drawing.Point(142, 8);
            this.cmbjasa.Name = "cmbjasa";
            this.cmbjasa.Size = new System.Drawing.Size(196, 24);
            this.cmbjasa.TabIndex = 18;
            this.cmbjasa.SelectedIndexChanged += new System.EventHandler(this.cmbjasa_SelectedIndexChanged);
            this.cmbjasa.SelectionChangeCommitted += new System.EventHandler(this.cmbjasa_SelectionChangeCommitted);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.BackColor = System.Drawing.SystemColors.Window;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(9, 9);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(114, 18);
            this.label9.TabIndex = 4;
            this.label9.Text = "Jasa Dikerjakan";
            // 
            // panel4
            // 
            this.panel4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel4.BackColor = System.Drawing.Color.White;
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel4.Controls.Add(this.dgvsparepartdiminta);
            this.panel4.Controls.Add(this.cmbSparepart);
            this.panel4.Controls.Add(this.label10);
            this.panel4.Location = new System.Drawing.Point(384, 236);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(577, 232);
            this.panel4.TabIndex = 10;
            // 
            // dgvsparepartdiminta
            // 
            this.dgvsparepartdiminta.AllowUserToAddRows = false;
            this.dgvsparepartdiminta.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvsparepartdiminta.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvsparepartdiminta.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvsparepartdiminta.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvsparepartdiminta.DefaultCellStyle = dataGridViewCellStyle4;
            this.dgvsparepartdiminta.Location = new System.Drawing.Point(1, 39);
            this.dgvsparepartdiminta.Name = "dgvsparepartdiminta";
            this.dgvsparepartdiminta.RowHeadersVisible = false;
            this.dgvsparepartdiminta.RowHeadersWidth = 51;
            this.dgvsparepartdiminta.RowTemplate.Height = 24;
            this.dgvsparepartdiminta.Size = new System.Drawing.Size(575, 193);
            this.dgvsparepartdiminta.TabIndex = 18;
            this.dgvsparepartdiminta.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvsparepartdiminta_CellContentClick_1);
            this.dgvsparepartdiminta.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvsparepartdiminta_CellDoubleClick);
            // 
            // cmbSparepart
            // 
            this.cmbSparepart.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSparepart.FormattingEnabled = true;
            this.cmbSparepart.Location = new System.Drawing.Point(186, 9);
            this.cmbSparepart.Name = "cmbSparepart";
            this.cmbSparepart.Size = new System.Drawing.Size(183, 24);
            this.cmbSparepart.TabIndex = 19;
            this.cmbSparepart.SelectedIndexChanged += new System.EventHandler(this.cmbSparepart_SelectedIndexChanged);
            this.cmbSparepart.SelectionChangeCommitted += new System.EventHandler(this.cmbSparepart_SelectionChangeCommitted);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.BackColor = System.Drawing.SystemColors.Window;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(3, 9);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(126, 18);
            this.label10.TabIndex = 5;
            this.label10.Text = "Sparepart Diminta";
            // 
            // panel5
            // 
            this.panel5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel5.BackColor = System.Drawing.Color.White;
            this.panel5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel5.Controls.Add(this.button2);
            this.panel5.Controls.Add(this.cmbstatus);
            this.panel5.Controls.Add(this.label11);
            this.panel5.Controls.Add(this.btnsimpan);
            this.panel5.Location = new System.Drawing.Point(24, 473);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(937, 49);
            this.panel5.TabIndex = 11;
            this.panel5.Paint += new System.Windows.Forms.PaintEventHandler(this.panel5_Paint);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.button2.BackColor = System.Drawing.Color.White;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Font = new System.Drawing.Font("Verdana", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.ForeColor = System.Drawing.Color.Black;
            this.button2.Location = new System.Drawing.Point(-1, -1);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(110, 50);
            this.button2.TabIndex = 23;
            this.button2.Text = "Back";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click_1);
            // 
            // cmbstatus
            // 
            this.cmbstatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbstatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbstatus.FormattingEnabled = true;
            this.cmbstatus.Location = new System.Drawing.Point(604, 14);
            this.cmbstatus.Name = "cmbstatus";
            this.cmbstatus.Size = new System.Drawing.Size(171, 24);
            this.cmbstatus.TabIndex = 19;
            this.cmbstatus.SelectedIndexChanged += new System.EventHandler(this.cmbstatus_SelectedIndexChanged);
            // 
            // label11
            // 
            this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label11.AutoSize = true;
            this.label11.BackColor = System.Drawing.SystemColors.Window;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(448, 15);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(128, 18);
            this.label11.TabIndex = 21;
            this.label11.Text = "Status Pengerjaan";
            // 
            // btnsimpan
            // 
            this.btnsimpan.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnsimpan.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(110)))), ((int)(((byte)(86)))));
            this.btnsimpan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnsimpan.Font = new System.Drawing.Font("Verdana", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnsimpan.ForeColor = System.Drawing.Color.White;
            this.btnsimpan.Location = new System.Drawing.Point(809, -1);
            this.btnsimpan.Name = "btnsimpan";
            this.btnsimpan.Size = new System.Drawing.Size(128, 49);
            this.btnsimpan.TabIndex = 22;
            this.btnsimpan.Text = "Simpan";
            this.btnsimpan.UseVisualStyleBackColor = false;
            this.btnsimpan.Click += new System.EventHandler(this.btnsimpan_Click);
            // 
            // DetailServis
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(989, 546);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "DetailServis";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DetailServis";
            this.Load += new System.EventHandler(this.DetailServis_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvjasadikerjakan)).EndInit();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvsparepartdiminta)).EndInit();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblkendaraan;
        private System.Windows.Forms.Label lblnopol;
        private System.Windows.Forms.Label lblnowo;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.DataGridView dgvjasadikerjakan;
        private System.Windows.Forms.DataGridView dgvsparepartdiminta;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button btnsimpan;
        private System.Windows.Forms.TextBox txttemuan;
        private System.Windows.Forms.ComboBox cmbjasa;
        private System.Windows.Forms.ComboBox cmbstatus;
        private System.Windows.Forms.ComboBox cmbSparepart;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox txtkeluhan;
    }
}