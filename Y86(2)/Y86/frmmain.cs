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
    public partial class frmmain : Form
    {
        public frmmain()
        {
            InitializeComponent();
        }

        private void btneditor_Click(object sender, EventArgs e)
        {
            frmeditor f = new frmeditor();
            f.Show();
        }

        private void btncpu_Click(object sender, EventArgs e)
        {
            frmtaby86 f = new frmtaby86();
            f.Show();
        }

        private void 编译器ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btneditor_Click(sender, e);
        }

        private void 处理器ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btncpu_Click(sender, e);
        }

        private void 关于ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmaboutall f = new frmaboutall();
            f.Show();
        }
    }
}
