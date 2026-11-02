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
    public partial class Laporan : Form
    {
        Koneksi konn = new Koneksi();
        private SqlDataAdapter da;
        private SqlDataReader dr;
        private SqlCommand cmd;
        private DataSet ds;
        public Laporan()
        {
            InitializeComponent();
        }

        private void Laporan_Load(object sender, EventArgs e)
        {
            dtpdari.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            dtpsampai.Value = DateTime.Today;
            cmbjenis.SelectedIndex = 0;
            TampilLaporan();
        }

        private void TampilLaporan()
        {
            if (cmbjenis.SelectedItem == null) return;

            string jenis = cmbjenis.SelectedItem.ToString();

            switch (jenis)
            {
                case "Omzet Harian":
                    TampilOmzetHarian();
                    break;
                case "Omzet Bulanan":
                    TampilOmzetBulanan();
                    break;
                case "Jasa Terlaris":
                    TampilJasaTerlaris();
                    break;
                case "Sparepart Terlaris":
                    TampilSparepartTerlaris();
                    break;
                case "Performa Mekanik":
                    TampilPerformaMekanik();
                    break;
                default:
                    dgvlaporan.DataSource = null;
                    break;
            }
        }

        private void TampilOmzetHarian()
        {
            try
            {
                using (SqlConnection conn = konn.Getconn())
                {
                    conn.Open();

                    string query = @"SELECT CAST(p.dibayar_pada AS DATE) AS Tanggal,
                                            COUNT(p.id) AS JumlahTransaksi,
                                            SUM(p.total_bayar) AS Omzet
                                     FROM pembayaran p
                                     WHERE CAST(p.dibayar_pada AS DATE) BETWEEN @dari AND @sampai
                                     GROUP BY CAST(p.dibayar_pada AS DATE)
                                     ORDER BY Tanggal";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@dari", dtpdari.Value.Date);
                        cmd.Parameters.AddWithValue("@sampai", dtpsampai.Value.Date);
                        da = new SqlDataAdapter(cmd);
                        ds = new DataSet();
                        da.Fill(ds, "laporan");
                        dgvlaporan.DataSource = ds.Tables["laporan"];
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat laporan omzet harian: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TampilOmzetBulanan()
        {
            try
            {
                using (SqlConnection conn = konn.Getconn())
                {
                    conn.Open();

                    string query = @"SELECT YEAR(p.dibayar_pada) AS Tahun,
                                            MONTH(p.dibayar_pada) AS Bulan,
                                            COUNT(p.id) AS JumlahTransaksi,
                                            SUM(p.total_bayar) AS Omzet
                                     FROM pembayaran p
                                     WHERE CAST(p.dibayar_pada AS DATE) BETWEEN @dari AND @sampai
                                     GROUP BY YEAR(p.dibayar_pada), MONTH(p.dibayar_pada)
                                     ORDER BY Tahun, Bulan";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@dari", dtpdari.Value.Date);
                        cmd.Parameters.AddWithValue("@sampai", dtpsampai.Value.Date);
                        da = new SqlDataAdapter(cmd);
                        ds = new DataSet();
                        da.Fill(ds, "laporan");
                        dgvlaporan.DataSource = ds.Tables["laporan"];
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat laporan omzet bulanan: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TampilJasaTerlaris()
        {
            try
            {
                using (SqlConnection conn = konn.Getconn())
                {
                    conn.Open();

                    string query = @"SELECT j.kode, j.nama_jasa, COUNT(wj.id) AS JumlahPakai, SUM(wj.tarif_saat_transaksi) AS Total
                                     FROM wo_jasa wj
                                     INNER JOIN jasa j ON wj.jasa_id = j.id
                                     INNER JOIN work_orders wo ON wj.wo_id = wo.id
                                     WHERE CAST(wo.dibuat_pada AS DATE) BETWEEN @dari AND @sampai
                                     GROUP BY j.kode, j.nama_jasa
                                     ORDER BY JumlahPakai DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@dari", dtpdari.Value.Date);
                        cmd.Parameters.AddWithValue("@sampai", dtpsampai.Value.Date);
                        da = new SqlDataAdapter(cmd);
                        ds = new DataSet();
                        da.Fill(ds, "laporan");
                        dgvlaporan.DataSource = ds.Tables["laporan"];
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat laporan jasa terlaris: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TampilSparepartTerlaris()
        {
            try
            {
                using (SqlConnection conn = konn.Getconn())
                {
                    conn.Open();

                    string query = @"SELECT sp.kode, sp.nama, SUM(wsp.qty) AS JumlahTerpakai, SUM(wsp.qty * wsp.harga_saat_transaksi) AS Total
                                     FROM wo_sparepart wsp
                                     INNER JOIN sparepart sp ON wsp.sparepart_id = sp.id
                                     INNER JOIN work_orders wo ON wsp.wo_id = wo.id
                                     WHERE CAST(wo.dibuat_pada AS DATE) BETWEEN @dari AND @sampai
                                     GROUP BY sp.kode, sp.nama
                                     ORDER BY JumlahTerpakai DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@dari", dtpdari.Value.Date);
                        cmd.Parameters.AddWithValue("@sampai", dtpsampai.Value.Date);
                        da = new SqlDataAdapter(cmd);
                        ds = new DataSet();
                        da.Fill(ds, "laporan");
                        dgvlaporan.DataSource = ds.Tables["laporan"];
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat laporan sparepart terlaris: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TampilPerformaMekanik()
        {
            try
            {
                using (SqlConnection conn = konn.Getconn())
                {
                    conn.Open();

                    string query = @"SELECT u.nama_lengkap AS Mekanik, COUNT(wo.id) AS WOSelesai
                                     FROM work_orders wo
                                     INNER JOIN users u ON wo.mekanik_id = u.id
                                     WHERE wo.status IN ('Selesai', 'Diserahkan', 'Sudah dibayar')
                                       AND CAST(wo.dibuat_pada AS DATE) BETWEEN @dari AND @sampai
                                     GROUP BY u.nama_lengkap
                                     ORDER BY WOSelesai DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@dari", dtpdari.Value.Date);
                        cmd.Parameters.AddWithValue("@sampai", dtpsampai.Value.Date);
                        da = new SqlDataAdapter(cmd);
                        ds = new DataSet();
                        da.Fill(ds, "laporan");
                        dgvlaporan.DataSource = ds.Tables["laporan"];
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat laporan performa mekanik: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cmbjenis_SelectedIndexChanged(object sender, EventArgs e)
        {
            TampilLaporan();
        }

        private void dgvlaporan_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvlaporan_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
