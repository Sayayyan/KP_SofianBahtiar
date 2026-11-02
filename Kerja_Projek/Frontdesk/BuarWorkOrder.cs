using Kerja_Projek.KumpulanFormDialog;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Kerja_Projek
{
    public partial class BuarWorkOrder : Form
    {
        Koneksi konn = new Koneksi();
        private SqlDataAdapter da;
        private SqlCommand cmd;

        public BuarWorkOrder()
        {
            InitializeComponent();
        }

        private void BuarWorkOrder_Load(object sender, EventArgs e)
        {
            LoadCustomer();
            LoadMekanik();
        }

        private void LoadMekanik()
        {
            try
            {
                using (SqlConnection conn = konn.Getconn())
                {
                    conn.Open();

                    string query = "SELECT id, nama_lengkap FROM users WHERE role_id = 5";
                    cmd = new SqlCommand(query, conn);

                    da = new SqlDataAdapter(cmd);
                    DataTable dtMekanik = new DataTable();
                    da.Fill(dtMekanik);

                    cmbMekanik.DataSource = dtMekanik;
                    cmbMekanik.DisplayMember = "nama_lengkap";
                    cmbMekanik.ValueMember = "id";
                    cmbMekanik.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat data mekanik: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadCustomer()
        {
            try
            {
                using (SqlConnection conn = konn.Getconn())
                {
                    conn.Open();

                    string query = "SELECT id, nama FROM customers";
                    cmd = new SqlCommand(query, conn);

                    da = new SqlDataAdapter(cmd);
                    DataTable dtCustomer = new DataTable();
                    da.Fill(dtCustomer);

                    cmbCustomer.SelectedIndexChanged -= cmbCustomer_SelectedIndexChanged;

                    cmbCustomer.DataSource = dtCustomer;
                    cmbCustomer.DisplayMember = "nama";
                    cmbCustomer.ValueMember = "id";
                    cmbCustomer.SelectedIndex = -1;

                    cmbCustomer.SelectedIndexChanged += cmbCustomer_SelectedIndexChanged;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat data customer: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadKendaraan(int customerId)
        {
            try
            {
                using (SqlConnection conn = konn.Getconn())
                {
                    conn.Open();

                    string query = "SELECT id, kendaraan AS nama_kendaraan FROM kendaraan WHERE customer_id = @customer_id";
                    cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@customer_id", customerId);

                    da = new SqlDataAdapter(cmd);
                    DataTable dtKendaraan = new DataTable();
                    da.Fill(dtKendaraan);

                    cmbKendaraan.DataSource = dtKendaraan;
                    cmbKendaraan.DisplayMember = "nama_kendaraan";
                    cmbKendaraan.ValueMember = "id";
                    cmbKendaraan.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat data kendaraan: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cmbCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbCustomer.SelectedValue != null && int.TryParse(cmbCustomer.SelectedValue.ToString(), out int customerId))
            {
                LoadKendaraan(customerId);
            }
            else
            {
                cmbKendaraan.DataSource = null;
            }
        }

        private string GenerateNomorWO(SqlConnection conn)
        {
            string todayStr = DateTime.Now.ToString("yyyyMMdd");
            string prefix = "WO-" + todayStr + "-";
            int countToday = 1;

            string query = "SELECT COUNT(*) FROM work_orders WHERE nomor_wo LIKE @prefix";
            using (SqlCommand cmdCount = new SqlCommand(query, conn))
            {
                cmdCount.Parameters.AddWithValue("@prefix", prefix + "%");
                countToday = (int)cmdCount.ExecuteScalar() + 1;
            }

            return prefix + countToday.ToString("D3");
        }

        private void btnSimpan_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (TambahCustomer customer = new TambahCustomer())
            {
                if (customer.ShowDialog() == DialogResult.OK)
                {
                    LoadCustomer();
                }
            }
        }

        private void cmbKendaraan_SelectedIndexChanged(object sender, EventArgs e) { }
        private void cmbMekanik_SelectedIndexChanged(object sender, EventArgs e) { }
        private void txtKeluhan_TextChanged(object sender, EventArgs e) { }

        private void button2_Click(object sender, EventArgs e)
        {
            if (cmbCustomer.SelectedValue == null)
            {
                MessageBox.Show("Silakan pilih Customer!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbKendaraan.SelectedValue == null)
            {
                MessageBox.Show("Silakan pilih Kendaraan!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbMekanik.SelectedValue == null)
            {
                MessageBox.Show("Silakan pilih Mekanik!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtKeluhan.Text))
            {
                MessageBox.Show("Keluhan tidak boleh kosong!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection conn = konn.Getconn())
                {
                    conn.Open();

                    string nomorWo = GenerateNomorWO(conn);

                    string query = @"INSERT INTO work_orders 
                                    (nomor_wo, customer_id, kendaraan_id, mekanik_id, keluhan, temuan, status, dibuat_oleh, dibuat_pada) 
                                    VALUES 
                                    (@nomor_wo, @customer_id, @kendaraan_id, @mekanik_id, @keluhan, @temuan, @status, @dibuat_oleh, @dibuat_pada)";

                    using (SqlCommand cmdInsert = new SqlCommand(query, conn))
                    {
                        cmdInsert.Parameters.AddWithValue("@nomor_wo", nomorWo);
                        cmdInsert.Parameters.AddWithValue("@customer_id", Convert.ToInt32(cmbCustomer.SelectedValue));
                        cmdInsert.Parameters.AddWithValue("@kendaraan_id", Convert.ToInt32(cmbKendaraan.SelectedValue));
                        cmdInsert.Parameters.AddWithValue("@mekanik_id", Convert.ToInt32(cmbMekanik.SelectedValue));
                        cmdInsert.Parameters.AddWithValue("@keluhan", txtKeluhan.Text.Trim());
                        cmdInsert.Parameters.AddWithValue("@temuan", DBNull.Value);
                        cmdInsert.Parameters.AddWithValue("@status", "Baru");
                        cmdInsert.Parameters.AddWithValue("@dibuat_oleh", usersession.UserId);
                        cmdInsert.Parameters.AddWithValue("@dibuat_pada", DateTime.Now);

                        cmdInsert.ExecuteNonQuery();
                    }

                    string queryLog = @"INSERT INTO audit_log (user_id, aksi, waktu) 
                                        VALUES (@user_id, @aksi, GETDATE())";

                    using (SqlCommand cmdLog = new SqlCommand(queryLog, conn))
                    {
                        cmdLog.Parameters.AddWithValue("@user_id", usersession.UserId);
                        cmdLog.Parameters.AddWithValue("@aksi", $"Membuat Work Order {nomorWo}");
                        cmdLog.ExecuteNonQuery();
                    }

                    MessageBox.Show("Work Order berhasil dibuat!\nNomor WO: " + nomorWo, "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal menyimpan Work Order: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}