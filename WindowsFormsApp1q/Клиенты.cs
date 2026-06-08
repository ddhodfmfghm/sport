using System;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;
using System.Drawing;

namespace WindowsFormsApp1
{
    public partial class Клиенты : Form
    {
        private string connectionString = Properties.Settings.Default.Microsoft_Access_База_данныхConnectionString;
        private DataTable dataTable;
        private DataView dataView;
        private string currentRole;
        private string currentSearchText = "";

        public Клиенты(string role)
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
                string sql = "SELECT * FROM Клиенты";
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

        // НОВЫЙ ВАРИАНТ ВАЛИДАЦИИ (адаптирован для повторного показа формы)
        private bool ValidateClientData(EditClientForm form)
        {
            if (string.IsNullOrWhiteSpace(form.LastName) || string.IsNullOrWhiteSpace(form.Phone))
            {
                MessageBox.Show("Поля 'Фамилия' и 'Телефон' обязательны для заполнения!",
                                "Валидация", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        // НОВАЯ КНОПКА ДОБАВЛЕНИЯ
        private void btnAdd_Click(object sender, EventArgs e)
        {
            EditClientForm editForm = new EditClientForm();

            // Запускаем цикл: пока пользователь нажимает ОК, но данные не валидны — форма будет открываться снова
            while (editForm.ShowDialog() == DialogResult.OK)
            {
                // Если данные НЕ прошли проверку, цикл продолжится, форма откроется снова с введенными данными
                if (!ValidateClientData(editForm)) continue;

                try
                {
                    using (OleDbConnection conn = new OleDbConnection(connectionString))
                    {
                        conn.Open();
                        string sql = @"INSERT INTO Клиенты ([Фамилия], [Имя], [Отчество], [Телефон], [Дата рождения]) 
                       VALUES (?,?,?,?,?)";
                        using (OleDbCommand cmd = new OleDbCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("@fam", editForm.LastName.Trim());
                            cmd.Parameters.AddWithValue("@im", string.IsNullOrWhiteSpace(editForm.FirstName) ? DBNull.Value : (object)editForm.FirstName.Trim());
                            cmd.Parameters.AddWithValue("@ot", string.IsNullOrWhiteSpace(editForm.Patronymic) ? DBNull.Value : (object)editForm.Patronymic.Trim());
                            cmd.Parameters.AddWithValue("@tel", editForm.Phone.Trim());
                            cmd.Parameters.AddWithValue("@dr", editForm.BirthDate == default(DateTime) ? DBNull.Value : (object)editForm.BirthDate);

                            cmd.ExecuteNonQuery();
                        }
                    }

                    LoadData();
                    if (!string.IsNullOrEmpty(currentSearchText)) ApplySearch(currentSearchText);
                    break; // Всё успешно записано, выходим из цикла открытия формы
                }
                catch (OleDbException ex)
                {
                    if (ex.ErrorCode == -2147467259 || ex.Message.Contains("изменения не были внесены в таблицу"))
                    {
                        MessageBox.Show("Клиент с таким номером телефона уже существует в базе данных!",
                                        "Дубликат данных", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        MessageBox.Show($"Ошибка базы данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    // После ошибки БД цикл продолжится, давая пользователю шанс исправить телефон прямо в форме
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Непредвиденная ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                }
            }
        }

        // НОВАЯ КНОПКА РЕДАКТИРОВАНИЯ
        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null) return;
            int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["ID клиента"].Value);

            EditClientForm editForm = new EditClientForm(id);

            // Запускаем такой же цикл для удержания формы открытой при ошибках
            while (editForm.ShowDialog() == DialogResult.OK)
            {
                if (!ValidateClientData(editForm)) continue;

                try
                {
                    using (OleDbConnection conn = new OleDbConnection(connectionString))
                    {
                        conn.Open();
                        string sql = @"UPDATE Клиенты 
                               SET [Фамилия]=?, [Имя]=?, [Отчество]=?, [Телефон]=?, [Дата рождения]=? 
                               WHERE [ID клиента]=?";
                        using (OleDbCommand cmd = new OleDbCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("@fam", editForm.LastName.Trim());
                            cmd.Parameters.AddWithValue("@im", string.IsNullOrWhiteSpace(editForm.FirstName) ? DBNull.Value : (object)editForm.FirstName.Trim());
                            cmd.Parameters.AddWithValue("@ot", string.IsNullOrWhiteSpace(editForm.Patronymic) ? DBNull.Value : (object)editForm.Patronymic.Trim());
                            cmd.Parameters.AddWithValue("@tel", editForm.Phone.Trim());
                            cmd.Parameters.AddWithValue("@dr", editForm.BirthDate == default(DateTime) ? DBNull.Value : (object)editForm.BirthDate);
                            cmd.Parameters.AddWithValue("@id", id);

                            cmd.ExecuteNonQuery();
                        }
                    }

                    LoadData();
                    if (!string.IsNullOrEmpty(currentSearchText)) ApplySearch(currentSearchText);
                    MessageBox.Show("Данные клиента успешно обновлены!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break; // Успех, выходим
                }
                catch (OleDbException ex)
                {
                    if (ex.ErrorCode == -2147467259 || ex.Message.Contains("изменения не были внесены в таблицу"))
                    {
                        MessageBox.Show("Не удалось сохранить изменения. Другой клиент с таким номером телефона уже существует!",
                                        "Дубликат данных", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        MessageBox.Show($"Ошибка базы данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
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
            if (MessageBox.Show("Удалить выбранного клиента?", "Подтверждение", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["ID клиента"].Value);
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();
                    string sql = "DELETE FROM Клиенты WHERE [ID клиента]=?";
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

    public class EditClientForm : Form
    {
        public string LastName { get; private set; }
        public string FirstName { get; private set; }
        public string Patronymic { get; private set; }
        public string Phone { get; private set; }
        public DateTime BirthDate { get; private set; }

        private TextBox txtLastName, txtFirstName, txtPatronymic, txtPhone;
        private DateTimePicker dtpBirth;
        private Button btnSave;

        public EditClientForm(int? id = null)
        {
            Text = id == null ? "Добавление клиента" : "Редактирование клиента";
            Width = 400;
            Height = 300;
            this.BackColor = System.Drawing.Color.FromArgb(30, 42, 58);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            Panel titlePanel = new Panel();
            titlePanel.BackColor = System.Drawing.Color.FromArgb(44, 62, 80);
            titlePanel.Dock = DockStyle.Top;
            titlePanel.Height = 40;
            Label titleLabel = new Label();
            titleLabel.Text = this.Text;
            titleLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            titleLabel.ForeColor = System.Drawing.Color.White;
            titleLabel.Location = new System.Drawing.Point(10, 8);
            titleLabel.AutoSize = false;
            titleLabel.Size = new System.Drawing.Size(300, 25);
            titlePanel.Controls.Add(titleLabel);
            this.Controls.Add(titlePanel);

            txtLastName = new TextBox() { Location = new System.Drawing.Point(120, 60), Width = 200, BackColor = System.Drawing.Color.FromArgb(236, 240, 241), Font = new System.Drawing.Font("Segoe UI", 10F), BorderStyle = BorderStyle.FixedSingle };
            txtFirstName = new TextBox() { Location = new System.Drawing.Point(120, 90), Width = 200, BackColor = System.Drawing.Color.FromArgb(236, 240, 241), Font = new System.Drawing.Font("Segoe UI", 10F), BorderStyle = BorderStyle.FixedSingle };
            txtPatronymic = new TextBox() { Location = new System.Drawing.Point(120, 120), Width = 200, BackColor = System.Drawing.Color.FromArgb(236, 240, 241), Font = new System.Drawing.Font("Segoe UI", 10F), BorderStyle = BorderStyle.FixedSingle };
            txtPhone = new TextBox() { Location = new System.Drawing.Point(120, 150), Width = 200, BackColor = System.Drawing.Color.FromArgb(236, 240, 241), Font = new System.Drawing.Font("Segoe UI", 10F), BorderStyle = BorderStyle.FixedSingle };
            dtpBirth = new DateTimePicker() { Location = new System.Drawing.Point(120, 180), Width = 200 };
            btnSave = new Button() { Text = "Сохранить", Location = new System.Drawing.Point(120, 220), Width = 100, BackColor = System.Drawing.Color.FromArgb(39, 174, 96), FlatStyle = FlatStyle.Flat, ForeColor = System.Drawing.Color.White, Font = new System.Drawing.Font("Segoe UI", 9F, FontStyle.Bold) };

            Controls.Add(new Label() { Text = "Фамилия:", Location = new System.Drawing.Point(20, 63), ForeColor = System.Drawing.Color.White });
            Controls.Add(txtLastName);
            Controls.Add(new Label() { Text = "Имя:", Location = new System.Drawing.Point(20, 93), ForeColor = System.Drawing.Color.White });
            Controls.Add(txtFirstName);
            Controls.Add(new Label() { Text = "Отчество:", Location = new System.Drawing.Point(20, 123), ForeColor = System.Drawing.Color.White });
            Controls.Add(txtPatronymic);
            Controls.Add(new Label() { Text = "Телефон:", Location = new System.Drawing.Point(20, 153), ForeColor = System.Drawing.Color.White });
            Controls.Add(txtPhone);
            Controls.Add(new Label() { Text = "Дата рождения:", Location = new System.Drawing.Point(20, 183), ForeColor = System.Drawing.Color.White });
            Controls.Add(dtpBirth);
            Controls.Add(btnSave);

            if (id.HasValue)
            {
                string connStr = Properties.Settings.Default.Microsoft_Access_База_данныхConnectionString;
                using (OleDbConnection conn = new OleDbConnection(connStr))
                {
                    conn.Open();
                    string sql = @"SELECT [Фамилия], [Имя], [Отчество], [Телефон], [Дата рождения] 
                               FROM Клиенты WHERE [ID клиента]=?";
                    using (OleDbCommand cmd = new OleDbCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id.Value);
                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtLastName.Text = reader.GetString(0);
                                txtFirstName.Text = reader.GetString(1);
                                txtPatronymic.Text = reader.IsDBNull(2) ? "" : reader.GetString(2);
                                txtPhone.Text = reader.IsDBNull(3) ? "" : reader.GetString(3);
                                dtpBirth.Value = reader.GetDateTime(4);
                            }
                        }
                    }
                }
            }

            btnSave.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtLastName.Text) || string.IsNullOrWhiteSpace(txtFirstName.Text))
                {
                    MessageBox.Show("Фамилия и имя обязательны");
                    return;
                }
                LastName = txtLastName.Text;
                FirstName = txtFirstName.Text;
                Patronymic = txtPatronymic.Text;
                Phone = txtPhone.Text;
                BirthDate = dtpBirth.Value.Date;
                DialogResult = DialogResult.OK;
                Close();
            };
        }
    }
}