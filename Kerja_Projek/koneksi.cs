using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace Kerja_Projek
{
    internal class Koneksi
    {
        public SqlConnection Getconn()
        {
            string lokasi = "Data source = LAPTOP-F6HFJFJC\\SQLEXPRESS; database = KerjaProjek; integrated security = true";
            SqlConnection conn = new SqlConnection(lokasi);
            return conn;
        }

        public static void CatatLog(string aksi)
        {
            try
            {
                using (SqlConnection conn = new Koneksi().Getconn())
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO audit_log (user_id, aksi, waktu) VALUES (@user_id, @aksi, GETDATE())", conn))
                    {
                        cmd.Parameters.AddWithValue("@user_id", usersession.UserId);
                        cmd.Parameters.AddWithValue("@aksi", aksi);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
            }
        }
    }
}
