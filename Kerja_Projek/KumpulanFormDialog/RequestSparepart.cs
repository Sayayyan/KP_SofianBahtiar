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
    public partial class RequestSparepart : Form
    {
        Koneksi konn = new Koneksi();
        private SqlDataAdapter da;
        private SqlDataReader dr;
        private SqlCommand cmd;
        private DataSet ds;
        private int woId = 0;

        public RequestSparepart()
        {
            InitializeComponent();
        }

        public RequestSparepart(int id) : this()
        {
            this.woId = id;
        }

        private void datacmbsparepart()
        {
            try
            {
                using (SqlConnection conn = konn.Getconn())
                {
                    conn.Open();

                    string query = "SELECT id, kode, nama, harga_jual, stok FROM sparepart ORDER BY nama ASC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            cmbsparepart.DataSource = dt;
                            cmbsparepart.DisplayMember = "nama";
                            cmbsparepart.ValueMember = "id";

                            cmbsparepart.SelectedIndex = -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat data sparepart: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmbsparepart_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void RequestSparepart_Load(object sender, EventArgs e)
        {
            datacmbsparepart();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (woId <= 0)
            {
                MessageBox.Show("ID Work Order tidak valid!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbsparepart.SelectedValue == null)
            {
                MessageBox.Show("Silakan pilih sparepart terlebih dahulu!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtqty.Text) || !int.TryParse(txtqty.Text.Trim(), out int qty) || qty <= 0)
            {
                MessageBox.Show("Qty harus berupa angka lebih dari 0!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtqty.Focus();
                return;
            }

            int sparepartId = Convert.ToInt32(cmbsparepart.SelectedValue);

            try
            {
                using (SqlConnection conn = konn.Getconn())
                {
                    conn.Open();

                    string query = @"INSERT INTO wo_sparepart (wo_id, sparepart_id, qty, harga_saat_transaksi, status_request)
                                     VALUES (@wo_id, @sparepart_id, @qty, (SELECT harga_jual FROM sparepart WHERE id = @sparepart_id), 'Diproses')";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@wo_id", woId);
                        cmd.Parameters.AddWithValue("@sparepart_id", sparepartId);
                        cmd.Parameters.AddWithValue("@qty", qty);
                        cmd.ExecuteNonQuery();
                    }

                    string updateWo = @"UPDATE work_orders SET status = 'Menunggu sparepart'
                                        WHERE id = @wo_id AND status NOT IN ('Diserahkan', 'Sudah dibayar')";
                    using (SqlCommand cmdWo = new SqlCommand(updateWo, conn))
                    {
                        cmdWo.Parameters.AddWithValue("@wo_id", woId);
                        cmdWo.ExecuteNonQuery();
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
                        cmdLog.Parameters.AddWithValue("@aksi", $"Mengajukan permintaan sparepart: {cmbsparepart.Text} ({nomorWo})");
                        cmdLog.ExecuteNonQuery();
                    }

                    MessageBox.Show("Permintaan sparepart berhasil dikirim ke gudang!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal mengirim permintaan sparepart: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
