using System;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;
using System.Drawing;

namespace WindowsFormsApp1
{
    public partial class Запрос3 : Form
    {
        private readonly string connectionString = Properties.Settings.Default.Microsoft_Access_База_данныхConnectionString;
        private DataTable dataTable;
        private DataView dataView;
        private string currentSearchText = "";

        public Запрос3()
        {
            InitializeComponent();
            LoadData();

            
            dataGridView1.CellFormatting += DataGridView1_CellFormatting;
        }

        private void LoadData()
        {
            
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

        private void button1_Click(object sender, EventArgs e) => Close();

        private void Запрос3_Load(object sender, EventArgs e)
        {
            // TODO: данная строка кода позволяет загрузить данные в таблицу "microsoft_Access_База_данныхDataSet.Запрос3". При необходимости она может быть перемещена или удалена.
            this.запрос3TableAdapter.Fill(this.microsoft_Access_База_данныхDataSet.Запрос3);

        }
    }
}