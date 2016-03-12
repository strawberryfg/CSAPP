namespace Y86
{
    partial class frmmain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmmain));
            this.btneditor = new System.Windows.Forms.Button();
            this.btncpu = new System.Windows.Forms.Button();
            this.pic3 = new System.Windows.Forms.PictureBox();
            this.pic2 = new System.Windows.Forms.PictureBox();
            this.pic6 = new System.Windows.Forms.PictureBox();
            this.pic5 = new System.Windows.Forms.PictureBox();
            this.pic4 = new System.Windows.Forms.PictureBox();
            this.pic1 = new System.Windows.Forms.PictureBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.y86ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.编译器ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.处理器ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.帮助ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.关于ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.pic3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic1)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btneditor
            // 
            this.btneditor.Location = new System.Drawing.Point(33, 105);
            this.btneditor.Name = "btneditor";
            this.btneditor.Size = new System.Drawing.Size(140, 67);
            this.btneditor.TabIndex = 0;
            this.btneditor.Text = "编译器";
            this.btneditor.UseVisualStyleBackColor = true;
            this.btneditor.Click += new System.EventHandler(this.btneditor_Click);
            // 
            // btncpu
            // 
            this.btncpu.Location = new System.Drawing.Point(32, 238);
            this.btncpu.Name = "btncpu";
            this.btncpu.Size = new System.Drawing.Size(141, 71);
            this.btncpu.TabIndex = 1;
            this.btncpu.Text = "处理器";
            this.btncpu.UseVisualStyleBackColor = true;
            this.btncpu.Click += new System.EventHandler(this.btncpu_Click);
            // 
            // pic3
            // 
            this.pic3.Image = global::Y86.Properties.Resources.pj3;
            this.pic3.Location = new System.Drawing.Point(985, 36);
            this.pic3.Name = "pic3";
            this.pic3.Size = new System.Drawing.Size(300, 300);
            this.pic3.TabIndex = 2;
            this.pic3.TabStop = false;
            // 
            // pic2
            // 
            this.pic2.Image = global::Y86.Properties.Resources.pj2;
            this.pic2.Location = new System.Drawing.Point(612, 36);
            this.pic2.Name = "pic2";
            this.pic2.Size = new System.Drawing.Size(300, 300);
            this.pic2.TabIndex = 2;
            this.pic2.TabStop = false;
            // 
            // pic6
            // 
            this.pic6.Image = global::Y86.Properties.Resources.pj6;
            this.pic6.Location = new System.Drawing.Point(985, 376);
            this.pic6.Name = "pic6";
            this.pic6.Size = new System.Drawing.Size(300, 300);
            this.pic6.TabIndex = 2;
            this.pic6.TabStop = false;
            // 
            // pic5
            // 
            this.pic5.Image = global::Y86.Properties.Resources.pj5;
            this.pic5.Location = new System.Drawing.Point(612, 376);
            this.pic5.Name = "pic5";
            this.pic5.Size = new System.Drawing.Size(300, 300);
            this.pic5.TabIndex = 2;
            this.pic5.TabStop = false;
            // 
            // pic4
            // 
            this.pic4.Image = global::Y86.Properties.Resources.pj4;
            this.pic4.Location = new System.Drawing.Point(235, 376);
            this.pic4.Name = "pic4";
            this.pic4.Size = new System.Drawing.Size(300, 300);
            this.pic4.TabIndex = 2;
            this.pic4.TabStop = false;
            // 
            // pic1
            // 
            this.pic1.Image = ((System.Drawing.Image)(resources.GetObject("pic1.Image")));
            this.pic1.Location = new System.Drawing.Point(235, 36);
            this.pic1.Name = "pic1";
            this.pic1.Size = new System.Drawing.Size(300, 300);
            this.pic1.TabIndex = 2;
            this.pic1.TabStop = false;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.y86ToolStripMenuItem,
            this.帮助ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1350, 25);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // y86ToolStripMenuItem
            // 
            this.y86ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.编译器ToolStripMenuItem,
            this.处理器ToolStripMenuItem});
            this.y86ToolStripMenuItem.Name = "y86ToolStripMenuItem";
            this.y86ToolStripMenuItem.Size = new System.Drawing.Size(41, 21);
            this.y86ToolStripMenuItem.Text = "Y86";
            // 
            // 编译器ToolStripMenuItem
            // 
            this.编译器ToolStripMenuItem.Name = "编译器ToolStripMenuItem";
            this.编译器ToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.编译器ToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.编译器ToolStripMenuItem.Text = "编译器";
            this.编译器ToolStripMenuItem.Click += new System.EventHandler(this.编译器ToolStripMenuItem_Click);
            // 
            // 处理器ToolStripMenuItem
            // 
            this.处理器ToolStripMenuItem.Name = "处理器ToolStripMenuItem";
            this.处理器ToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.处理器ToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.处理器ToolStripMenuItem.Text = "处理器";
            this.处理器ToolStripMenuItem.Click += new System.EventHandler(this.处理器ToolStripMenuItem_Click);
            // 
            // 帮助ToolStripMenuItem
            // 
            this.帮助ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.关于ToolStripMenuItem});
            this.帮助ToolStripMenuItem.Name = "帮助ToolStripMenuItem";
            this.帮助ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.帮助ToolStripMenuItem.Text = "帮助";
            // 
            // 关于ToolStripMenuItem
            // 
            this.关于ToolStripMenuItem.Name = "关于ToolStripMenuItem";
            this.关于ToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F4)));
            this.关于ToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.关于ToolStripMenuItem.Text = "关于";
            this.关于ToolStripMenuItem.Click += new System.EventHandler(this.关于ToolStripMenuItem_Click);
            // 
            // frmmain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1350, 730);
            this.Controls.Add(this.pic3);
            this.Controls.Add(this.pic2);
            this.Controls.Add(this.pic6);
            this.Controls.Add(this.pic5);
            this.Controls.Add(this.pic4);
            this.Controls.Add(this.pic1);
            this.Controls.Add(this.btncpu);
            this.Controls.Add(this.btneditor);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "frmmain";
            this.Text = "Y86处理器";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)(this.pic3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic1)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btneditor;
        private System.Windows.Forms.Button btncpu;
        private System.Windows.Forms.PictureBox pic1;
        private System.Windows.Forms.PictureBox pic4;
        private System.Windows.Forms.PictureBox pic5;
        private System.Windows.Forms.PictureBox pic6;
        private System.Windows.Forms.PictureBox pic2;
        private System.Windows.Forms.PictureBox pic3;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem y86ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 编译器ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 处理器ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 帮助ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 关于ToolStripMenuItem;
    }
}