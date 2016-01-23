using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CKPaint
{
    /*
        Created by Caner Karadag on 19.05.15.
    */
    public partial class NewPage : Form
    {
        public NewPage()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (txtName.Text == "")
            {
                MessageBox.Show("Lütfen çalışma sayfası adını giriniz.");
            }
            else if (txtHeight.Text == "")
            {
                MessageBox.Show("Lütfen yüksekliği giriniz.");
            }
            else if (txtWidth.Text == "")
            {
                MessageBox.Show("Lütfen genişliği giriniz.");
            }
            else
            {
                CKPaint p = CKPaint.Instance();
                p.title = txtName.Text;
                p.Text = txtName.Text + " - CK Paint";
                p.NewFile(Convert.ToInt32(txtWidth.Text), Convert.ToInt32(txtHeight.Text));
                p.Show();
                this.Close();
            }
        }

        private void txtWidth_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void txtHeight_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}