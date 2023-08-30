using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors.Repository;


namespace DateRange
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            myGridView1.FilterPopupExcelData += MyGridView1_FilterPopupExcelData;
        }

        private void MyGridView1_FilterPopupExcelData(object sender, DevExpress.XtraGrid.Views.Grid.FilterPopupExcelDataEventArgs e) {
            if(e.Column.FieldName == "Range Date")
            e.AddFilter("Custom filter", String.Format("[{1}] = #{0}#", DateTime.Today.Date, e.Column.FieldName));
        }

        RepositoryItemDateEdit riDateEdit = new RepositoryItemDateEdit();
        private void Form1_Load(object sender, EventArgs e)
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Range Date", typeof(DateTime));
            dataTable.Columns.Add("Event", typeof(string));
            object[] row0 = new object[2] { DateTime.Today - new TimeSpan(1, 0, 0, 0), "Yesterday" };
            object[] row1 = new object[2] { DateTime.Today, "Today" };
            object[] row2 = new object[2] { DateTime.Today + new TimeSpan(1, 0, 0, 0), "Tomorrow" };
            dataTable.Rows.Add(row0);
            dataTable.Rows.Add(row1);
            dataTable.Rows.Add(row2);
            myGridControl1.DataSource = dataTable;
            myGridView1.Columns["Range Date"].ColumnEdit = riDateEdit;
        }
    }
}
