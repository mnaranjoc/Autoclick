using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Autoclicker
{
    public partial class frmImportDialog : Form
    {
        public string textToImport { get; set; }

        public frmImportDialog()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textToImport = txtToImport.Text;
        }
    }
}
