using Kerja_Projek.Admin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kerja_Projek
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void lbllogo_Click(object sender, EventArgs e)
        {

        }

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

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            openForm(new Dashboard());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openForm(new MasterSparePart());
        }

        private void button7_Click(object sender, EventArgs e)
        {
            openForm(new MasterJasa());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            openForm(new MasterUser());
        }

        private void button4_Click(object sender, EventArgs e)
        {
            openForm(new Laporan());
        }

        private void button5_Click(object sender, EventArgs e)
        {
            openForm(new AuditLog());
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            Koneksi.CatatLog("Logout");

            FormLogin login = new FormLogin();
            login.Show();
            this.Hide();
        }
    }
}
