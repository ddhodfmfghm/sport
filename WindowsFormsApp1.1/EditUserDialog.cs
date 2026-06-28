using System;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class EditUserDialog : Form
    {
        public string Login { get; private set; }
        public string Password { get; private set; }
        public string Role { get; private set; }
        private readonly bool isEditMode;
        private CheckBox chkShowPassword;

        public EditUserDialog(string existingLogin = null, string existingRole = null, string existingPassword = null)
        {
            InitializeComponent();
            isEditMode = !string.IsNullOrEmpty(existingLogin);

            // Создаём чекбокс "Показать пароль" программно
            chkShowPassword = new CheckBox();
            chkShowPassword.Text = "Показать пароль";
            chkShowPassword.ForeColor = System.Drawing.Color.White;
            chkShowPassword.AutoSize = true;
            chkShowPassword.Location = new System.Drawing.Point(120, 110); // подстраиваем под макет формы
            chkShowPassword.CheckedChanged += (s, e) =>
            {
                txtPassword.PasswordChar = chkShowPassword.Checked ? '\0' : '*';
                txtConfirm.PasswordChar = chkShowPassword.Checked ? '\0' : '*';
            };
            this.Controls.Add(chkShowPassword);

            if (isEditMode)
            {
                Text = "Редактирование пользователя";
                txtLogin.Text = existingLogin;
                txtLogin.Enabled = false;
                cmbRole.Text = existingRole;

                // Если передан пароль, показываем его в открытом виде
                if (!string.IsNullOrEmpty(existingPassword))
                {
                    txtPassword.Text = existingPassword;
                    txtConfirm.Text = existingPassword;
                    chkShowPassword.Checked = false;
                    txtPassword.PasswordChar = '*';
                    txtConfirm.PasswordChar = '*';
                }
                lblPasswordInfo.Text = "Не изменяйте, чтобы не менять пароль";
            }
            else
            {
                Text = "Добавление пользователя";
                lblPasswordInfo.Text = "Введите пароль";
                chkShowPassword.Visible = false; // при добавлении чекбокс не нужен
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Text;
            string confirm = txtConfirm.Text;
            string role = cmbRole.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(login))
            {
                MessageBox.Show("Логин обязателен", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!isEditMode && string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Пароль обязателен для нового пользователя", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!string.IsNullOrEmpty(password) && password != confirm)
            {
                MessageBox.Show("Пароли не совпадают", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(role))
            {
                MessageBox.Show("Выберите роль", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Login = login;
            Password = password;
            Role = role;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}