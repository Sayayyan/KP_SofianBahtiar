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
    public partial class StokSparePart : Form
    {
        Koneksi konn = new Koneksi();
        private SqlDataAdapter da;
        private SqlDataReader dr;
        private SqlCommand cmd;
        private DataSet ds;

        public StokSparePart()
        {
            InitializeComponent();
        }

        public void CariData(string keyword)
        {
            SqlConnection conn = konn.Getconn();
            try
            {
                conn.Open();
                string query = "SELECT kode, nama, harga_beli, harga_jual, stok, stok_minimum FROM sparepart " +
                               "WHERE kode LIKE @cari OR nama LIKE @cari";

                cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@cari", "%" + keyword + "%");

                da = new SqlDataAdapter(cmd);
                ds = new DataSet();
                da.Fill(ds, "sparepart");

                dataGridView1.DataSource = ds.Tables["sparepart"];
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

        public void TampilData()
        {
            SqlConnection conn = konn.Getconn();
            try
            {
                conn.Open();
                string query = "SELECT kode, nama, harga_beli, harga_jual, stok, stok_minimum FROM sparepart";

                cmd = new SqlCommand(query, conn);
                da = new SqlDataAdapter(cmd);
                ds = new DataSet();

                da.Fill(ds, "sparepart");

                dataGridView1.DataSource = ds.Tables["sparepart"];

                dataGridView1.Columns["kode"].HeaderText = "Kode";
                dataGridView1.Columns["nama"].HeaderText = "Nama Sparepart";
                dataGridView1.Columns["harga_beli"].HeaderText = "Harga Beli";
                dataGridView1.Columns["harga_jual"].HeaderText = "Harga Jual";
                dataGridView1.Columns["stok"].HeaderText = "Stok";
                dataGridView1.Columns["stok_minimum"].HeaderText = "Stok Min";
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

        private void StokSparePart_Load(object sender, EventArgs e)
        {
            dataGridView1.CellFormatting += dataGridView1_CellFormatting;

            TampilData();
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || dataGridView1.Rows[e.RowIndex].IsNewRow) return;

            if (dataGridView1.Columns[e.ColumnIndex].Name == "stok")
            {
                var valStok = dataGridView1.Rows[e.RowIndex].Cells["stok"].Value;
                var valStokMin = dataGridView1.Rows[e.RowIndex].Cells["stok_minimum"].Value;

                if (valStok != null && valStok != DBNull.Value &&
                    valStokMin != null && valStokMin != DBNull.Value)
                {
                    int stok = Convert.ToInt32(valStok);
                    int stokMin = Convert.ToInt32(valStokMin);

                    if (stok < stokMin)
                    {
                        e.CellStyle.BackColor = Color.Red;
                        e.CellStyle.ForeColor = Color.White;
                        e.CellStyle.Font = new Font(dataGridView1.Font, FontStyle.Bold);
                    }
                    else if (stok <= stokMin + 15)
                    {
                        e.CellStyle.BackColor = Color.Gold;
                        e.CellStyle.ForeColor = Color.Black;
                        e.CellStyle.Font = new Font(dataGridView1.Font, FontStyle.Bold);
                    }
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void txtcarisparepart_Enter(object sender, EventArgs e)
        {
            if (txtcarisparepart.Text == "Cari Sparepart")
            {
                txtcarisparepart.Text = "";
                txtcarisparepart.ForeColor = Color.Black;
            }
        }

        private void txtcarisparepart_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtcarisparepart.Text))
            {
                txtcarisparepart.Text = "Cari Sparepart";
                txtcarisparepart.ForeColor = Color.Gray;
            }
        }

        private void txtcarisparepart_TextChanged(object sender, EventArgs e)
        {
            if (txtcarisparepart.Text != "Cari Sparepart" && !string.IsNullOrWhiteSpace(txtcarisparepart.Text))
            {
                CariData(txtcarisparepart.Text);
            }
            else if (txtcarisparepart.Text == "" || txtcarisparepart.Text == "Cari Sparepart")
            {
                TampilData();
            }
        }
    }
}