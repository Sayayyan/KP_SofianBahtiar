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
    public partial class TambahCustomer : Form
    {
        Koneksi konn = new Koneksi();
        private SqlDataReader dr;
        private SqlDataAdapter da;
        private SqlCommand cmd;
        private DataSet ds;

        void kondisiawal()
        {
            txtnama.Text = "";
            txtnohp.Text = "";
            txtalamat.Text = "";
        }

        public TambahCustomer()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtnama.Text) || string.IsNullOrEmpty(txtalamat.Text) || string.IsNullOrEmpty(txtnohp.Text))
            {
                MessageBox.Show("Pastikan semua data terisi", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                try
                {
                    using (SqlConnection conn = konn.Getconn())
                    {
                        conn.Open();

                        string namaInput = txtnama.Text.Trim();
                        string noHpInput = txtnohp.Text.Trim();
                        string alamatInput = txtalamat.Text.Trim();

                        string query = "INSERT INTO CUSTOMERS (nama, no_hp, alamat, dibuat_pada) VALUES (@nama, @nohp, @alamat, @dibuat_pada)";

                        using (cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@nama", namaInput);
                            cmd.Parameters.AddWithValue("@nohp", noHpInput);
                            cmd.Parameters.AddWithValue("@alamat", alamatInput);
                            cmd.Parameters.AddWithValue("@dibuat_pada", DateTime.Now);

                            cmd.ExecuteNonQuery();
                        }

                        string queryLog = @"INSERT INTO audit_log (user_id, aksi, waktu) 
                                            VALUES (@user_id, @aksi, GETDATE())";

                        using (SqlCommand cmdLog = new SqlCommand(queryLog, conn))
                        {
                            cmdLog.Parameters.AddWithValue("@user_id", usersession.UserId);
                            cmdLog.Parameters.AddWithValue("@aksi", $"Menambahkan customer: {namaInput}");
                            cmdLog.ExecuteNonQuery();
                        }

                        MessageBox.Show("Data customer berhasil ditambahkan!", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        this.DialogResult = DialogResult.OK;
                        kondisiawal();
                        this.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal menambahkan customer: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void TambahCustomer_Load(object sender, EventArgs e)
        {

        }
    }
}