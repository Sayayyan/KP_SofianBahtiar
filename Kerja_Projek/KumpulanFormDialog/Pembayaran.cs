using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kerja_Projek.KumpulanFormDialog
{
    public partial class Pembayaran : Form
    {
        Koneksi konn = new Koneksi();
        private SqlDataAdapter da;
        private SqlDataReader dr;
        private SqlCommand cmd;
        private DataSet ds;
        private int woId = 0;

        public int WoId
        {
            get { return woId; }
        }

        public Pembayaran()
        {
            InitializeComponent();
        }

        public Pembayaran(int id) : this()
        {
            this.woId = id;
        }

        private decimal GetTotalTagihan()
        {
            decimal total = 0;
            try
            {
                using (SqlConnection conn = konn.Getconn())
                {
                    conn.Open();

                    string query = @"
                        SELECT ISNULL((SELECT SUM(tarif_saat_transaksi) FROM wo_jasa WHERE wo_id = @wo_id), 0)
                             + ISNULL((SELECT SUM(qty * harga_saat_transaksi) FROM wo_sparepart WHERE wo_id = @wo_id AND status_request = 'Diterima'), 0) AS Total";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@wo_id", woId);
                        object result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            total = Convert.ToDecimal(result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal mengambil total tagihan: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return total;
        }

        private void Pembayaran_Load(object sender, EventArgs e)
        {
            if (woId <= 0)
            {
                MessageBox.Show("ID Work Order tidak valid!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal total = GetTotalTagihan();
            lbltotal.Text = total.ToString("N0");
            txtuangdiberi.Text = total.ToString("N0");
            if (comboBox1.Items.Count > 0)
                comboBox1.SelectedIndex = 0;
            KembalianHitung();
        }

        private void KembalianHitung()
        {
            decimal total = 0;
            decimal.TryParse(lbltotal.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out total);

            decimal uangDiberi = 0;
            if (!string.IsNullOrWhiteSpace(txtuangdiberi.Text))
            {
                decimal.TryParse(txtuangdiberi.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out uangDiberi);
            }

            decimal kembalian = uangDiberi - total;
            if (kembalian < 0) kembalian = 0;
            lblkembalian.Text = kembalian.ToString("N0");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (woId <= 0)
            {
                MessageBox.Show("ID Work Order tidak valid!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Silakan pilih metode pembayaran terlebih dahulu!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string metode = comboBox1.SelectedItem.ToString();

            decimal total = 0;
            decimal.TryParse(lbltotal.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out total);

            if (string.IsNullOrWhiteSpace(txtuangdiberi.Text) || !decimal.TryParse(txtuangdiberi.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out decimal jumlahDiterima))
            {
                MessageBox.Show("Uang yang dibayarkan harus berupa angka yang valid!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtuangdiberi.Focus();
                return;
            }

            if (jumlahDiterima < total)
            {
                MessageBox.Show("Uang yang dibayarkan kurang dari total tagihan!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtuangdiberi.Focus();
                return;
            }

            decimal kembalian = jumlahDiterima - total;

            try
            {
                using (SqlConnection conn = konn.Getconn())
                {
                    conn.Open();
                    SqlTransaction trans = conn.BeginTransaction();

                    try
                    {
                        string insertBayar = @"INSERT INTO pembayaran (wo_id, metode, diskon, total_bayar, jumlah_diterima, kembalian, kasir_id, dibayar_pada)
                                                VALUES (@wo_id, @metode, 0, @total, @jumlah_diterima, @kembalian, @kasir_id, GETDATE())";

                        using (SqlCommand cmd = new SqlCommand(insertBayar, conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@wo_id", woId);
                            cmd.Parameters.AddWithValue("@metode", metode);
                            cmd.Parameters.AddWithValue("@total", total);
                            cmd.Parameters.AddWithValue("@jumlah_diterima", jumlahDiterima);
                            cmd.Parameters.AddWithValue("@kembalian", kembalian);
                            cmd.Parameters.AddWithValue("@kasir_id", usersession.UserId);
                            cmd.ExecuteNonQuery();
                        }

                        string updateWO = @"UPDATE work_orders SET status = 'Sudah dibayar' WHERE id = @wo_id";
                        using (SqlCommand cmd = new SqlCommand(updateWO, conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@wo_id", woId);
                            cmd.ExecuteNonQuery();
                        }

                        string nomorWo = "";
                        using (SqlCommand cmdNo = new SqlCommand("SELECT nomor_wo FROM work_orders WHERE id = @wo_id", conn, trans))
                        {
                            cmdNo.Parameters.AddWithValue("@wo_id", woId);
                            object hasilNo = cmdNo.ExecuteScalar();
                            if (hasilNo != null && hasilNo != DBNull.Value)
                            {
                                nomorWo = hasilNo.ToString();
                            }
                        }

                        string queryLog = @"INSERT INTO audit_log (user_id, aksi, waktu) 
                                            VALUES (@user_id, @aksi, GETDATE())";
                        using (SqlCommand cmdLog = new SqlCommand(queryLog, conn, trans))
                        {
                            cmdLog.Parameters.AddWithValue("@user_id", usersession.UserId);
                            cmdLog.Parameters.AddWithValue("@aksi", $"Membayar {nomorWo}");
                            cmdLog.ExecuteNonQuery();
                        }

                        trans.Commit();

                        MessageBox.Show("Pembayaran berhasil disimpan!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        CetakStruk(total, jumlahDiterima, kembalian, metode);

                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    catch (Exception)
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal menyimpan pembayaran: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CetakStruk(decimal total, decimal uangDiberi, decimal kembalian, string metode)
        {
            string nomorWo = "";
            string customer = "";
            string kendaraan = "";
            string noPolisi = "";
            List<string[]> items = new List<string[]>();

            try
            {
                using (SqlConnection conn = konn.Getconn())
                {
                    conn.Open();

                    using (SqlCommand cmdx = new SqlCommand(@"
                SELECT wo.nomor_wo, c.nama AS nama_customer,
                       k.kendaraan AS kendaraan, k.no_polisi AS no_polisi
                FROM work_orders wo
                INNER JOIN customers c ON wo.customer_id = c.id
                INNER JOIN kendaraan k ON wo.kendaraan_id = k.id
                WHERE wo.id = @wo_id", conn))
                    {
                        cmdx.Parameters.AddWithValue("@wo_id", woId);
                        using (SqlDataReader rd = cmdx.ExecuteReader())
                        {
                            if (rd.Read())
                            {
                                nomorWo = rd["nomor_wo"].ToString();
                                customer = rd["nama_customer"].ToString();
                                kendaraan = rd["kendaraan"].ToString();
                                noPolisi = rd["no_polisi"].ToString();
                            }
                        }
                    }

                    using (SqlCommand cmdx = new SqlCommand(@"
                SELECT j.nama_jasa AS nama, 1 AS qty, wj.tarif_saat_transaksi AS harga
                FROM wo_jasa wj
                INNER JOIN jasa j ON wj.jasa_id = j.id
                WHERE wj.wo_id = @wo_id", conn))
                    {
                        cmdx.Parameters.AddWithValue("@wo_id", woId);
                        using (SqlDataReader rd = cmdx.ExecuteReader())
                        {
                            while (rd.Read())
                            {
                                items.Add(new string[] { rd["nama"].ToString(), rd["qty"].ToString(), rd["harga"].ToString() });
                            }
                        }
                    }

                    using (SqlCommand cmdx = new SqlCommand(@"
                SELECT sp.nama, wsp.qty, wsp.harga_saat_transaksi AS harga
                FROM wo_sparepart wsp
                INNER JOIN sparepart sp ON wsp.sparepart_id = sp.id
                WHERE wsp.wo_id = @wo_id AND wsp.status_request = 'Diterima'", conn))
                    {
                        cmdx.Parameters.AddWithValue("@wo_id", woId);
                        using (SqlDataReader rd = cmdx.ExecuteReader())
                        {
                            while (rd.Read())
                            {
                                items.Add(new string[] { rd["nama"].ToString(), rd["qty"].ToString(), rd["harga"].ToString() });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal mengambil data struk: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            List<string> lines = new List<string>();
            lines.Add("================================");
            lines.Add("           BENGKELYANN          ");
            lines.Add("  Jl. Mangun Jaya, Tambun       ");
            lines.Add("  Selatan, Kabupaten Bekasi     ");
            lines.Add("        STRUK PEMBAYARAN        ");
            lines.Add("================================");
            lines.Add("No. WO   : " + nomorWo);
            lines.Add("Pelanggan: " + customer);
            lines.Add("Kendaraan: " + kendaraan);
            lines.Add("No. Polisi: " + noPolisi);
            lines.Add("Kasir    : " + usersession.Username);
            lines.Add("Metode   : " + metode);
            lines.Add("Waktu    : " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            lines.Add("--------------------------------");
            lines.Add("RINCIAN TAGIHAN :");
            foreach (string[] item in items)
            {
                int qty = Convert.ToInt32(item[1]);
                decimal harga = Convert.ToDecimal(item[2]);
                lines.Add(item[0]);
                lines.Add(string.Format("  {0} x {1:N0} = {2:N0}", qty, harga, qty * harga));
            }
            lines.Add("--------------------------------");
            lines.Add(string.Format("Total       : {0:N0}", total));
            lines.Add(string.Format("Uang Diberi : {0:N0}", uangDiberi));
            lines.Add(string.Format("Kembalian   : {0:N0}", kembalian));
            lines.Add("================================");
            lines.Add("          TERIMA KASIH          ");

            PrintDocument pd = new PrintDocument();

            pd.DefaultPageSettings.PaperSize = new PaperSize("Receipt", 280, 800);
            pd.DefaultPageSettings.Margins = new Margins(10, 10, 10, 10);

            System.Drawing.Image logoStruk = Properties.Resources.logopkk;

            int lineIndex = 0;
            float logoWidth = 0f;
            float logoHeight = 0f;

            if (logoStruk != null && logoStruk.Width > 0 && logoStruk.Height > 0)
            {
                float maxLogoSize = 64f;
                float ratio = Math.Min(maxLogoSize / logoStruk.Width, maxLogoSize / logoStruk.Height);
                logoWidth = logoStruk.Width * ratio;
                logoHeight = logoStruk.Height * ratio;
            }

            Font printFont = new Font("Consolas", 9.5F, FontStyle.Regular);

            pd.BeginPrint += (s, e) =>
            {
                lineIndex = 0;
            };

            pd.PrintPage += (s, e) =>
            {
                float lineHeight = printFont.GetHeight(e.Graphics) + 2;

                float printableWidth = e.MarginBounds.Width;
                float startX = e.MarginBounds.Left;
                float y = e.MarginBounds.Top;

                if (lineIndex == 0 && logoStruk != null)
                {
                    float logoX = startX + (printableWidth - logoWidth) / 2f;
                    e.Graphics.DrawImage(logoStruk, logoX, y, logoWidth, logoHeight);
                    y += logoHeight + 4f;
                }

                while (lineIndex < lines.Count)
                {
                    if (y + lineHeight > e.MarginBounds.Bottom)
                    {
                        e.HasMorePages = true;
                        return;
                    }

                    string currentLine = lines[lineIndex];
                    e.Graphics.DrawString(currentLine, printFont, Brushes.Black, startX, y);

                    y += lineHeight;
                    lineIndex++;
                }
                e.HasMorePages = false;
            };

            this.Hide();

            using (PrintPreviewDialog pv = new PrintPreviewDialog())
            {
                pv.Document = pd;
                pv.WindowState = FormWindowState.Maximized;
                pv.ShowDialog();
            }

            printFont.Dispose();
            pd.Dispose();
        }

        private void txtuangdiberi_TextChanged(object sender, EventArgs e)
        {
            KembalianHitung();
        }
    }
}