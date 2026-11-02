using Kerja_Projek.KumpulanFormDialog;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Kerja_Projek
{
    public partial class DataSupplier : Form
    {
        Koneksi konn = new Koneksi();

        public DataSupplier()
        {
            InitializeComponent();
        }

        private void DataSupplier_Load(object sender, EventArgs e)
        {
            TampilSuppliers();
        }

        void TampilSuppliers()
        {
            try
            {
                using (SqlConnection conn = konn.Getconn())
                {
                    conn.Open();

                    string query = "SELECT id, nama, kontak, alamat FROM suppliers ORDER BY id DESC";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dataGridView1.DataSource = dt;
                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                    if (dataGridView1.Columns["id"] != null)
                        dataGridView1.Columns["id"].Visible = false;

                    if (dataGridView1.Columns["nama"] != null)
                        dataGridView1.Columns["nama"].HeaderText = "Nama Supplier";

                    if (dataGridView1.Columns["kontak"] != null)
                        dataGridView1.Columns["kontak"].HeaderText = "Kontak / No. Telp";

                    if (dataGridView1.Columns["alamat"] != null)
                        dataGridView1.Columns["alamat"].HeaderText = "Alamat";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (TambahSupplier supplier = new TambahSupplier())
            {
                if (supplier.ShowDialog() == DialogResult.OK)
                {
                    TampilSuppliers();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null && dataGridView1.CurrentRow.Index > -1)
            {
                object idVal = dataGridView1.CurrentRow.Cells["id"].Value;
                if (idVal == null || idVal == DBNull.Value)
                {
                    MessageBox.Show("Data supplier tidak valid!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int id = Convert.ToInt32(idVal);
                using (EditSupplier frmEdit = new EditSupplier(id))
                {
                    if (frmEdit.ShowDialog() == DialogResult.OK)
                    {
                        TampilSuppliers();
                    }
                }
            }
            else
            {
                MessageBox.Show("Silakan pilih data yang ingin diedit terlebih dahulu!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null && dataGridView1.CurrentRow.Index > -1)
            {
                object idVal = dataGridView1.CurrentRow.Cells["id"].Value;
                string nama = dataGridView1.CurrentRow.Cells["nama"].Value?.ToString() ?? "";

                if (idVal == null || idVal == DBNull.Value)
                {
                    MessageBox.Show("Data supplier tidak valid!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult dialogResult = MessageBox.Show($"Apakah Anda yakin ingin menghapus supplier '{nama}'?", "Konfirmasi Hapus", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dialogResult != DialogResult.Yes)
                {
                    return;
                }

                try
                {
                    using (SqlConnection conn = konn.Getconn())
                    {
                        conn.Open();
                        string query = "DELETE FROM suppliers WHERE id = @id";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@id", Convert.ToInt32(idVal));

                        int result = cmd.ExecuteNonQuery();

                        if (result > 0)
                        {
                            SqlCommand cmdLog = new SqlCommand("INSERT INTO audit_log (user_id, aksi, waktu) VALUES (@user_id, @aksi, GETDATE())", conn);
                            cmdLog.Parameters.AddWithValue("@user_id", usersession.UserId);
                            cmdLog.Parameters.AddWithValue("@aksi", $"Menghapus supplier: {nama}");
                            cmdLog.ExecuteNonQuery();

                            MessageBox.Show("Data supplier berhasil dihapus.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            TampilSuppliers();
                        }
                        else
                        {
                            MessageBox.Show("Data supplier gagal dihapus.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal menghapus data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Silakan pilih data yang ingin dihapus terlebih dahulu!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
    }
}