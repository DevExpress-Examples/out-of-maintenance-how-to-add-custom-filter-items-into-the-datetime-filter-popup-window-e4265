Imports DevExpress.XtraGrid
Imports DevExpress.XtraGrid.Registrator
Imports DevExpress.XtraGrid.Views.Base

Namespace DateRange

    <System.ComponentModel.DesignerCategory("")>
    Public Class MyGridControl
        Inherits GridControl

        Protected Overrides Sub RegisterAvailableViewsCore(ByVal collection As InfoCollection)
            MyBase.RegisterAvailableViewsCore(collection)
            collection.Add(New MyGridViewInfoRegistrator())
        End Sub
    End Class

    Public Class MyGridViewInfoRegistrator
        Inherits GridInfoRegistrator

        Public Overrides ReadOnly Property ViewName As String
            Get
                Return "MyGridView"
            End Get
        End Property

        Public Overrides Function CreateView(ByVal grid As GridControl) As BaseView
            Return New MyGridView(TryCast(grid, GridControl))
        End Function
    End Class
End Namespace
