using System;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Главная : Form
    {
        private int currentUserId;
        private string currentRole;
        private int? currentClientId;

        public Главная(int userId, string role, int? clientId)
        {
            InitializeComponent();
            currentUserId = userId;
            currentRole = role;
            currentClientId = clientId;

            lblWelcome.Text = $"Добро пожаловать";
            управлениеПользователямиToolStripMenuItem.Visible = false;
            if (currentRole != "Admin")
            {
                // Скрываем все лишние пункты для обычного пользователя
                абонементыToolStripMenuItem.Visible = false;
                клиентыToolStripMenuItem.Visible = false;
                посещенияToolStripMenuItem.Visible = false;
                queriesToolStripMenuItem.Visible = false;      // Запросы
                управлениеПользователямиToolStripMenuItem.Visible = false;

                // Добавляем "Мои абонементы" вместо "Абонементы"
                ToolStripMenuItem моиАбонементы = new ToolStripMenuItem("Мои абонементы");
                моиАбонементы.Click += (s, e) => {
                    МоиАбонементы myForm = new МоиАбонементы(currentUserId, currentClientId);
                    myForm.Show();
                };
                tablesToolStripMenuItem.DropDownItems.Add(моиАбонементы);
            }
            else
            {
                // Для админа добавляем тоже "Мои абонементы" (опционально)
                ToolStripMenuItem моиАбонементы = new ToolStripMenuItem("Мои абонементы (админ)");
                моиАбонементы.Click += (s, e) => {
                    МоиАбонементы myForm = new МоиАбонементы(currentUserId, currentClientId);
                    myForm.Show();
                };
                tablesToolStripMenuItem.DropDownItems.Add(моиАбонементы);
            }

            // Кнопка смены пароля (доступна всем)
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
            UserManagement um = new UserManagement();
            um.ShowDialog();
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
    }
}