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
    public partial class MasterJasa : Form
    {
        Koneksi konn = new Koneksi();
        private SqlDataAdapter da;
        private SqlDataReader dr;
        private DataSet ds;
        private SqlCommand cmd;

        private string placeholderText = "Cari jasa...";

        public MasterJasa()
        {
            InitializeComponent();
            this.txtcarijasa.Enter += new System.EventHandler(this.txtcarijasa_Enter);
            this.txtcarijasa.Leave += new System.EventHandler(this.txtcarijasa_Leave);
        }

        private void MasterJasa_Load(object sender, EventArgs e)
        {
            tampiljasa();
            SetPlaceholder();
        }

        void tampiljasa()
        {
            SqlConnection conn = konn.Getconn();
            try
            {
                conn.Open();
                string query = "SELECT id, kode, nama_jasa, tarif FROM jasa";

                cmd = new SqlCommand(query, conn);
                da = new SqlDataAdapter(cmd);
                ds = new DataSet();

                da.Fill(ds, "jasa");

                dataGridView1.DataSource = ds.Tables["jasa"];

                dataGridView1.Columns["id"].Visible = false;
                dataGridView1.Columns["kode"].HeaderText = "Kode";
                dataGridView1.Columns["nama_jasa"].HeaderText = "Nama Jasa";
                dataGridView1.Columns["tarif"].HeaderText = "Tarif";
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

        void CariJasa()
        {
            if (string.IsNullOrWhiteSpace(txtcarijasa.Text) || txtcarijasa.Text == placeholderText)
            {
                tampiljasa();
                return;
            }

            SqlConnection conn = konn.Getconn();
            try
            {
                conn.Open();
                string query = "SELECT id, kode, nama_jasa, tarif FROM jasa " +
                       "WHERE kode LIKE @cari " +
                       "OR nama_jasa LIKE @cari " +
                       "OR CAST(tarif AS VARCHAR(50)) LIKE @cari";

                cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@cari", "%" + txtcarijasa.Text + "%");

                da = new SqlDataAdapter(cmd);
                ds = new DataSet();

                da.Fill(ds, "jasa");
                dataGridView1.DataSource = ds.Tables["jasa"];

                dataGridView1.Columns["id"].Visible = false;
                dataGridView1.Columns["kode"].HeaderText = "Kode";
                dataGridView1.Columns["nama_jasa"].HeaderText = "Nama Jasa";
                dataGridView1.Columns["tarif"].HeaderText = "Tarif";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal mencari data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        private void SetPlaceholder()
        {
            txtcarijasa.Text = placeholderText;
            txtcarijasa.ForeColor = Color.Gray;
        }

        private void txtcarijasa_Enter(object sender, EventArgs e)
        {
            if (txtcarijasa.Text == placeholderText)
            {
                txtcarijasa.Text = "";
                txtcarijasa.ForeColor = Color.Black;
            }
        }

        private void txtcarijasa_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtcarijasa.Text))
            {
                SetPlaceholder();
            }
        }

        private void txtcarijasa_TextChanged(object sender, EventArgs e)
        {
            if (txtcarijasa.Text != placeholderText)
            {
                CariJasa();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TambahJasa tambah = new TambahJasa();
            if (tambah.ShowDialog() == DialogResult.OK)
            {
                tampiljasa();
            }
        }

        private void btnedit_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Silakan pilih data jasa yang ingin diedit terlebih dahulu!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            object idVal = dataGridView1.CurrentRow.Cells["id"].Value;

            if (idVal == null || idVal == DBNull.Value)
            {
                MessageBox.Show("Data jasa tidak valid!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int id = Convert.ToInt32(idVal);

            using (EditJasa frmEdit = new EditJasa(id))
            {
                if (frmEdit.ShowDialog() == DialogResult.OK)
                {
                    tampiljasa();
                }
            }
        }

        private void btnhapus_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Silakan pilih data jasa yang ingin dihapus terlebih dahulu!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            object idVal = dataGridView1.CurrentRow.Cells["id"].Value;
            string nama = dataGridView1.CurrentRow.Cells["nama_jasa"].Value == null ? "" : dataGridView1.CurrentRow.Cells["nama_jasa"].Value.ToString();

            if (idVal == null || idVal == DBNull.Value)
            {
                MessageBox.Show("Data jasa tidak valid!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Apakah Anda yakin ingin menghapus jasa '" + nama + "'?", "Konfirmasi Hapus", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
            {
                return;
            }

            SqlConnection conn = konn.Getconn();
            try
            {
                conn.Open();

                cmd = new SqlCommand("DELETE FROM jasa WHERE id = @id", conn);
                cmd.Parameters.AddWithValue("@id", Convert.ToInt32(idVal));

                if (cmd.ExecuteNonQuery() > 0)
                {
                    SqlCommand cmdLog = new SqlCommand("INSERT INTO audit_log (user_id, aksi, waktu) VALUES (@user_id, @aksi, GETDATE())", conn);
                    cmdLog.Parameters.AddWithValue("@user_id", usersession.UserId);
                    cmdLog.Parameters.AddWithValue("@aksi", "Menghapus Jasa " + nama);
                    cmdLog.ExecuteNonQuery();

                    MessageBox.Show("Data jasa berhasil dihapus!", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    tampiljasa();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal menghapus data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}