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
    public partial class TambahUser : Form
    {
        Koneksi konn = new Koneksi();
        private SqlDataReader dr;
        private SqlDataAdapter da;
        private SqlCommand cmd;
        private DataSet ds;

        void kondisiawal()
        {
            txtnama.Text = "";
            txtpassword.Text = "";
            txtusername.Text = "";
        }
        public TambahUser()
        {
            InitializeComponent();
        }

        private void TambahUser_Load(object sender, EventArgs e)
        {
            cmbrole.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string namalengkap = txtnama.Text.Trim();
            string username = txtusername.Text.Trim();
            string password = txtpassword.Text.Trim();

            if (string.IsNullOrEmpty(namalengkap) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || cmbrole.SelectedIndex == -1)
            {
                MessageBox.Show("Semua kolom (Nama, Username, Password, Role) harus diisi!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string passwordpolos = txtpassword.Text.Trim();

            string roleDipilih = cmbrole.SelectedItem.ToString().ToUpper();
            int roleId = 0;

            switch (roleDipilih)
            {
                case "ADMIN":
                    roleId = 1;
                    break;
                case "FRONTDESK":
                    roleId = 2;
                    break;
                case "GUDANG":
                    roleId = 3;
                    break;
                case "KASIR":
                    roleId = 4;
                    break;
                case "MEKANIK":
                    roleId = 5;
                    break;
                default:
                    MessageBox.Show("Role tidak valid!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
            }

            string passwordhash = hashpassword.HashpasswordSHA256(passwordpolos);

            try
            {
                using (SqlConnection conn = konn.Getconn())
                {
                    conn.Open();

                    string query = @"INSERT INTO users (nama_lengkap, username, password_hash, role_id, is_aktif, dibuat_pada) 
                                     VALUES (@nama_lengkap, @username, @password_hash, @role_id, @is_aktif, @dibuat_pada)";

                    using (cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@nama_lengkap", namalengkap);
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password_hash", passwordhash);
                        cmd.Parameters.AddWithValue("@role_id", roleId);
                        cmd.Parameters.AddWithValue("@is_aktif", 1);
                        cmd.Parameters.AddWithValue("@dibuat_pada", DateTime.Now);

                        cmd.ExecuteNonQuery();
                    }

                    string queryLog = @"INSERT INTO audit_log (user_id, aksi, waktu) 
                                        VALUES (@user_id, @aksi, GETDATE())";

                    using (SqlCommand cmdLog = new SqlCommand(queryLog, conn))
                    {
                        cmdLog.Parameters.AddWithValue("@user_id", usersession.UserId);
                        cmdLog.Parameters.AddWithValue("@aksi", $"Menambahkan user: {namalengkap}");
                        cmdLog.ExecuteNonQuery();
                    }

                    MessageBox.Show("Data user berhasil disimpan!", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                kondisiawal();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal menyimpan data: " + ex.Message, "Error Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
