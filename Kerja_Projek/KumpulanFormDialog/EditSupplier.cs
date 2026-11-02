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
    public partial class EditSupplier : Form
    {
        Koneksi konn = new Koneksi();
        private SqlDataReader dr;
        private SqlCommand cmd;
        private int supplierId;

        public EditSupplier(int id)
        {
            InitializeComponent();
            this.supplierId = id;
        }

        private void EditSupplier_Load(object sender, EventArgs e)
        {
            LoadDataSupplier();
        }

        private void LoadDataSupplier()
        {
            SqlConnection conn = konn.Getconn();
            try
            {
                conn.Open();
                string query = "SELECT nama, kontak, alamat FROM suppliers WHERE id = @id";
                cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", supplierId);

                dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    txtnama.Text = dr["nama"].ToString();
                    txtkontak.Text = dr["kontak"].ToString();
                    txtalamat.Text = dr["alamat"].ToString();
                }
                dr.Close();
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

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtnama.Text))
            {
                MessageBox.Show("Nama supplier wajib diisi!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtnama.Focus();
                return;
            }

            SqlConnection conn = konn.Getconn();
            try
            {
                conn.Open();

                string query = "UPDATE suppliers SET nama = @nama, kontak = @kontak, alamat = @alamat WHERE id = @id";
                cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@nama", txtnama.Text.Trim());
                cmd.Parameters.AddWithValue("@kontak", txtkontak.Text.Trim());
                cmd.Parameters.AddWithValue("@alamat", txtalamat.Text.Trim());
                cmd.Parameters.AddWithValue("@id", supplierId);

                int result = cmd.ExecuteNonQuery();

                if (result > 0)
                {
                    string queryLog = @"INSERT INTO audit_log (user_id, aksi, waktu) 
                                        VALUES (@user_id, @aksi, GETDATE())";

                    using (SqlCommand cmdLog = new SqlCommand(queryLog, conn))
                    {
                        cmdLog.Parameters.AddWithValue("@user_id", usersession.UserId);
                        cmdLog.Parameters.AddWithValue("@aksi", $"Mengedit supplier: {txtnama.Text.Trim()}");
                        cmdLog.ExecuteNonQuery();
                    }

                    MessageBox.Show("Data supplier berhasil diperbarui!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Data gagal diperbarui.", "Gagal", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Terjadi kesalahan: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
