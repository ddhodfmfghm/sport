using System;
using System.Data.OleDb;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class RegisterForm : Form
    {
        private readonly string connectionString = Properties.Settings.Default.Microsoft_Access_База_данныхConnectionString;

        public RegisterForm()
        {
            InitializeComponent();
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Text;
            string confirm = txtConfirm.Text;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Заполните логин и пароль.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (password != confirm)
            {
                MessageBox.Show("Пароль и подтверждение не совпадают.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (password.Length < 4)
            {
                MessageBox.Show("Пароль должен содержать не менее 4 символов.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    string checkSql = "SELECT COUNT(*) FROM Пользователи WHERE Логин = ?";
                    using (OleDbCommand cmdCheck = new OleDbCommand(checkSql, conn))
                    {
                        cmdCheck.Parameters.AddWithValue("@login", login);
                        int exists = (int)cmdCheck.ExecuteScalar();
                        if (exists > 0)
                        {
                            MessageBox.Show("Пользователь с таким логином уже существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    string insertClientSql = "INSERT INTO Клиенты (Фамилия, Имя, Телефон) VALUES (?, ?, ?)";
                    int clientId;
                    using (OleDbCommand cmdClient = new OleDbCommand(insertClientSql, conn))
                    {
                        cmdClient.Parameters.AddWithValue("@fam", login);
                        cmdClient.Parameters.AddWithValue("@im", "Новый клиент");
                        cmdClient.Parameters.AddWithValue("@tel", "Не указан");
                        cmdClient.ExecuteNonQuery();

                        string getLastId = "SELECT @@IDENTITY";
                        using (OleDbCommand cmdLast = new OleDbCommand(getLastId, conn))
                        {
                            clientId = Convert.ToInt32(cmdLast.ExecuteScalar());
                        }
                    }

                    string insertUserSql = @"INSERT INTO Пользователи (Логин, Пароль, Роль, Заблокирован, Попытки, ID_клиента) 
                                             VALUES (?, ?, 'User', 0, 0, ?)";
                    using (OleDbCommand cmdInsert = new OleDbCommand(insertUserSql, conn))
                    {
                        cmdInsert.Parameters.AddWithValue("@login", login);
                        cmdInsert.Parameters.AddWithValue("@pwd", password);
                        cmdInsert.Parameters.AddWithValue("@clientId", clientId);
                        cmdInsert.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Регистрация прошла успешно! Теперь вы можете войти.\n\nВаш профиль клиента создан автоматически.",
                                "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при регистрации: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}