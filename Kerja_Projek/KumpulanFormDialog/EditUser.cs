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
    public partial class EditUser : Form
    {
        Koneksi konn = new Koneksi();
        private SqlDataAdapter da;
        private SqlDataReader dr;
        private SqlCommand cmd;
        private DataSet ds;
        private int userId;

        public EditUser(int id)
        {
            InitializeComponent();
            this.userId = id;
        }

        private void EditUser_Load(object sender, EventArgs e)
        {
            IsiComboRole();
            LoadDataUser();
        }

        private void IsiComboRole()
        {
            cmbrole.Items.Clear();
            cmbrole.Items.Add("Admin");
            cmbrole.Items.Add("Frontdesk");
            cmbrole.Items.Add("Gudang");
            cmbrole.Items.Add("Kasir");
            cmbrole.Items.Add("Mekanik");
        }

        private void LoadDataUser()
        {
            SqlConnection conn = konn.Getconn();
            try
            {
                conn.Open();
                string query = "SELECT nama_lengkap, username, role_id FROM users WHERE id = @id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", userId);

                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    txtnama.Text = dr["nama_lengkap"].ToString();
                    txtusername.Text = dr["username"].ToString();

                    int roleId = Convert.ToInt32(dr["role_id"]);
                    if (roleId >= 1 && roleId <= 5)
                    {
                        cmbrole.SelectedIndex = roleId - 1;
                    }
                }
                dr.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat data user: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }
        }

        private void btnedit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtnama.Text) || string.IsNullOrWhiteSpace(txtusername.Text) || cmbrole.SelectedIndex == -1)
            {
                MessageBox.Show("Nama, Username, dan Role tidak boleh kosong!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SqlConnection conn = konn.Getconn();
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;

                int roleId = cmbrole.SelectedIndex + 1;

                if (!string.IsNullOrWhiteSpace(txtpassword.Text))
                {
                    cmd.CommandText = "UPDATE users SET nama_lengkap = @nama, username = @username, role_id = @role, password_hash = @password WHERE id = @id";
                    cmd.Parameters.AddWithValue("@password", txtpassword.Text);
                }
                else
                {
                    cmd.CommandText = "UPDATE users SET nama_lengkap = @nama, username = @username, role_id = @role WHERE id = @id";
                }

                cmd.Parameters.AddWithValue("@nama", txtnama.Text);
                cmd.Parameters.AddWithValue("@username", txtusername.Text);
                cmd.Parameters.AddWithValue("@role", roleId);
                cmd.Parameters.AddWithValue("@id", userId);

                cmd.ExecuteNonQuery();

                string queryLog = @"INSERT INTO audit_log (user_id, aksi, waktu) 
                                    VALUES (@user_id, @aksi, GETDATE())";

                using (SqlCommand cmdLog = new SqlCommand(queryLog, conn))
                {
                    cmdLog.Parameters.AddWithValue("@user_id", usersession.UserId);
                    cmdLog.Parameters.AddWithValue("@aksi", $"Mengedit user: {txtnama.Text.Trim()}");
                    cmdLog.ExecuteNonQuery();
                }

                MessageBox.Show("Data user berhasil diperbarui!", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal update data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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