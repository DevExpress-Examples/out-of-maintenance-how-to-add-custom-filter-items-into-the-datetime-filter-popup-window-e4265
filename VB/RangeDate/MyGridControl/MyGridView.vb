Imports System
Imports System.Linq
Imports DevExpress.XtraGrid
Imports DevExpress.XtraGrid.Columns
Imports DevExpress.XtraGrid.Views.Base
Imports DevExpress.XtraGrid.Views.Grid
Imports DevExpress.Utils.Serializing
Imports System.Collections.Generic
Imports System.ComponentModel
Imports DevExpress.Data.Filtering
Imports DevExpress.XtraEditors
Imports DevExpress.XtraEditors.Controls


Namespace DateRange
    <System.ComponentModel.DesignerCategory("")> _
    Public Class MyGridView
        Inherits GridView

        Public Sub New()
            Me.New(Nothing)
        End Sub
        Public Sub New(ByVal grid As GridControl)
            MyBase.New(grid)

        End Sub
        Protected Overrides Function CreateColumnCollection() As GridColumnCollection
            Return New MyGridColumnCollection(Me)
        End Function
        <Browsable(False), XtraSerializableProperty(XtraSerializationVisibility.Collection, True, True, True, 0, XtraSerializationFlags.DefaultValue), XtraSerializablePropertyId(2), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)> _
        Public Shadows ReadOnly Property Columns() As MyGridColumnCollection
            Get
                Return TryCast(MyBase.Columns, MyGridColumnCollection)
            End Get
        End Property

        Protected Overrides Function CreateDateFilterPopup(ByVal column As GridColumn, ByVal ownerControl As System.Windows.Forms.Control, ByVal creator As Object) As DateFilterPopup
            Return New MyDateFilterPopup(Me, column, ownerControl, creator)
        End Function

        Private Enum some
            Greater
            DateRange
        End Enum

        Protected Overrides Sub RaiseFilterPopupDate(ByVal filterPopup As DateFilterPopup, ByVal list As List(Of FilterDateElement))
            Dim filter As CriteriaOperator = New BinaryOperator(filterPopup.Column.FieldName, Date.Today, BinaryOperatorType.Greater)

            list.Add(New FilterDateElement(Localizer.Active.GetLocalizedString(StringId.FilterClauseGreater), "", filter))
            filter = New BinaryOperator(filterPopup.Column.FieldName, Date.Today, BinaryOperatorType.Less)
            list.Add(New FilterDateElement(Localizer.Active.GetLocalizedString(StringId.FilterClauseLess), "", filter))
            filter = New BetweenOperator(filterPopup.Column.FieldName, Date.Today, Date.Today)
            list.Add(New FilterDateElement(Localizer.Active.GetLocalizedString(StringId.FilterClauseBetween), "", filter))
            MyBase.RaiseFilterPopupDate(filterPopup, list)
        End Sub
    End Class
End Namespace