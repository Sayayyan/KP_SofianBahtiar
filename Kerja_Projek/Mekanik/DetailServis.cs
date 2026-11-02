using Kerja_Projek.Fondasi;
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
    public partial class DetailServis : Form
    {
        Koneksi konn = new Koneksi();
        private SqlCommand cmd;
        private SqlDataReader dr;
        private SqlDataAdapter da;
        private DataSet ds;

        public int WorkOrderId { get; set; }

        public DetailServis()
        {
            InitializeComponent();
        }

        private int GetStatusLevel(string statusLabel)
        {
            switch (statusLabel)
            {
                case "Belum dikerjakan": return 1;
                case "Menunggu sparepart": return 2;
                case "Sedang dikerjakan": return 3;
                case "Selesai": return 4;
                default: return 0;
            }
        }

        private string currentStatusLabel = "";

        private string StatusKeLabel(string dbStatus)
        {
            if (string.IsNullOrWhiteSpace(dbStatus)) return "";

            string status = dbStatus.Trim().ToLower();

            switch (status)
            {
                case "baru":
                case "belum dikerjakan":
                    return "Belum dikerjakan";

                case "dikerjakan":
                case "sedang dikerjakan":
                    return "Sedang dikerjakan";

                case "menunggu sparepart":
                    return "Menunggu sparepart";

                case "selesai":
                    return "Selesai";

                default:
                    return dbStatus;
            }
        }

        private void SetupDataGrid()
        {
            if (dgvjasadikerjakan.Columns.Count == 0)
            {
                dgvjasadikerjakan.Columns.Add("id", "ID");
                dgvjasadikerjakan.Columns["id"].Visible = false;
                dgvjasadikerjakan.Columns.Add("nama_jasa", "Nama Jasa");
            }

            if (dgvsparepartdiminta.Columns.Count == 0)
            {
                dgvsparepartdiminta.Columns.Add("id", "ID");
                dgvsparepartdiminta.Columns["id"].Visible = false;
                dgvsparepartdiminta.Columns.Add("nama_sparepart", "Nama Sparepart");
                dgvsparepartdiminta.Columns.Add("jumlah", "Jumlah");
                dgvsparepartdiminta.Columns.Add("status", "Status");
                dgvsparepartdiminta.Columns.Add("alasan", "Alasan Tolak");
            }
        }

        public DetailServis(int workOrderId) : this()
        {
            this.WorkOrderId = workOrderId;
        }

        private void DetailServis_Load(object sender, EventArgs e)
        {
            SetupDataGrid();

            if (cmbstatus.Items.Count == 0)
            {
                cmbstatus.Items.AddRange(new object[]
                {
                    "Belum dikerjakan",
                    "Menunggu sparepart",
                    "Sedang dikerjakan",
                    "Selesai"
                });
            }
            
            if (WorkOrderId > 0)
            {
                LoadDetailData();
                LoadDataJasaTerpilih();
                LoadDataSparepartTerpilih();
            }

            LoadDataJasa();
            LoadDataSparepart();
        }

        private void LoadDataJasa()
        {
            try
            {
                using (SqlConnection conn = konn.Getconn())
                {
                    conn.Open();

                    string query = "SELECT id, kode, nama_jasa, tarif FROM jasa ORDER BY nama_jasa ASC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            cmbjasa.DataSource = dt;
                            cmbjasa.DisplayMember = "nama_jasa";
                            cmbjasa.ValueMember = "id";

                            cmbjasa.SelectedIndex = -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat data jasa: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadDataSparepart()
        {
            try
            {
                using (SqlConnection conn = konn.Getconn())
                {
                    conn.Open();

                    string query = "SELECT id, kode, nama FROM sparepart ORDER BY nama ASC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            cmbSparepart.DataSource = dt;
                            cmbSparepart.DisplayMember = "nama";
                            cmbSparepart.ValueMember = "id";

                            cmbSparepart.SelectedIndex = -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat data sparepart: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadDetailData()
        {
            try
            {
                using (SqlConnection conn = konn.Getconn())
                {
                    conn.Open();

                    string query = @"SELECT
                                wo.nomor_wo,
                                c.nama AS nama_customer,
                                k.no_polisi,
                                k.kendaraan AS nama_kendaraan,
                                wo.keluhan,
                                wo.temuan,
                                wo.status
                             FROM work_orders wo
                             INNER JOIN customers c ON wo.customer_id = c.id
                             INNER JOIN kendaraan k ON wo.kendaraan_id = k.id
                             WHERE wo.id = @id";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", WorkOrderId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                lblnowo.Text = reader["nomor_wo"].ToString();
                                lblnopol.Text = reader["no_polisi"].ToString();
                                lblkendaraan.Text = reader["nama_kendaraan"].ToString();
                                txtkeluhan.Text = reader["keluhan"].ToString();
                                txttemuan.Text = reader["temuan"] != DBNull.Value ? reader["temuan"].ToString() : "";

                                if (reader["status"] != DBNull.Value)
                                {
                                    string statusText = StatusKeLabel(reader["status"].ToString());
                                    currentStatusLabel = statusText;

                                    int index = cmbstatus.FindStringExact(statusText);
                                    if (index != -1)
                                    {
                                        cmbstatus.SelectedIndex = index;
                                    }
                                    else
                                    {
                                        cmbstatus.Text = statusText;
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("Data detail servis tidak ditemukan!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat detail servis: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Panel1_Paint(object sender, PaintEventArgs e) { }
        private void Panel2_Paint(object sender, PaintEventArgs e) { }
        private void panel2_Paint_1(object sender, PaintEventArgs e) { }
        private void lblnopol_Click(object sender, EventArgs e) { }
        private void lblnowo_Click(object sender, EventArgs e) { }
        private void lblkendaraan_Click(object sender, EventArgs e) { }
        private void lblkeluhan_Click(object sender, EventArgs e) { }
        private void txttemuan_TextChanged(object sender, EventArgs e) { }

        private void dgvsparepartdiminta_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void cmbjasa_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbjasa.SelectedIndex != -1 && cmbjasa.SelectedValue != null)
            {
                if (cmbjasa.SelectedValue is DataRowView) return;

                int jasaId = Convert.ToInt32(cmbjasa.SelectedValue);
                string namaJasa = cmbjasa.Text;

                bool sudahAda = false;
                foreach (DataGridViewRow row in dgvjasadikerjakan.Rows)
                {
                    if (row.Cells["id"].Value != null && Convert.ToInt32(row.Cells["id"].Value) == jasaId)
                    {
                        sudahAda = true;
                        break;
                    }
                }

                if (!sudahAda)
                {
                    dgvjasadikerjakan.Rows.Add(jasaId, namaJasa);
                }
                else
                {
                    MessageBox.Show("Jasa ini sudah ditambahkan ke daftar!", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                cmbjasa.SelectedIndex = -1;
            }
        }

        private void LoadDataSparepartTerpilih()
        {
            try
            {
                using (SqlConnection conn = konn.Getconn())
                {
                    conn.Open();

                    string query = @"SELECT sp.id, sp.nama, wsp.qty, wsp.status_request, wsp.alasan_tolak
                                    FROM wo_sparepart wsp
                                    INNER JOIN sparepart sp ON wsp.sparepart_id = sp.id
                                    WHERE wsp.wo_id = @wo_id";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@wo_id", WorkOrderId);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            dgvsparepartdiminta.Rows.Clear();
                            foreach (DataRow r in dt.Rows)
                            {
                                int id = Convert.ToInt32(r["id"]);
                                string nama = r["nama"].ToString();
                                int qty = Convert.ToInt32(r["qty"]);

                                string status = r["status_request"] != DBNull.Value ? r["status_request"].ToString() : "";
                                if (status == "Diproses") status = "Menunggu";
                                else if (status == "Diterima") status = "Disetujui";
                                else if (status == "Ditolak") status = "Ditolak";

                                string alasan = r["alasan_tolak"] != DBNull.Value ? r["alasan_tolak"].ToString() : "";

                                dgvsparepartdiminta.Rows.Add(id, nama, qty, status, alasan);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat sparepart terpilih: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadDataJasaTerpilih()
        {
            try
            {
                using (SqlConnection conn = konn.Getconn())
                {
                    conn.Open();
                    string query = @"SELECT j.id, j.nama_jasa
                                    FROM wo_jasa wj
                                    INNER JOIN jasa j ON wj.jasa_id = j.id
                                    WHERE wj.wo_id = @wo_id";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@wo_id", WorkOrderId);
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            dgvjasadikerjakan.Rows.Clear();
                            while (dr.Read())
                            {
                                dgvjasadikerjakan.Rows.Add(dr["id"], dr["nama_jasa"]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat jasa terpilih: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void panel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dgvjasadikerjakan_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvsparepartdiminta_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void cmbjasa_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cmbjasa.SelectedIndex != -1 && cmbjasa.SelectedValue != null)
            {
                if (cmbjasa.SelectedItem is DataRowView row)
                {
                    string id = row["id"].ToString();
                    string namaJasa = row["nama_jasa"].ToString();

                    bool isExist = false;
                    foreach (DataGridViewRow gridRow in dgvjasadikerjakan.Rows)
                    {
                        if (gridRow.Cells["id"].Value != null && gridRow.Cells["id"].Value.ToString() == id)
                        {
                            isExist = true;
                            break;
                        }
                    }

                    if (!isExist)
                    {
                        dgvjasadikerjakan.Rows.Add(id, namaJasa);
                    }
                    else
                    {
                        MessageBox.Show("Jasa ini sudah dimasukkan!", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    cmbjasa.SelectedIndex = -1;
                }
            }
        }

        private void cmbSparepart_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cmbSparepart_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cmbSparepart.SelectedIndex != -1 && cmbSparepart.SelectedValue != null)
            {
                if (cmbSparepart.SelectedItem is DataRowView row)
                {
                    string id = row["id"].ToString();
                    string namaSparepart = row["nama"].ToString();

                    bool isExist = false;

                    foreach (DataGridViewRow gridRow in dgvsparepartdiminta.Rows)
                    {
                        if (gridRow.Cells["id"].Value != null && gridRow.Cells["id"].Value.ToString() == id)
                        {
                            isExist = true;
                            break;
                        }
                    }

                    if (isExist)
                    {
                        MessageBox.Show("Sparepart sudah diminta!", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        dgvsparepartdiminta.Rows.Add(id, namaSparepart, 1, "", "");
                    }

                    cmbSparepart.SelectedIndex = -1;
                }
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            MainFormMekanik parentForm = Application.OpenForms.OfType<MainFormMekanik>().FirstOrDefault(f => f.Visible)
                            ?? Application.OpenForms.OfType<MainFormMekanik>().FirstOrDefault();

            if (parentForm != null)
            {
                JobMekanik frmjob = new JobMekanik();
                parentForm.openForm(frmjob);
                this.Close();
            }
            else
            {
                this.Close();
            }
        }

        private void btnsimpan_Click(object sender, EventArgs e)
        {
            if (WorkOrderId <= 0)
            {
                MessageBox.Show("ID Work Order tidak valid atau belum dipilih!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection conn = konn.Getconn())
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    int totalRequestBaru = 0;

                    foreach (DataGridViewRow row in dgvsparepartdiminta.Rows)
                    {
                        if (row.IsNewRow || row.Cells["id"].Value == null) continue;

                        int sparepartId = Convert.ToInt32(row.Cells["id"].Value);
                        int qty = Convert.ToInt32(row.Cells["jumlah"].Value);

                        string insertQuery = @"
                    INSERT INTO wo_sparepart (wo_id, sparepart_id, qty, harga_saat_transaksi, status_request)
                    SELECT @wo_id, @sparepart_id, @qty, harga_jual, 'Diproses'
                    FROM sparepart
                    WHERE id = @sparepart_id
                    AND NOT EXISTS (
                        SELECT 1 FROM wo_sparepart w2
                        WHERE w2.wo_id = @wo_id AND w2.sparepart_id = @sparepart_id
                        AND w2.status_request IN ('Diproses', 'Diterima')
                    )";

                        using (SqlCommand cmdIns = new SqlCommand(insertQuery, conn, transaction))
                        {
                            cmdIns.Parameters.AddWithValue("@wo_id", WorkOrderId);
                            cmdIns.Parameters.AddWithValue("@sparepart_id", sparepartId);
                            cmdIns.Parameters.AddWithValue("@qty", qty);

                            totalRequestBaru += cmdIns.ExecuteNonQuery();
                        }
                    }

                    string deleteJasaQuery = "DELETE FROM wo_jasa WHERE wo_id = @wo_id";
                    using (SqlCommand cmdDelJasa = new SqlCommand(deleteJasaQuery, conn, transaction))
                    {
                        cmdDelJasa.Parameters.AddWithValue("@wo_id", WorkOrderId);
                        cmdDelJasa.ExecuteNonQuery();
                    }

                    foreach (DataGridViewRow row in dgvjasadikerjakan.Rows)
                    {
                        if (row.IsNewRow || row.Cells["id"].Value == null) continue;

                        int jasaId = Convert.ToInt32(row.Cells["id"].Value);

                        string insertJasaQuery = @"
                    INSERT INTO wo_jasa (wo_id, jasa_id, tarif_saat_transaksi)
                    SELECT @wo_id, @jasa_id, tarif
                    FROM jasa
                    WHERE id = @jasa_id";

                        using (SqlCommand cmdInsJasa = new SqlCommand(insertJasaQuery, conn, transaction))
                        {
                            cmdInsJasa.Parameters.AddWithValue("@wo_id", WorkOrderId);
                            cmdInsJasa.Parameters.AddWithValue("@jasa_id", jasaId);
                            cmdInsJasa.ExecuteNonQuery();
                        }
                    }

                    string statusTarget = "";

                    if (totalRequestBaru > 0)
                    {
                        statusTarget = "Menunggu sparepart";
                    }
                    else
                    {
                        statusTarget = cmbstatus.SelectedItem != null
                            ? cmbstatus.SelectedItem.ToString()
                            : cmbstatus.Text;

                        if (string.IsNullOrWhiteSpace(statusTarget))
                        {
                            statusTarget = currentStatusLabel;
                        }
                    }

                    string updateWo = @"UPDATE work_orders 
                               SET status = @status,
                                   temuan = @temuan
                               WHERE id = @wo_id AND status NOT IN ('Diserahkan', 'Sudah dibayar')";

                    using (SqlCommand cmdWo = new SqlCommand(updateWo, conn, transaction))
                    {
                        cmdWo.Parameters.AddWithValue("@status", statusTarget);
                        cmdWo.Parameters.AddWithValue("@temuan", txttemuan.Text.Trim());
                        cmdWo.Parameters.AddWithValue("@wo_id", WorkOrderId);
                        cmdWo.ExecuteNonQuery();
                    }

                    string nomorWo = "";
                    using (SqlCommand cmdNo = new SqlCommand("SELECT nomor_wo FROM work_orders WHERE id = @wo_id", conn, transaction))
                    {
                        cmdNo.Parameters.AddWithValue("@wo_id", WorkOrderId);
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
                        cmdLog.Parameters.AddWithValue("@aksi", $"Mengedit detail servis {nomorWo}");
                        cmdLog.ExecuteNonQuery();
                    }

                    transaction.Commit();

                    SetCmbStatusText(statusTarget);

                    if (totalRequestBaru > 0)
                    {
                        MessageBox.Show($"Data berhasil disimpan dan {totalRequestBaru} request sparepart dikirim ke gudang. Status diubah menjadi 'Menunggu sparepart'.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show($"Data pengerjaan berhasil disimpan dengan status '{statusTarget}'.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    MainFormMekanik parentForm = Application.OpenForms.OfType<MainFormMekanik>().FirstOrDefault(f => f.Visible)
                                                ?? Application.OpenForms.OfType<MainFormMekanik>().FirstOrDefault();

                    if (parentForm != null)
                    {
                        JobMekanik frmjob = new JobMekanik();
                        parentForm.openForm(frmjob);
                        this.Close();
                    }
                    else
                    {
                        this.Close();
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Gagal menyimpan data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void SetCmbStatusText(string status)
        {
            if (cmbstatus.Items.Contains(status))
            {
                cmbstatus.SelectedItem = status;
            }
            else
            {
                cmbstatus.Text = status;
            }
        }

        private void cmbstatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbstatus.SelectedItem == null || string.IsNullOrEmpty(currentStatusLabel))
                return;

            string selectedStatus = cmbstatus.SelectedItem.ToString();

            int currentLevel = GetStatusLevel(currentStatusLabel);
            int newLevel = GetStatusLevel(selectedStatus);

            if (newLevel < currentLevel)
            {
                MessageBox.Show($"Status tidak boleh dimundurkan dari '{currentStatusLabel}' ke '{selectedStatus}'!",
                                "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                cmbstatus.SelectedIndexChanged -= cmbstatus_SelectedIndexChanged;
                cmbstatus.SelectedItem = currentStatusLabel;
                cmbstatus.SelectedIndexChanged += cmbstatus_SelectedIndexChanged;
            }
            else
            {
                currentStatusLabel = selectedStatus;
            }
        }

        private void dgvjasadikerjakan_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && !dgvjasadikerjakan.Rows[e.RowIndex].IsNewRow)
            {
                dgvjasadikerjakan.Rows.RemoveAt(e.RowIndex);
            }
        }

        private void dgvsparepartdiminta_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && !dgvsparepartdiminta.Rows[e.RowIndex].IsNewRow)
            {
                string namaSparepart = dgvsparepartdiminta.Rows[e.RowIndex].Cells["nama_sparepart"].Value?.ToString();

                DialogResult result = MessageBox.Show($"Apakah Anda yakin ingin menghapus '{namaSparepart}' dari daftar?",
                  "Konfirmasi Hapus", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    dgvsparepartdiminta.Rows.RemoveAt(e.RowIndex);
                }
            }
        }
    }
}