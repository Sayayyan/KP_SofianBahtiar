using Kerja_Projek.KumpulanFormDialog;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Kerja_Projek
{
    public partial class PermintaanSparepart : Form
    {
        private readonly Koneksi konn = new Koneksi();
        private int selectedWoId = 0;

        public PermintaanSparepart()
        {
            InitializeComponent();
            this.button1.Click += new System.EventHandler(this.button1_Click);
            this.button2.Click += new System.EventHandler(this.button2_Click);
            this.button3.Click += new System.EventHandler(this.button3_Click);
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            this.dataGridView1.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellDoubleClick);
            this.txtfilter.TextChanged += new System.EventHandler(this.txtfilter_TextChanged);
            this.Load += new System.EventHandler(this.PermintaanSparepart_Load);
        }

        private void PermintaanSparepart_Load(object sender, EventArgs e)
        {
            TampilPermintaan();
        }

        private void TampilPermintaan(string cari = "")
        {
            try
            {
                using (SqlConnection conn = konn.Getconn())
                {
                    conn.Open();

                    string query = @"SELECT 
                                        wo.id AS wo_id,
                                        wo.nomor_wo,
                                        u.nama_lengkap AS diminta_oleh,
                                        COUNT(wsp.id) AS jumlah_item,
                                        SUM(wsp.qty) AS total_qty,
                                        SUM(wsp.qty * wsp.harga_saat_transaksi) AS total_harga
                                     FROM wo_sparepart wsp
                                     INNER JOIN work_orders wo ON wsp.wo_id = wo.id
                                     LEFT JOIN users u ON wo.mekanik_id = u.id
                                     WHERE wsp.status_request = 'Diproses'";

                    if (!string.IsNullOrWhiteSpace(cari))
                    {
                        query += " AND (wo.nomor_wo LIKE @cari OR u.nama_lengkap LIKE @cari)";
                    }

                    query += @" GROUP BY wo.id, wo.nomor_wo, u.nama_lengkap
                                ORDER BY wo.id DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (!string.IsNullOrWhiteSpace(cari))
                        {
                            cmd.Parameters.AddWithValue("@cari", "%" + cari + "%");
                        }

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable tmp = new DataTable();
                            da.Fill(tmp);
                            dataGridView1.DataSource = tmp;

                            FormatGridColumns();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat permintaan sparepart: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormatGridColumns()
        {
            if (dataGridView1.Columns["wo_id"] != null)
                dataGridView1.Columns["wo_id"].Visible = false;
            if (dataGridView1.Columns["nomor_wo"] != null)
                dataGridView1.Columns["nomor_wo"].HeaderText = "Nomor WO";
            if (dataGridView1.Columns["diminta_oleh"] != null)
                dataGridView1.Columns["diminta_oleh"].HeaderText = "Diminta Oleh";
            if (dataGridView1.Columns["jumlah_item"] != null)
                dataGridView1.Columns["jumlah_item"].HeaderText = "Jumlah Item";
            if (dataGridView1.Columns["total_qty"] != null)
                dataGridView1.Columns["total_qty"].HeaderText = "Total Qty";
            if (dataGridView1.Columns["total_harga"] != null)
            {
                dataGridView1.Columns["total_harga"].HeaderText = "Total Harga";
                dataGridView1.Columns["total_harga"].DefaultCellStyle.Format = "N0";
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
            if (row.Cells["wo_id"].Value == null || row.Cells["wo_id"].Value == DBNull.Value) return;

            selectedWoId = Convert.ToInt32(row.Cells["wo_id"].Value);
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
            if (row.Cells["wo_id"].Value == null || row.Cells["wo_id"].Value == DBNull.Value) return;

            selectedWoId = Convert.ToInt32(row.Cells["wo_id"].Value);
            BukaDetail();
        }

        private void txtfilter_TextChanged(object sender, EventArgs e)
        {
            TampilPermintaan(txtfilter.Text.Trim());
        }

        private void BukaDetail()
        {
            if (selectedWoId <= 0)
            {
                MessageBox.Show("Silakan pilih permintaan sparepart terlebih dahulu!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DetailPermintaanSparepart detail = new DetailPermintaanSparepart(selectedWoId);
            detail.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            BukaDetail();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (selectedWoId <= 0)
            {
                MessageBox.Show("Silakan pilih permintaan sparepart terlebih dahulu!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Setujui semua permintaan sparepart pada Work Order ini?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            using (SqlConnection conn = konn.Getconn())
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    DataTable pending = new DataTable();
                    string selectPending = @"SELECT wsp.id, wsp.sparepart_id, wsp.qty, sp.nama
                                             FROM wo_sparepart wsp
                                             INNER JOIN sparepart sp ON wsp.sparepart_id = sp.id
                                             WHERE wsp.wo_id = @wo_id AND wsp.status_request = 'Diproses'";
                    using (SqlCommand cmdPending = new SqlCommand(selectPending, conn, transaction))
                    {
                        cmdPending.Parameters.AddWithValue("@wo_id", selectedWoId);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmdPending))
                        {
                            da.Fill(pending);
                        }
                    }

                    if (pending.Rows.Count == 0)
                    {
                        transaction.Rollback();
                        MessageBox.Show("Tidak ada permintaan sparepart yang menunggu pada WO ini!", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    foreach (DataRow r in pending.Rows)
                    {
                        int permintaanId = Convert.ToInt32(r["id"]);
                        int sparepartId = Convert.ToInt32(r["sparepart_id"]);
                        int qtyRequested = Convert.ToInt32(r["qty"]);
                        string nama = r["nama"].ToString();

                        string checkStok = "SELECT stok FROM sparepart WHERE id = @sparepart_id";
                        int stokSaatIni = 0;
                        using (SqlCommand cmdStok = new SqlCommand(checkStok, conn, transaction))
                        {
                            cmdStok.Parameters.AddWithValue("@sparepart_id", sparepartId);
                            object res = cmdStok.ExecuteScalar();
                            if (res != null && res != DBNull.Value)
                                stokSaatIni = Convert.ToInt32(res);
                        }

                        if (stokSaatIni < qtyRequested)
                        {
                            transaction.Rollback();
                            MessageBox.Show($"Stok tidak mencukupi untuk '{nama}'! Stok tersedia: {stokSaatIni}, dibutuhkan: {qtyRequested}",
                                            "Gagal Process", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        string updateRequest = "UPDATE wo_sparepart SET status_request = 'Diterima' WHERE id = @id";
                        using (SqlCommand cmdUpdateReq = new SqlCommand(updateRequest, conn, transaction))
                        {
                            cmdUpdateReq.Parameters.AddWithValue("@id", permintaanId);
                            cmdUpdateReq.ExecuteNonQuery();
                        }

                        string updateStok = "UPDATE sparepart SET stok = stok - @qty WHERE id = @sparepart_id";
                        using (SqlCommand cmdDeduct = new SqlCommand(updateStok, conn, transaction))
                        {
                            cmdDeduct.Parameters.AddWithValue("@qty", qtyRequested);
                            cmdDeduct.Parameters.AddWithValue("@sparepart_id", sparepartId);
                            cmdDeduct.ExecuteNonQuery();
                        }
                    }

                    string checkPending = "SELECT COUNT(*) FROM wo_sparepart WHERE wo_id = @wo_id AND status_request = 'Diproses'";
                    using (SqlCommand cmdCek = new SqlCommand(checkPending, conn, transaction))
                    {
                        cmdCek.Parameters.AddWithValue("@wo_id", selectedWoId);
                        int pendingCount = Convert.ToInt32(cmdCek.ExecuteScalar());

                        if (pendingCount == 0)
                        {
                            string updateWO = "UPDATE work_orders SET status = 'Dikerjakan' WHERE id = @wo_id AND status = 'Menunggu sparepart'";
                            using (SqlCommand cmdWO = new SqlCommand(updateWO, conn, transaction))
                            {
                                cmdWO.Parameters.AddWithValue("@wo_id", selectedWoId);
                                cmdWO.ExecuteNonQuery();
                            }
                        }
                    }

                    string nomorWo = "";
                    using (SqlCommand cmdNo = new SqlCommand("SELECT nomor_wo FROM work_orders WHERE id = @wo_id", conn, transaction))
                    {
                        cmdNo.Parameters.AddWithValue("@wo_id", selectedWoId);
                        object hasilNo = cmdNo.ExecuteScalar();
                        if (hasilNo != null && hasilNo != DBNull.Value)
                        {
                            nomorWo = hasilNo.ToString();
                        }
                    }

                    string queryLog = @"INSERT INTO audit_log (user_id, aksi, waktu) 
                                        VALUES (@user_id, @aksi, GETDATE())";
                    using (SqlCommand cmdLog = new SqlCommand(queryLog, conn, transaction))
                    {
                        cmdLog.Parameters.AddWithValue("@user_id", usersession.UserId);
                        cmdLog.Parameters.AddWithValue("@aksi", $"Menyetujui permintaan sparepart {nomorWo}");
                        cmdLog.ExecuteNonQuery();
                    }

                    transaction.Commit();

                    MessageBox.Show("Permintaan sparepart berhasil disetujui dan stok telah diperbarui!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    selectedWoId = 0;
                    TampilPermintaan(txtfilter.Text.Trim());
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Gagal menyetujui permintaan: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (selectedWoId <= 0)
            {
                MessageBox.Show("Silakan pilih permintaan sparepart terlebih dahulu!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            TolakPermintaan tolak = new TolakPermintaan(selectedWoId);
            tolak.ShowDialog();
            selectedWoId = 0;
            TampilPermintaan(txtfilter.Text.Trim());
        }
    }
}