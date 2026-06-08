using System;
using System.Data.OleDb;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class ChangePasswordForm : Form
    {
        private readonly int userId;
        private readonly string connectionString = Properties.Settings.Default.Microsoft_Access_База_данныхConnectionString;

        public ChangePasswordForm(int currentUserId)
        {
            InitializeComponent();
            userId = currentUserId;
        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            string oldPwd = txtOldPassword.Text;
            string newPwd = txtNewPassword.Text;
            string confirm = txtConfirmPassword.Text;

            if (string.IsNullOrEmpty(oldPwd) || string.IsNullOrEmpty(newPwd))
            {
                MessageBox.Show("Заполните все поля", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (newPwd != confirm)
            {
                MessageBox.Show("Новый пароль и подтверждение не совпадают", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (newPwd.Length < 4)
            {
                MessageBox.Show("Пароль должен содержать не менее 4 символов", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();
                    // Проверка старого пароля
                    string checkSql = "SELECT Пароль FROM Пользователи WHERE ID = ?";
                    using (OleDbCommand cmd = new OleDbCommand(checkSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", userId);
                        string storedHash = cmd.ExecuteScalar()?.ToString();
                        string oldHash = HashHelper.ComputeSha256Hash(oldPwd);
                        if (storedHash != oldHash)
                        {
                            MessageBox.Show("Неверный текущий пароль", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    // Обновление пароля
                    string updateSql = "UPDATE Пользователи SET Пароль = ? WHERE ID = ?";
                    using (OleDbCommand cmd = new OleDbCommand(updateSql, conn))
                    {
                        string newHash = HashHelper.ComputeSha256Hash(newPwd);
                        cmd.Parameters.AddWithValue("@pwd", newHash);
                        cmd.Parameters.AddWithValue("@id", userId);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Пароль успешно изменён!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}