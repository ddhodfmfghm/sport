using System;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;
using System.Drawing;
using System.Security.Cryptography;

namespace WindowsFormsApp1
{
    public partial class UserManagement : Form
    {
        private readonly string connectionString = Properties.Settings.Default.Microsoft_Access_База_данныхConnectionString;
        private DataTable usersTable;
        private DataView dataView;

        public UserManagement()
        {
            InitializeComponent();
            btnAdd.Click += BtnAdd_Click;
            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;
            btnBlock.Click += BtnBlock_Click;
            btnUnblock.Click += BtnUnblock_Click;
            btnResetPassword.Click += BtnResetPassword_Click;
            btnResetAttempts.Click += BtnResetAttempts_Click;
            btnSearch.Click += BtnSearch_Click;
            btnRefresh.Click += BtnRefresh_Click;

            txtSearch.Enter += (s, e) =>
            {
                if (txtSearch.Text == "Поиск по логину или роли...")
                {
                    txtSearch.Text = "";
                    txtSearch.ForeColor = Color.Black;
                }
            };
            txtSearch.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    txtSearch.Text = "Поиск по логину или роли...";
                    txtSearch.ForeColor = Color.Gray;
                }
            };

            LoadUsers();
        }

        private void LoadUsers(string searchText = "")
        {
            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();
                    string sql = "SELECT ID, Логин, Роль, Заблокирован, Попытки, ID_клиента FROM Пользователи";
                    OleDbDataAdapter da = new OleDbDataAdapter(sql, conn);
                    usersTable = new DataTable();
                    da.Fill(usersTable);
                }

                dataView = new DataView(usersTable);
                if (!string.IsNullOrWhiteSpace(searchText) && searchText != "Поиск по логину или роли...")
                {
                    dataView.RowFilter = $"Логин LIKE '%{EscapeLikeValue(searchText)}%' OR Роль LIKE '%{EscapeLikeValue(searchText)}%'";
                }
                dgvUsers.DataSource = dataView;

                if (dgvUsers.Columns.Count > 0) dgvUsers.Columns[0].Visible = false;
                if (dgvUsers.Columns.Count > 1) dgvUsers.Columns[1].HeaderText = "Логин";
                if (dgvUsers.Columns.Count > 2) dgvUsers.Columns[2].HeaderText = "Роль";
                if (dgvUsers.Columns.Count > 3) dgvUsers.Columns[3].HeaderText = "Заблокирован";
                if (dgvUsers.Columns.Count > 4) dgvUsers.Columns[4].HeaderText = "Неудачные попытки";
                if (dgvUsers.Columns.Count > 5) dgvUsers.Columns[5].HeaderText = "ID клиента";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string EscapeLikeValue(string value) => value.Replace("[", "[[]").Replace("%", "[%]");

        private int? GetSelectedUserId()
        {
            if (dgvUsers.CurrentRow == null)
            {
                MessageBox.Show("Выберите пользователя.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }
            return Convert.ToInt32(dgvUsers.CurrentRow.Cells[0].Value);
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            using (EditUserDialog dialog = new EditUserDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (OleDbConnection conn = new OleDbConnection(connectionString))
                        {
                            conn.Open();
                            string checkSql = "SELECT COUNT(*) FROM Пользователи WHERE Логин = ?";
                            using (OleDbCommand cmd = new OleDbCommand(checkSql, conn))
                            {
                                cmd.Parameters.AddWithValue("@login", dialog.Login);
                                if ((int)cmd.ExecuteScalar() > 0)
                                {
                                    MessageBox.Show("Логин уже существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return;
                                }
                            }
                            string insert = "INSERT INTO Пользователи (Логин, Пароль, Роль, Заблокирован, Попытки, ID_клиента) VALUES (?,?,?,0,0,NULL)";
                            using (OleDbCommand cmd = new OleDbCommand(insert, conn))
                            {
                                cmd.Parameters.AddWithValue("@login", dialog.Login);
                                cmd.Parameters.AddWithValue("@pwd", dialog.Password);
                                cmd.Parameters.AddWithValue("@role", dialog.Role);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        LoadUsers();
                        MessageBox.Show("Пользователь добавлен.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex) { MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                }
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            int? id = GetSelectedUserId();
            if (id == null) return;

            string login = dgvUsers.CurrentRow.Cells[1].Value.ToString();
            string role = dgvUsers.CurrentRow.Cells[2].Value.ToString();
            string currentPassword = "";

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT Пароль FROM Пользователи WHERE ID = ?";
                using (OleDbCommand cmd = new OleDbCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                        currentPassword = result.ToString();
                }
            }

            using (EditUserDialog dialog = new EditUserDialog(login, role, currentPassword))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (OleDbConnection conn = new OleDbConnection(connectionString))
                        {
                            conn.Open();
                            if (!string.IsNullOrEmpty(dialog.Password))
                            {
                                string sql = "UPDATE Пользователи SET Роль=?, Пароль=? WHERE ID=?";
                                using (OleDbCommand cmd = new OleDbCommand(sql, conn))
                                {
                                    cmd.Parameters.AddWithValue("@role", dialog.Role);
                                    cmd.Parameters.AddWithValue("@pwd", dialog.Password);
                                    cmd.Parameters.AddWithValue("@id", id);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                string sql = "UPDATE Пользователи SET Роль=? WHERE ID=?";
                                using (OleDbCommand cmd = new OleDbCommand(sql, conn))
                                {
                                    cmd.Parameters.AddWithValue("@role", dialog.Role);
                                    cmd.Parameters.AddWithValue("@id", id);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                        LoadUsers();
                        MessageBox.Show("Данные обновлены.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex) { MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                }
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            int? id = GetSelectedUserId();
            if (id == null) return;
            if (MessageBox.Show("Удалить пользователя?", "Подтверждение", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    using (OleDbConnection conn = new OleDbConnection(connectionString))
                    {
                        conn.Open();
                        string sql = "DELETE FROM Пользователи WHERE ID=?";
                        using (OleDbCommand cmd = new OleDbCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", id);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    LoadUsers();
                    MessageBox.Show("Пользователь удалён.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex) { MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
        }

        private void BtnBlock_Click(object sender, EventArgs e)
        {
            int? id = GetSelectedUserId();
            if (id == null) return;
            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();
                    string sql = "UPDATE Пользователи SET Заблокирован=true WHERE ID=?";
                    using (OleDbCommand cmd = new OleDbCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
                LoadUsers();
                MessageBox.Show("Пользователь заблокирован.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) { MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void BtnUnblock_Click(object sender, EventArgs e)
        {
            int? id = GetSelectedUserId();
            if (id == null) return;
            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();
                    string sql = "UPDATE Пользователи SET Заблокирован=false, Попытки=0 WHERE ID=?";
                    using (OleDbCommand cmd = new OleDbCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
                LoadUsers();
                MessageBox.Show("Пользователь разблокирован.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) { MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void BtnResetPassword_Click(object sender, EventArgs e)
        {
            int? id = GetSelectedUserId();
            if (id == null) return;

            string newPass = GenerateRandomPassword(8);

            if (MessageBox.Show($"Сбросить пароль на новый случайный пароль?\n\nОн будет показан в следующем окне.", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    using (OleDbConnection conn = new OleDbConnection(connectionString))
                    {
                        conn.Open();
                        string sql = "UPDATE Пользователи SET Пароль=? WHERE ID=?";
                        using (OleDbCommand cmd = new OleDbCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("@pwd", newPass);
                            cmd.Parameters.AddWithValue("@id", id);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    MessageBox.Show($"Пароль успешно сброшен!\n\nНовый пароль: {newPass}\n\nПожалуйста, скопируйте и передайте его пользователю.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private string GenerateRandomPassword(int length)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            char[] chars = new char[length];
            using (var crypto = RandomNumberGenerator.Create())
            {
                byte[] data = new byte[length];
                crypto.GetBytes(data);
                for (int i = 0; i < length; i++)
                {
                    int index = data[i] % validChars.Length;
                    chars[i] = validChars[index];
                }
            }
            return new string(chars);
        }

        private void BtnResetAttempts_Click(object sender, EventArgs e)
        {
            int? id = GetSelectedUserId();
            if (id == null) return;
            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();
                    string sql = "UPDATE Пользователи SET Попытки=0 WHERE ID=?";
                    using (OleDbCommand cmd = new OleDbCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
                LoadUsers();
                MessageBox.Show("Попытки сброшены.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) { MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            LoadUsers(txtSearch.Text);
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "Поиск по логину или роли...";
            txtSearch.ForeColor = System.Drawing.Color.Gray;
            LoadUsers();
        }

        private void panelTop_Paint(object sender, PaintEventArgs e) { }
    }
}