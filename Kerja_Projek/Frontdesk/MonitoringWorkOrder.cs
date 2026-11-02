using Kerja_Projek.KumpulanFormDialog;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Kerja_Projek
{
    public partial class MonitoringWorkOrder : Form
    {
        Koneksi konn = new Koneksi();
        private SqlDataAdapter da;
        private SqlCommand cmd;
        private DataTable dt;

        public MonitoringWorkOrder()
        {
            InitializeComponent();
        }

        private void MonitoringWorkOrder_Load(object sender, EventArgs e)
        {
            LoadStatus();
            TampilWorkOrder();
        }

        private void LoadStatus()
        {
            cmbstatus.SelectedIndexChanged -= cmbstatus_SelectedIndexChanged;

            cmbstatus.Items.Clear();
            cmbstatus.Items.Add("Semua");
            cmbstatus.Items.Add("Baru");
            cmbstatus.Items.Add("Dikerjakan");
            cmbstatus.Items.Add("Menunggu sparepart");
            cmbstatus.Items.Add("Selesai");
            cmbstatus.Items.Add("Diserahkan");
            cmbstatus.Items.Add("Sudah dibayar");

            cmbstatus.SelectedIndex = 0;
            cmbstatus.SelectedIndexChanged += cmbstatus_SelectedIndexChanged;
        }

        public void TampilWorkOrder()
        {
            try
            {
                using (SqlConnection conn = konn.Getconn())
                {
                    conn.Open();

                    string query = @"SELECT 
                                        wo.id, 
                                        wo.nomor_wo, 
                                        c.nama AS nama_customer, 
                                        k.kendaraan AS nama_kendaraan, 
                                        m.nama_lengkap AS nama_mekanik, 
                                        wo.keluhan, 
                                        wo.temuan, 
                                        wo.status, 
                                        u.nama_lengkap AS pembuat, 
                                        wo.dibuat_pada, 
                                        wo.pengambil 
                                     FROM work_orders wo
                                     LEFT JOIN customers c ON wo.customer_id = c.id
                                     LEFT JOIN kendaraan k ON wo.kendaraan_id = k.id
                                     LEFT JOIN users m ON wo.mekanik_id = m.id
                                     LEFT JOIN users u ON wo.dibuat_oleh = u.id
                                     ORDER BY wo.id DESC";

                    cmd = new SqlCommand(query, conn);
                    da = new SqlDataAdapter(cmd);
                    dt = new DataTable();
                    da.Fill(dt);

                    drgmonitoringwo.DataSource = dt;
                    SetHeaderGrid();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal tampil data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetHeaderGrid()
        {
            if (drgmonitoringwo.Columns["id"] != null)
                drgmonitoringwo.Columns["id"].Visible = false;

            if (drgmonitoringwo.Columns["nomor_wo"] != null)
                drgmonitoringwo.Columns["nomor_wo"].HeaderText = "No. WO";

            if (drgmonitoringwo.Columns["nama_customer"] != null)
                drgmonitoringwo.Columns["nama_customer"].HeaderText = "Nama Customer";

            if (drgmonitoringwo.Columns["nama_kendaraan"] != null)
                drgmonitoringwo.Columns["nama_kendaraan"].HeaderText = "Kendaraan";

            if (drgmonitoringwo.Columns["nama_mekanik"] != null)
                drgmonitoringwo.Columns["nama_mekanik"].HeaderText = "Nama Mekanik";

            if (drgmonitoringwo.Columns["keluhan"] != null)
                drgmonitoringwo.Columns["keluhan"].HeaderText = "Keluhan";

            if (drgmonitoringwo.Columns["temuan"] != null)
                drgmonitoringwo.Columns["temuan"].HeaderText = "Temuan";

            if (drgmonitoringwo.Columns["status"] != null)
                drgmonitoringwo.Columns["status"].HeaderText = "Status";

            if (drgmonitoringwo.Columns["pembuat"] != null)
                drgmonitoringwo.Columns["pembuat"].HeaderText = "Dibuat Oleh";

            if (drgmonitoringwo.Columns["dibuat_pada"] != null)
                drgmonitoringwo.Columns["dibuat_pada"].HeaderText = "Tanggal Dibuat";

            if (drgmonitoringwo.Columns["pengambil"] != null)
                drgmonitoringwo.Columns["pengambil"].HeaderText = "Nama Pengambil";
        }

        private void cmbstatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (dt != null && cmbstatus.SelectedItem != null)
                {
                    DataView dv = dt.DefaultView;
                    string selectedStatus = cmbstatus.SelectedItem.ToString();

                    if (selectedStatus == "Semua" || string.IsNullOrWhiteSpace(selectedStatus))
                    {
                        dv.RowFilter = "";
                    }
                    else
                    {
                        dv.RowFilter = string.Format("status = '{0}'", selectedStatus.Replace("'", "''"));
                    }

                    drgmonitoringwo.DataSource = dv;
                    SetHeaderGrid();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saat memfilter data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (drgmonitoringwo.CurrentRow != null && drgmonitoringwo.CurrentRow.Cells["id"].Value != DBNull.Value)
            {
                int selectedWOId = Convert.ToInt32(drgmonitoringwo.CurrentRow.Cells["id"].Value);
                string currentStatus = drgmonitoringwo.CurrentRow.Cells["status"].Value?.ToString();
                string nomorWO = drgmonitoringwo.CurrentRow.Cells["nomor_wo"].Value == null ? "" : drgmonitoringwo.CurrentRow.Cells["nomor_wo"].Value.ToString();

                if (currentStatus != "Selesai")
                {
                    MessageBox.Show("Hanya Work Order dengan status 'Selesai' yang dapat diserahkan!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (MessageBox.Show("Apakah anda yakin ingin menyerahkan kendaraan ini?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        using (SqlConnection conn = konn.Getconn())
                        {
                            conn.Open();
                            cmd = new SqlCommand("UPDATE work_orders SET status = 'Diserahkan' WHERE id = @id", conn);
                            cmd.Parameters.AddWithValue("@id", selectedWOId);
                            cmd.ExecuteNonQuery();

                            SqlCommand cmdLog = new SqlCommand("INSERT INTO audit_log (user_id, aksi, waktu) VALUES (@user_id, @aksi, GETDATE())", conn);
                            cmdLog.Parameters.AddWithValue("@user_id", usersession.UserId);
                            cmdLog.Parameters.AddWithValue("@aksi", $"Menyerahkan kendaraan {nomorWO}");
                            cmdLog.ExecuteNonQuery();

                            MessageBox.Show("Status kendaraan berhasil diperbarui!", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            TampilWorkOrder();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Gagal update data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Pilih data work order terlebih dahulu!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (drgmonitoringwo.CurrentRow == null || drgmonitoringwo.CurrentRow.Cells["id"].Value == null || drgmonitoringwo.CurrentRow.Cells["id"].Value == DBNull.Value)
            {
                MessageBox.Show("Pilih data work order terlebih dahulu!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int selectedWOId = Convert.ToInt32(drgmonitoringwo.CurrentRow.Cells["id"].Value);
            string nomorWO = drgmonitoringwo.CurrentRow.Cells["nomor_wo"].Value?.ToString() ?? "";

            using (SerahTerima lanjut = new SerahTerima(selectedWOId, nomorWO))
            {
                if (lanjut.ShowDialog() == DialogResult.OK)
                {
                    TampilWorkOrder();
                }
            }
        }

        private void drgmonitoringwo_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
        private void drgmonitoringwo_CellContentClick_1(object sender, DataGridViewCellEventArgs e) { }
        private void cmbstatus_SelectedIndexChanged_1(object sender, EventArgs e) { }
    }
}