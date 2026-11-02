using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Kerja_Projek
{
    public partial class DetailPermintaanSparepart : Form
    {
        private readonly Koneksi konn = new Koneksi();
        private readonly int woId;

        public DetailPermintaanSparepart(int workOrderId)
        {
            InitializeComponent();
            woId = workOrderId;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            this.Load += new System.EventHandler(this.DetailPermintaanSparepart_Load);
        }

        public int WoId
        {
            get { return woId; }
        }

        private void DetailPermintaanSparepart_Load(object sender, EventArgs e)
        {
            if (woId <= 0)
            {
                MessageBox.Show("ID Work Order tidak valid!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            LoadHeader();
            LoadDetail();
        }

        private void LoadHeader()
        {
            try
            {
                using (SqlConnection conn = konn.Getconn())
                {
                    conn.Open();

                    string query = @"SELECT wo.nomor_wo, u.nama_lengkap, wo.status
                                     FROM work_orders wo
                                     LEFT JOIN users u ON wo.mekanik_id = u.id
                                     WHERE wo.id = @wo_id";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@wo_id", woId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                lblnowo.Text = "Nomor WO: " + reader["nomor_wo"].ToString();
                                lblmekanik.Text = "Diminta Oleh: " + (reader["nama_lengkap"] != DBNull.Value ? reader["nama_lengkap"].ToString() : "-");
                                lblstatus.Text = "Status: " + reader["status"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat header permintaan: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string StatusLabel(string status)
        {
            if (string.IsNullOrWhiteSpace(status)) return "-";

            switch (status.Trim())
            {
                case "Diproses": return "Menunggu";
                case "Diterima": return "Disetujui";
                case "Ditolak": return "Ditolak";
                default: return status;
            }
        }

        private void LoadDetail()
        {
            try
            {
                using (SqlConnection conn = konn.Getconn())
                {
                    conn.Open();

                    string query = @"SELECT wsp.id,
                                            sp.kode AS kode_sparepart,
                                            sp.nama AS nama_sparepart,
                                            wsp.qty,
                                            wsp.harga_saat_transaksi,
                                            wsp.status_request,
                                            wsp.alasan_tolak
                                     FROM wo_sparepart wsp
                                     INNER JOIN sparepart sp ON wsp.sparepart_id = sp.id
                                     WHERE wsp.wo_id = @wo_id
                                     ORDER BY wsp.id DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@wo_id", woId);

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable tmp = new DataTable();
                            da.Fill(tmp);

                            foreach (DataRow r in tmp.Rows)
                            {
                                r["status_request"] = StatusLabel(r["status_request"].ToString());
                            }

                            dataGridView1.DataSource = tmp;
                            FormatGridColumns();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat detail permintaan: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormatGridColumns()
        {
            if (dataGridView1.Columns["id"] != null)
                dataGridView1.Columns["id"].Visible = false;
            if (dataGridView1.Columns["kode_sparepart"] != null)
                dataGridView1.Columns["kode_sparepart"].HeaderText = "Kode Sparepart";
            if (dataGridView1.Columns["nama_sparepart"] != null)
                dataGridView1.Columns["nama_sparepart"].HeaderText = "Nama Sparepart";
            if (dataGridView1.Columns["qty"] != null)
                dataGridView1.Columns["qty"].HeaderText = "Qty";
            if (dataGridView1.Columns["harga_saat_transaksi"] != null)
            {
                dataGridView1.Columns["harga_saat_transaksi"].HeaderText = "Harga";
                dataGridView1.Columns["harga_saat_transaksi"].DefaultCellStyle.Format = "N0";
            }
            if (dataGridView1.Columns["status_request"] != null)
                dataGridView1.Columns["status_request"].HeaderText = "Status";
            if (dataGridView1.Columns["alasan_tolak"] != null)
                dataGridView1.Columns["alasan_tolak"].HeaderText = "Alasan Tolak";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}