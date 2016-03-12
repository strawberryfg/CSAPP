using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
namespace Y86
{
    public partial class frmeditor : Form
    {
        public frmeditor()
        {
            InitializeComponent();
        }
        const int NUM = 74;
        const int LINENUM = 38;
        int lastrow = -1;
        bool modified=false;
        string[] reg = { "%eax", "%ecx", "%edx", "%ebx", "%esp", "%ebp", "%esi", "%edi" }; //register
        int numen = -1;
        string V = "";

        private int getregi(string s1)
        {
            try
            {
                for (int i = 0; i < 8; i++) if (s1.Substring(0, 4).Equals(reg[i])) return i;
                return -1;
            }
            catch(System.Exception err)
            {
                return -1;
            }            
        }

        private string littleendian(string s1)
        {
            try
            {
                string s2 ="";
                s2 = s2 + s1.Substring(6, 2);
                s2 = s2 + s1.Substring(4, 2);
                s2 = s2 + s1.Substring(2, 2);
                s2 = s2 + s1.Substring(0, 2);
                s2=s2.ToUpper();
            return s2;
            }
            catch(System.Exception err)
            {
                return "";
            }

        }

        private void getintfrominstruction(string slines,int st)
        {
            try
            {
                int neg = 1;
                int num = 0;
                int en = 0;
                if (!slines[st].Equals('$') || (slines[st - 1].Equals(' ') && slines[st - 2].Equals('p'))) st--;
                if (slines[st + 1].Equals('-'))
                {
                    neg = -1;
                    en = st + 2;
                    while (en < slines.Length && !slines[en].Equals(',') && !slines[en].Equals('(') && !slines[en].Equals('\n') && !slines[en].Equals(' ')) en++;
                    en--;
                    num = Convert.ToInt32(slines.Substring(st + 2, en - st - 1));
                }
                else if (slines.Substring(st + 1, 2).Equals("0x"))
                {
                    en = st + 3;
                    while (en < slines.Length && !slines[en].Equals(',') && !slines[en].Equals('(') && !slines[en].Equals('\n') && !slines[en].Equals(' ')) en++;
                    en--;
                    num = Convert.ToInt32(slines.Substring(st + 3, en - st - 2), 16);
                }
                else
                {
                    en = st + 1;
                    while (en < slines.Length && !slines[en].Equals(',') && !slines[en].Equals('(') && !slines[en].Equals('\n') && !slines[en].Equals(' ')) en++;
                    en--;
                    num = Convert.ToInt32(slines.Substring(st + 1, en - st));
                }
                num = num * neg;
                numen = en;
                V = Convert.ToString(num, 16).PadLeft(8, '0');
                V = littleendian(V);
            }
            catch(System.Exception err)
            {
                return;
            }            
        }

        private void rch_VScroll(object sender, EventArgs e)
        {
            try
            {
                Point p = rch.Location;
                int first = rch.GetCharIndexFromPosition(p);
                int fstline = rch.GetLineFromCharIndex(first);
                for (int i = 0; i < LINENUM; i++)
                {
                    lst.Items[i] = (fstline + i).ToString().PadLeft(5, ' ');
                }
                if (lastrow >= fstline) //current window
                {
                    lblshow.Visible = true;
                    if (fstline == 0)
                    {
                        if (lastrow == 1) lblshow.Location = new Point(66, 23 + 17);
                        else lblshow.Location = new Point(66, 23);
                    }
                    else
                    {
                        lblshow.Location = new Point(66, 23 + 17 * (lastrow - fstline + 1));
                    }
                }
                else lblshow.Visible = false;
            }
            catch(System.Exception err)
            {
                return;
            }            
        }
       
        private void lst_DrawItem(object sender, DrawItemEventArgs e)
        {
            try
            {
                e.DrawBackground();
                e.DrawFocusRectangle();
                e.Graphics.DrawString(lst.Items[e.Index].ToString(), e.Font, new SolidBrush(Color.Blue), e.Bounds);
            }
            catch(System.Exception err)
            {
                return;
            }

        }

        private void lst_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            try
            {
                e.ItemHeight = 17;            
            }
            catch(System.Exception err)
            {
                return;
            }            
        }

        private void rch_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    if (rch.Text.Equals(""))
                    {
                        lblshow.Text = "";
                        lblshow.Location = new Point(66, 23);
                        lastrow = -1;
                        return;
                    }

                    Point p = rch.Location;
                    int first = rch.GetCharIndexFromPosition(p);
                    int fstline = rch.GetLineFromCharIndex(first);
                    int ind = rch.SelectionStart;
                    int row = rch.GetLineFromCharIndex(ind);
                    lblshow.Text = "";
                    int realrow = row;
                    string[] slines = rch.Text.Split('\n');
                    int cnt = slines.Count();
                    if (row > cnt - 1) realrow--;
                    lblshow.Text = rch.Lines[realrow];
                    for (int i = 0; i < NUM - rch.Lines[realrow].Length; i++)
                        lblshow.Text = lblshow.Text + " ";
                    lblshow.Visible = true;
                    if (fstline == 0)
                    {
                        if (row == 1) lblshow.Location = new Point(66, 23 + 17);
                        else lblshow.Location = new Point(66, 23);
                    }
                    else
                    {
                        lblshow.Location = new Point(66, 23 + 17 * (row - fstline + 1));
                    }
                    lastrow = row;
                }
                catch (System.IndexOutOfRangeException err)
                {
                    MessageBox.Show(err.ToString());
                }            
            }
            catch(System.Exception err)
            {
                return;
            }            
        }




        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void 新建ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (modified == true)
                {
                    DialogResult ret = MessageBox.Show(Owner, "文本已被修改，是否保存改变？", "新建", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                    if (ret == DialogResult.Yes)
                    {
                        保存ToolStripMenuItem_Click(sender, e);
                    }
                    else if (ret == DialogResult.No)
                    {
                        lblshow.Text = "";
                        lblshow.Location = new Point(66, 23);
                        lastrow = -1;
                        rch.Text = "";
                        modified = false;
                        this.Text = "编辑器 - 无标题";
                    }
                }
            }
            catch(System.Exception err)
            {
                return;
            }                       
        }

        private void 打开ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (modified == true)
                {
                    DialogResult ret = MessageBox.Show(Owner, "文本已被修改，是否保存改变？", "新建", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                    if (ret == DialogResult.Yes)
                    {
                        保存ToolStripMenuItem_Click(sender, e);
                        return;
                    }
                }
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.Filter = "所有文件(*.*)|*.*";
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    rch.Text = "";
                    string filename = openFileDialog1.FileName;
                    try
                    {
                        System.IO.StreamReader reader = new System.IO.StreamReader(filename, System.Text.Encoding.Default);
                        string input;
                        while ((input = reader.ReadLine()) != null)
                        {
                            rch.Text = rch.Text + input + "\n";
                        }

                        reader.Close();
                    }
                    catch (System.IO.FileNotFoundException err2)
                    {
                        MessageBox.Show(err2.ToString());
                        return;
                    }
                }
            }
            catch(System.Exception err)
            {
                return;
            }            
        }

        private void 关于ToolStripMenuItem1_Click(object sender, EventArgs e)
        {          
            try
            {
                frmabouteditor f = new frmabouteditor();
                f.Show();
            }
            catch(System.Exception err)
            {
                return;
            }
        }

        private void 保存ToolStripMenuItem_Click(object sender, EventArgs e)
        {         
            try
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "所有文件(*.*)|*.*";
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string filename = saveFileDialog1.FileName;
                    string[] slines = rch.Text.Split('\n');
                    int cnt = slines.Count();
                    using (StreamWriter sw = File.CreateText(filename))
                    {

                        for (int i = 1; i <= cnt; i++)
                        {
                            sw.WriteLine(slines[i - 1]);
                        }
                        sw.Close();
                    }
                    modified = false;
                    this.Text = "编辑器 - 已保存";
                }
            }
            catch(System.Exception err)
            {
                return;
            }            
        }

        private void rch_TextChanged(object sender, EventArgs e)
        {          
            try
            {
                modified = true;
                this.Text = "编辑器 - 无标题 *";
                rch_SelectionChanged(sender, e);
            }
            catch(System.Exception err)
            {
                return;
            }

        }

        private void 汇编ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                string[] slines = rch.Text.Split('\n');
                string[] conv = rch.Text.Split('\n');
                int cnt = slines.Count();
                int nowaddr = 0;
                for (int i = 0; i < cnt; i++)
                {
                    conv[i] = "";
                    conv[i] = conv[i] + "  0x";

                    conv[i] = conv[i] + Convert.ToString(nowaddr, 16).PadLeft(3, '0');
                    conv[i] = conv[i] + ": ";
                    int st = 0;
                    for (int j = 0; j < slines[i].Length; j++) if (!slines[i][j].Equals(" ")) { st = j; break; }
                    int len = slines[i].Length;
                    string two = slines[i].Substring(st, Math.Min(2, len));
                    string three = slines[i].Substring(st, Math.Min(3, len));
                    string four = slines[i].Substring(st, Math.Min(4, len));
                    string five = slines[i].Substring(st, Math.Min(5, len));
                    string six = slines[i].Substring(st, Math.Min(6, len));
                    if (four.Equals("halt"))
                    {
                        conv[i] = conv[i] + "00            | " + slines[i];
                        nowaddr += 1;
                    }
                    else if (three.Equals("nop"))
                    {
                        conv[i] = conv[i] + "10            | " + slines[i];
                        nowaddr += 1;
                    }
                    else if (six.Equals("rrmovl"))
                    {
                        int rA = getregi(slines[i].Substring(st + 7, 4));
                        int rB = getregi(slines[i].Substring(st + 12, 4));
                        conv[i] = conv[i] + "20" + rA.ToString() + rB.ToString();
                        for (int u = 0; u < 10; u++) conv[i] = conv[i] + " ";
                        conv[i] = conv[i] + "| " + slines[i];
                        nowaddr += 2;
                    }
                    else if (six.Equals("irmovl"))
                    {
                        getintfrominstruction(slines[i], st + 7);
                        int rB = getregi(slines[i].Substring(numen + 2, 4));
                        conv[i] = conv[i] + "30F" + rB.ToString() + V + "  ";
                        conv[i] = conv[i] + "| " + slines[i];
                        nowaddr += 6;
                    }
                    else if (six.Equals("rmmovl"))
                    {
                        int rA = getregi(slines[i].Substring(st + 7, 4));
                        getintfrominstruction(slines[i], st + 12);
                        int rB = getregi(slines[i].Substring(numen + 2, 4));
                        conv[i] = conv[i] + "40" + rA.ToString() + rB.ToString() + V + "  ";
                        conv[i] = conv[i] + "| " + slines[i];
                        nowaddr += 6;
                    }
                    else if (six.Equals("mrmovl"))
                    {
                        getintfrominstruction(slines[i], st + 7);
                        int rB = getregi(slines[i].Substring(numen + 2, 4));
                        int rA = getregi(slines[i].Substring(numen + 8, 4));
                        conv[i] = conv[i] + "50" + rA.ToString() + rB.ToString() + V + "  ";
                        conv[i] = conv[i] + "| " + slines[i];
                        nowaddr += 6;
                    }
                    else if (four.Equals("addl"))
                    {
                        int rA = getregi(slines[i].Substring(st + 5, 4));
                        int rB = getregi(slines[i].Substring(st + 10, 4));
                        conv[i] = conv[i] + "60" + rA.ToString() + rB.ToString();
                        for (int u = 0; u < 10; u++) conv[i] = conv[i] + " ";
                        conv[i] = conv[i] + "| " + slines[i];
                        nowaddr += 2;
                    }
                    else if (four.Equals("subl"))
                    {
                        int rA = getregi(slines[i].Substring(st + 5, 4));
                        int rB = getregi(slines[i].Substring(st + 10, 4));
                        conv[i] = conv[i] + "61" + rA.ToString() + rB.ToString();
                        for (int u = 0; u < 10; u++) conv[i] = conv[i] + " ";
                        conv[i] = conv[i] + "| " + slines[i];
                        nowaddr += 2;
                    }
                    else if (four.Equals("andl"))
                    {
                        int rA = getregi(slines[i].Substring(st + 5, 4));
                        int rB = getregi(slines[i].Substring(st + 10, 4));
                        conv[i] = conv[i] + "62" + rA.ToString() + rB.ToString();
                        for (int u = 0; u < 10; u++) conv[i] = conv[i] + " ";
                        conv[i] = conv[i] + "| " + slines[i];
                        nowaddr += 2;
                    }
                    else if (four.Equals("xorl"))
                    {
                        int rA = getregi(slines[i].Substring(st + 5, 4));
                        int rB = getregi(slines[i].Substring(st + 10, 4));
                        conv[i] = conv[i] + "63" + rA.ToString() + rB.ToString();
                        for (int u = 0; u < 10; u++) conv[i] = conv[i] + " ";
                        conv[i] = conv[i] + "| " + slines[i];
                        nowaddr += 2;
                    }
                    else if (three.Equals("jmp")) //jmp 0x
                    {
                        getintfrominstruction(slines[i], st + 4);
                        conv[i] = conv[i] + "70" + V + "    ";
                        conv[i] = conv[i] + "| " + slines[i];
                        nowaddr += 5;
                    }
                    else if (three.Equals("jle")) //jmp 0x
                    {
                        getintfrominstruction(slines[i], st + 4);
                        conv[i] = conv[i] + "71" + V + "    ";
                        conv[i] = conv[i] + "| " + slines[i];
                        nowaddr += 5;
                    }
                    else if (two.Equals("jl")) //jmp 0x
                    {
                        getintfrominstruction(slines[i], st + 3);
                        conv[i] = conv[i] + "72" + V + "    ";
                        conv[i] = conv[i] + "| " + slines[i];
                        nowaddr += 5;
                    }
                    else if (two.Equals("je")) //jmp 0x
                    {
                        getintfrominstruction(slines[i], st + 3);
                        conv[i] = conv[i] + "73" + V + "    ";
                        conv[i] = conv[i] + "| " + slines[i];
                        nowaddr += 5;
                    }
                    else if (three.Equals("jne")) //jmp 0x
                    {
                        getintfrominstruction(slines[i], st + 4);
                        conv[i] = conv[i] + "74" + V + "    ";
                        conv[i] = conv[i] + "| " + slines[i];
                        nowaddr += 5;
                    }
                    else if (three.Equals("jge")) //jmp 0x
                    {
                        getintfrominstruction(slines[i], st + 4);
                        conv[i] = conv[i] + "75" + V + "    ";
                        conv[i] = conv[i] + "| " + slines[i];
                        nowaddr += 5;
                    }
                    else if (two.Equals("jg")) //jmp 0x
                    {
                        getintfrominstruction(slines[i], st + 3);
                        conv[i] = conv[i] + "76" + V + "    ";
                        conv[i] = conv[i] + "| " + slines[i];
                        nowaddr += 5;
                    }
                    else if (six.Equals("cmovle"))
                    {
                        int rA = getregi(slines[i].Substring(st + 7, 4));
                        int rB = getregi(slines[i].Substring(st + 12, 4));
                        conv[i] = conv[i] + "21" + rA.ToString() + rB.ToString();
                        for (int u = 0; u < 10; u++) conv[i] = conv[i] + " ";
                        conv[i] = conv[i] + "| " + slines[i];
                        nowaddr += 2;
                    }
                    else if (five.Equals("cmovl"))
                    {
                        int rA = getregi(slines[i].Substring(st + 6, 4));
                        int rB = getregi(slines[i].Substring(st + 11, 4));
                        conv[i] = conv[i] + "22" + rA.ToString() + rB.ToString();
                        for (int u = 0; u < 10; u++) conv[i] = conv[i] + " ";
                        conv[i] = conv[i] + "| " + slines[i];
                        nowaddr += 2;
                    }
                    else if (five.Equals("cmove"))
                    {
                        int rA = getregi(slines[i].Substring(st + 6, 4));
                        int rB = getregi(slines[i].Substring(st + 11, 4));
                        conv[i] = conv[i] + "23" + rA.ToString() + rB.ToString();
                        for (int u = 0; u < 10; u++) conv[i] = conv[i] + " ";
                        conv[i] = conv[i] + "| " + slines[i];
                        nowaddr += 2;
                    }
                    else if (six.Equals("cmovne"))
                    {
                        int rA = getregi(slines[i].Substring(st + 7, 4));
                        int rB = getregi(slines[i].Substring(st + 12, 4));
                        conv[i] = conv[i] + "24" + rA.ToString() + rB.ToString();
                        for (int u = 0; u < 10; u++) conv[i] = conv[i] + " ";
                        conv[i] = conv[i] + "| " + slines[i];
                        nowaddr += 2;
                    }
                    else if (six.Equals("cmovge"))
                    {
                        int rA = getregi(slines[i].Substring(st + 7, 4));
                        int rB = getregi(slines[i].Substring(st + 12, 4));
                        conv[i] = conv[i] + "25" + rA.ToString() + rB.ToString();
                        for (int u = 0; u < 10; u++) conv[i] = conv[i] + " ";
                        conv[i] = conv[i] + "| " + slines[i];
                        nowaddr += 2;
                    }
                    else if (five.Equals("cmovg"))
                    {
                        int rA = getregi(slines[i].Substring(st + 6, 4));
                        int rB = getregi(slines[i].Substring(st + 11, 4));
                        conv[i] = conv[i] + "26" + rA.ToString() + rB.ToString();
                        for (int u = 0; u < 10; u++) conv[i] = conv[i] + " ";
                        conv[i] = conv[i] + "| " + slines[i];
                        nowaddr += 2;
                    }
                    else if (four.Equals("call")) //jmp 0x
                    {
                        getintfrominstruction(slines[i], st + 5);
                        conv[i] = conv[i] + "80" + V + "    ";
                        conv[i] = conv[i] + "| " + slines[i];
                        nowaddr += 5;
                    }
                    else if (three.Equals("ret"))
                    {
                        conv[i] = conv[i] + "90            | " + slines[i];
                        nowaddr += 1;
                    }
                    else if (five.Equals("pushl"))
                    {
                        int rA = getregi(slines[i].Substring(st + 6, 4));
                        conv[i] = conv[i] + "A0" + rA.ToString() + "F";
                        for (int u = 0; u < 10; u++) conv[i] = conv[i] + " ";
                        conv[i] = conv[i] + "| " + slines[i];
                        nowaddr += 2;
                    }
                    else if (four.Equals("popl"))
                    {
                        int rA = getregi(slines[i].Substring(st + 5, 4));
                        conv[i] = conv[i] + "B0" + rA.ToString() + "F";
                        for (int u = 0; u < 10; u++) conv[i] = conv[i] + " ";
                        conv[i] = conv[i] + "| " + slines[i];
                        nowaddr += 2;
                    }
                }

                if (modified == true)
                {
                    DialogResult ret = MessageBox.Show(Owner, "文本已被修改，汇编前是否保存改变？", "汇编", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                    if (ret == DialogResult.Yes)
                    {
                        保存ToolStripMenuItem_Click(sender, e);
                    }
                    rch.Text = "";
                    for (int i = 0; i < cnt; i++) rch.Text = rch.Text + conv[i] + "\n";
                    lblshow.Text = "";
                    lblshow.Location = new Point(66, 23);
                    lastrow = -1;
                    this.Text = "编辑器 - 已汇编";
                }       
            }
            catch(System.Exception err)
            {
                return;
            }            
        }

        private void frmeditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (modified == true)
                {
                    DialogResult ret = MessageBox.Show(Owner, "文本已被修改，是否保存改变？", "汇编", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                    if (ret == DialogResult.Yes)
                    {
                        保存ToolStripMenuItem_Click(sender, e);
                    }
                    else if (ret == DialogResult.Cancel)
                    {
                        e.Cancel = true;
                    }
                }
            }
            catch(System.Exception err)
            {
                return;
            }                        
        }

    }
}
