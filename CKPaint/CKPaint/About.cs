using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
        }

        private void logoPictureBox_Click(object sender, EventArgs e)
        {
            Process.Start("http://ckyazilim.com");
        }

        private void lblLink_Click(object sender, EventArgs e)
        {
            Process.Start("http://ckyazilim.com");
        }
    }
}
