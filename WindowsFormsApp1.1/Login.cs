using System;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Login : Form
    {
        private string connectionString = Properties.Settings.Default.Microsoft_Access_База_данныхConnectionString;

        public Login()
        {
            InitializeComponent();
            EnsureDatabaseStructure();
            EnsureAdminExists();
        }

        private void EnsureDatabaseStructure()
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();
                DataTable dt = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, "Пользователи", null });
                if (dt.Rows.Count == 0)
                {
                    string createTable = @"CREATE TABLE Пользователи (
                        ID AUTOINCREMENT PRIMARY KEY,
                        Логин TEXT(255) NOT NULL UNIQUE,
                        Пароль TEXT(255) NOT NULL,
                        Роль TEXT(50) NOT NULL,
                        Заблокирован BIT DEFAULT 0,
                        Попытки INTEGER DEFAULT 0,
                        Дата_создания DATETIME DEFAULT NOW(),
                        ID_клиента INTEGER
                    )";
                    using (OleDbCommand cmd = new OleDbCommand(createTable, conn))
                        cmd.ExecuteNonQuery();
                }
                else
                {
                    DataTable columns = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, new object[] { null, null, "Пользователи", null });
                    bool hasClientId = false;
                    foreach (DataRow row in columns.Rows)
                    {
                        if (row["COLUMN_NAME"].ToString() == "ID_клиента")
                        {
                            hasClientId = true;
                            break;
                        }
                    }
                    if (!hasClientId)
                    {
                        string alterSql = "ALTER TABLE Пользователи ADD COLUMN ID_клиента INTEGER";
                        using (OleDbCommand cmd = new OleDbCommand(alterSql, conn))
                            cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private void EnsureAdminExists()
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();
                string checkAdmin = "SELECT COUNT(*) FROM Пользователи WHERE Логин = 'admin'";
                using (OleDbCommand cmd = new OleDbCommand(checkAdmin, conn))
                {
                    int count = (int)cmd.ExecuteScalar();
                    if (count == 0)
                    {
                        string insert = "INSERT INTO Пользователи (Логин, Пароль, Роль, Заблокирован, Попытки, ID_клиента) VALUES ('admin', @pwd, 'Admin', 0, 0, NULL)";
                        using (OleDbCommand insertCmd = new OleDbCommand(insert, conn))
                        {
                            insertCmd.Parameters.AddWithValue("@pwd", "admin123");
                            insertCmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите логин и пароль", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT ID, Логин, Пароль, Роль, Заблокирован, Попытки, ID_клиента FROM Пользователи WHERE Логин = ?";
                using (OleDbCommand cmd = new OleDbCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@login", login);
                    using (OleDbDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int blockedInt = Convert.ToInt32(reader["Заблокирован"]);
                            bool isBlocked = (blockedInt != 0);
                            int attempts = Convert.ToInt32(reader["Попытки"]);
                            string storedPassword = reader.GetString(2);

                            if (isBlocked)
                            {
                                MessageBox.Show("Аккаунт заблокирован. Обратитесь к администратору.", "Доступ запрещён", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                            if (password == storedPassword)
                            {
                                ResetAttempts(login);
                                int userId = reader.GetInt32(0);
                                string role = reader.GetString(3);
                                int? clientId = reader.IsDBNull(6) ? (int?)null : reader.GetInt32(6);
                                Главная main = new Главная(userId, role, clientId);
                                main.Show();
                                this.Hide();
                            }
                            else
                            {
                                attempts++;
                                UpdateAttempts(login, attempts);
                                int remaining = 3 - attempts;
                                if (attempts >= 3)
                                {
                                    BlockUser(login);
                                    MessageBox.Show("Превышено количество попыток. Аккаунт заблокирован.", "Блокировка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                                else
                                {
                                    MessageBox.Show($"Неверный пароль. Осталось попыток: {remaining}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Пользователь не найден", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            using (RegisterForm regForm = new RegisterForm())
            {
                if (regForm.ShowDialog() == DialogResult.OK)
                {
                    txtLogin.Clear();
                    txtPassword.Clear();
                }
            }
        }

        private void ResetAttempts(string login)
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();
                string sql = "UPDATE Пользователи SET Попытки = 0 WHERE Логин = ?";
                using (OleDbCommand cmd = new OleDbCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@login", login);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void UpdateAttempts(string login, int attempts)
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();
                string sql = "UPDATE Пользователи SET Попытки = ? WHERE Логин = ?";
                using (OleDbCommand cmd = new OleDbCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@attempts", attempts);
                    cmd.Parameters.AddWithValue("@login", login);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void BlockUser(string login)
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();
                string sql = "UPDATE Пользователи SET Заблокирован = true WHERE Логин = ?";
                using (OleDbCommand cmd = new OleDbCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@login", login);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}