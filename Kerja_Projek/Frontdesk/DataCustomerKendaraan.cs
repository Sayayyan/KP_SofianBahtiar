using Kerja_Projek.KumpulanFormDialog;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Kerja_Projek
{
    public partial class DataCustomerKendaraan : Form
    {
        Koneksi konn = new Koneksi();
        private SqlCommand cmd;
        private DataTable dtCustomer;

        public DataCustomerKendaraan()
        {
            InitializeComponent();
            txtcustomer.Enter += txtcustomer_Enter;
            txtcustomer.Leave += txtcustomer_Leave;
        }

        public void TampilCustomer()
        {
            try
            {
                using (SqlConnection conn = konn.Getconn())
                {
                    conn.Open();

                    string query = "SELECT id, nama, no_hp, alamat, dibuat_pada FROM customers";
                    cmd = new SqlCommand(query, conn);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    dtCustomer = new DataTable();
                    da.Fill(dtCustomer);

                    dgvCustomer.DataSource = dtCustomer;

                    SetHeaderCustomer();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal tampil data customer: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetHeaderCustomer()
        {
            if (dgvCustomer.Columns["id"] != null)
                dgvCustomer.Columns["id"].Visible = false;

            if (dgvCustomer.Columns["nama"] != null)
                dgvCustomer.Columns["nama"].HeaderText = "Nama Customer";

            if (dgvCustomer.Columns["no_hp"] != null)
                dgvCustomer.Columns["no_hp"].HeaderText = "No. Telepon";

            if (dgvCustomer.Columns["alamat"] != null)
                dgvCustomer.Columns["alamat"].HeaderText = "Alamat";

            if (dgvCustomer.Columns["dibuat_pada"] != null)
                dgvCustomer.Columns["dibuat_pada"].HeaderText = "Tanggal Terdaftar";
        }

        private void SetHeaderKendaraan()
        {
            if (dgvKendaraan.Columns["id"] != null)
                dgvKendaraan.Columns["id"].Visible = false;

            if (dgvKendaraan.Columns["customer_id"] != null)
                dgvKendaraan.Columns["customer_id"].Visible = false;

            if (dgvKendaraan.Columns["nama_customer"] != null)
                dgvKendaraan.Columns["nama_customer"].HeaderText = "Nama Pemilik";

            if (dgvKendaraan.Columns["no_polisi"] != null)
                dgvKendaraan.Columns["no_polisi"].HeaderText = "No. Polisi";

            if (dgvKendaraan.Columns["kendaraan"] != null)
                dgvKendaraan.Columns["kendaraan"].HeaderText = "Kendaraan";

            if (dgvKendaraan.Columns["tahun"] != null)
                dgvKendaraan.Columns["tahun"].HeaderText = "Tahun";
        }

        private void MasterCustomerKendaraan_Load(object sender, EventArgs e)
        {
            TampilCustomer();
        }

        private void dgvCustomer_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            try
            {
                DataGridViewRow row = dgvCustomer.Rows[e.RowIndex];
                object selectedCustomerId = row.Cells["id"].Value;

                if (selectedCustomerId == null || selectedCustomerId == DBNull.Value) return;

                using (SqlConnection conn = konn.Getconn())
                {
                    conn.Open();

                    string query = @"SELECT k.id, 
                                            k.customer_id, 
                                            c.nama AS nama_customer, 
                                            k.no_polisi, 
                                            k.kendaraan, 
                                            k.tahun 
                                     FROM kendaraan k
                                     INNER JOIN customers c ON k.customer_id = c.id
                                     WHERE k.customer_id = @customer_id";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@customer_id", selectedCustomerId);

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dtKendaraan = new DataTable();
                        da.Fill(dtKendaraan);

                        dgvKendaraan.DataSource = dtKendaraan;
                        SetHeaderKendaraan();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat data kendaraan: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dgvCustomer.CurrentRow != null && dgvCustomer.CurrentRow.Cells["id"].Value != null)
            {
                int selectedCustomerId = Convert.ToInt32(dgvCustomer.CurrentRow.Cells["id"].Value);

                using (TambahKendaraan kendaraan = new TambahKendaraan(selectedCustomerId))
                {
                    if (kendaraan.ShowDialog() == DialogResult.OK)
                    {
                        dgvCustomer_CellClick(dgvCustomer, new DataGridViewCellEventArgs(0, dgvCustomer.CurrentRow.Index));
                    }
                }
            }
            else
            {
                MessageBox.Show("Pilih customer terlebih dahulu!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TambahCustomer customer = new TambahCustomer();

            if (customer.ShowDialog() == DialogResult.OK)
            {
                TampilCustomer();
            }
        }

        private void txtcustomer_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (dtCustomer != null)
                {
                    DataView dv = dtCustomer.DefaultView;

                    if (txtcustomer.Text == "Cari Customer..." || string.IsNullOrWhiteSpace(txtcustomer.Text))
                    {
                        dv.RowFilter = "";
                    }
                    else
                    {
                        dv.RowFilter = string.Format("nama LIKE '%{0}%' OR no_hp LIKE '%{0}%'", txtcustomer.Text.Replace("'", "''"));
                    }

                    dgvCustomer.DataSource = dv;
                    SetHeaderCustomer();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saat mencari data: " + ex.Message);
            }
        }

        private void txtcustomer_Enter(object sender, EventArgs e)
        {
            if (txtcustomer.Text == "Cari Customer...")
            {
                txtcustomer.Text = "";
                txtcustomer.ForeColor = Color.Black;
            }
        }

        private void txtcustomer_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtcustomer.Text))
            {
                txtcustomer.Text = "Cari Customer...";
                txtcustomer.ForeColor = Color.Gray;
            }
        }

        private void Panel2_Paint(object sender, PaintEventArgs e) { }
        private void panel1_Paint(object sender, PaintEventArgs e) { }
        private void panel3_Paint(object sender, PaintEventArgs e) { }
        private void dgvCustomer_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
    }
}