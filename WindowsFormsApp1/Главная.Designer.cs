namespace WindowsFormsApp1
{
    partial class Главная
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.tablesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.абонементыToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.клиентыToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.посещенияToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.услугиToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.queriesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.запрос1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.запрос2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.запрос3ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.управлениеПользователямиToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.выйтиИзУчётнойЗаписиToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panelWelcome = new System.Windows.Forms.Panel();
            this.lblWelcome = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.panelWelcome.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Location = new System.Drawing.Point(480, 200);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(100, 40);
            this.button1.TabIndex = 4;
            this.button1.Text = "ВЫХОД";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(62)))), ((int)(((byte)(80)))));
            this.menuStrip1.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.menuStrip1.ForeColor = System.Drawing.Color.White;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tablesToolStripMenuItem,
            this.queriesToolStripMenuItem,
            this.управлениеПользователямиToolStripMenuItem,
            this.выйтиИзУчётнойЗаписиToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(600, 27);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // tablesToolStripMenuItem
            // 
            this.tablesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.абонементыToolStripMenuItem,
            this.клиентыToolStripMenuItem,
            this.посещенияToolStripMenuItem,
            this.услугиToolStripMenuItem});
            this.tablesToolStripMenuItem.Name = "tablesToolStripMenuItem";
            this.tablesToolStripMenuItem.Size = new System.Drawing.Size(76, 23);
            this.tablesToolStripMenuItem.Text = "Таблицы";
            // 
            // абонементыToolStripMenuItem
            // 
            this.абонементыToolStripMenuItem.Name = "абонементыToolStripMenuItem";
            this.абонементыToolStripMenuItem.Size = new System.Drawing.Size(159, 24);
            this.абонементыToolStripMenuItem.Text = "Абонементы";
            this.абонементыToolStripMenuItem.Click += new System.EventHandler(this.абонементыToolStripMenuItem_Click);
            // 
            // клиентыToolStripMenuItem
            // 
            this.клиентыToolStripMenuItem.Name = "клиентыToolStripMenuItem";
            this.клиентыToolStripMenuItem.Size = new System.Drawing.Size(159, 24);
            this.клиентыToolStripMenuItem.Text = "Клиенты";
            this.клиентыToolStripMenuItem.Click += new System.EventHandler(this.клиентыToolStripMenuItem_Click);
            // 
            // посещенияToolStripMenuItem
            // 
            this.посещенияToolStripMenuItem.Name = "посещенияToolStripMenuItem";
            this.посещенияToolStripMenuItem.Size = new System.Drawing.Size(159, 24);
            this.посещенияToolStripMenuItem.Text = "Посещения";
            this.посещенияToolStripMenuItem.Click += new System.EventHandler(this.посещенияToolStripMenuItem_Click);
            // 
            // услугиToolStripMenuItem
            // 
            this.услугиToolStripMenuItem.Name = "услугиToolStripMenuItem";
            this.услугиToolStripMenuItem.Size = new System.Drawing.Size(159, 24);
            this.услугиToolStripMenuItem.Text = "Услуги";
            this.услугиToolStripMenuItem.Click += new System.EventHandler(this.услугиToolStripMenuItem_Click);
            // 
            // queriesToolStripMenuItem
            // 
            this.queriesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.запрос1ToolStripMenuItem,
            this.запрос2ToolStripMenuItem,
            this.запрос3ToolStripMenuItem});
            this.queriesToolStripMenuItem.Name = "queriesToolStripMenuItem";
            this.queriesToolStripMenuItem.Size = new System.Drawing.Size(76, 23);
            this.queriesToolStripMenuItem.Text = "Запросы";
            // 
            // запрос1ToolStripMenuItem
            // 
            this.запрос1ToolStripMenuItem.Name = "запрос1ToolStripMenuItem";
            this.запрос1ToolStripMenuItem.Size = new System.Drawing.Size(135, 24);
            this.запрос1ToolStripMenuItem.Text = "Запрос 1";
            this.запрос1ToolStripMenuItem.Click += new System.EventHandler(this.запрос1ToolStripMenuItem_Click);
            // 
            // запрос2ToolStripMenuItem
            // 
            this.запрос2ToolStripMenuItem.Name = "запрос2ToolStripMenuItem";
            this.запрос2ToolStripMenuItem.Size = new System.Drawing.Size(135, 24);
            this.запрос2ToolStripMenuItem.Text = "Запрос 2";
            this.запрос2ToolStripMenuItem.Click += new System.EventHandler(this.запрос2ToolStripMenuItem_Click);
            // 
            // запрос3ToolStripMenuItem
            // 
            this.запрос3ToolStripMenuItem.Name = "запрос3ToolStripMenuItem";
            this.запрос3ToolStripMenuItem.Size = new System.Drawing.Size(135, 24);
            this.запрос3ToolStripMenuItem.Text = "Запрос 3";
            this.запрос3ToolStripMenuItem.Click += new System.EventHandler(this.запрос3ToolStripMenuItem_Click);
            // 
            // управлениеПользователямиToolStripMenuItem
            // 
            this.управлениеПользователямиToolStripMenuItem.Name = "управлениеПользователямиToolStripMenuItem";
            this.управлениеПользователямиToolStripMenuItem.Size = new System.Drawing.Size(203, 23);
            this.управлениеПользователямиToolStripMenuItem.Text = "Управление пользователями";
            this.управлениеПользователямиToolStripMenuItem.Click += new System.EventHandler(this.управлениеПользователямиToolStripMenuItem_Click);
            // 
            // выйтиИзУчётнойЗаписиToolStripMenuItem
            // 
            this.выйтиИзУчётнойЗаписиToolStripMenuItem.Name = "выйтиИзУчётнойЗаписиToolStripMenuItem";
            this.выйтиИзУчётнойЗаписиToolStripMenuItem.Size = new System.Drawing.Size(61, 23);
            this.выйтиИзУчётнойЗаписиToolStripMenuItem.Text = "Выйти";
            this.выйтиИзУчётнойЗаписиToolStripMenuItem.Click += new System.EventHandler(this.выйтиИзУчётнойЗаписиToolStripMenuItem_Click);
            // 
            // panelWelcome
            // 
            this.panelWelcome.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.panelWelcome.Controls.Add(this.lblWelcome);
            this.panelWelcome.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelWelcome.Location = new System.Drawing.Point(0, 27);
            this.panelWelcome.Name = "panelWelcome";
            this.panelWelcome.Size = new System.Drawing.Size(600, 60);
            this.panelWelcome.TabIndex = 3;
            // 
            // lblWelcome
            // 
            this.lblWelcome.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblWelcome.ForeColor = System.Drawing.Color.White;
            this.lblWelcome.Location = new System.Drawing.Point(20, 15);
            this.lblWelcome.Name = "lblWelcome";
            this.lblWelcome.Size = new System.Drawing.Size(560, 30);
            this.lblWelcome.TabIndex = 0;
            this.lblWelcome.Text = "Добро пожаловать!";
            this.lblWelcome.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Главная
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(42)))), ((int)(((byte)(58)))));
            this.ClientSize = new System.Drawing.Size(600, 260);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.panelWelcome);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Главная";
            this.Text = "Спортивный комплекс - Главная";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panelWelcome.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tablesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem queriesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem абонементыToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem клиентыToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem посещенияToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem услугиToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem запрос1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem запрос2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem запрос3ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem управлениеПользователямиToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem выйтиИзУчётнойЗаписиToolStripMenuItem;
        private System.Windows.Forms.Panel panelWelcome;
        private System.Windows.Forms.Label lblWelcome;
    }
}