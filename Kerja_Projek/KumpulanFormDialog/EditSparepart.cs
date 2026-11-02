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
    public partial class EditSparepart : Form
    {
        Koneksi konn = new Koneksi();
        private SqlDataReader dr;
        private SqlCommand cmd;
        private int sparepartId;

        public EditSparepart(int id)
        {
            InitializeComponent();
            this.sparepartId = id;
        }

        private void EditSparepart_Load(object sender, EventArgs e)
        {
            LoadSupplier();
            LoadDataSparepart();
        }

        private void LoadSupplier()
        {
            SqlConnection conn = konn.Getconn();
            try
            {
                conn.Open();
                string query = "SELECT id, nama FROM suppliers";
                cmd = new SqlCommand(query, conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                cmbsupplier.DataSource = dt;
                cmbsupplier.DisplayMember = "nama";
                cmbsupplier.ValueMember = "id";
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

        private void LoadDataSparepart()
        {
            SqlConnection conn = konn.Getconn();
            try
            {
                conn.Open();
                string query = "SELECT kode, nama, harga_beli, harga_jual, stok, stok_minimum, supplier_id FROM sparepart WHERE id = @id";
                cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", sparepartId);

                dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    txtkode.Text = dr["kode"].ToString();
                    txtnama.Text = dr["nama"].ToString();
                    txthargabeli.Text = dr["harga_beli"].ToString();
                    txthargajual.Text = dr["harga_jual"].ToString();
                    txtstok.Text = dr["stok"].ToString();
                    txtstokmin.Text = dr["stok_minimum"].ToString();

                    if (dr["supplier_id"] != DBNull.Value)
                    {
                        cmbsupplier.SelectedValue = Convert.ToInt32(dr["supplier_id"]);
                    }
                }
                dr.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat data sparepart: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (cmbsupplier.SelectedValue == null)
            {
                MessageBox.Show("Pilih Supplier terlebih dahulu!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtnama.Text))
            {
                MessageBox.Show("Nama spare part wajib diisi!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtnama.Focus();
                return;
            }

            SqlConnection conn = konn.Getconn();
            try
            {
                conn.Open();

                string query = @"UPDATE sparepart 
                                 SET kode = @kode, nama = @nama, harga_beli = @harga_beli, harga_jual = @harga_jual, 
                                     stok = @stok, stok_minimum = @stok_minimum, supplier_id = @supplier_id 
                                 WHERE id = @id";

                cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@kode", txtkode.Text.Trim());
                cmd.Parameters.AddWithValue("@nama", txtnama.Text.Trim());
                cmd.Parameters.AddWithValue("@harga_beli", Convert.ToDecimal(string.IsNullOrEmpty(txthargabeli.Text) ? "0" : txthargabeli.Text));
                cmd.Parameters.AddWithValue("@harga_jual", Convert.ToDecimal(string.IsNullOrEmpty(txthargajual.Text) ? "0" : txthargajual.Text));
                cmd.Parameters.AddWithValue("@stok", Convert.ToInt32(string.IsNullOrEmpty(txtstok.Text) ? "0" : txtstok.Text));
                cmd.Parameters.AddWithValue("@stok_minimum", Convert.ToInt32(string.IsNullOrEmpty(txtstokmin.Text) ? "0" : txtstokmin.Text));
                cmd.Parameters.AddWithValue("@supplier_id", Convert.ToInt32(cmbsupplier.SelectedValue));
                cmd.Parameters.AddWithValue("@id", sparepartId);

                int result = cmd.ExecuteNonQuery();

                if (result > 0)
                {
                    SqlCommand cmdLog = new SqlCommand("INSERT INTO audit_log (user_id, aksi, waktu) VALUES (@user_id, @aksi, GETDATE())", conn);
                    cmdLog.Parameters.AddWithValue("@user_id", usersession.UserId);
                    cmdLog.Parameters.AddWithValue("@aksi", $"Mengedit sparepart: {txtnama.Text.Trim()}");
                    cmdLog.ExecuteNonQuery();

                    MessageBox.Show("Data spare part berhasil diperbarui!", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memperbarui data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
