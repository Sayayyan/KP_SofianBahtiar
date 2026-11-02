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
    public partial class SerahTerima : Form
    {
        Koneksi konn = new Koneksi();
        private SqlDataAdapter da;
        private SqlDataReader dr;
        private SqlCommand cmd;
        private DataSet ds;
        private int woId = 0;
        private string nomorWo = "";

        public int WoId
        {
            get { return woId; }
        }

        public SerahTerima()
        {
            InitializeComponent();
            this.button1.Click += new System.EventHandler(this.button1_Click);
        }

        public SerahTerima(int id, string nomor) : this()
        {
            this.woId = id;
            this.nomorWo = nomor;
        }

        private void SerahTerima_Load(object sender, EventArgs e)
        {
            txtnomorwo.Clear();
            if (woId > 0)
            {
                txtnomorwo.Text = nomorWo;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string namaPengambil = txtpengambil.Text.Trim();

            if (string.IsNullOrWhiteSpace(namaPengambil))
            {
                MessageBox.Show("Nama pengambil wajib diisi!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtpengambil.Focus();
                return;
            }

            if (woId <= 0)
            {
                MessageBox.Show("Work Order tidak valid atau belum terpilih!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection conn = konn.Getconn())
                {
                    conn.Open();

                    string query = "UPDATE work_orders SET status = 'Diserahkan', pengambil = @pengambil WHERE id = @id";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", woId);
                        cmd.Parameters.AddWithValue("@pengambil", namaPengambil);
                        cmd.ExecuteNonQuery();
                    }

                    string nomor = nomorWo;
                    if (string.IsNullOrWhiteSpace(nomor))
                    {
                        using (SqlCommand cmdNo = new SqlCommand("SELECT nomor_wo FROM work_orders WHERE id = @wo_id", conn))
                        {
                            cmdNo.Parameters.AddWithValue("@wo_id", woId);
                            object hasilNo = cmdNo.ExecuteScalar();
                            if (hasilNo != null && hasilNo != DBNull.Value)
                            {
                                nomor = hasilNo.ToString();
                            }
                        }
                    }

                    string queryLog = @"INSERT INTO audit_log (user_id, aksi, waktu) 
                                        VALUES (@user_id, @aksi, GETDATE())";
                    using (SqlCommand cmdLog = new SqlCommand(queryLog, conn))
                    {
                        cmdLog.Parameters.AddWithValue("@user_id", usersession.UserId);
                        cmdLog.Parameters.AddWithValue("@aksi", $"Menyerahkan kendaraan {nomor}");
                        cmdLog.ExecuteNonQuery();
                    }

                    MessageBox.Show("Kendaraan berhasil diserahkan ke " + namaPengambil + "!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal menyimpan serah terima: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}