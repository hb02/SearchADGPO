using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SearchADGPO
{
    public partial class GPOReportViewer : Form
    {
        public GPOReportViewer()
        {
            InitializeComponent();
            webBrowser1.Navigate(Form1.ReportURL);

        }
    }
}
