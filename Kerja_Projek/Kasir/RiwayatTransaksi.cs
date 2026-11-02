using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Kerja_Projek
{
    public partial class RiwayatTransaksi : Form
    {
        Koneksi konn = new Koneksi();
        private SqlDataAdapter da;
        private SqlDataReader dr;
        private SqlCommand cmd;
        private DataSet ds;

        public RiwayatTransaksi()
        {
            InitializeComponent();
            this.button1.Click += new System.EventHandler(this.button1_Click);
        }

        private void TampilTransaksi()
        {
            try
            {
                using (SqlConnection conn = konn.Getconn())
                {
                    conn.Open();

                    string query = @"SELECT 
                                        p.id,
                                        wo.nomor_wo,
                                        p.metode,
                                        p.total_bayar,
                                        p.jumlah_diterima,
                                        p.kembalian,
                                        u.nama_lengkap AS kasir,
                                        p.dibayar_pada
                                     FROM pembayaran p
                                     INNER JOIN work_orders wo ON p.wo_id = wo.id
                                     LEFT JOIN users u ON p.kasir_id = u.id
                                     WHERE CAST(p.dibayar_pada AS DATE) BETWEEN @dari AND @sampai
                                     ORDER BY p.dibayar_pada DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@dari", dtpdari.Value.Date);
                        cmd.Parameters.AddWithValue("@sampai", dtpsampai.Value.Date);

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            dataGridView1.DataSource = dt;

                            if (dataGridView1.Columns["id"] != null)
                                dataGridView1.Columns["id"].Visible = false;
                            if (dataGridView1.Columns["nomor_wo"] != null)
                                dataGridView1.Columns["nomor_wo"].HeaderText = "Nomor WO";
                            if (dataGridView1.Columns["metode"] != null)
                                dataGridView1.Columns["metode"].HeaderText = "Metode";
                            if (dataGridView1.Columns["total_bayar"] != null)
                                dataGridView1.Columns["total_bayar"].HeaderText = "Total Bayar";
                            if (dataGridView1.Columns["jumlah_diterima"] != null)
                                dataGridView1.Columns["jumlah_diterima"].HeaderText = "Uang Diberi";
                            if (dataGridView1.Columns["kembalian"] != null)
                                dataGridView1.Columns["kembalian"].HeaderText = "Kembalian";
                            if (dataGridView1.Columns["kasir"] != null)
                                dataGridView1.Columns["kasir"].HeaderText = "Kasir";
                            if (dataGridView1.Columns["dibayar_pada"] != null)
                                dataGridView1.Columns["dibayar_pada"].HeaderText = "Tanggal";

                            decimal total = 0;
                            foreach (DataRow r in dt.Rows)
                            {
                                total += Convert.ToDecimal(r["total_bayar"]);
                            }
                            lbltotalkas.Text = total.ToString("N0");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat riwayat transaksi: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dtpdari.Value.Date > dtpsampai.Value.Date)
            {
                MessageBox.Show("Tanggal awal tidak boleh lebih besar dari tanggal akhir!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            TampilTransaksi();
        }

        private void btnexportpdf_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("Tidak ada data untuk diekspor ke PDF!", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (dtpdari.Value.Date > dtpsampai.Value.Date)
            {
                MessageBox.Show("Tanggal awal tidak boleh lebih besar dari tanggal akhir!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataTable dt = dataGridView1.DataSource as DataTable;
            if (dt == null || dt.Rows.Count == 0) return;

            string defaultFileName = "Lap-" + DateTime.Now.ToString("MMMM-yyyy", new CultureInfo("id-ID")) + ".pdf";

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PDF Files|*.pdf";
            saveFileDialog.Title = "Simpan Laporan PDF";
            saveFileDialog.FileName = defaultFileName;

            if (saveFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            try
            {
                List<DataGridViewColumn> visCols = new List<DataGridViewColumn>();
                for (int i = 0; i < dataGridView1.Columns.Count; i++)
                {
                    if (dataGridView1.Columns[i].Visible)
                        visCols.Add(dataGridView1.Columns[i]);
                }
                if (visCols.Count == 0) return;
                Document doc = new Document(PageSize.A4, 36f, 36f, 36f, 100f);
                doc.AddTitle("Laporan Riwayat Transaksi");

                iTextSharp.text.Font fTitle = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14);
                iTextSharp.text.Font fBengkel = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
                iTextSharp.text.Font fSub = FontFactory.GetFont(FontFactory.HELVETICA, 9);
                iTextSharp.text.Font fHead = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9, BaseColor.WHITE);
                iTextSharp.text.Font fCell = FontFactory.GetFont(FontFactory.HELVETICA, 8, BaseColor.BLACK);

                string outFile = saveFileDialog.FileName;
                string tmpFile = Path.Combine(Path.GetTempPath(), "riwayat_" + Guid.NewGuid().ToString("N") + ".pdf");
                string tglStr = "Bekasi, " + DateTime.Now.ToString("dd MMMM yyyy", new CultureInfo("id-ID"));

                using (FileStream fs = new FileStream(tmpFile, FileMode.Create, FileAccess.Write))
                {
                    PdfWriter writer = PdfWriter.GetInstance(doc, fs);
                    string strPeriode = "Periode: " + dtpdari.Value.ToString("dd/MM/yyyy") + " s/d " + dtpsampai.Value.ToString("dd/MM/yyyy");
                    string strTotal = "Total Pendapatan: " + lbltotalkas.Text;
                    HalamanFooter footer = new HalamanFooter();
                    footer.TeksPeriode = strPeriode;
                    footer.TeksTotal = strTotal;
                    writer.PageEvent = footer;
                    doc.Open();

                    Paragraph bengkel = new Paragraph("BengkelYann", fBengkel);
                    bengkel.Alignment = Element.ALIGN_CENTER;
                    doc.Add(bengkel);

                    Paragraph judul = new Paragraph("LAPORAN RIWAYAT TRANSAKSI", fTitle);
                    judul.Alignment = Element.ALIGN_CENTER;
                    doc.Add(judul);

                    Paragraph alamat = new Paragraph("Jalan Mangun Jaya, Tambun Selatan, Kabupaten Bekasi", fSub);
                    alamat.Alignment = Element.ALIGN_CENTER;
                    doc.Add(alamat);

                    Paragraph periode = new Paragraph(strPeriode, fSub);
                    periode.Alignment = Element.ALIGN_CENTER;
                    doc.Add(periode);

                    Paragraph total = new Paragraph(strTotal, fSub);
                    total.Alignment = Element.ALIGN_CENTER;
                    total.SpacingAfter = 2f;
                    doc.Add(total);

                    Paragraph garisPemisah = new Paragraph();
                    garisPemisah.Add(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(1.5f, 100f, BaseColor.GRAY, Element.ALIGN_CENTER, -2f)));
                    garisPemisah.SpacingBefore = 2f;
                    garisPemisah.SpacingAfter = 8f;
                    doc.Add(garisPemisah);

                    PdfPTable tabel = new PdfPTable(visCols.Count);
                    tabel.WidthPercentage = 100f;
                    tabel.HeaderRows = 1;
                    tabel.SplitLate = false;
                    tabel.SplitRows = true;

                    BaseColor biru = new BaseColor(70, 130, 180);
                    BaseColor abu = new BaseColor(245, 245, 245);

                    foreach (DataGridViewColumn c in visCols)
                    {
                        PdfPCell h = new PdfPCell(new Phrase(c.HeaderText, fHead));
                        h.BackgroundColor = biru;
                        h.HorizontalAlignment = Element.ALIGN_CENTER;
                        h.VerticalAlignment = Element.ALIGN_MIDDLE;
                        h.Padding = 5f;
                        tabel.AddCell(h);
                    }

                    for (int r = 0; r < dt.Rows.Count; r++)
                    {
                        for (int c = 0; c < visCols.Count; c++)
                        {
                            string colName = visCols[c].Name;
                            object raw = dt.Columns.Contains(colName) ? dt.Rows[r][colName] : null;
                            string val = FormatNilaiSel(colName, raw);

                            PdfPCell cell = new PdfPCell(new Phrase(val, fCell));
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Padding = 4f;
                            if (r % 2 == 1)
                                cell.BackgroundColor = abu;
                            tabel.AddCell(cell);
                        }
                    }

                    doc.Add(tabel);
                    doc.Close();
                    writer.Close();
                }

                using (PdfReader reader = new PdfReader(tmpFile))
                {
                    using (FileStream fsOut = new FileStream(outFile, FileMode.Create, FileAccess.Write))
                    {
                        using (PdfStamper stamper = new PdfStamper(reader, fsOut))
                        {
                            int last = reader.NumberOfPages;
                            PdfContentByte cb = stamper.GetOverContent(last);
                            iTextSharp.text.Rectangle ps = reader.GetPageSizeWithRotation(last);

                            float marginR = 36f;
                            float xRight = ps.Width - marginR - 20f;
                            float sy = 108f;

                            BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
                            string namaEksportir = usersession.NamaLengkap;
                            if (string.IsNullOrWhiteSpace(namaEksportir))
                            {
                                namaEksportir = usersession.Username;
                            }
                            if (string.IsNullOrWhiteSpace(namaEksportir))
                            {
                                namaEksportir = "-";
                            }

                            cb.BeginText();
                            cb.SetFontAndSize(bf, 9);
                            cb.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, tglStr, xRight, sy, 0);
                            cb.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, "( " + namaEksportir + " )", xRight, sy - 60f, 0);
                            cb.EndText();
                        }
                    }
                    reader.Close();
                }

                try { File.Delete(tmpFile); } catch { }

                Koneksi.CatatLog("Mengekspor laporan riwayat transaksi ke PDF");

                MessageBox.Show("Laporan PDF berhasil dibuat", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal mengekspor ke PDF: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private string FormatNilaiSel(string colName, object raw)
        {
            string val = raw == null || raw == DBNull.Value ? "" : raw.ToString();
            if (colName == "total_bayar" || colName == "jumlah_diterima" || colName == "kembalian")
            {
                decimal d;
                if (decimal.TryParse(val, NumberStyles.Any, CultureInfo.CurrentCulture, out d))
                    return d.ToString("N0");
                if (decimal.TryParse(val, NumberStyles.Any, CultureInfo.InvariantCulture, out d))
                    return d.ToString("N0");
            }
            if (colName == "dibayar_pada")
            {
                DateTime t;
                if (DateTime.TryParse(val, out t))
                    return t.ToString("dd/MM/yyyy HH:mm");
            }
            return val;
        }

        private void RiwayatTransaksi_Load(object sender, EventArgs e)
        {
            dtpdari.Value = DateTime.Today;
            dtpsampai.Value = DateTime.Today;
            TampilTransaksi();
        }

        private static byte[] _logoBytes;

        private static byte[] LogoBytes()
        {
            if (_logoBytes != null) return _logoBytes;
            using (System.Drawing.Bitmap bmp = Properties.Resources.logopkk)
            using (MemoryStream ms = new MemoryStream())
            {
                bmp.Save(ms, ImageFormat.Png);
                _logoBytes = ms.ToArray();
            }
            return _logoBytes;
        }

        private class HalamanFooter : PdfPageEventHelper
        {
            public string TeksBengkel = "BengkelYann";
            public string TeksJudul = "LAPORAN RIWAYAT TRANSAKSI";
            public string TeksAlamat = "Jalan Mangun Jaya, Tambun Selatan, Kabupaten Bekasi";
            public string TeksPeriode = "";
            public string TeksTotal = "";

            public override void OnEndPage(PdfWriter writer, Document document)
            {
                iTextSharp.text.Font f = FontFactory.GetFont(FontFactory.HELVETICA, 8, BaseColor.GRAY);
                PdfPTable ft = new PdfPTable(1);
                ft.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
                PdfPCell c = new PdfPCell(new Phrase("Halaman " + writer.PageNumber, f));
                c.Border = iTextSharp.text.Rectangle.NO_BORDER;
                c.HorizontalAlignment = Element.ALIGN_CENTER;
                ft.AddCell(c);
                ft.WriteSelectedRows(0, -1, document.LeftMargin, 25f, writer.DirectContent);

                if (writer.PageNumber == 1)
                {
                    try
                    {
                        BaseFont bJudul = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false);
                        BaseFont bSub = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
                        float wBengkel = bJudul.GetWidthPoint(TeksBengkel, 16f);
                        float wJudul = bJudul.GetWidthPoint(TeksJudul, 14f);
                        float wAlamat = bSub.GetWidthPoint(TeksAlamat, 9f);
                        float wPeriode = bSub.GetWidthPoint(TeksPeriode, 9f);
                        float wTotal = bSub.GetWidthPoint(TeksTotal, 9f);
                        float maxW = Math.Max(Math.Max(Math.Max(wBengkel, wJudul), wAlamat), Math.Max(wPeriode, wTotal));

                        float logoW = 65f;
                        float gap = 6f;
                        float cx = document.PageSize.Width / 2f;
                        float x = cx - (maxW / 2f) - gap - logoW;
                        float top = document.PageSize.Height - document.TopMargin;
                        float y = top - 70f;

                        iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(LogoBytes());
                        logo.ScaleAbsolute(logoW, 65f);
                        logo.SetAbsolutePosition(x, y);
                        writer.DirectContent.AddImage(logo);
                    }
                    catch { }
                }
            }
        }
    }
}