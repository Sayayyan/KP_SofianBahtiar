using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kerja_Projek.KumpulanFormDialog
{
    public partial class TambahKendaraan : Form
    {
        Koneksi konn = new Koneksi();
        private SqlDataAdapter da;
        private SqlDataReader dr;
        private DataSet ds;
        private SqlCommand cmd;
        public int CustomerId { get; set; }

        public TambahKendaraan(int customerId) : this()
        {
            this.CustomerId = customerId;
        }

        public TambahKendaraan()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtnopol.Text) ||
                string.IsNullOrEmpty(txtkendaraan.Text) ||
                string.IsNullOrEmpty(txttahun.Text))
            {
                MessageBox.Show("Harap lengkapi semua data!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txttahun.Text.Trim(), out int tahun))
            {
                MessageBox.Show("Tahun harus berupa angka!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (CustomerId <= 0)
            {
                MessageBox.Show("ID customer belum teridentifikasi", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                using (SqlConnection conn = konn.Getconn())
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    string nopolInput = txtnopol.Text.Trim();
                    string kendaraanInput = txtkendaraan.Text.Trim();

                    string query = @"INSERT INTO kendaraan (customer_id, no_polisi, kendaraan, tahun)
                                     VALUES (@customer, @nopol, @kendaraan, @tahun)";

                    using (cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@customer", CustomerId);
                        cmd.Parameters.AddWithValue("@nopol", nopolInput);
                        cmd.Parameters.AddWithValue("@kendaraan", kendaraanInput);
                        cmd.Parameters.AddWithValue("@tahun", tahun);

                        cmd.ExecuteNonQuery();
                    }

                    string queryLog = @"INSERT INTO audit_log (user_id, aksi, waktu) 
                                        VALUES (@user_id, @aksi, GETDATE())";

                    using (SqlCommand cmdLog = new SqlCommand(queryLog, conn))
                    {
                        cmdLog.Parameters.AddWithValue("@user_id", usersession.UserId);
                        cmdLog.Parameters.AddWithValue("@aksi", $"Menambahkan kendaraan baru ({kendaraanInput} - {nopolInput}) untuk ID Customer: {CustomerId}");
                        cmdLog.ExecuteNonQuery();
                    }

                    MessageBox.Show("Data kendaraan berhasil ditambahkan!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal menyimpan data kendaraan: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TambahKendaraan_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}