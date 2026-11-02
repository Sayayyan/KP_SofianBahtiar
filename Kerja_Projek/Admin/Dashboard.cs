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
using System.Windows.Forms.DataVisualization.Charting;

namespace Kerja_Projek
{
    public partial class Dashboard : Form
    {
        Koneksi konn = new Koneksi();
        private SqlDataReader dr;
        private SqlDataAdapter da;
        private DataSet ds;
        private SqlCommand cmd;
        private DataGridView dgvStokMenipis;

        public Dashboard()
        {
            InitializeComponent();
        }

        private void Dashboard_Load(object sender, EventArgs e)
        {
            hitungWoHariIni();
            hitungDikerjakan();
            hitungOmzet();
            hitungSiapDiAmbil();
            tampilGrafikPendapatan();
            tampilStokMenipis();
            tampilWorkOrderTerbaru();
        }

        private void hitungWoHariIni()
        {
            try
            {
                using (SqlConnection conn = konn.Getconn())
                {
                    conn.Open();
                    cmd = new SqlCommand("SELECT COUNT(id) AS Jumlah FROM work_orders WHERE CAST(dibuat_pada AS DATE) = @date", conn);
                    cmd.Parameters.AddWithValue("@date", DateTime.Now.Date);
                    object hasil = cmd.ExecuteScalar();
                    int jumlah = (hasil == null || hasil == DBNull.Value) ? 0 : Convert.ToInt32(hasil);
                    lblJumlahWo.Text = jumlah.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat jumlah WO hari ini: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void hitungDikerjakan()
        {
            try
            {
                using (SqlConnection conn = konn.Getconn())
                {
                    conn.Open();
                    cmd = new SqlCommand("SELECT COUNT(id) AS Jumlah FROM work_orders WHERE status IN ('Baru', 'Dikerjakan', 'Menunggu sparepart')", conn);
                    object hasil = cmd.ExecuteScalar();
                    int jumlah = (hasil == null || hasil == DBNull.Value) ? 0 : Convert.ToInt32(hasil);
                    lblDikerjakan.Text = jumlah.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat data dikerjakan: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void hitungOmzet()
        {
            try
            {
                using (SqlConnection conn = konn.Getconn())
                {
                    conn.Open();
                    cmd = new SqlCommand("SELECT ISNULL(SUM(total_bayar), 0) AS Total FROM pembayaran WHERE CAST(dibayar_pada AS DATE) = @date", conn);
                    cmd.Parameters.AddWithValue("@date", DateTime.Now.Date);
                    object hasil = cmd.ExecuteScalar();
                    decimal total = (hasil == null || hasil == DBNull.Value) ? 0 : Convert.ToDecimal(hasil);
                    lblOmzet.Text = total.ToString("N0");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat omzet: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void hitungSiapDiAmbil()
        {
            try
            {
                using (SqlConnection conn = konn.Getconn())
                {
                    conn.Open();
                    cmd = new SqlCommand("SELECT COUNT(id) AS Jumlah FROM work_orders WHERE status = 'Selesai'", conn);
                    object hasil = cmd.ExecuteScalar();
                    int jumlah = (hasil == null || hasil == DBNull.Value) ? 0 : Convert.ToInt32(hasil);
                    lblsiapdiambil.Text = jumlah.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat data siap diambil: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tampilGrafikPendapatan()
        {
            try
            {
                using (SqlConnection conn = konn.Getconn())
                {
                    conn.Open();

                    DateTime awalWaktu = DateTime.Now.Date.AddDays(-6);
                    DateTime akhirWaktu = DateTime.Now.Date;

                    string query = @"SELECT CAST(dibayar_pada AS DATE) AS Tanggal, 
                                            ISNULL(SUM(total_bayar), 0) AS Total 
                                     FROM pembayaran 
                                     WHERE CAST(dibayar_pada AS DATE) BETWEEN @awalWaktu AND @akhirWaktu
                                     GROUP BY CAST(dibayar_pada AS DATE)";

                    cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@awalWaktu", awalWaktu);
                    cmd.Parameters.AddWithValue("@akhirWaktu", akhirWaktu);

                    Dictionary<DateTime, decimal> dataOmzet = new Dictionary<DateTime, decimal>();
                    dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        DateTime tgl = Convert.ToDateTime(dr["Tanggal"]).Date;
                        decimal total = Convert.ToDecimal(dr["Total"]);
                        dataOmzet[tgl] = total;
                    }
                    dr.Close();

                    chart1.Series.Clear();
                    Series series = chart1.Series.Add("Pendapatan");
                    series.ChartType = SeriesChartType.Column;
                    series.IsValueShownAsLabel = false;

                    ChartArea chartArea = chart1.ChartAreas[0];
                    chartArea.AxisX.Interval = 1;
                    chartArea.AxisX.LabelStyle.IsStaggered = false;
                    chartArea.AxisX.LabelStyle.Angle = 0;
                    chartArea.AxisY.LabelStyle.Format = "N0";

                    for (DateTime tgl = awalWaktu; tgl <= akhirWaktu; tgl = tgl.AddDays(1))
                    {
                        decimal total = dataOmzet.ContainsKey(tgl) ? dataOmzet[tgl] : 0;
                        series.Points.AddXY(tgl.ToString("dd/MM"), total);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat grafik pendapatan: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void siapkanGridStok()
        {
            if (dgvStokMenipis != null) return;

            dgvStokMenipis = new DataGridView();
            dgvStokMenipis.Name = "dgvStokMenipis";
            dgvStokMenipis.AllowUserToAddRows = false;
            dgvStokMenipis.ReadOnly = true;
            dgvStokMenipis.RowHeadersVisible = false;
            dgvStokMenipis.AllowUserToResizeRows = false;
            dgvStokMenipis.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvStokMenipis.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvStokMenipis.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvStokMenipis.Location = new Point(3, 30);
            dgvStokMenipis.Size = new Size(panel6.ClientSize.Width - 6, panel6.ClientSize.Height - 36);
            panel6.Controls.Add(dgvStokMenipis);
        }

        private void tampilStokMenipis()
        {
            siapkanGridStok();
            try
            {
                using (SqlConnection conn = konn.Getconn())
                {
                    conn.Open();

                    string query = @"SELECT kode, nama, stok, stok_minimum
                                     FROM sparepart
                                     WHERE stok <= stok_minimum
                                     ORDER BY stok ASC, nama ASC";

                    da = new SqlDataAdapter(query, conn);
                    ds = new DataSet();
                    da.Fill(ds, "stok");
                    dgvStokMenipis.DataSource = ds.Tables["stok"];

                    if (dgvStokMenipis.Columns.Contains("kode")) dgvStokMenipis.Columns["kode"].HeaderText = "Kode";
                    if (dgvStokMenipis.Columns.Contains("nama")) dgvStokMenipis.Columns["nama"].HeaderText = "Nama";
                    if (dgvStokMenipis.Columns.Contains("stok")) dgvStokMenipis.Columns["stok"].HeaderText = "Stok";
                    if (dgvStokMenipis.Columns.Contains("stok_minimum")) dgvStokMenipis.Columns["stok_minimum"].HeaderText = "Min";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat stok menipis: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tampilWorkOrderTerbaru()
        {
            try
            {
                using (SqlConnection conn = konn.Getconn())
                {
                    conn.Open();

                    string query = @"SELECT TOP 10 wo.nomor_wo, c.nama AS customer, k.kendaraan, u.nama_lengkap AS mekanik, wo.status, wo.dibuat_pada
                                     FROM work_orders wo
                                     LEFT JOIN customers c ON wo.customer_id = c.id
                                     LEFT JOIN kendaraan k ON wo.kendaraan_id = k.id
                                     LEFT JOIN users u ON wo.mekanik_id = u.id
                                     ORDER BY wo.id DESC";

                    da = new SqlDataAdapter(query, conn);
                    ds = new DataSet();
                    da.Fill(ds, "wo");
                    dataGridView1.DataSource = ds.Tables["wo"];

                    dataGridView1.AllowUserToAddRows = false;
                    dataGridView1.ReadOnly = true;
                    dataGridView1.RowHeadersVisible = false;
                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                    if (dataGridView1.Columns.Contains("nomor_wo")) dataGridView1.Columns["nomor_wo"].HeaderText = "No. WO";
                    if (dataGridView1.Columns.Contains("customer")) dataGridView1.Columns["customer"].HeaderText = "Customer";
                    if (dataGridView1.Columns.Contains("kendaraan")) dataGridView1.Columns["kendaraan"].HeaderText = "Kendaraan";
                    if (dataGridView1.Columns.Contains("mekanik")) dataGridView1.Columns["mekanik"].HeaderText = "Mekanik";
                    if (dataGridView1.Columns.Contains("status")) dataGridView1.Columns["status"].HeaderText = "Status";
                    if (dataGridView1.Columns.Contains("dibuat_pada")) dataGridView1.Columns["dibuat_pada"].HeaderText = "Tanggal";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat work order terbaru: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void panel6_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void lblJumlahWo_Click(object sender, EventArgs e)
        {

        }

        private void lblDikerjakan_Click(object sender, EventArgs e)
        {

        }

        private void lblOmzet_Click(object sender, EventArgs e)
        {

        }

        private void lblsiapdiambil_Click(object sender, EventArgs e)
        {

        }
    }
}
