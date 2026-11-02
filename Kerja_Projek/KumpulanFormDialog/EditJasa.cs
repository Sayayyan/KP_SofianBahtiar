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
    public partial class EditJasa : Form
    {
        Koneksi konn = new Koneksi();
        private SqlDataReader dr;
        private SqlCommand cmd;
        private int jasaId;

        public EditJasa(int id)
        {
            InitializeComponent();
            this.jasaId = id;
        }

        private void EditJasa_Load(object sender, EventArgs e)
        {
            LoadDataJasa();
        }

        private void LoadDataJasa()
        {
            SqlConnection conn = konn.Getconn();
            try
            {
                conn.Open();
                string query = "SELECT kode, nama_jasa, tarif FROM jasa WHERE id = @id";
                cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", jasaId);

                dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    txtkode.Text = dr["kode"].ToString();
                    txtnamajasa.Text = dr["nama_jasa"].ToString();
                    txttarif.Text = dr["tarif"].ToString();
                }
                dr.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat data jasa: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtnamajasa.Text) || string.IsNullOrWhiteSpace(txttarif.Text))
            {
                MessageBox.Show("Semua data harus diisi!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txttarif.Text, out decimal tarif))
            {
                MessageBox.Show("Tarif harus berupa angka!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SqlConnection conn = konn.Getconn();
            try
            {
                conn.Open();

                string query = "UPDATE jasa SET nama_jasa = @nama_jasa, tarif = @tarif WHERE id = @id";
                cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@nama_jasa", txtnamajasa.Text.Trim());
                cmd.Parameters.AddWithValue("@tarif", tarif);
                cmd.Parameters.AddWithValue("@id", jasaId);

                int result = cmd.ExecuteNonQuery();

                if (result > 0)
                {
                    SqlCommand cmdLog = new SqlCommand("INSERT INTO audit_log (user_id, aksi, waktu) VALUES (@user_id, @aksi, GETDATE())", conn);
                    cmdLog.Parameters.AddWithValue("@user_id", usersession.UserId);
                    cmdLog.Parameters.AddWithValue("@aksi", $"Mengedit Jasa {txtnamajasa.Text.Trim()}");
                    cmdLog.ExecuteNonQuery();

                    MessageBox.Show("Data jasa berhasil diperbarui!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

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
