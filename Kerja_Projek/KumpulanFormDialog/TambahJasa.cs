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
    public partial class TambahJasa : Form
    {
        Koneksi konn = new Koneksi();
        private SqlCommand cmd;
        private SqlDataReader dr;

        public TambahJasa()
        {
            InitializeComponent();
        }

        private void TambahJasa_Load(object sender, EventArgs e)
        {
            AutoKode();
            txtkode.ReadOnly = true;
        }
        private void AutoKode()
        {
            long hitung;
            string urutan;
            SqlConnection conn = konn.Getconn();
            conn.Open();

            cmd = new SqlCommand("SELECT kode FROM jasa WHERE kode IN (SELECT MAX(kode) FROM jasa) ORDER BY kode DESC", conn);
            dr = cmd.ExecuteReader();
            dr.Read();

            if (dr.HasRows)
            {
                hitung = Convert.ToInt64(dr["kode"].ToString().Substring(2)) + 1;
                urutan = "JS" + hitung.ToString("D3");
            }
            else
            {
                urutan = "JS001";
            }

            dr.Close();
            conn.Close();
            txtkode.Text = urutan;
        }
        private void kondisiawal()
        {
            txtnamajasa.Clear();
            txttarif.Clear();
            AutoKode();
            txtnamajasa.Focus();
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

            try
            {
                SqlConnection conn = konn.Getconn();
                conn.Open();

                string query = "INSERT INTO jasa (kode, nama_jasa, tarif) VALUES (@kode, @nama_jasa, @tarif)";
                cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@kode", txtkode.Text);
                cmd.Parameters.AddWithValue("@nama_jasa", txtnamajasa.Text);
                cmd.Parameters.AddWithValue("@tarif", tarif);

                cmd.ExecuteNonQuery();

                string queryLog = @"INSERT INTO audit_log (user_id, aksi, waktu) 
                                        VALUES (@user_id, @aksi, GETDATE())";

                using (SqlCommand cmdLog = new SqlCommand(queryLog, conn))
                {
                    cmdLog.Parameters.AddWithValue("@user_id", usersession.UserId);
                    cmdLog.Parameters.AddWithValue("@aksi", $"Menambahkan Jasa {txtnamajasa.Text}");
                    cmdLog.ExecuteNonQuery();
                }

                MessageBox.Show("Data jasa berhasil disimpan!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                kondisiawal();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal menyimpan data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtkode_TextChanged(object sender, EventArgs e) { }
        private void txtnamajasa_TextChanged(object sender, EventArgs e) { }
        private void txttarif_TextChanged(object sender, EventArgs e) { }
    }
}