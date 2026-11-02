using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kerja_Projek.Fondasi
{
    public partial class MainFormGudang : Form
    {
        private void openForm(Form childForm)
        {
            foreach (Control control in panel3.Controls)
            {
                if (control is Form activeForm && activeForm.GetType() == childForm.GetType())
                {
                    activeForm.BringToFront();
                    childForm.Dispose();
                    return;
                }
            }

            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;

            panel3.Controls.Add(childForm);
            panel3.Tag = childForm;
            childForm.Show();
            childForm.BringToFront();
        }
        public MainFormGudang()
        {
            InitializeComponent();
        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void MainFormGudang_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            openForm(new BarangMasuk());
        }

        private void button7_Click(object sender, EventArgs e)
        {
            openForm(new DataSupplier());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            openForm(new PermintaanSparepart());
        }

        private void button4_Click(object sender, EventArgs e)
        {
            openForm(new StokSparePart());
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Koneksi.CatatLog("Logout");

            FormLogin login = new FormLogin();
            login.Show();
            this.Hide();
        }

        private void lbllogo_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
