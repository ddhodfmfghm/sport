using System;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;
using System.Drawing;

namespace WindowsFormsApp1
{
    public partial class Посещения : Form
    {
        private string connectionString = Properties.Settings.Default.Microsoft_Access_База_данныхConnectionString;
        private DataTable dataTable;
        private DataView dataView;
        private string currentRole;
        private string currentSearchText = "";

        public Посещения(string role)
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
                string sql = "SELECT * FROM Посещения";
                if (!string.IsNullOrEmpty(filter))
                    sql += " WHERE " + filter;
                OleDbDataAdapter da = new OleDbDataAdapter(sql, conn);
                dataTable = new DataTable();
                da.Fill(dataTable);
                dataView = dataTable.DefaultView;
                dataGridView1.DataSource = dataView;
            }
        }

        // ИСПРАВЛЕНО: Безопасный поиск с использованием CStr для MS Access
        private void ApplySearch(string searchText)
        {
            currentSearchText = searchText;

            if (string.IsNullOrWhiteSpace(searchText) || searchText == "Поиск...")
            {
                dataView.RowFilter = "";
                dataGridView1.Refresh();
                return;
            }

            string filter = "";
            string escapedSearchText = searchText.Replace("'", "''");

            foreach (DataColumn col in dataTable.Columns)
            {
                if (filter.Length > 0) filter += " OR ";

                if (col.DataType == typeof(string))
                {
                    filter += $"[{col.ColumnName}] LIKE '%{escapedSearchText}%'";
                }
                else
                {
                    // Заменено CONVERT на CStr для совместимости RowFilter и Access
                    // Правильный синтаксис для приведения типов внутри RowFilter
                    filter += $"CONVERT([{col.ColumnName}], 'System.String') LIKE '%{escapedSearchText}%'";

                }
            }

            dataView.RowFilter = filter;
            dataGridView1.Refresh();
        }

        private void DataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (string.IsNullOrEmpty(currentSearchText) || currentSearchText == "Поиск...") return;

            if (e.Value != null && e.Value != DBNull.Value)
            {
                string cellText = e.Value.ToString();

                if (cellText.IndexOf(currentSearchText, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    e.CellStyle.BackColor = Color.Yellow;
                    e.CellStyle.ForeColor = Color.Black;
                    e.CellStyle.SelectionBackColor = Color.Gold;
                    return;
                }
            }

            e.CellStyle.BackColor = dataGridView1.DefaultCellStyle.BackColor;
            e.CellStyle.ForeColor = dataGridView1.DefaultCellStyle.ForeColor;
            e.CellStyle.SelectionBackColor = dataGridView1.DefaultCellStyle.SelectionBackColor;
        }

        private bool ValidateVisitData(EditVisitForm form)
        {
            if (form.AbonementId <= 0)
            {
                MessageBox.Show("Пожалуйста, укажите корректный ID абонемента!",
                                "Валидация", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (form.Duration <= 0)
            {
                MessageBox.Show("Длительность посещения должна быть больше 0 минут!",
                                "Валидация", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (form.StartDateTime == default(DateTime))
            {
                MessageBox.Show("Пожалуйста, укажите корректную дату и время начала посещения!",
                                "Валидация", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            EditVisitForm editForm = new EditVisitForm();

            while (editForm.ShowDialog() == DialogResult.OK)
            {
                if (!ValidateVisitData(editForm)) continue;

                try
                {
                    using (OleDbConnection conn = new OleDbConnection(connectionString))
                    {
                        conn.Open();

                        

                        using (OleDbTransaction transaction = conn.BeginTransaction())
                        {
                            try
                            {
                                // Добавление записи о посещении
                                string sql = @"INSERT INTO Посещения ([ID абонемента], [Дата начала], [Длительность(мин)]) VALUES (?,?,?)";
                                using (OleDbCommand cmd = new OleDbCommand(sql, conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@a", editForm.AbonementId);
                                    cmd.Parameters.Add("@dt", OleDbType.Date).Value = editForm.StartDateTime;
                                    cmd.Parameters.AddWithValue("@dur", Convert.ToInt32(editForm.Duration));
                                    cmd.ExecuteNonQuery();
                                }

                              

                                transaction.Commit();
                            }
                            catch
                            {
                                transaction.Rollback();
                                throw;
                            }
                        }
                    }

                    LoadData();
                    if (!string.IsNullOrEmpty(currentSearchText)) ApplySearch(currentSearchText);
                    break;
                }
                catch (OleDbException ex)
                {
                    if (ex.ErrorCode == -2147467259 || ex.Message.Contains("связанная запись"))
                    {
                        MessageBox.Show("Ошибка добавления! Указанного ID абонемента не существует.",
                                        "Ошибка данных", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        MessageBox.Show($"Код ошибки: {ex.ErrorCode}\nСообщение: {ex.Message}\nИсточник: {ex.Source}",
                    "Детальная ошибка БД", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
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
            int visitId = Convert.ToInt32(dataGridView1.CurrentRow.Cells["ID посещения"].Value);

            EditVisitForm editForm = new EditVisitForm(visitId);

            while (editForm.ShowDialog() == DialogResult.OK)
            {
                if (!ValidateVisitData(editForm)) continue;

                try
                {
                    using (OleDbConnection conn = new OleDbConnection(connectionString))
                    {
                        conn.Open();

                        // Проверяем, существует ли вообще редактируемое посещение
                        string getOldSql = "SELECT [ID посещения] FROM Посещения WHERE [ID посещения] = ?";
                        using (OleDbCommand getOldCmd = new OleDbCommand(getOldSql, conn))
                        {
                            getOldCmd.Parameters.AddWithValue("@id", visitId);
                            object result = getOldCmd.ExecuteScalar();
                            if (result == null)
                            {
                                MessageBox.Show("Редактируемое посещение не найдено в базе!",
                                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;
                            }
                        }

                        using (OleDbTransaction transaction = conn.BeginTransaction())
                        {
                            try
                            {
                                // Обновляем исключительно данные самого посещения
                                string updateVisitSql = @"UPDATE Посещения SET 
                                                  [ID абонемента] = ?, 
                                                  [Дата начала] = ?, 
                                                  [Длительность(мин)] = ?
                                                  WHERE [ID посещения] = ?";

                                using (OleDbCommand updateVisitCmd = new OleDbCommand(updateVisitSql, conn, transaction))
                                {
                                    // В OleDb (Access) параметры ДОЛЖНЫ добавляться строго в порядке их появления в SQL-запросе!
                                    updateVisitCmd.Parameters.AddWithValue("@abonementId", editForm.AbonementId);
                                    updateVisitCmd.Parameters.Add("@startDate", OleDbType.Date).Value = editForm.StartDateTime;
                                    updateVisitCmd.Parameters.AddWithValue("@duration", Convert.ToInt32(editForm.Duration));
                                    updateVisitCmd.Parameters.AddWithValue("@visitId", visitId);

                                    updateVisitCmd.ExecuteNonQuery();
                                }

                                transaction.Commit();
                            }
                            catch
                            {
                                transaction.Rollback();
                                throw;
                            }
                        }
                    }

                    LoadData();
                    if (!string.IsNullOrEmpty(currentSearchText)) ApplySearch(currentSearchText);
                    break;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при редактировании посещения: {ex.Message}",
                                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                }
            }
        }


        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null) return;
            if (MessageBox.Show("Удалить выбранное посещение?", "Подтверждение", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["ID посещения"].Value);
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();
                    string sql = "DELETE FROM Посещения WHERE [ID посещения]=?";
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

        private void Посещения_Load(object sender, EventArgs e)
        {

        }
    }

    public class EditVisitForm : Form
    {
        public int AbonementId { get; private set; }
        public DateTime StartDateTime { get; private set; }
        public int Duration { get; private set; }

        private TextBox txtAbonementId;
        private DateTimePicker dtpStart;
        private TextBox txtDuration;
        private Button btnSave;

        public EditVisitForm(int? id = null)
        {
            Text = id == null ? "Добавление посещения" : "Редактирование посещения";
            Width = 400;
            Height = 250;

            txtAbonementId = new TextBox() { Location = new System.Drawing.Point(150, 20), Width = 150 };
            dtpStart = new DateTimePicker() { Location = new System.Drawing.Point(150, 50), Width = 150 };
            txtDuration = new TextBox() { Location = new System.Drawing.Point(150, 80), Width = 150 };
            btnSave = new Button() { Text = "Сохранить", Location = new System.Drawing.Point(150, 130), Width = 100 };

            Controls.Add(new Label() { Text = "ID абонемента:", Location = new System.Drawing.Point(20, 23) });
            Controls.Add(txtAbonementId);
            Controls.Add(new Label() { Text = "Дата начала:", Location = new System.Drawing.Point(20, 53) });
            Controls.Add(dtpStart);
            Controls.Add(new Label() { Text = "Длительность (мин):", Location = new System.Drawing.Point(20, 83) });
            Controls.Add(txtDuration);
            Controls.Add(btnSave);

            if (id.HasValue)
            {
                string connStr = Properties.Settings.Default.Microsoft_Access_База_данныхConnectionString;
                using (OleDbConnection conn = new OleDbConnection(connStr))
                {
                    conn.Open();
                    string sql = @"SELECT [ID абонемента], [Дата начала], [Длительность(мин)] 
                                   FROM Посещения WHERE [ID посещения]=?";
                    using (OleDbCommand cmd = new OleDbCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id.Value);
                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtAbonementId.Text = reader.GetInt32(0).ToString();
                                dtpStart.Value = reader.GetDateTime(1);
                                txtDuration.Text = Convert.ToString(reader.GetValue(2));

                            }
                        }
                    }
                }
            }

            btnSave.Click += (s, e) =>
            {
                if (!int.TryParse(txtAbonementId.Text, out int aid) || !int.TryParse(txtDuration.Text, out int dur))
                {
                    MessageBox.Show("Проверьте числовые поля");
                    return;
                }
                AbonementId = aid;
                StartDateTime = dtpStart.Value.Date;
                Duration = dur;
                DialogResult = DialogResult.OK;
                Close();
            };
        }
    }
}