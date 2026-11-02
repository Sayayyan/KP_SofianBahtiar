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
    public partial class Invoice : Form
    {
        Koneksi konn = new Koneksi();
        private SqlDataReader dr;
        private SqlDataAdapter da;
        private SqlCommand cmd;
        private DataSet ds;
        private int selectedWoId = 0;

        public Invoice()
        {
            InitializeComponent();
        }

        private void LoadSiapTagih()
        {
            try
            {
                using (SqlConnection conn = konn.Getconn())
                {
                    conn.Open();

                    string query = @"SELECT 
                                        wo.id,
                                        wo.nomor_wo, 
                                        (k.kendaraan + ' (' + k.no_polisi + ')') AS kendaraan, 
                                        wo.status 
                                     FROM work_orders wo
                                     INNER JOIN kendaraan k ON wo.kendaraan_id = k.id
                                     WHERE wo.status = 'Selesai'
                                     ORDER BY wo.id DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            dgvsiaptagih.DataSource = dt;

                            if (dgvsiaptagih.Columns["id"] != null)
                                dgvsiaptagih.Columns["id"].Visible = false;

                            if (dgvsiaptagih.Columns["nomor_wo"] != null)
                                dgvsiaptagih.Columns["nomor_wo"].HeaderText = "Nomor WO";

                            if (dgvsiaptagih.Columns["kendaraan"] != null)
                                dgvsiaptagih.Columns["kendaraan"].HeaderText = "Kendaraan";

                            if (dgvsiaptagih.Columns["status"] != null)
                                dgvsiaptagih.Columns["status"].HeaderText = "Status";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat data siap tagih: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadRincianTagihan(int woId)
        {
            try
            {
                using (SqlConnection conn = konn.Getconn())
                {
                    conn.Open();

                    string query = @"SELECT 'Jasa' AS Jenis, j.nama_jasa AS Nama, 1 AS Qty, wj.tarif_saat_transaksi AS Harga
                                     FROM wo_jasa wj
                                     INNER JOIN jasa j ON wj.jasa_id = j.id
                                     WHERE wj.wo_id = @wo_id
                                     UNION ALL
                                     SELECT 'Sparepart', sp.nama, wsp.qty, wsp.harga_saat_transaksi
                                     FROM wo_sparepart wsp
                                     INNER JOIN sparepart sp ON wsp.sparepart_id = sp.id
                                     WHERE wsp.wo_id = @wo_id AND wsp.status_request = 'Diterima'";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@wo_id", woId);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            dgvrinciantagihan.DataSource = dt;

                            if (dgvrinciantagihan.Columns["Jenis"] != null)
                                dgvrinciantagihan.Columns["Jenis"].HeaderText = "Jenis";
                            if (dgvrinciantagihan.Columns["Nama"] != null)
                                dgvrinciantagihan.Columns["Nama"].HeaderText = "Nama";
                            if (dgvrinciantagihan.Columns["Qty"] != null)
                                dgvrinciantagihan.Columns["Qty"].HeaderText = "Qty";
                            if (dgvrinciantagihan.Columns["Harga"] != null)
                                dgvrinciantagihan.Columns["Harga"].HeaderText = "Harga";

                            decimal total = 0;
                            foreach (DataRow r in dt.Rows)
                            {
                                int qty = Convert.ToInt32(r["Qty"]);
                                decimal harga = Convert.ToDecimal(r["Harga"]);
                                total += qty * harga;
                            }

                            lbltotal.Text = total.ToString("N0");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat rincian tagihan: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvsiaptagih_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = dgvsiaptagih.Rows[e.RowIndex];
            if (row.Cells["id"].Value == null || row.Cells["id"].Value == DBNull.Value) return;

            selectedWoId = Convert.ToInt32(row.Cells["id"].Value);
            LoadRincianTagihan(selectedWoId);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (selectedWoId <= 0)
            {
                MessageBox.Show("Silakan pilih Work Order yang akan dibayar terlebih dahulu!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Pembayaran lanjut = new Pembayaran(selectedWoId);
            lanjut.ShowDialog();
            LoadSiapTagih();
            dgvrinciantagihan.DataSource = null;
            lbltotal.Text = "-";
            selectedWoId = 0;
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvsiaptagih_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Invoice_Load(object sender, EventArgs e)
        {
            LoadSiapTagih();
        }
    }
}
