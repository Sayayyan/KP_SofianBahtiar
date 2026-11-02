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
    public partial class AuditLog : Form
    {
        Koneksi konn = new Koneksi();
        private SqlDataAdapter da;
        private SqlDataReader dr;
        private SqlCommand cmd;
        private DataSet ds;

        public AuditLog()
        {
            InitializeComponent();
        }
        private void LoadAuditLog(DateTime? tglMulai = null, DateTime? tglSelesai = null)
        {
            try
            {
                using (SqlConnection conn = konn.Getconn())
                {
                    conn.Open();

                    StringBuilder query = new StringBuilder();
                    query.Append(@"SELECT 
                                    a.id,
                                    u.username,
                                    u.nama_lengkap,
                                    r.nama_role,
                                    a.aksi,
                                    a.waktu
                                  FROM audit_log a 
                                  INNER JOIN users u ON u.id = a.user_id 
                                  INNER JOIN role r ON r.id = u.role_id");

                    if (tglMulai.HasValue && tglSelesai.HasValue)
                    {
                        query.Append(" WHERE CAST(a.waktu AS DATE) BETWEEN @tglMulai AND @tglSelesai");
                    }

                    query.Append(" ORDER BY a.waktu DESC");

                    using (SqlCommand cmd = new SqlCommand(query.ToString(), conn))
                    {
                        if (tglMulai.HasValue && tglSelesai.HasValue)
                        {
                            cmd.Parameters.AddWithValue("@tglMulai", tglMulai.Value.Date);
                            cmd.Parameters.AddWithValue("@tglSelesai", tglSelesai.Value.Date);
                        }

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        dataGridView1.DataSource = dt;

                        if (dataGridView1.Columns.Contains("id"))
                            dataGridView1.Columns["id"].HeaderText = "ID";
                        if (dataGridView1.Columns.Contains("username"))
                            dataGridView1.Columns["username"].HeaderText = "Username";
                        if (dataGridView1.Columns.Contains("nama_lengkap"))
                            dataGridView1.Columns["nama_lengkap"].HeaderText = "Nama Lengkap";
                        if (dataGridView1.Columns.Contains("nama_role"))
                            dataGridView1.Columns["nama_role"].HeaderText = "Role";
                        if (dataGridView1.Columns.Contains("aksi"))
                            dataGridView1.Columns["aksi"].HeaderText = "Aksi";
                        if (dataGridView1.Columns.Contains("waktu"))
                            dataGridView1.Columns["waktu"].HeaderText = "Waktu";

                        dataGridView1.AllowUserToAddRows = false;
                        dataGridView1.ReadOnly = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat data Audit Log: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AuditLog_Load(object sender, EventArgs e)
        {
            dtp1.Value = DateTime.Today;
            dtp2.Value = DateTime.Today;

            LoadAuditLog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DateTime tglAwal = dtp1.Value.Date;
            DateTime tglAkhir = dtp2.Value.Date;

            if (tglAwal > tglAkhir)
            {
                MessageBox.Show("Tanggal awal tidak boleh lebih besar dari tanggal akhir!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            LoadAuditLog(tglAwal, tglAkhir);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dtp2_ValueChanged(object sender, EventArgs e)
        {

        }

        private void dtp1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}