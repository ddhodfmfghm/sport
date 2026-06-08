using System;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;

namespace WindowsFormsApp1
{
    public partial class Абонементы : Form
    {
        private string connectionString = Properties.Settings.Default.Microsoft_Access_База_данныхConnectionString;
        private DataTable dataTable;
        private DataView dataView;
        private string currentRole;
        private string currentSearchText = "";

        public Абонементы(string role)
        {
            InitializeComponent();
            currentRole = role;
            if (currentRole != "Admin")
            {

            }
            LoadData();


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
                string sql = "SELECT * FROM Абонементы";
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
            currentSearchText = searchText;

            if (string.IsNullOrWhiteSpace(searchText) || searchText == "Поиск...")
            {
                dataView.RowFilter = "";
                dataGridView1.Refresh();
                return;
            }

            List<string> filterParts = new List<string>();
            string escapedSearchText = searchText.Replace("'", "''");

            foreach (DataColumn col in dataTable.Columns)
            {
                // 1. Поиск по текстовым полям (ФИО, Адрес, Тариф)
                if (col.DataType == typeof(string))
                {
                    filterParts.Add($"[{col.ColumnName}] LIKE '%{escapedSearchText}%'");
                }
                // 2. Поиск по датам (Дата рождения, Дата подключения)
                else if (col.DataType == typeof(DateTime))
                {
                    filterParts.Add($"CONVERT([{col.ColumnName}], 'System.String') LIKE '%{escapedSearchText}%'");
                }
                // 3. Поиск по числам (Лицевой счет, Номер телефона, ID, Баланс)
                else if (col.DataType == typeof(int) || col.DataType == typeof(long) || col.DataType == typeof(decimal))
                {
                    filterParts.Add($"CONVERT([{col.ColumnName}], 'System.String') LIKE '%{escapedSearchText}%'");
                }
            }

            dataView.RowFilter = string.Join(" OR ", filterParts);
            dataGridView1.Refresh(); // Принудительно обновляем сетку для перерисовки цветов
        }

        private void DataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Если поиск пустой, красить ничего не нужно
            if (string.IsNullOrEmpty(currentSearchText) || currentSearchText == "Поиск...") return;

            if (e.Value != null && e.Value != DBNull.Value)
            {
                string cellText = e.Value.ToString();

                // Проверяем совпадение без учета регистра
                if (cellText.IndexOf(currentSearchText, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    e.CellStyle.BackColor = Color.Yellow;
                    e.CellStyle.ForeColor = Color.Black;
                    e.CellStyle.SelectionBackColor = Color.Gold;
                    return; // Выходим, чтобы код ниже не сбросил цвет
                }
            }

            // Возвращаем стандартный цвет, если совпадений нет
            e.CellStyle.BackColor = dataGridView1.DefaultCellStyle.BackColor;
            e.CellStyle.ForeColor = dataGridView1.DefaultCellStyle.ForeColor;
            e.CellStyle.SelectionBackColor = dataGridView1.DefaultCellStyle.SelectionBackColor;
        }

        private bool ValidateAbonementData(EditAbonementForm form)
        {
            // 1. Проверка логики дат
            if (form.EndDate <= form.StartDate)
            {
                MessageBox.Show("Дата окончания абонемента должна быть строго позже даты начала!",
                                "Ошибка валидации", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // 2. Проверка лимита посещений
            if (form.Limit < 0)
            {
                MessageBox.Show("Лимит посещений не может быть отрицательным числом!",
                                "Ошибка валидации", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // 3. Проверка статуса (на случай, если поле осталось пустым)
            if (string.IsNullOrWhiteSpace(form.Status))
            {
                MessageBox.Show("Поле 'Статус' должно быть заполнено!",
                                "Ошибка валидации", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }


        private void btnAdd_Click(object sender, EventArgs e)
        {
            EditAbonementForm editForm = new EditAbonementForm();

            // Запускаем цикл: пока нажимают ОК, но данные или ключи не верны — форма удерживается
            while (editForm.ShowDialog() == DialogResult.OK)
            {
                if (!ValidateAbonementData(editForm)) continue;

                try
                {
                    using (OleDbConnection conn = new OleDbConnection(connectionString))
                    {
                        conn.Open();
                        string sql = @"INSERT INTO Абонементы ([ID клиента], [ID услуги], [Дата начала], [Дата окончания], [Лимит посещений], Статус) 
                               VALUES (?,?,?,?,?,?)";
                        using (OleDbCommand cmd = new OleDbCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("@c", editForm.ClientId);
                            cmd.Parameters.AddWithValue("@s", editForm.ServiceId);
                            cmd.Parameters.AddWithValue("@ds", editForm.StartDate);
                            cmd.Parameters.AddWithValue("@de", editForm.EndDate);
                            cmd.Parameters.AddWithValue("@limit", editForm.Limit);
                            cmd.Parameters.AddWithValue("@stat", editForm.Status.Trim());

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
                    if (ex.ErrorCode == -2147467259 || ex.Message.Contains("связанная запись"))
                    {
                        MessageBox.Show("Ошибка добавления! Указанного клиента или услуги не существует в базе данных.",
                                        "Ошибка данных", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        MessageBox.Show($"Ошибка базы данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    // Цикл продолжится, форма останется открытой для исправления ID клиента/услуги
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
            int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["ID абонемента"].Value);

            EditAbonementForm editForm = new EditAbonementForm(id);

            // Запускаем цикл удержания формы при редактировании
            while (editForm.ShowDialog() == DialogResult.OK)
            {
                if (!ValidateAbonementData(editForm)) continue;

                try
                {
                    using (OleDbConnection conn = new OleDbConnection(connectionString))
                    {
                        conn.Open();
                        string sql = @"UPDATE Абонементы 
                               SET [ID клиента]=?, [ID услуги]=?, [Дата начала]=?, [Дата окончания]=?, [Лимит посещений]=?, Статус=? 
                               WHERE [ID абонемента]=?";
                        using (OleDbCommand cmd = new OleDbCommand(sql, conn))
                        {
                            // Для OleDb строго соблюдаем хронологический порядок параметров
                            cmd.Parameters.AddWithValue("@c", editForm.ClientId);
                            cmd.Parameters.AddWithValue("@s", editForm.ServiceId);
                            cmd.Parameters.AddWithValue("@ds", editForm.StartDate);
                            cmd.Parameters.AddWithValue("@de", editForm.EndDate);
                            cmd.Parameters.AddWithValue("@limit", editForm.Limit);
                            cmd.Parameters.AddWithValue("@stat", editForm.Status.Trim());

                            // ID находится в секции WHERE, поэтому идет последним
                            cmd.Parameters.AddWithValue("@id", id);

                            cmd.ExecuteNonQuery();
                        }
                    }

                    // Успех: обновляем данные, показываем сообщение и выходим
                    LoadData();
                    if (!string.IsNullOrEmpty(currentSearchText)) ApplySearch(currentSearchText);
                    MessageBox.Show("Данные абонемента успешно обновлены!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                }
                catch (OleDbException ex)
                {
                    if (ex.ErrorCode == -2147467259 || ex.Message.Contains("связанная запись"))
                    {
                        MessageBox.Show("Ошибка изменения! Указанного клиента или услуги не существует в базе данных.",
                                        "Ошибка данных", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        MessageBox.Show($"Ошибка базы данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    // Форма остается открытой для корректировки ID
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
            if (MessageBox.Show("Удалить выбранную запись?", "Подтверждение", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["ID абонемента"].Value);
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();
                    string sql = "DELETE FROM Абонементы WHERE [ID абонемента]=?";
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

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {

        }
    }

    public class EditAbonementForm : Form
    {
        public int ClientId { get; private set; }
        public int ServiceId { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public int Limit { get; private set; }
        public string Status { get; private set; }

        private TextBox txtClientId, txtServiceId, txtLimit, txtStatus;
        private DateTimePicker dtpStart, dtpEnd;
        private Button btnSave;

        public EditAbonementForm(int? id = null)
        {
            Text = id == null ? "Добавление абонемента" : "Редактирование абонемента";
            Width = 400;
            Height = 300;
            this.BackColor = System.Drawing.Color.FromArgb(30, 42, 58);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            txtClientId = new TextBox() { Location = new System.Drawing.Point(120, 20), Width = 150, BackColor = System.Drawing.Color.FromArgb(236, 240, 241) };
            txtServiceId = new TextBox() { Location = new System.Drawing.Point(120, 50), Width = 150, BackColor = System.Drawing.Color.FromArgb(236, 240, 241) };
            dtpStart = new DateTimePicker() { Location = new System.Drawing.Point(120, 80), Width = 150 };
            dtpEnd = new DateTimePicker() { Location = new System.Drawing.Point(120, 110), Width = 150 };
            txtLimit = new TextBox() { Location = new System.Drawing.Point(120, 140), Width = 150, BackColor = System.Drawing.Color.FromArgb(236, 240, 241) };
            txtStatus = new TextBox() { Location = new System.Drawing.Point(120, 170), Width = 150, BackColor = System.Drawing.Color.FromArgb(236, 240, 241) };
            btnSave = new Button() { Text = "Сохранить", Location = new System.Drawing.Point(120, 210), Width = 100, BackColor = System.Drawing.Color.FromArgb(39, 174, 96), FlatStyle = FlatStyle.Flat, ForeColor = System.Drawing.Color.White, Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold) };

            Controls.Add(new Label() { Text = "ID клиента:", Location = new System.Drawing.Point(20, 23), ForeColor = System.Drawing.Color.White });
            Controls.Add(txtClientId);
            Controls.Add(new Label() { Text = "ID услуги:", Location = new System.Drawing.Point(20, 53), ForeColor = System.Drawing.Color.White });
            Controls.Add(txtServiceId);
            Controls.Add(new Label() { Text = "Дата начала:", Location = new System.Drawing.Point(20, 83), ForeColor = System.Drawing.Color.White });
            Controls.Add(dtpStart);
            Controls.Add(new Label() { Text = "Дата окончания:", Location = new System.Drawing.Point(20, 113), ForeColor = System.Drawing.Color.White });
            Controls.Add(dtpEnd);
            Controls.Add(new Label() { Text = "Лимит посещений:", Location = new System.Drawing.Point(20, 143), ForeColor = System.Drawing.Color.White });
            Controls.Add(txtLimit);
            Controls.Add(new Label() { Text = "Статус:", Location = new System.Drawing.Point(20, 173), ForeColor = System.Drawing.Color.White });
            Controls.Add(txtStatus);
            Controls.Add(btnSave);

            if (id.HasValue)
            {
                string connStr = Properties.Settings.Default.Microsoft_Access_База_данныхConnectionString;
                using (OleDbConnection conn = new OleDbConnection(connStr))
                {
                    conn.Open();
                    string sql = @"SELECT [ID клиента], [ID услуги], [Дата начала], [Дата окончания], [Лимит посещений], Статус 
                               FROM Абонементы WHERE [ID абонемента]=?";
                    using (OleDbCommand cmd = new OleDbCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id.Value);
                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtClientId.Text = reader.GetInt32(0).ToString();
                                txtServiceId.Text = reader.GetInt32(1).ToString();
                                dtpStart.Value = reader.GetDateTime(2);
                                dtpEnd.Value = reader.GetDateTime(3);
                                txtLimit.Text = reader.GetInt32(4).ToString();
                                txtStatus.Text = reader.GetString(5);
                            }
                        }
                    }
                }
            }

            btnSave.Click += (s, e) =>
            {
                if (!int.TryParse(txtClientId.Text, out int cid) || !int.TryParse(txtServiceId.Text, out int sid) || !int.TryParse(txtLimit.Text, out int lim))
                {
                    MessageBox.Show("Проверьте числовые поля");
                    return;
                }
                ClientId = cid;
                ServiceId = sid;
                StartDate = dtpStart.Value.Date;
                EndDate = dtpEnd.Value.Date;
                Limit = lim;
                Status = txtStatus.Text;
                DialogResult = DialogResult.OK;
                Close();
            };
        }
    }
}