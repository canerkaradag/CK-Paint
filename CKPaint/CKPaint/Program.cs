using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CKPaint
{
    static class Program
    {
        /*
            Created by Caner Karadag on 19.05.15.
        */
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(CKPaint.Instance());
        }
    }
}
