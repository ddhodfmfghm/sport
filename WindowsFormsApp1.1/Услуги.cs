using System;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;
using System.Drawing;

namespace WindowsFormsApp1
{
    public partial class Услуги : Form
    {
        private string connectionString = Properties.Settings.Default.Microsoft_Access_База_данныхConnectionString;
        private DataTable dataTable;
        private DataView dataView;
        private string currentRole;
        private string currentSearchText = "";

        public Услуги(string role)
        {
            InitializeComponent();
            currentRole = role;
            if (currentRole != "Admin")
            {
                btnAdd.Enabled = false;
                btnEdit.Enabled = false;
                btnDelete.Enabled = false;
                btnAdd.Visible = false;
                btnEdit.Visible = false;
                btnDelete.Visible = false;
            }
            LoadData();

            btnAdd.Click += btnAdd_Click;
            btnEdit.Click += btnEdit_Click;
            btnDelete.Click += btnDelete_Click;
            btnSearch.Click += btnSearch_Click;
            button1.Click += button1_Click;

            txtSearch.Enter += (s, e) =>
            {
                if (txtSearch.Text == "Поиск...")
                {
                    txtSearch.Text = "";
                    txtSearch.ForeColor = Color.Black;
                }
            };
            txtSearch.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    txtSearch.Text = "Поиск...";
                    txtSearch.ForeColor = Color.Gray;
                }
            };
            txtSearch.TextChanged += (s, e) =>
            {
                if (txtSearch.Text == "Поиск...") return;
                currentSearchText = txtSearch.Text;
                ApplySearch(currentSearchText);
            };
            dataGridView1.CellFormatting += DataGridView1_CellFormatting;
        }

        private void LoadData(string filter = "")
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT * FROM Услуги";
                if (!string.IsNullOrEmpty(filter))
                    sql += " WHERE " + filter;
                OleDbDataAdapter da = new OleDbDataAdapter(sql, conn);
                dataTable = new DataTable();
                da.Fill(dataTable);
                dataView = dataTable.DefaultView;
                dataGridView1.DataSource = dataView;
            }
        }

        private void ApplySearch(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText) || searchText == "Поиск...")
            {
                dataView.RowFilter = "";
                return;
            }
            string filter = "";
            foreach (DataColumn col in dataTable.Columns)
            {
                if (col.DataType == typeof(string))
                {
                    if (filter.Length > 0) filter += " OR ";
                    filter += $"[{col.ColumnName}] LIKE '%{searchText.Replace("'", "''")}%'";
                }
            }
            dataView.RowFilter = filter;
        }

        private void DataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (string.IsNullOrEmpty(currentSearchText) || currentSearchText == "Поиск...") return;
            if (e.Value != null && e.Value.ToString().IndexOf(currentSearchText, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                e.CellStyle.BackColor = Color.Yellow;
                e.CellStyle.ForeColor = Color.Black;
                e.CellStyle.SelectionBackColor = Color.Gold;
            }
            else
            {
                e.CellStyle.BackColor = dataGridView1.DefaultCellStyle.BackColor;
                e.CellStyle.ForeColor = dataGridView1.DefaultCellStyle.ForeColor;
                e.CellStyle.SelectionBackColor = dataGridView1.DefaultCellStyle.SelectionBackColor;
            }
        }

        private bool ValidateServiceData(EditServiceForm form, out int durationValue)
        {
            durationValue = 0;

            // 1. Проверка названия
            if (string.IsNullOrWhiteSpace(form.ServiceName))
            {
                MessageBox.Show("Название услуги не может быть пустым!", "Ошибка валидации", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // 2. Проверка стоимости
            if (form.Cost < 0)
            {
                MessageBox.Show("Стоимость не может быть отрицательной!", "Ошибка валидации", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // 3. Проверка длительности
            if (!int.TryParse(form.Duration, out durationValue) || durationValue <= 0)
            {
                MessageBox.Show("Длительность должна быть корректным числом больше 0 минут!", "Ошибка валидации", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }


        private void btnAdd_Click(object sender, EventArgs e)
        {
            EditServiceForm editForm = new EditServiceForm();

            // Запускаем цикл: форма удерживается, пока данные не запишутся или не нажмут Отмену
            while (editForm.ShowDialog() == DialogResult.OK)
            {
                // Вызываем валидацию. Если не прошла — возвращаемся к началу цикла
                if (!ValidateServiceData(editForm, out int durationValue)) continue;

                try
                {
                    using (OleDbConnection conn = new OleDbConnection(connectionString))
                    {
                        conn.Open();
                        string sql = @"INSERT INTO Услуги ([Название услуги], Стоимость, [Длительность(мин)], Описание) 
                               VALUES (?,?,?,?)";
                        using (OleDbCommand cmd = new OleDbCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("@n", editForm.ServiceName.Trim());
                            cmd.Parameters.AddWithValue("@c", editForm.Cost);
                            cmd.Parameters.AddWithValue("@d", durationValue);
                            cmd.Parameters.AddWithValue("@desc", string.IsNullOrWhiteSpace(editForm.Description) ? DBNull.Value : (object)editForm.Description.Trim());

                            cmd.ExecuteNonQuery();
                        }
                    }

                    // Успех: обновляем данные и выходим из цикла
                    LoadData();
                    if (!string.IsNullOrEmpty(currentSearchText)) ApplySearch(currentSearchText);
                    break;
                }
                catch (OleDbException ex)
                {
                    if (ex.ErrorCode == -2147467259 || ex.Message.Contains("изменения не были внесены в таблицу"))
                    {
                        MessageBox.Show($"Услуга с названием '{editForm.ServiceName.Trim()}' уже существует в базе данных!",
                                        "Дубликат данных", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        MessageBox.Show($"Ошибка базы данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    // Форма остается открытой, пользователь может изменить название
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Непредвиденная ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                }
            }
        }


        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null) return;
            int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["ID услуги"].Value);

            EditServiceForm editForm = new EditServiceForm(id);

            // Запускаем цикл для удержания формы при ошибках или дубликатах имени
            while (editForm.ShowDialog() == DialogResult.OK)
            {
                if (!ValidateServiceData(editForm, out int durationValue)) continue;

                try
                {
                    using (OleDbConnection conn = new OleDbConnection(connectionString))
                    {
                        conn.Open();
                        string sql = @"UPDATE Услуги 
                               SET [Название услуги]=?, Стоимость=?, [Длительность(мин)]=?, Описание=? 
                               WHERE [ID услуги]=?";
                        using (OleDbCommand cmd = new OleDbCommand(sql, conn))
                        {
                            // В OleDb важен строгий порядок параметров!
                            cmd.Parameters.AddWithValue("@n", editForm.ServiceName.Trim());
                            cmd.Parameters.AddWithValue("@c", editForm.Cost);
                            cmd.Parameters.AddWithValue("@d", durationValue);
                            cmd.Parameters.AddWithValue("@desc", string.IsNullOrWhiteSpace(editForm.Description) ? DBNull.Value : (object)editForm.Description.Trim());
                            cmd.Parameters.AddWithValue("@id", id);

                            cmd.ExecuteNonQuery();
                        }
                    }

                    // Успех: обновляем данные и выходим
                    LoadData();
                    if (!string.IsNullOrEmpty(currentSearchText)) ApplySearch(currentSearchText);
                    MessageBox.Show("Данные услуги успешно обновлены!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                }
                catch (OleDbException ex)
                {
                    if (ex.ErrorCode == -2147467259 || ex.Message.Contains("изменения не были внесены в таблицу"))
                    {
                        MessageBox.Show($"Не удалось сохранить изменения. Услуга с названием '{editForm.ServiceName.Trim()}' уже существует!",
                                        "Дубликат данных", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        MessageBox.Show($"Ошибка базы данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    // Форма остается открытой, можно исправить название услуги
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Непредвиденная ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                }
            }
        }


        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null) return;
            if (MessageBox.Show("Удалить выбранную услугу?", "Подтверждение", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["ID услуги"].Value);
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();
                    string sql = "DELETE FROM Услуги WHERE [ID услуги]=?";
                    using (OleDbCommand cmd = new OleDbCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
                LoadData();
                if (!string.IsNullOrEmpty(currentSearchText)) ApplySearch(currentSearchText);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            currentSearchText = txtSearch.Text;
            ApplySearch(currentSearchText);
        }

        private void button1_Click(object sender, EventArgs e) => Close();

        private void btnAdd_Click_1(object sender, EventArgs e)
        {

        }
    }

    public class EditServiceForm : Form
    {
        public string ServiceName { get; private set; }
        public decimal Cost { get; private set; }
        public string Duration { get; private set; }
        public string Description { get; private set; }

        private TextBox txtName, txtCost, txtDuration, txtDescription;
        private Button btnSave;

        public EditServiceForm(int? id = null)
        {
            Text = id == null ? "Добавление услуги" : "Редактирование услуги";
            Width = 450;
            Height = 300;

            txtName = new TextBox() { Location = new System.Drawing.Point(150, 20), Width = 250 };
            txtCost = new TextBox() { Location = new System.Drawing.Point(150, 50), Width = 150 };
            txtDuration = new TextBox() { Location = new System.Drawing.Point(150, 80), Width = 150 };
            txtDescription = new TextBox() { Location = new System.Drawing.Point(150, 110), Width = 250, Height = 60, Multiline = true };
            btnSave = new Button() { Text = "Сохранить", Location = new System.Drawing.Point(150, 190), Width = 100 };

            Controls.Add(new Label() { Text = "Название услуги:", Location = new System.Drawing.Point(20, 23) });
            Controls.Add(txtName);
            Controls.Add(new Label() { Text = "Стоимость:", Location = new System.Drawing.Point(20, 53) });
            Controls.Add(txtCost);
            Controls.Add(new Label() { Text = "Длительность (мин):", Location = new System.Drawing.Point(20, 83) });
            Controls.Add(txtDuration);
            Controls.Add(new Label() { Text = "Описание:", Location = new System.Drawing.Point(20, 113) });
            Controls.Add(txtDescription);
            Controls.Add(btnSave);

            if (id.HasValue)
            {
                string connStr = Properties.Settings.Default.Microsoft_Access_База_данныхConnectionString;
                using (OleDbConnection conn = new OleDbConnection(connStr))
                {
                    conn.Open();
                    string sql = @"SELECT [Название услуги], Стоимость, [Длительность(мин)], Описание 
                                   FROM Услуги WHERE [ID услуги]=?";
                    using (OleDbCommand cmd = new OleDbCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id.Value);
                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtName.Text = reader.GetString(0);
                                txtCost.Text = reader.GetDecimal(1).ToString();
                                txtDuration.Text = reader.IsDBNull(2) ? "" : reader.GetString(2);
                                txtDescription.Text = reader.IsDBNull(3) ? "" : reader.GetString(3);
                            }
                        }
                    }
                }
            }

            btnSave.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    MessageBox.Show("Название услуги обязательно");
                    return;
                }
                if (!decimal.TryParse(txtCost.Text, out decimal cost))
                {
                    MessageBox.Show("Некорректная стоимость");
                    return;
                }
                ServiceName = txtName.Text;
                Cost = cost;
                Duration = txtDuration.Text;
                Description = txtDescription.Text;
                DialogResult = DialogResult.OK;
                Close();
            };
        }
    }
}