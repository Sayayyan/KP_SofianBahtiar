using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kerja_Projek
{
    public partial class BarangMasuk : Form
    {
        Koneksi konn = new Koneksi();
        private SqlDataAdapter da;
        private SqlDataReader dr;
        private DataSet ds;
        private SqlCommand cmd;

        private int idUserAktif = 3;
        public BarangMasuk()
        {
            InitializeComponent();
        }

        private void ClearInput()
        {
            cmbsupplier.SelectedIndex = -1;
            cmbsparepart.SelectedIndex = -1;
            txtqty.Clear();
            txthargabeli.Clear();
        }

        private void LoadSupplier()
        {
            SqlConnection conn = konn.Getconn();
            try
            {
                conn.Open();
                string query = "SELECT id, nama FROM suppliers";
                cmd = new SqlCommand(query, conn);
                da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                cmbsupplier.DataSource = dt;
                cmbsupplier.DisplayMember = "nama";
                cmbsupplier.ValueMember = "id";
                cmbsupplier.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat supplier: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }
        }

        private void LoadSparepart(int supplierId = 0)
        {
            cmbsparepart.DataSource = null;
            cmbsparepart.Items.Clear();
            txthargabeli.Clear();

            if (supplierId == 0) return;

            SqlConnection conn = konn.Getconn();
            try
            {
                conn.Open();
                string query = "SELECT id, nama, harga_beli FROM sparepart WHERE supplier_id = @supplier_id";
                cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@supplier_id", supplierId);

                da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    cmbsparepart.DataSource = dt;
                    cmbsparepart.DisplayMember = "nama";
                    cmbsparepart.ValueMember = "id";
                    cmbsparepart.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat sparepart: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }
        }

        private void TampilDataBarangMasuk()
        {
            SqlConnection conn = konn.Getconn();
            try
            {
                conn.Open();

                string query = @"SELECT 
                            suppliers.nama AS [Nama Supplier], 
                            sparepart.nama AS [Nama Sparepart], 
                            barang_masuk.qty AS [Qty], 
                            barang_masuk.harga_beli AS [Harga Beli], 
                            barang_masuk.diterima_pada AS [Tanggal], 
                            users.username AS [Dicatat Oleh]
                         FROM barang_masuk
                         INNER JOIN suppliers ON barang_masuk.supplier_id = suppliers.id
                         INNER JOIN sparepart ON barang_masuk.sparepart_id = sparepart.id
                         LEFT JOIN users ON barang_masuk.dicatat_oleh = users.id
                         ORDER BY barang_masuk.id DESC";

                cmd = new SqlCommand(query, conn);
                da = new SqlDataAdapter(cmd);
                ds = new DataSet();
                da.Fill(ds, "barang_masuk");

                dataGridView1.DataSource = ds.Tables["barang_masuk"];
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat data barang masuk: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }
        }

        private void cmbsparepart_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbsparepart.SelectedIndex != -1 && cmbsparepart.SelectedItem is DataRowView drv)
            {
                if (drv["harga_beli"] != DBNull.Value)
                {
                    txthargabeli.Text = Convert.ToDecimal(drv["harga_beli"]).ToString("G0");
                }
                else
                {
                    txthargabeli.Text = "0";
                }
            }
            else
            {
                txthargabeli.Clear();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (cmbsupplier.SelectedValue == null)
            {
                MessageBox.Show("Pilih Supplier terlebih dahulu!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (cmbsparepart.SelectedValue == null)
            {
                MessageBox.Show("Pilih Sparepart terlebih dahulu!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtqty.Text) || !int.TryParse(txtqty.Text, out int qty) || qty <= 0)
            {
                MessageBox.Show("Jumlah (Qty) harus berupa angka lebih dari 0!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int supplierId = Convert.ToInt32(cmbsupplier.SelectedValue);
            int sparepartId = Convert.ToInt32(cmbsparepart.SelectedValue);
            decimal hargaBeli = Convert.ToDecimal(string.IsNullOrEmpty(txthargabeli.Text) ? "0" : txthargabeli.Text);
            DateTime tanggal = Convert.ToDateTime(string.IsNullOrEmpty(txttanggal.Text) ? DateTime.Now.ToString() : txttanggal.Text);

            SqlConnection conn = konn.Getconn();
            conn.Open();

            SqlTransaction transaction = conn.BeginTransaction();

            try
            {
                string queryInsert = @"INSERT INTO barang_masuk 
                              (sparepart_id, supplier_id, qty, harga_beli, diterima_pada, dicatat_oleh) 
                              VALUES 
                              (@sparepart_id, @supplier_id, @qty, @harga_beli, @diterima_pada, @dicatat_oleh)";

                SqlCommand cmdInsert = new SqlCommand(queryInsert, conn, transaction);
                cmdInsert.Parameters.AddWithValue("@sparepart_id", sparepartId);
                cmdInsert.Parameters.AddWithValue("@supplier_id", supplierId);
                cmdInsert.Parameters.AddWithValue("@qty", qty);
                cmdInsert.Parameters.AddWithValue("@harga_beli", hargaBeli);
                cmdInsert.Parameters.AddWithValue("@diterima_pada", tanggal);

                cmdInsert.Parameters.AddWithValue("@dicatat_oleh", usersession.UserId);

                cmdInsert.ExecuteNonQuery();

                string queryUpdateStok = @"UPDATE sparepart 
                                   SET stok = stok + @qty 
                                   WHERE id = @sparepart_id";

                SqlCommand cmdUpdate = new SqlCommand(queryUpdateStok, conn, transaction);
                cmdUpdate.Parameters.AddWithValue("@qty", qty);
                cmdUpdate.Parameters.AddWithValue("@sparepart_id", sparepartId);

                cmdUpdate.ExecuteNonQuery();

                string queryLog = @"INSERT INTO audit_log (user_id, aksi, waktu) 
                                    VALUES (@user_id, @aksi, GETDATE())";

                SqlCommand cmdLog = new SqlCommand(queryLog, conn, transaction);
                cmdLog.Parameters.AddWithValue("@user_id", usersession.UserId);
                cmdLog.Parameters.AddWithValue("@aksi", $"Menambahkan barang masuk: {cmbsparepart.Text} ({qty} buah)");
                cmdLog.ExecuteNonQuery();

                transaction.Commit();

                MessageBox.Show("Barang masuk berhasil dicatat dan stok sparepart telah diperbarui!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                ClearInput();
                TampilDataBarangMasuk();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                MessageBox.Show("Gagal memproses transaksi: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void BarangMasuk_Load(object sender, EventArgs e)
        {
            txttanggal.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            txttanggal.ReadOnly = true;

            timer1.Interval = 1000;
            timer1.Tick += timer1_Tick;
            timer1.Start();

            UpdateWaktuRealtime();

            cmbsparepart.SelectedIndexChanged += cmbsparepart_SelectedIndexChanged;
            cmbsupplier.SelectedIndexChanged += cmbsupplier_SelectedIndexChanged;

            LoadSupplier();
            LoadSparepart(0);
            TampilDataBarangMasuk();
        }

        private void UpdateWaktuRealtime()
        {
            txttanggal.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        private void cmbsupplier_SelectedIndexChanged(object sender, EventArgs e)
        {
            txthargabeli.Clear();

            if (cmbsupplier.SelectedIndex != -1 && cmbsupplier.SelectedValue != null && cmbsupplier.SelectedValue is int)
            {
                int supplierId = Convert.ToInt32(cmbsupplier.SelectedValue);
                LoadSparepart(supplierId);
            }
            else if (cmbsupplier.SelectedIndex != -1 && cmbsupplier.SelectedValue is DataRowView drv)
            {
                int supplierId = Convert.ToInt32(drv["id"]);
                LoadSparepart(supplierId);
            }
            else
            {
                LoadSparepart(0);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateWaktuRealtime();
        }
    }
}
