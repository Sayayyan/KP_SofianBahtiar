using Kerja_Projek.KumpulanFormDialog;
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

namespace Kerja_Projek
{
    public partial class MasterUser : Form
    {
        Koneksi konn = new Koneksi();
        private SqlDataAdapter da;
        private DataSet ds;

        public MasterUser()
        {
            InitializeComponent();
        }

        private void IsiComboRole()
        {
            cmbrole.Items.Clear();
            cmbrole.Items.Add("Semua Role");
            cmbrole.Items.Add("Admin");
            cmbrole.Items.Add("Frontdesk");
            cmbrole.Items.Add("Gudang");
            cmbrole.Items.Add("Kasir");
            cmbrole.Items.Add("Mekanik");

            cmbrole.SelectedIndex = 0;
        }

        public void TampilData()
        {
            SqlConnection conn = konn.Getconn();
            try
            {
                conn.Open();

                string query = @"SELECT 
                                    id, 
                                    nama_lengkap, 
                                    username,
                                    role_id, 
                                    CASE role_id 
                                        WHEN 1 THEN 'Admin' 
                                        WHEN 2 THEN 'Frontdesk' 
                                        WHEN 3 THEN 'Gudang' 
                                        WHEN 4 THEN 'Kasir' 
                                        WHEN 5 THEN 'Mekanik' 
                                        ELSE '-' 
                                    END AS nama_role,
                                    is_aktif, 
                                    dibuat_pada 
                                 FROM users";

                SqlCommand cmd = new SqlCommand(query, conn);
                da = new SqlDataAdapter(cmd);
                ds = new DataSet();

                da.Fill(ds, "users");

                dataGridView1.DataSource = ds.Tables["users"];

                if (dataGridView1.Columns.Contains("id")) dataGridView1.Columns["id"].Visible = false;
                if (dataGridView1.Columns.Contains("nama_lengkap")) dataGridView1.Columns["nama_lengkap"].HeaderText = "Nama";
                if (dataGridView1.Columns.Contains("username")) dataGridView1.Columns["username"].HeaderText = "Username";
                if (dataGridView1.Columns.Contains("role_id")) dataGridView1.Columns["role_id"].Visible = false;
                if (dataGridView1.Columns.Contains("nama_role")) dataGridView1.Columns["nama_role"].HeaderText = "Role";
                if (dataGridView1.Columns.Contains("is_aktif")) dataGridView1.Columns["is_aktif"].HeaderText = "Status Aktif";
                if (dataGridView1.Columns.Contains("dibuat_pada")) dataGridView1.Columns["dibuat_pada"].HeaderText = "Dibuat Pada";

                dataGridView1.AllowUserToAddRows = false;
                dataGridView1.ReadOnly = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        private int GetRoleIdFromCombo()
        {
            if (cmbrole.SelectedItem == null) return 0;

            string selectedText = cmbrole.SelectedItem.ToString().Trim().ToLower();

            switch (selectedText)
            {
                case "admin": return 1;
                case "frontdesk": return 2;
                case "gudang": return 3;
                case "kasir": return 4;
                case "mekanik": return 5;
                default: return 0;
            }
        }

        private void FilterData()
        {
            if (ds == null || !ds.Tables.Contains("users")) return;

            DataView dv = ds.Tables["users"].DefaultView;
            string filter = "";

            string kataKunci = txtcariuser.Text.Trim().Replace("'", "''");
            if (!string.IsNullOrEmpty(kataKunci) && kataKunci != "Cari User...")
            {
                filter += $"(nama_lengkap LIKE '%{kataKunci}%' OR username LIKE '%{kataKunci}%')";
            }

            int selectedRoleId = GetRoleIdFromCombo();
            if (selectedRoleId > 0)
            {
                if (!string.IsNullOrEmpty(filter)) filter += " AND ";
                filter += $"role_id = {selectedRoleId}";
            }

            dv.RowFilter = filter;
        }

        private void MasterUser_Load(object sender, EventArgs e)
        {
            IsiComboRole();
            TampilData();
            txtcariuser.ForeColor = Color.Gray;
        }

        private void txtcariuser_TextChanged(object sender, EventArgs e)
        {
            FilterData();
        }

        private void cmbrole_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterData();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TambahUser tambah = new TambahUser();
            if (tambah.ShowDialog() == DialogResult.OK)
            {
                TampilData();
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }

        private void txtcariuser_Enter(object sender, EventArgs e)
        {
            if (txtcariuser.Text == "Cari User...")
            {
                txtcariuser.Text = "";
                txtcariuser.ForeColor = Color.Black;
            }
        }

        private void txtcariuser_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtcariuser.Text))
            {
                txtcariuser.Text = "Cari User...";
                txtcariuser.ForeColor = Color.Gray;
            }
        }

        private void btnedit_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null && dataGridView1.CurrentRow.Index > -1)
            {
                int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["id"].Value);

                EditUser frmEdit = new EditUser(id);

                if (frmEdit.ShowDialog() == DialogResult.OK)
                {
                    TampilData();
                }
            }
            else
            {
                MessageBox.Show("Silakan pilih data yang ingin diedit terlebih dahulu!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnhapus_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null && dataGridView1.CurrentRow.Index > -1)
            {
                string id = dataGridView1.CurrentRow.Cells["id"].Value.ToString();
                string namaUser = dataGridView1.CurrentRow.Cells["nama_lengkap"].Value.ToString();

                DialogResult dialogResult = MessageBox.Show($"Apakah Anda yakin ingin menghapus user '{namaUser}'?", "Konfirmasi Hapus", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (dialogResult == DialogResult.Yes)
                {
                    SqlConnection conn = konn.Getconn();
                    try
                    {
                        conn.Open();
                        string query = "DELETE FROM users WHERE id = @id";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@id", id);

                        int result = cmd.ExecuteNonQuery();

                        if (result > 0)
                        {
                            SqlCommand cmdLog = new SqlCommand("INSERT INTO audit_log (user_id, aksi, waktu) VALUES (@user_id, @aksi, GETDATE())", conn);
                            cmdLog.Parameters.AddWithValue("@user_id", usersession.UserId);
                            cmdLog.Parameters.AddWithValue("@aksi", $"Menghapus user: {namaUser}");
                            cmdLog.ExecuteNonQuery();

                            MessageBox.Show("Data user berhasil dihapus.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            TampilData();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Gagal menghapus data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open) conn.Close();
                    }
                }
            }
            else
            {
                MessageBox.Show("Silakan pilih data yang ingin dihapus terlebih dahulu!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
