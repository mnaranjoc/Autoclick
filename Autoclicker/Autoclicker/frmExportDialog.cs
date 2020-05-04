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
    public partial class frmExportDialog : Form
    {
        public string actionsToExport { get; set; }

        public frmExportDialog()
        {
            InitializeComponent();
        }

        private void frmExportDialog_Load(object sender, EventArgs e)
        {
            txtToExport.Text = actionsToExport;
        }
    }
}
