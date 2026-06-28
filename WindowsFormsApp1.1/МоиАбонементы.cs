using System;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;

namespace WindowsFormsApp1
{
    public partial class МоиАбонементы : Form
    {
        private string connectionString = Properties.Settings.Default.Microsoft_Access_База_данныхConnectionString;
        private DataTable dataTable;
        private DataView dataView;
        private int currentUserId;
        private int? currentClientId;
        private string currentSearchText = "";

        public МоиАбонементы(int userId, int? clientId)
        {
            InitializeComponent();
            currentUserId = userId;
            currentClientId = clientId;

            if (!currentClientId.HasValue)
            {
                EnsureClientExists();
            }

            LoadData();
            ConfigureEvents();

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

        private void EnsureClientExists()
        {
            if (currentClientId.HasValue) return;

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT ID_клиента FROM Пользователи WHERE ID = ?";
                using (OleDbCommand cmd = new OleDbCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", currentUserId);
                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        currentClientId = Convert.ToInt32(result);
                        return;
                    }
                }

                string insertClient = "INSERT INTO Клиенты (Фамилия, Имя, Телефон) VALUES ('Пользователь', 'ID " + currentUserId + "', 'Не указан')";
                int newClientId;
                using (OleDbCommand cmd = new OleDbCommand(insertClient, conn))
                {
                    cmd.ExecuteNonQuery();
                    string getLastId = "SELECT @@IDENTITY";
                    using (OleDbCommand cmdLast = new OleDbCommand(getLastId, conn))
                    {
                        newClientId = Convert.ToInt32(cmdLast.ExecuteScalar());
                    }
                }
                string updateUser = "UPDATE Пользователи SET ID_клиента = ? WHERE ID = ?";
                using (OleDbCommand cmd = new OleDbCommand(updateUser, conn))
                {
                    cmd.Parameters.AddWithValue("@clientId", newClientId);
                    cmd.Parameters.AddWithValue("@userId", currentUserId);
                    cmd.ExecuteNonQuery();
                }
                currentClientId = newClientId;
                MessageBox.Show("Для вас создан профиль клиента. Теперь вы можете записываться на услуги.",
                                "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void LoadData()
        {
            if (!currentClientId.HasValue) return;
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();
                string sql = @"SELECT а.[ID абонемента], у.[Название услуги], а.[Дата начала], а.[Дата окончания], 
                                       а.[Лимит посещений], а.Статус, у.Стоимость, у.[Длительность(мин)]
                                FROM Абонементы а
                                INNER JOIN Услуги у ON а.[ID услуги] = у.[ID услуги]
                                WHERE а.[ID клиента] = ?";
                OleDbDataAdapter da = new OleDbDataAdapter(sql, conn);
                da.SelectCommand.Parameters.AddWithValue("@clientId", currentClientId.Value);
                dataTable = new DataTable();
                da.Fill(dataTable);
                dataView = dataTable.DefaultView;
                dataGridView1.DataSource = dataView;
            }
        }

        private void ConfigureEvents()
        {
            btnAdd.Click += btnAdd_Click;
            btnSearch.Click += btnSearch_Click;
            button1.Click += button1_Click;
            btnRefresh.Click += btnRefresh_Click;
        }

        private void ApplySearch(string searchText)
        {
            if (dataView == null) return;
            if (string.IsNullOrWhiteSpace(searchText) || searchText == "Поиск...")
            {
                dataView.RowFilter = "";
                dataGridView1.Refresh();
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
            dataGridView1.Refresh();
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

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!currentClientId.HasValue)
            {
                MessageBox.Show("Профиль клиента не найден. Обратитесь к администратору.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var form = new FullSubscriptionForm(currentClientId.Value, connectionString, currentUserId))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    // Если клиент был создан заново (например, старый не существовал), обновляем currentClientId
                    if (form.NewClientId != -1 && form.NewClientId != currentClientId.Value)
                    {
                        currentClientId = form.NewClientId;
                    }

                    LoadData();
                    if (!string.IsNullOrEmpty(currentSearchText)) ApplySearch(currentSearchText);
                    MessageBox.Show("Вы успешно записались!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            currentSearchText = txtSearch.Text;
            ApplySearch(currentSearchText);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadData();
            if (!string.IsNullOrEmpty(currentSearchText)) ApplySearch(currentSearchText);
        }

        private void button1_Click(object sender, EventArgs e) => Close();
    }

    // --------------------------------------------------------------------------
    // ПОЛНАЯ ФОРМА ЗАПИСИ (с датой без времени, разрешена запись на сегодня)
    // --------------------------------------------------------------------------
    public class FullSubscriptionForm : Form
    {
        private int clientId;
        private string connectionString;
        private int userId;
        public int NewClientId { get; private set; } = -1; // Возвращает новый ID клиента, если был создан

        private TextBox txtLastName, txtFirstName, txtPatronymic, txtPhone;
        private DateTimePicker dtpBirth;
        private ComboBox cmbServices;
        private Label lblServiceDetails;
        private DateTimePicker dtpStart, dtpEnd;
        private NumericUpDown nudLimit;
        private TextBox txtComment;
        private Button btnSave, btnCancel;

        public FullSubscriptionForm(int clientId, string connString, int userId)
        {
            this.clientId = clientId;
            this.connectionString = connString;
            this.userId = userId;

            Text = "Полная запись на услугу";
            Width = 600;
            Height = 650;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = System.Drawing.Color.FromArgb(30, 42, 58);

            // Заголовок
            Panel titlePanel = new Panel();
            titlePanel.BackColor = System.Drawing.Color.FromArgb(44, 62, 80);
            titlePanel.Dock = DockStyle.Top;
            titlePanel.Height = 40;
            Label titleLabel = new Label();
            titleLabel.Text = "Заполните все данные для записи";
            titleLabel.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            titleLabel.ForeColor = System.Drawing.Color.White;
            titleLabel.Location = new System.Drawing.Point(10, 6);
            titleLabel.AutoSize = false;
            titleLabel.Size = new System.Drawing.Size(400, 30);
            titlePanel.Controls.Add(titleLabel);
            this.Controls.Add(titlePanel);

            int yPos = 55;
            int labelX = 25;
            int fieldX = 170;
            int fieldWidth = 320;
            int rowHeight = 32;

            // ---- Блок клиента ----
            Label lblClientTitle = new Label() { Text = "ДАННЫЕ КЛИЕНТА", Location = new System.Drawing.Point(labelX, yPos), ForeColor = System.Drawing.Color.FromArgb(100, 200, 255), Font = new System.Drawing.Font("Segoe UI", 10F, FontStyle.Bold) };
            yPos += 28;

            Label lblLastName = new Label() { Text = "Фамилия*:", Location = new System.Drawing.Point(labelX, yPos), ForeColor = System.Drawing.Color.White };
            txtLastName = new TextBox() { Location = new System.Drawing.Point(fieldX, yPos - 3), Width = fieldWidth, BackColor = System.Drawing.Color.FromArgb(236, 240, 241) };
            yPos += rowHeight;

            Label lblFirstName = new Label() { Text = "Имя*:", Location = new System.Drawing.Point(labelX, yPos), ForeColor = System.Drawing.Color.White };
            txtFirstName = new TextBox() { Location = new System.Drawing.Point(fieldX, yPos - 3), Width = fieldWidth, BackColor = System.Drawing.Color.FromArgb(236, 240, 241) };
            yPos += rowHeight;

            Label lblPatronymic = new Label() { Text = "Отчество:", Location = new System.Drawing.Point(labelX, yPos), ForeColor = System.Drawing.Color.White };
            txtPatronymic = new TextBox() { Location = new System.Drawing.Point(fieldX, yPos - 3), Width = fieldWidth, BackColor = System.Drawing.Color.FromArgb(236, 240, 241) };
            yPos += rowHeight;

            Label lblPhone = new Label() { Text = "Телефон*:", Location = new System.Drawing.Point(labelX, yPos), ForeColor = System.Drawing.Color.White };
            txtPhone = new TextBox() { Location = new System.Drawing.Point(fieldX, yPos - 3), Width = fieldWidth, BackColor = System.Drawing.Color.FromArgb(236, 240, 241) };
            yPos += rowHeight;

            Label lblBirth = new Label() { Text = "Дата рождения:", Location = new System.Drawing.Point(labelX, yPos), ForeColor = System.Drawing.Color.White };
            dtpBirth = new DateTimePicker() { Location = new System.Drawing.Point(fieldX, yPos - 3), Width = fieldWidth, Format = DateTimePickerFormat.Short };
            yPos += rowHeight + 5;

            // ---- Блок услуги ----
            Label lblServiceTitle = new Label() { Text = "ВЫБОР УСЛУГИ", Location = new System.Drawing.Point(labelX, yPos), ForeColor = System.Drawing.Color.FromArgb(100, 200, 255), Font = new System.Drawing.Font("Segoe UI", 10F, FontStyle.Bold) };
            yPos += 28;

            Label lblService = new Label() { Text = "Услуга*:", Location = new System.Drawing.Point(labelX, yPos), ForeColor = System.Drawing.Color.White };
            cmbServices = new ComboBox() { Location = new System.Drawing.Point(fieldX, yPos - 3), Width = fieldWidth, DropDownStyle = ComboBoxStyle.DropDownList, BackColor = System.Drawing.Color.FromArgb(236, 240, 241) };
            yPos += rowHeight;

            lblServiceDetails = new Label()
            {
                Location = new System.Drawing.Point(fieldX, yPos),
                Width = fieldWidth,
                Height = 45,
                ForeColor = System.Drawing.Color.White,
                Text = "Выберите услугу для просмотра деталей",
                Font = new System.Drawing.Font("Segoe UI", 9F)
            };
            yPos += 55;

            // ---- Блок абонемента ----
            Label lblSubsTitle = new Label() { Text = "ПАРАМЕТРЫ АБОНЕМЕНТА", Location = new System.Drawing.Point(labelX, yPos), ForeColor = System.Drawing.Color.FromArgb(100, 200, 255), Font = new System.Drawing.Font("Segoe UI", 10F, FontStyle.Bold) };
            yPos += 28;

            Label lblStart = new Label() { Text = "Дата начала*:", Location = new System.Drawing.Point(labelX, yPos), ForeColor = System.Drawing.Color.White };
            dtpStart = new DateTimePicker() { Location = new System.Drawing.Point(fieldX, yPos - 3), Width = fieldWidth, Format = DateTimePickerFormat.Short };
            yPos += rowHeight;

            Label lblEnd = new Label() { Text = "Дата окончания*:", Location = new System.Drawing.Point(labelX, yPos), ForeColor = System.Drawing.Color.White };
            dtpEnd = new DateTimePicker() { Location = new System.Drawing.Point(fieldX, yPos - 3), Width = fieldWidth, Format = DateTimePickerFormat.Short };
            yPos += rowHeight;

            Label lblLimit = new Label() { Text = "Лимит посещений:", Location = new System.Drawing.Point(labelX, yPos), ForeColor = System.Drawing.Color.White };
            nudLimit = new NumericUpDown() { Location = new System.Drawing.Point(fieldX, yPos - 3), Width = 100, Minimum = 1, Maximum = 100, Value = 10, BackColor = System.Drawing.Color.FromArgb(236, 240, 241) };
            yPos += rowHeight;

            Label lblComment = new Label() { Text = "Комментарий:", Location = new System.Drawing.Point(labelX, yPos), ForeColor = System.Drawing.Color.White };
            txtComment = new TextBox() { Location = new System.Drawing.Point(fieldX, yPos - 3), Width = fieldWidth, Height = 40, Multiline = true, BackColor = System.Drawing.Color.FromArgb(236, 240, 241) };
            yPos += 50;

            // ---- Кнопки ----
            btnSave = new Button() { Text = "ЗАПИСАТЬСЯ", Location = new System.Drawing.Point(fieldX, yPos + 10), Width = 150, Height = 35, BackColor = System.Drawing.Color.FromArgb(39, 174, 96), FlatStyle = FlatStyle.Flat, ForeColor = System.Drawing.Color.White, Font = new System.Drawing.Font("Segoe UI", 9F, FontStyle.Bold) };
            btnCancel = new Button() { Text = "ОТМЕНА", Location = new System.Drawing.Point(fieldX + 170, yPos + 10), Width = 120, Height = 35, BackColor = System.Drawing.Color.FromArgb(149, 165, 166), FlatStyle = FlatStyle.Flat, ForeColor = System.Drawing.Color.White, Font = new System.Drawing.Font("Segoe UI", 9F, FontStyle.Bold) };

            Controls.Add(lblClientTitle);
            Controls.Add(lblLastName);
            Controls.Add(txtLastName);
            Controls.Add(lblFirstName);
            Controls.Add(txtFirstName);
            Controls.Add(lblPatronymic);
            Controls.Add(txtPatronymic);
            Controls.Add(lblPhone);
            Controls.Add(txtPhone);
            Controls.Add(lblBirth);
            Controls.Add(dtpBirth);

            Controls.Add(lblServiceTitle);
            Controls.Add(lblService);
            Controls.Add(cmbServices);
            Controls.Add(lblServiceDetails);

            Controls.Add(lblSubsTitle);
            Controls.Add(lblStart);
            Controls.Add(dtpStart);
            Controls.Add(lblEnd);
            Controls.Add(dtpEnd);
            Controls.Add(lblLimit);
            Controls.Add(nudLimit);
            Controls.Add(lblComment);
            Controls.Add(txtComment);

            Controls.Add(btnSave);
            Controls.Add(btnCancel);

            LoadClientData();
            LoadServices();

            dtpStart.Value = DateTime.Today;
            dtpEnd.Value = DateTime.Today.AddMonths(1);

            cmbServices.SelectedIndexChanged += (s, e) =>
            {
                if (cmbServices.SelectedItem is DataRowView row)
                {
                    string name = row["Название услуги"].ToString();
                    decimal cost = Convert.ToDecimal(row["Стоимость"]);
                    string duration = row["Длительность(мин)"].ToString();
                    string desc = row["Описание"] != DBNull.Value ? row["Описание"].ToString() : "Нет описания";
                    lblServiceDetails.Text = $"Название: {name}\nСтоимость: {cost:C2} | Длительность: {duration} мин.\nОписание: {desc}";
                }
            };

            btnSave.Click += BtnSave_Click;
            btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
        }

        private void LoadClientData()
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT Фамилия, Имя, Отчество, Телефон, [Дата рождения] FROM Клиенты WHERE [ID клиента] = ?";
                using (OleDbCommand cmd = new OleDbCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", clientId);
                    using (OleDbDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtLastName.Text = reader.GetString(0);
                            txtFirstName.Text = reader.GetString(1);
                            txtPatronymic.Text = reader.IsDBNull(2) ? "" : reader.GetString(2);
                            txtPhone.Text = reader.IsDBNull(3) ? "" : reader.GetString(3);
                            dtpBirth.Value = reader.IsDBNull(4) ? DateTime.Today.AddYears(-20) : reader.GetDateTime(4);
                        }
                        else
                        {
                            // Клиент не найден – очищаем поля, они будут заполнены заново при сохранении
                            txtLastName.Text = "";
                            txtFirstName.Text = "";
                            txtPatronymic.Text = "";
                            txtPhone.Text = "";
                            dtpBirth.Value = DateTime.Today.AddYears(-20);
                        }
                    }
                }
            }
        }

        private void LoadServices()
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT [ID услуги], [Название услуги], Стоимость, [Длительность(мин)], Описание FROM Услуги";
                using (OleDbCommand cmd = new OleDbCommand(sql, conn))
                using (OleDbDataReader reader = cmd.ExecuteReader())
                {
                    DataTable dt = new DataTable();
                    dt.Load(reader);
                    cmbServices.DataSource = dt;
                    cmbServices.DisplayMember = "Название услуги";
                    cmbServices.ValueMember = "ID услуги";
                }
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            // Проверка обязательных полей
            if (string.IsNullOrWhiteSpace(txtLastName.Text) || string.IsNullOrWhiteSpace(txtFirstName.Text) || string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                MessageBox.Show("Фамилия, Имя и Телефон обязательны.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (cmbServices.SelectedItem == null)
            {
                MessageBox.Show("Выберите услугу.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (dtpEnd.Value.Date <= dtpStart.Value.Date)
            {
                MessageBox.Show("Дата окончания должна быть позже даты начала.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (dtpStart.Value.Date < DateTime.Today)
            {
                MessageBox.Show("Дата начала не может быть раньше сегодняшнего дня.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int serviceId = (int)cmbServices.SelectedValue;
            int limit = (int)nudLimit.Value;
            string comment = txtComment.Text.Trim();

            DataRowView selectedRow = (DataRowView)cmbServices.SelectedItem;
            int durationMinutes = 0;
            if (selectedRow != null && selectedRow["Длительность(мин)"] != DBNull.Value)
            {
                int.TryParse(selectedRow["Длительность(мин)"].ToString(), out durationMinutes);
            }

            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();
                    using (OleDbTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // Проверяем, существует ли клиент, и если нет – создаём
                            int actualClientId = EnsureClientExists(conn, transaction);

                            // 1. Обновляем данные клиента (сохраняем введённые персональные данные)
                            string updateClient = @"UPDATE Клиенты SET 
                                Фамилия = ?, Имя = ?, Отчество = ?, Телефон = ?, [Дата рождения] = ?
                                WHERE [ID клиента] = ?";
                            using (OleDbCommand cmd = new OleDbCommand(updateClient, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@fam", txtLastName.Text.Trim());
                                cmd.Parameters.AddWithValue("@im", txtFirstName.Text.Trim());
                                cmd.Parameters.AddWithValue("@ot", string.IsNullOrWhiteSpace(txtPatronymic.Text) ? DBNull.Value : (object)txtPatronymic.Text.Trim());
                                cmd.Parameters.AddWithValue("@tel", txtPhone.Text.Trim());
                                cmd.Parameters.AddWithValue("@dr", dtpBirth.Value.Date);
                                cmd.Parameters.AddWithValue("@id", actualClientId);
                                cmd.ExecuteNonQuery();
                            }

                            // 2. Создаём абонемент (дата начала – только дата, без времени)
                            string insertSubs = @"INSERT INTO Абонементы ([ID клиента], [ID услуги], [Дата начала], [Дата окончания], [Лимит посещений], Статус) 
                                                  VALUES (?, ?, ?, ?, ?, 'Активен')";
                            int abonementId;
                            using (OleDbCommand cmd = new OleDbCommand(insertSubs, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@c", actualClientId);
                                cmd.Parameters.AddWithValue("@s", serviceId);
                                cmd.Parameters.AddWithValue("@ds", dtpStart.Value.Date);
                                cmd.Parameters.AddWithValue("@de", dtpEnd.Value.Date);
                                cmd.Parameters.AddWithValue("@limit", limit);
                                cmd.ExecuteNonQuery();
                                using (OleDbCommand cmdIdentity = new OleDbCommand("SELECT @@IDENTITY", conn, transaction))
                                {
                                    abonementId = Convert.ToInt32(cmdIdentity.ExecuteScalar());
                                }
                            }

                            // 3. Добавляем запись в Посещения (первое посещение)
                            if (durationMinutes > 0)
                            {
                                string insertVisit = @"INSERT INTO Посещения ([ID абонемента], [Дата начала], [Длительность(мин)]) 
                                                       VALUES (?, ?, ?)";
                                using (OleDbCommand cmd = new OleDbCommand(insertVisit, conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@abonementId", abonementId);
                                    cmd.Parameters.AddWithValue("@startDate", dtpStart.Value.Date);
                                    cmd.Parameters.AddWithValue("@duration", durationMinutes);
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            // 4. Если клиент был только что создан – обновляем ID_клиента у пользователя и сохраняем новый ID
                            if (actualClientId != clientId) // значит, создали нового
                            {
                                string updateUser = "UPDATE Пользователи SET ID_клиента = ? WHERE ID = ?";
                                using (OleDbCommand cmd = new OleDbCommand(updateUser, conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@clientId", actualClientId);
                                    cmd.Parameters.AddWithValue("@userId", userId);
                                    cmd.ExecuteNonQuery();
                                }
                                NewClientId = actualClientId; // сохраняем новый ID для возврата
                            }
                            else
                            {
                                NewClientId = -1; // не создавали нового
                            }

                            transaction.Commit();

                            if (!string.IsNullOrEmpty(comment))
                                MessageBox.Show($"Ваш комментарий: {comment}", "Комментарий сохранён", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            DialogResult = DialogResult.OK;
                            Close();
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int EnsureClientExists(OleDbConnection conn, OleDbTransaction transaction)
        {
            // Проверяем, есть ли клиент с таким ID
            string checkSql = "SELECT COUNT(*) FROM Клиенты WHERE [ID клиента] = ?";
            using (OleDbCommand cmd = new OleDbCommand(checkSql, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@id", clientId);
                int count = (int)cmd.ExecuteScalar();
                if (count > 0) return clientId;
            }

            // Клиента нет – создаём нового
            string insertClient = @"INSERT INTO Клиенты (Фамилия, Имя, Отчество, Телефон, [Дата рождения]) 
                                    VALUES (?, ?, ?, ?, ?)";
            using (OleDbCommand cmd = new OleDbCommand(insertClient, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@fam", txtLastName.Text.Trim());
                cmd.Parameters.AddWithValue("@im", txtFirstName.Text.Trim());
                cmd.Parameters.AddWithValue("@ot", string.IsNullOrWhiteSpace(txtPatronymic.Text) ? DBNull.Value : (object)txtPatronymic.Text.Trim());
                cmd.Parameters.AddWithValue("@tel", txtPhone.Text.Trim());
                cmd.Parameters.AddWithValue("@dr", dtpBirth.Value.Date);
                cmd.ExecuteNonQuery();

                using (OleDbCommand cmdIdentity = new OleDbCommand("SELECT @@IDENTITY", conn, transaction))
                {
                    int newId = Convert.ToInt32(cmdIdentity.ExecuteScalar());
                    return newId;
                }
            }
        }
    }
}