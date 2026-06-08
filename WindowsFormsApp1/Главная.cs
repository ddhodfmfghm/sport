using System;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Главная : Form
    {
        private int currentUserId;
        private string currentRole;

        public Главная(int userId, string role)
        {
            InitializeComponent();
            currentUserId = userId;
            currentRole = role;

            lblWelcome.Text = $"Добро пожаловать";

            
          
            управлениеПользователямиToolStripMenuItem.Visible = false;
            

            ToolStripMenuItem сменаПароляToolStripMenuItem = new ToolStripMenuItem("Сменить пароль");
            сменаПароляToolStripMenuItem.Click += СменаПароляToolStripMenuItem_Click;
            menuStrip1.Items.Add(сменаПароляToolStripMenuItem);
        }

        private void button1_Click(object sender, EventArgs e) => Application.Exit();

        private void абонементыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Абонементы a = new Абонементы(currentRole);
            a.Show();
        }

        private void клиентыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Клиенты b = new Клиенты(currentRole);
            b.Show();
        }

        private void посещенияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Посещения c = new Посещения(currentRole);
            c.Show();
        }

        private void услугиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Услуги d = new Услуги(currentRole);
            d.Show();
        }

        private void запрос1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Запрос1 e1 = new Запрос1();
            e1.Show();
        }

        private void запрос2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Запрос2 f = new Запрос2();
            f.Show();
        }

        private void запрос3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Запрос3 h = new Запрос3();
            h.Show();
        }

        private void управлениеПользователямиToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void СменаПароляToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (ChangePasswordForm form = new ChangePasswordForm(currentUserId))
                form.ShowDialog();
        }

        private void выйтиИзУчётнойЗаписиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Login l = new Login();
            l.Show();
            this.Hide();
        }

        private void asdasToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}