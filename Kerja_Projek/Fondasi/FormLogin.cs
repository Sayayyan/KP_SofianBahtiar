using Kerja_Projek.Fondasi;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace Kerja_Projek
{
    public partial class FormLogin : Form
    {
        Koneksi konn = new Koneksi();
        private SqlCommand cmd;
        private SqlDataReader dr;

        public FormLogin()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string usernameinput = txtusername.Text.Trim();
            string passwordpolos = txtpassword.Text.Trim();

            if (string.IsNullOrEmpty(usernameinput) || string.IsNullOrEmpty(passwordpolos))
            {
                MessageBox.Show("Username dan Password tidak boleh kosong!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string passwordhashed = hashpassword.HashpasswordSHA256(passwordpolos);

            try
            {
                using (SqlConnection conn = konn.Getconn())
                {
                    conn.Open();

                    string query = @"SELECT id, nama_lengkap, role_id 
                                     FROM users 
                                     WHERE username = @username 
                                       AND password_hash = @password_hash 
                                       AND is_aktif = 1";

                    using (cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", usernameinput);
                        cmd.Parameters.AddWithValue("@password_hash", passwordhashed);

                        using (dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                int idUser = Convert.ToInt32(dr["id"]);
                                string namaLengkap = dr["nama_lengkap"].ToString();
                                int roleId = Convert.ToInt32(dr["role_id"]);

                                usersession.UserId = idUser;
                                usersession.Username = usernameinput;
                                usersession.NamaLengkap = namaLengkap;
                                usersession.RoleId = roleId;

                                dr.Close();

                                string queryAudit = @"INSERT INTO audit_log (user_id, aksi, waktu) 
                                                     VALUES (@user_id, @aksi, GETDATE())";

                                using (SqlCommand cmdAudit = new SqlCommand(queryAudit, conn))
                                {
                                    cmdAudit.Parameters.AddWithValue("@user_id", idUser);
                                    cmdAudit.Parameters.AddWithValue("@aksi", "Login");
                                    cmdAudit.ExecuteNonQuery();
                                }

                                foreach (Form f in Application.OpenForms.Cast<Form>().ToList())
                                {
                                    if (f is MainForm || f is MainFormFrontdesk || f is MainFormGudang || f is MainFormKasir || f is MainFormMekanik)
                                    {
                                        f.Close();
                                    }
                                }

                                switch (roleId)
                                {
                                    case 1:
                                        MainForm formutama = new MainForm();
                                        formutama.Show();
                                        break;

                                    case 2:
                                        MainFormFrontdesk formfrontdesk = new MainFormFrontdesk();
                                        formfrontdesk.Show();
                                        break;

                                    case 3:
                                        MainFormGudang formgudang = new MainFormGudang();
                                        formgudang.Show();
                                        break;

                                    case 4:
                                        MainFormKasir formkasir = new MainFormKasir();
                                        formkasir.Show();
                                        break;

                                    case 5:
                                        MainFormMekanik formmekanik = new MainFormMekanik();
                                        formmekanik.Show();
                                        break;

                                    default:
                                        MessageBox.Show("Role ID tidak dikenali!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        return;
                                }
                                MessageBox.Show($"Login Berhasil! Selamat datang, {namaLengkap}.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                this.Hide();
                            }
                            else
                            {
                                MessageBox.Show("Username atau Password salah, atau akun Anda belum aktif!", "Login Gagal", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Terjadi kesalahan koneksi database: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {

        }

        private void txtusername_TextChanged(object sender, EventArgs e)
        {

        }
    }
}