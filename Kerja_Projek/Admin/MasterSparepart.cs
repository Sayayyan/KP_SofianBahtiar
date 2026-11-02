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

namespace Kerja_Projek.Admin
{
    public partial class MasterSparePart : Form
    {
        Koneksi konn = new Koneksi();
        private SqlDataAdapter da;
        private SqlDataReader dr;
        private DataSet ds;
        private SqlCommand cmd;

        private string placeholderText = "Cari sparepart...";
        public MasterSparePart()
        {
            InitializeComponent();
            this.txtcarisparepart.Enter += new System.EventHandler(this.txtcarisparepart_Enter);
            this.txtcarisparepart.Leave += new System.EventHandler(this.txtcarisparepart_Leave);
        }

        public void TampilData()
        {
            SqlConnection conn = konn.Getconn();
            try
            {
                conn.Open();
                string query = @"SELECT 
                            sp.id,
                            sp.kode, 
                            sp.nama, 
                            sp.harga_beli, 
                            sp.harga_jual, 
                            sp.stok, 
                            sp.stok_minimum,
                            sup.nama AS nama_supplier
                         FROM sparepart sp
                         LEFT JOIN suppliers sup ON sp.supplier_id = sup.id";

                cmd = new SqlCommand(query, conn);
                da = new SqlDataAdapter(cmd);
                ds = new DataSet();

                da.Fill(ds, "sparepart");

                dataGridView1.DataSource = ds.Tables["sparepart"];

                dataGridView1.Columns["id"].Visible = false;
                dataGridView1.Columns["kode"].HeaderText = "Kode";
                dataGridView1.Columns["nama"].HeaderText = "Nama Sparepart";
                dataGridView1.Columns["harga_beli"].HeaderText = "Harga Beli";
                dataGridView1.Columns["harga_jual"].HeaderText = "Harga Jual";
                dataGridView1.Columns["stok"].HeaderText = "Stok";
                dataGridView1.Columns["stok_minimum"].HeaderText = "Stok Min";
                dataGridView1.Columns["nama_supplier"].HeaderText = "Supplier";

                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
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

        private void button1_Click(object sender, EventArgs e)
        {
            using (TambahSparePart tambah = new TambahSparePart())
            {
                if (tambah.ShowDialog() == DialogResult.OK)
                {
                    TampilData();
                }
            }
        }

        private void btnedit_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Silakan pilih data sparepart yang ingin diedit terlebih dahulu!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            object idVal = dataGridView1.CurrentRow.Cells["id"].Value;

            if (idVal == null || idVal == DBNull.Value)
            {
                MessageBox.Show("Data sparepart tidak valid!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int id = Convert.ToInt32(idVal);

            using (EditSparepart frmEdit = new EditSparepart(id))
            {
                if (frmEdit.ShowDialog() == DialogResult.OK)
                {
                    TampilData();
                }
            }
        }

        private void btnhapus_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Silakan pilih data sparepart yang ingin dihapus terlebih dahulu!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            object idVal = dataGridView1.CurrentRow.Cells["id"].Value;
            string nama = dataGridView1.CurrentRow.Cells["nama"].Value == null ? "" : dataGridView1.CurrentRow.Cells["nama"].Value.ToString();

            if (idVal == null || idVal == DBNull.Value)
            {
                MessageBox.Show("Data sparepart tidak valid!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Apakah Anda yakin ingin menghapus sparepart '" + nama + "'?", "Konfirmasi Hapus", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
            {
                return;
            }

            SqlConnection conn = konn.Getconn();
            try
            {
                conn.Open();

                cmd = new SqlCommand("DELETE FROM sparepart WHERE id = @id", conn);
                cmd.Parameters.AddWithValue("@id", Convert.ToInt32(idVal));

                if (cmd.ExecuteNonQuery() > 0)
                {
                    SqlCommand cmdLog = new SqlCommand("INSERT INTO audit_log (user_id, aksi, waktu) VALUES (@user_id, @aksi, GETDATE())", conn);
                    cmdLog.Parameters.AddWithValue("@user_id", usersession.UserId);
                    cmdLog.Parameters.AddWithValue("@aksi", "Menghapus sparepart: " + nama);
                    cmdLog.ExecuteNonQuery();

                    MessageBox.Show("Data sparepart berhasil dihapus!", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    TampilData();
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

        private void MasterSparePart_Load(object sender, EventArgs e)
        {
            TampilData();
            txtcarisparepart.Text = placeholderText;
            txtcarisparepart.ForeColor = Color.Gray;
        }

        private void txtcarisparepart_Enter(object sender, EventArgs e)
        {
            if (txtcarisparepart.Text == placeholderText)
            {
                txtcarisparepart.Text = "";
                txtcarisparepart.ForeColor = Color.Black;
            }
        }

        private void txtcarisparepart_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtcarisparepart.Text))
            {
                txtcarisparepart.Text = placeholderText;
                txtcarisparepart.ForeColor = Color.Gray;
            }
        }

        private void txtcarisparepart_TextChanged(object sender, EventArgs e)
        {
            if (ds != null && ds.Tables["sparepart"] != null)
            {
                string keyword = txtcarisparepart.Text.Trim();

                if (keyword == placeholderText || string.IsNullOrWhiteSpace(keyword))
                {
                    ds.Tables["sparepart"].DefaultView.RowFilter = "";
                }
                else
                {
                    keyword = keyword.Replace("'", "''");

                    string filter = $"kode LIKE '%{keyword}%' OR nama LIKE '%{keyword}%' OR nama_supplier LIKE '%{keyword}%'";

                    ds.Tables["sparepart"].DefaultView.RowFilter = filter;
                }
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
