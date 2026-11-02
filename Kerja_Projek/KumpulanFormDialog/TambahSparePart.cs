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

namespace Kerja_Projek.KumpulanFormDialog
{
    public partial class TambahSparePart : Form
    {
        Koneksi konn = new Koneksi();
        private SqlDataAdapter da;
        private SqlDataReader dr;
        private DataSet ds;
        private SqlCommand cmd;

        public TambahSparePart()
        {
            InitializeComponent();
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

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
                MessageBox.Show("Gagal memuat data supplier: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }
        }
        private void AutoKode()
        {
            SqlConnection conn = konn.Getconn();
            try
            {
                conn.Open();
                string query = "SELECT TOP 1 kode FROM sparepart WHERE kode LIKE 'SP-%' ORDER BY kode DESC";
                cmd = new SqlCommand(query, conn);
                dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    string kodeTerakhir = dr["kode"].ToString();

                    int urutan = Convert.ToInt32(kodeTerakhir.Substring(3)) + 1;

                    txtkode.Text = "SP-" + urutan.ToString("D3");
                }
                else
                {
                    txtkode.Text = "SP-001";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal meng-generate kode otomatis: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (dr != null && !dr.IsClosed) dr.Close();
                if (conn.State == ConnectionState.Open) conn.Close();
            }
        }

        private void TambahSparePart_Load(object sender, EventArgs e)
        {
            AutoKode();
            LoadSupplier();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (cmbsupplier.SelectedValue == null)
            {
                MessageBox.Show("Pilih Supplier terlebih dahulu!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtkode.Text) || string.IsNullOrWhiteSpace(txtnama.Text))
            {
                MessageBox.Show("Kode dan Nama spare part wajib diisi!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SqlConnection conn = konn.Getconn();

            string query = @"INSERT INTO sparepart 
                    (kode, nama, harga_beli, harga_jual, stok, stok_minimum, supplier_id) 
                    VALUES 
                    (@kode, @nama, @harga_beli, @harga_jual, @stok, @stok_minimum, @supplier_id)";

            try
            {
                cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@kode", txtkode.Text.Trim());
                cmd.Parameters.AddWithValue("@nama", txtnama.Text.Trim());

                cmd.Parameters.AddWithValue("@harga_beli", Convert.ToDecimal(string.IsNullOrEmpty(txthargabeli.Text) ? "0" : txthargabeli.Text));
                cmd.Parameters.AddWithValue("@harga_jual", Convert.ToDecimal(string.IsNullOrEmpty(txthargajual.Text) ? "0" : txthargajual.Text));
                cmd.Parameters.AddWithValue("@stok", Convert.ToInt32(string.IsNullOrEmpty(txtstok.Text) ? "0" : txtstok.Text));
                cmd.Parameters.AddWithValue("@stok_minimum", Convert.ToInt32(string.IsNullOrEmpty(txtstokmin.Text) ? "0" : txtstokmin.Text));

                cmd.Parameters.AddWithValue("@supplier_id", Convert.ToInt32(cmbsupplier.SelectedValue));

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    string queryLog = @"INSERT INTO audit_log (user_id, aksi, waktu) 
                                        VALUES (@user_id, @aksi, GETDATE())";

                    using (SqlCommand cmdLog = new SqlCommand(queryLog, conn))
                    {
                        cmdLog.Parameters.AddWithValue("@user_id", usersession.UserId);
                        cmdLog.Parameters.AddWithValue("@aksi", $"Menambahkan sparepart: {txtnama.Text.Trim()} ({txtkode.Text.Trim()})");
                        cmdLog.ExecuteNonQuery();
                    }

                    MessageBox.Show("Data spare part berhasil disimpan!", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal menyimpan data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmbsupplier_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}