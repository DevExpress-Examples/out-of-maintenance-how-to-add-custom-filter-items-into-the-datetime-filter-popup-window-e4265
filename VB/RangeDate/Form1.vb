Imports System
Imports System.Data
Imports System.Windows.Forms
Imports DevExpress.XtraEditors.Repository

Namespace DateRange

    Public Partial Class Form1
        Inherits Form

        Public Sub New()
            InitializeComponent()
            AddHandler myGridView1.FilterPopupExcelData, AddressOf MyGridView1_FilterPopupExcelData
        End Sub

        Private Sub MyGridView1_FilterPopupExcelData(ByVal sender As Object, ByVal e As DevExpress.XtraGrid.Views.Grid.FilterPopupExcelDataEventArgs)
            If Equals(e.Column.FieldName, "Range Date") Then e.AddFilter("Custom filter", String.Format("[{1}] = #{0}#", Date.Today.Date, e.Column.FieldName))
        End Sub

        Private riDateEdit As RepositoryItemDateEdit = New RepositoryItemDateEdit()

        Private Sub Form1_Load(ByVal sender As Object, ByVal e As EventArgs)
            Dim dataTable As DataTable = New DataTable()
            dataTable.Columns.Add("Range Date", GetType(Date))
            dataTable.Columns.Add("Event", GetType(String))
            Dim row0 As Object() = New Object(1) {Date.Today - New TimeSpan(1, 0, 0, 0), "Yesterday"}
            Dim row1 As Object() = New Object(1) {Date.Today, "Today"}
            Dim row2 As Object() = New Object(1) {Date.Today + New TimeSpan(1, 0, 0, 0), "Tomorrow"}
            dataTable.Rows.Add(row0)
            dataTable.Rows.Add(row1)
            dataTable.Rows.Add(row2)
            myGridControl1.DataSource = dataTable
            myGridView1.Columns("Range Date").ColumnEdit = riDateEdit
        End Sub
    End Class
End Namespace
