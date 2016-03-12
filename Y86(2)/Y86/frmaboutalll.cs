using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Y86
{
    public partial class frmaboutall : Form
    {
        public frmaboutall()
        {
            InitializeComponent();
        }

        private void btnquit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
