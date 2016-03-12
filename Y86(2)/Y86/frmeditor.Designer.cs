namespace Y86
{
    partial class frmeditor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmeditor));
            this.rch = new System.Windows.Forms.RichTextBox();
            this.lst = new System.Windows.Forms.ListBox();
            this.lblshow = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.文件ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.新建ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.打开ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.保存ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.退出ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.汇编ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.汇编ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.关于ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.关于ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // rch
            // 
            this.rch.BackColor = System.Drawing.Color.White;
            this.rch.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rch.Font = new System.Drawing.Font("Comic Sans MS", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rch.Location = new System.Drawing.Point(67, 23);
            this.rch.Name = "rch";
            this.rch.Size = new System.Drawing.Size(473, 656);
            this.rch.TabIndex = 0;
            this.rch.Text = "";
            this.rch.SelectionChanged += new System.EventHandler(this.rch_SelectionChanged);
            this.rch.VScroll += new System.EventHandler(this.rch_VScroll);
            this.rch.TextChanged += new System.EventHandler(this.rch_TextChanged);
            // 
            // lst
            // 
            this.lst.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.lst.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lst.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.lst.Font = new System.Drawing.Font("Comic Sans MS", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lst.FormattingEnabled = true;
            this.lst.ItemHeight = 15;
            this.lst.Items.AddRange(new object[] {
            "   1",
            "   2",
            "   3",
            "   4",
            "   5",
            "   6",
            "   7",
            "   8",
            "   9",
            "  10",
            "  11",
            "  12",
            "  13",
            "  14",
            "  15",
            "  16",
            "  17",
            "  18",
            "  19",
            "  20",
            "  21",
            "  22",
            "  23",
            "  24",
            "  25",
            "  26",
            "  27",
            "  28",
            "  29",
            "  30",
            "  31",
            "  32",
            "  33",
            "  34",
            "  35",
            "  36",
            "  37",
            "  38"});
            this.lst.Location = new System.Drawing.Point(28, 23);
            this.lst.Name = "lst";
            this.lst.Size = new System.Drawing.Size(40, 660);
            this.lst.TabIndex = 1;
            this.lst.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.lst_DrawItem);
            this.lst.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.lst_MeasureItem);
            // 
            // lblshow
            // 
            this.lblshow.AutoSize = true;
            this.lblshow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.lblshow.Font = new System.Drawing.Font("Comic Sans MS", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblshow.ForeColor = System.Drawing.Color.Black;
            this.lblshow.Location = new System.Drawing.Point(67, 22);
            this.lblshow.Name = "lblshow";
            this.lblshow.Size = new System.Drawing.Size(0, 16);
            this.lblshow.TabIndex = 2;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.文件ToolStripMenuItem,
            this.汇编ToolStripMenuItem,
            this.关于ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(541, 25);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 文件ToolStripMenuItem
            // 
            this.文件ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.新建ToolStripMenuItem,
            this.打开ToolStripMenuItem,
            this.保存ToolStripMenuItem,
            this.退出ToolStripMenuItem});
            this.文件ToolStripMenuItem.Name = "文件ToolStripMenuItem";
            this.文件ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.文件ToolStripMenuItem.Text = "文件";
            // 
            // 新建ToolStripMenuItem
            // 
            this.新建ToolStripMenuItem.Name = "新建ToolStripMenuItem";
            this.新建ToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.新建ToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.新建ToolStripMenuItem.Text = "新建";
            this.新建ToolStripMenuItem.Click += new System.EventHandler(this.新建ToolStripMenuItem_Click);
            // 
            // 打开ToolStripMenuItem
            // 
            this.打开ToolStripMenuItem.Name = "打开ToolStripMenuItem";
            this.打开ToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.打开ToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.打开ToolStripMenuItem.Text = "打开";
            this.打开ToolStripMenuItem.Click += new System.EventHandler(this.打开ToolStripMenuItem_Click);
            // 
            // 保存ToolStripMenuItem
            // 
            this.保存ToolStripMenuItem.Name = "保存ToolStripMenuItem";
            this.保存ToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.保存ToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.保存ToolStripMenuItem.Text = "保存";
            this.保存ToolStripMenuItem.Click += new System.EventHandler(this.保存ToolStripMenuItem_Click);
            // 
            // 退出ToolStripMenuItem
            // 
            this.退出ToolStripMenuItem.Name = "退出ToolStripMenuItem";
            this.退出ToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.退出ToolStripMenuItem.Text = "退出";
            this.退出ToolStripMenuItem.Click += new System.EventHandler(this.退出ToolStripMenuItem_Click);
            // 
            // 汇编ToolStripMenuItem
            // 
            this.汇编ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.汇编ToolStripMenuItem1});
            this.汇编ToolStripMenuItem.Name = "汇编ToolStripMenuItem";
            this.汇编ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.汇编ToolStripMenuItem.Text = "汇编";
            // 
            // 汇编ToolStripMenuItem1
            // 
            this.汇编ToolStripMenuItem1.Name = "汇编ToolStripMenuItem1";
            this.汇编ToolStripMenuItem1.ShortcutKeys = System.Windows.Forms.Keys.F10;
            this.汇编ToolStripMenuItem1.Size = new System.Drawing.Size(128, 22);
            this.汇编ToolStripMenuItem1.Text = "汇编";
            this.汇编ToolStripMenuItem1.Click += new System.EventHandler(this.汇编ToolStripMenuItem1_Click);
            // 
            // 关于ToolStripMenuItem
            // 
            this.关于ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.关于ToolStripMenuItem1});
            this.关于ToolStripMenuItem.Name = "关于ToolStripMenuItem";
            this.关于ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.关于ToolStripMenuItem.Text = "关于";
            // 
            // 关于ToolStripMenuItem1
            // 
            this.关于ToolStripMenuItem1.Name = "关于ToolStripMenuItem1";
            this.关于ToolStripMenuItem1.Size = new System.Drawing.Size(100, 22);
            this.关于ToolStripMenuItem1.Text = "关于";
            this.关于ToolStripMenuItem1.Click += new System.EventHandler(this.关于ToolStripMenuItem1_Click);
            // 
            // frmeditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(541, 702);
            this.Controls.Add(this.lblshow);
            this.Controls.Add(this.rch);
            this.Controls.Add(this.lst);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "frmeditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "编辑器";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmeditor_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox rch;
        private System.Windows.Forms.ListBox lst;
        private System.Windows.Forms.Label lblshow;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 文件ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 打开ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 新建ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 保存ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 退出ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 汇编ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 汇编ToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem 关于ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 关于ToolStripMenuItem1;
    }
}