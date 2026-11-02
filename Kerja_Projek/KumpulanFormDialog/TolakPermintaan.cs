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
    public partial class TolakPermintaan : Form
    {
        Koneksi konn = new Koneksi();
        private SqlDataReader dr;
        private SqlDataAdapter da;
        private SqlCommand cmd;
        private DataSet ds;
        private int woId = 0;

        public int WoId
        {
            get { return woId; }
        }

        public TolakPermintaan()
        {
            InitializeComponent();
        }

        public TolakPermintaan(int id) : this()
        {
            this.woId = id;
        }

        private void TolakPermintaan_Load(object sender, EventArgs e)
        {
            textBox3.Clear();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (woId <= 0)
            {
                MessageBox.Show("ID Work Order tidak valid!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(textBox3.Text))
            {
                MessageBox.Show("Alasan penolakan wajib diisi!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox3.Focus();
                return;
            }

            try
            {
                using (SqlConnection conn = konn.Getconn())
                {
                    conn.Open();

                    string query = @"UPDATE wo_sparepart SET status_request = 'Ditolak', alasan_tolak = @alasan
                                     WHERE wo_id = @wo_id AND status_request = 'Diproses'";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@wo_id", woId);
                        cmd.Parameters.AddWithValue("@alasan", textBox3.Text.Trim());
                        cmd.ExecuteNonQuery();
                    }

                    using (SqlCommand cmdCek = new SqlCommand(
                        "SELECT COUNT(*) FROM wo_sparepart WHERE wo_id = @wo_id AND status_request = 'Diproses'", conn))
                    {
                        cmdCek.Parameters.AddWithValue("@wo_id", woId);
                        int pending = Convert.ToInt32(cmdCek.ExecuteScalar());

                        if (pending == 0)
                        {
                            using (SqlCommand cmdSt = new SqlCommand(
                                "UPDATE work_orders SET status = 'Dikerjakan' WHERE id = @wo_id AND status = 'Menunggu sparepart'", conn))
                            {
                                cmdSt.Parameters.AddWithValue("@wo_id", woId);
                                cmdSt.ExecuteNonQuery();
                            }
                        }
                    }

                    string nomorWo = "";
                    using (SqlCommand cmdNo = new SqlCommand("SELECT nomor_wo FROM work_orders WHERE id = @wo_id", conn))
                    {
                        cmdNo.Parameters.AddWithValue("@wo_id", woId);
                        object hasilNo = cmdNo.ExecuteScalar();
                        if (hasilNo != null && hasilNo != DBNull.Value)
                        {
                            nomorWo = hasilNo.ToString();
                        }
                    }

                    string queryLog = @"INSERT INTO audit_log (user_id, aksi, waktu) 
                                        VALUES (@user_id, @aksi, GETDATE())";
                    using (SqlCommand cmdLog = new SqlCommand(queryLog, conn))
                    {
                        cmdLog.Parameters.AddWithValue("@user_id", usersession.UserId);
                        cmdLog.Parameters.AddWithValue("@aksi", $"Menolak permintaan sparepart {nomorWo}");
                        cmdLog.ExecuteNonQuery();
                    }

                    MessageBox.Show("Permintaan sparepart ditolak!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal menolak permintaan: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}