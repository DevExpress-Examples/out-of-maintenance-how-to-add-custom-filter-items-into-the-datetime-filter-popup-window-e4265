Imports DevExpress.XtraGrid.Columns
Imports DevExpress.Utils.Controls

Namespace DateRange

    Public Class MyOptionsColumnFilter
        Inherits OptionsColumnFilter

        Protected Friend UseFilterPopupRangeDateMode As Boolean = False

        Private filterPopupModeField As FilterPopupModeExtended

        Public Overloads Property FilterPopupMode As FilterPopupModeExtended
            Get
                Return filterPopupModeField
            End Get

            Set(ByVal value As FilterPopupModeExtended)
                If FilterPopupMode = value Then Return
                Dim prevValue As FilterPopupModeExtended = FilterPopupMode
                filterPopupModeField = value
                OnChanged(New BaseOptionChangedEventArgs("FilterPopupMode", prevValue, FilterPopupMode))
            End Set
        End Property
    End Class
End Namespace
