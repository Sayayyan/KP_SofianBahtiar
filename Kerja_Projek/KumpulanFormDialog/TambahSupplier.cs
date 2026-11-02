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
    public partial class TambahSupplier : Form
    {
        Koneksi konn = new Koneksi();
        private SqlDataAdapter da;
        private SqlDataReader dr;
        private SqlCommand cmd;
        private DataSet ds;
        public TambahSupplier()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtnama.Text))
            {
                MessageBox.Show("Nama supplier wajib diisi!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtnama.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtkontak.Text))
            {
                MessageBox.Show("Kontak supplier wajib diisi!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtkontak.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtalamat.Text))
            {
                MessageBox.Show("Alamat supplier wajib diisi!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtalamat.Focus();
                return;
            }

            SqlConnection conn = konn.Getconn();
            try
            {
                conn.Open();

                string query = "INSERT INTO suppliers (nama, kontak, alamat) VALUES (@nama, @kontak, @alamat)";
                cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@nama", txtnama.Text.Trim());
                cmd.Parameters.AddWithValue("@kontak", txtkontak.Text.Trim());
                cmd.Parameters.AddWithValue("@alamat", txtalamat.Text.Trim());

                int result = cmd.ExecuteNonQuery();

                if (result > 0)
                {
                    string queryLog = @"INSERT INTO audit_log (user_id, aksi, waktu) 
                                        VALUES (@user_id, @aksi, GETDATE())";

                    using (SqlCommand cmdLog = new SqlCommand(queryLog, conn))
                    {
                        cmdLog.Parameters.AddWithValue("@user_id", usersession.UserId);
                        cmdLog.Parameters.AddWithValue("@aksi", $"Menambahkan supplier: {txtnama.Text.Trim()}");
                        cmdLog.ExecuteNonQuery();
                    }

                    MessageBox.Show("Data supplier berhasil disimpan!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Data gagal disimpan.", "Gagal", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Terjadi kesalahan: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
            }
        }

        private void TambahSupplier_Load(object sender, EventArgs e)
        {

        }

        private void txtalamat_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtkontak_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtnama_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
