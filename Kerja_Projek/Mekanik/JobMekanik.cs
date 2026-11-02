using Kerja_Projek.Fondasi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kerja_Projek
{
    public partial class JobMekanik : Form
    {
        Koneksi konn = new Koneksi();
        private SqlDataAdapter da;
        private SqlDataReader dr;
        private DataSet ds;
        private SqlCommand cmd;
        private DataTable dtWo;

        public JobMekanik()
        {
            InitializeComponent();
        }

        public void TampilData()
        {
            SqlConnection conn = konn.Getconn();
            try
            {
                conn.Open();
                
                string query = @"SELECT wo.id, wo.nomor_wo, 
                                        c.nama AS nama_customer, 
                                        k.kendaraan AS nama_kendaraan,
                                        u.nama_lengkap AS nama_mekanik,
                                        wo.keluhan, wo.temuan, wo.status, wo.dibuat_pada
                                 FROM work_orders wo
                                 LEFT JOIN users u ON wo.mekanik_id = u.id
                                 LEFT JOIN customers c ON wo.customer_id = c.id
                                 LEFT JOIN kendaraan k ON wo.kendaraan_id = k.id
                                 WHERE wo.mekanik_id = @mekanik_id
                                   AND (wo.status IS NULL OR wo.status NOT IN ('Sudah dibayar', 'Diserahkan'))";

                cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@mekanik_id", usersession.UserId);

                da = new SqlDataAdapter(cmd);
                ds = new DataSet();

                da.Fill(ds, "work_orders");
                dtWo = ds.Tables["work_orders"];
                dataGridView1.DataSource = dtWo;

                SetHeaderGrid();
                TerapkanFilterStatus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat data Work Order: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        private void SetHeaderGrid()
        {
            if (dataGridView1.Columns["id"] != null) 
            {
                dataGridView1.Columns["id"].Visible = false;
            }
            if (dataGridView1.Columns["nomor_wo"] != null) dataGridView1.Columns["nomor_wo"].HeaderText = "No. WO";
            if (dataGridView1.Columns["nama_customer"] != null) dataGridView1.Columns["nama_customer"].HeaderText = "Nama Customer";
            if (dataGridView1.Columns["nama_kendaraan"] != null) dataGridView1.Columns["nama_kendaraan"].HeaderText = "Nama Kendaraan";
            if (dataGridView1.Columns["nama_mekanik"] != null) dataGridView1.Columns["nama_mekanik"].HeaderText = "Nama Mekanik";
            if (dataGridView1.Columns["keluhan"] != null) dataGridView1.Columns["keluhan"].HeaderText = "Keluhan";
            if (dataGridView1.Columns["temuan"] != null) dataGridView1.Columns["temuan"].HeaderText = "Temuan";
            if (dataGridView1.Columns["status"] != null) dataGridView1.Columns["status"].HeaderText = "Status";
            if (dataGridView1.Columns["dibuat_pada"] != null) dataGridView1.Columns["dibuat_pada"].HeaderText = "Tanggal";
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null && dataGridView1.CurrentRow.Index >= 0 && !dataGridView1.CurrentRow.IsNewRow)
            {
                if (dataGridView1.Columns.Contains("id"))
                {
                    var cellValue = dataGridView1.CurrentRow.Cells["id"].Value;

                    if (cellValue != null && cellValue != DBNull.Value)
                    {
                        int workOrderId = Convert.ToInt32(cellValue);

                        DetailServis lanjut = new DetailServis(workOrderId);

                        MainFormMekanik parentForm = Application.OpenForms.OfType<MainFormMekanik>().FirstOrDefault(f => f.Visible)
                            ?? Application.OpenForms.OfType<MainFormMekanik>().FirstOrDefault();

                        if (parentForm != null)
                        {
                            parentForm.openForm(lanjut);
                        }
                        else
                        {
                            lanjut.Show();
                        }
                    }
                    else
                    {
                        MessageBox.Show("ID Work Order kosong atau tidak valid!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Kolom 'id' tidak ditemukan pada DataGridView!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Silakan pilih data Work Order terlebih dahulu!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void JobMekanik_Load(object sender, EventArgs e)
        {
            cmbstatus.Items.Clear();
            cmbstatus.Items.Add("Semua");
            cmbstatus.Items.Add("Belum dikerjakan");
            cmbstatus.Items.Add("Sedang dikerjakan");
            cmbstatus.Items.Add("Menunggu sparepart");
            cmbstatus.Items.Add("Selesai");
            cmbstatus.SelectedIndex = 0;

            TampilData();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            TerapkanFilterStatus();
        }

        private void TerapkanFilterStatus()
        {
            if (dtWo == null) return;

            string selected = cmbstatus.SelectedItem != null ? cmbstatus.SelectedItem.ToString() : "";

            if (selected == "Semua" || selected == "")
            {
                dtWo.DefaultView.RowFilter = "";
            }
            else
            {
                string dbStatus = selected;
                if (selected == "Belum dikerjakan")
                {
                    dbStatus = "Baru";
                }
                else if (selected == "Sedang dikerjakan")
                {
                    dbStatus = "Dikerjakan";
                }
                dtWo.DefaultView.RowFilter = "status = '" + dbStatus.Replace("'", "''") + "'";
            }

            dataGridView1.DataSource = dtWo.DefaultView;
            SetHeaderGrid();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void JobMekanik_Activated(object sender, EventArgs e)
        {
            TampilData();
        }
    }
}