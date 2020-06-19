namespace craftersmine.OCVM.GUI
{
    partial class VMForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.machineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.shutdownMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.restartMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.configureMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.restart = new System.Windows.Forms.ToolStripButton();
            this.shutdown = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.configure = new System.Windows.Forms.ToolStripButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.status = new System.Windows.Forms.ToolStripStatusLabel();
            this.display1 = new craftersmine.OCVM.Core.Display();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.machineToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(508, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // machineToolStripMenuItem
            // 
            this.machineToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.shutdownMenu,
            this.restartMenu,
            this.toolStripMenuItem1,
            this.configureMenu,
            this.toolStripMenuItem2,
            this.exitMenu});
            this.machineToolStripMenuItem.Name = "machineToolStripMenuItem";
            this.machineToolStripMenuItem.Size = new System.Drawing.Size(65, 20);
            this.machineToolStripMenuItem.Text = "Machine";
            // 
            // shutdownMenu
            // 
            this.shutdownMenu.Image = global::craftersmine.OCVM.GUI.Properties.Resources.stop;
            this.shutdownMenu.Name = "shutdownMenu";
            this.shutdownMenu.Size = new System.Drawing.Size(128, 22);
            this.shutdownMenu.Text = "Shutdown";
            // 
            // restartMenu
            // 
            this.restartMenu.Image = global::craftersmine.OCVM.GUI.Properties.Resources.restart;
            this.restartMenu.Name = "restartMenu";
            this.restartMenu.Size = new System.Drawing.Size(128, 22);
            this.restartMenu.Text = "Restart";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(125, 6);
            // 
            // configureMenu
            // 
            this.configureMenu.Image = global::craftersmine.OCVM.GUI.Properties.Resources.configure;
            this.configureMenu.Name = "configureMenu";
            this.configureMenu.Size = new System.Drawing.Size(128, 22);
            this.configureMenu.Text = "Configure";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(125, 6);
            // 
            // exitMenu
            // 
            this.exitMenu.Image = global::craftersmine.OCVM.GUI.Properties.Resources.exit;
            this.exitMenu.Name = "exitMenu";
            this.exitMenu.Size = new System.Drawing.Size(128, 22);
            this.exitMenu.Text = "Exit";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutMenu});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutMenu
            // 
            this.aboutMenu.Image = global::craftersmine.OCVM.GUI.Properties.Resources.help;
            this.aboutMenu.Name = "aboutMenu";
            this.aboutMenu.Size = new System.Drawing.Size(107, 22);
            this.aboutMenu.Text = "About";
            this.aboutMenu.Click += new System.EventHandler(this.aboutMenu_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.restart,
            this.shutdown,
            this.toolStripSeparator1,
            this.configure});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(508, 25);
            this.toolStrip1.TabIndex = 4;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // restart
            // 
            this.restart.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.restart.Image = global::craftersmine.OCVM.GUI.Properties.Resources.restart;
            this.restart.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.restart.Name = "restart";
            this.restart.Size = new System.Drawing.Size(23, 22);
            this.restart.Text = "Restart";
            this.restart.Click += new System.EventHandler(this.Restart_Click);
            // 
            // shutdown
            // 
            this.shutdown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.shutdown.Image = global::craftersmine.OCVM.GUI.Properties.Resources.stop;
            this.shutdown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.shutdown.Name = "shutdown";
            this.shutdown.Size = new System.Drawing.Size(23, 22);
            this.shutdown.Text = "Shutdown";
            this.shutdown.Click += new System.EventHandler(this.Shutdown_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // configure
            // 
            this.configure.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.configure.Image = global::craftersmine.OCVM.GUI.Properties.Resources.configure;
            this.configure.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.configure.Name = "configure";
            this.configure.Size = new System.Drawing.Size(23, 22);
            this.configure.Text = "Configure";
            this.configure.Click += new System.EventHandler(this.Configure_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.status});
            this.statusStrip1.Location = new System.Drawing.Point(0, 306);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.ShowItemToolTips = true;
            this.statusStrip1.Size = new System.Drawing.Size(508, 24);
            this.statusStrip1.TabIndex = 5;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // status
            // 
            this.status.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.status.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.status.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.status.Name = "status";
            this.status.Size = new System.Drawing.Size(462, 19);
            this.status.Spring = true;
            this.status.Text = "{status}";
            this.status.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // display1
            // 
            this.display1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(27)))), ((int)(((byte)(27)))));
            this.display1.CursorPosition = new System.Drawing.Point(0, 0);
            this.display1.DisplayHeight = 16;
            this.display1.DisplayWidth = 50;
            this.display1.EnableCursor = true;
            this.display1.Font = new System.Drawing.Font("Lucida Console", 12F);
            this.display1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.display1.Location = new System.Drawing.Point(0, 49);
            this.display1.Name = "display1";
            this.display1.ShowCharactersBounds = false;
            this.display1.Size = new System.Drawing.Size(508, 256);
            this.display1.TabIndex = 6;
            // 
            // VMForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(508, 330);
            this.Controls.Add(this.display1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "VMForm";
            this.Text = "OpenComputer Virtual Machine - {vmName}";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripMenuItem machineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem shutdownMenu;
        private System.Windows.Forms.ToolStripMenuItem restartMenu;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem configureMenu;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem exitMenu;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutMenu;
        private System.Windows.Forms.ToolStripButton restart;
        private System.Windows.Forms.ToolStripButton shutdown;
        private System.Windows.Forms.ToolStripStatusLabel status;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton configure;
        private Core.Display display1;
    }
}

