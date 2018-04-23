Imports System
Imports System.Linq
Imports DevExpress.XtraGrid
Imports DevExpress.XtraGrid.Columns
Imports System.Collections.Generic
Imports DevExpress.XtraGrid.Views.Base

Namespace DateRange
    <System.ComponentModel.DesignerCategory("")> _
    Public Class MyGridColumn
        Inherits GridColumn

        Public Shadows ReadOnly Property OptionsFilter() As MyOptionsColumnFilter
            Get
                Return TryCast(MyBase.OptionsFilter, MyOptionsColumnFilter)
            End Get
        End Property
        Protected Overrides Function CreateOptionsFilter() As OptionsColumnFilter
            Return New MyOptionsColumnFilter()
        End Function
        Protected Overrides Function GetFilterPopupMode() As FilterPopupMode
            Dim modeExtended As FilterPopupModeExtended = OptionsFilter.FilterPopupMode
            Dim mode As FilterPopupMode = FilterPopupMode.List
            If modeExtended = FilterPopupModeExtended.Default AndAlso (ColumnType.Equals(GetType(Date)) OrElse ColumnType.Equals(GetType(Date?))) AndAlso FilterMode <> ColumnFilterMode.DisplayText Then
                modeExtended = FilterPopupModeExtended.DateSmart
            End If
            If modeExtended = FilterPopupModeExtended.Default Then
                modeExtended = FilterPopupModeExtended.List
            End If
            OptionsFilter.UseFilterPopupRangeDateMode = False
            Select Case modeExtended.ToString()
                Case "Default"
                    mode = FilterPopupMode.Default
                    Exit Select
                Case "List"
                    mode = FilterPopupMode.List
                    Exit Select
                Case "CheckedList"
                    mode = FilterPopupMode.CheckedList
                    Exit Select
                Case "Date"
                    mode = FilterPopupMode.Date
                    Exit Select
                Case "DateSmart"
                    mode = FilterPopupMode.DateSmart
                    Exit Select
                Case "DateAlt"
                    mode = FilterPopupMode.DateAlt
                    Exit Select
                Case "DateRange"
                    mode = FilterPopupMode.Date
                    OptionsFilter.UseFilterPopupRangeDateMode = True
                    Exit Select
                Case Else
            End Select
            Return mode
        End Function
    End Class

    Public Enum FilterPopupModeExtended
        [Default]
        List
        CheckedList
        [Date]
        DateSmart
        DateAlt
        DateRange
    End Enum

    Public Class MyGridColumnCollection
        Inherits GridColumnCollection

        Public Sub New(ByVal view As ColumnView)
            MyBase.New(view)
        End Sub
        Protected Overrides Function CreateColumn() As GridColumn
            Return New MyGridColumn()
        End Function
        Default Public Shadows ReadOnly Property Item(ByVal fieldName As String) As MyGridColumn
            Get
                Return TryCast(ColumnByFieldName(fieldName), MyGridColumn)
            End Get
        End Property
        Default Public Shadows ReadOnly Property Item(ByVal index As Integer) As MyGridColumn
            Get
                Return CType(List(index), MyGridColumn)
            End Get
        End Property
    End Class
End Namespace
