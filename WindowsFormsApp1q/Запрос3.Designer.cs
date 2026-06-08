namespace WindowsFormsApp1
{
    partial class Запрос3
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.iDАбонементаDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.датаНачалаDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.датаОкончанияDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.фамилияDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.имяDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.статусDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.запрос3BindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.microsoft_Access_База_данныхDataSet = new WindowsFormsApp1.Microsoft_Access_База_данныхDataSet();
            this.button1 = new System.Windows.Forms.Button();
            this.запрос3TableAdapter = new WindowsFormsApp1.Microsoft_Access_База_данныхDataSetTableAdapters.Запрос3TableAdapter();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.запрос3BindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.microsoft_Access_База_данныхDataSet)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.dataGridView1.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(240)))), ((int)(((byte)(241)))));
            this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.iDАбонементаDataGridViewTextBoxColumn,
            this.датаНачалаDataGridViewTextBoxColumn,
            this.датаОкончанияDataGridViewTextBoxColumn,
            this.фамилияDataGridViewTextBoxColumn,
            this.имяDataGridViewTextBoxColumn,
            this.статусDataGridViewTextBoxColumn});
            this.dataGridView1.DataSource = this.запрос3BindingSource;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.dataGridView1.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(189)))), ((int)(((byte)(195)))), ((int)(((byte)(199)))));
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(800, 460);
            this.dataGridView1.TabIndex = 0;
            // 
            // iDАбонементаDataGridViewTextBoxColumn
            // 
            this.iDАбонементаDataGridViewTextBoxColumn.DataPropertyName = "ID абонемента";
            this.iDАбонементаDataGridViewTextBoxColumn.HeaderText = "ID абонемента";
            this.iDАбонементаDataGridViewTextBoxColumn.Name = "iDАбонементаDataGridViewTextBoxColumn";
            // 
            // датаНачалаDataGridViewTextBoxColumn
            // 
            this.датаНачалаDataGridViewTextBoxColumn.DataPropertyName = "Дата начала";
            this.датаНачалаDataGridViewTextBoxColumn.HeaderText = "Дата начала";
            this.датаНачалаDataGridViewTextBoxColumn.Name = "датаНачалаDataGridViewTextBoxColumn";
            // 
            // датаОкончанияDataGridViewTextBoxColumn
            // 
            this.датаОкончанияDataGridViewTextBoxColumn.DataPropertyName = "Дата окончания";
            this.датаОкончанияDataGridViewTextBoxColumn.HeaderText = "Дата окончания";
            this.датаОкончанияDataGridViewTextBoxColumn.Name = "датаОкончанияDataGridViewTextBoxColumn";
            // 
            // фамилияDataGridViewTextBoxColumn
            // 
            this.фамилияDataGridViewTextBoxColumn.DataPropertyName = "Фамилия";
            this.фамилияDataGridViewTextBoxColumn.HeaderText = "Фамилия";
            this.фамилияDataGridViewTextBoxColumn.Name = "фамилияDataGridViewTextBoxColumn";
            // 
            // имяDataGridViewTextBoxColumn
            // 
            this.имяDataGridViewTextBoxColumn.DataPropertyName = "Имя";
            this.имяDataGridViewTextBoxColumn.HeaderText = "Имя";
            this.имяDataGridViewTextBoxColumn.Name = "имяDataGridViewTextBoxColumn";
            // 
            // статусDataGridViewTextBoxColumn
            // 
            this.статусDataGridViewTextBoxColumn.DataPropertyName = "Статус";
            this.статусDataGridViewTextBoxColumn.HeaderText = "Статус";
            this.статусDataGridViewTextBoxColumn.Name = "статусDataGridViewTextBoxColumn";
            // 
            // запрос3BindingSource
            // 
            this.запрос3BindingSource.DataMember = "Запрос3";
            this.запрос3BindingSource.DataSource = this.microsoft_Access_База_данныхDataSet;
            // 
            // microsoft_Access_База_данныхDataSet
            // 
            this.microsoft_Access_База_данныхDataSet.DataSetName = "Microsoft_Access_База_данныхDataSet";
            this.microsoft_Access_База_данныхDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(165)))), ((int)(((byte)(166)))));
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Location = new System.Drawing.Point(700, 420);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(90, 30);
            this.button1.TabIndex = 0;
            this.button1.Text = "ЗАКРЫТЬ";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // запрос3TableAdapter
            // 
            this.запрос3TableAdapter.ClearBeforeFill = true;
            // 
            // Запрос3
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(42)))), ((int)(((byte)(58)))));
            this.ClientSize = new System.Drawing.Size(800, 460);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.dataGridView1);
            this.Name = "Запрос3";
            this.Text = "Запрос 3";
            this.Load += new System.EventHandler(this.Запрос3_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.запрос3BindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.microsoft_Access_База_данныхDataSet)).EndInit();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button button1;
        private Microsoft_Access_База_данныхDataSet microsoft_Access_База_данныхDataSet;
        private System.Windows.Forms.BindingSource запрос3BindingSource;
        private Microsoft_Access_База_данныхDataSetTableAdapters.Запрос3TableAdapter запрос3TableAdapter;
        private System.Windows.Forms.DataGridViewTextBoxColumn iDАбонементаDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn датаНачалаDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn датаОкончанияDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn фамилияDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn имяDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn статусDataGridViewTextBoxColumn;
    }
}