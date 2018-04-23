Imports System
Imports System.Linq
Imports DevExpress.XtraGrid.Columns
Imports System.Collections.Generic
Imports DevExpress.Utils.Controls

Namespace DateRange
    Public Class MyOptionsColumnFilter
        Inherits OptionsColumnFilter

        Protected Friend UseFilterPopupRangeDateMode As Boolean = False

        Private filterPopupMode_Renamed As FilterPopupModeExtended

        Public Shadows Property FilterPopupMode() As FilterPopupModeExtended
            Get
                Return filterPopupMode_Renamed
            End Get
            Set(ByVal value As FilterPopupModeExtended)
                If FilterPopupMode = value Then
                    Return
                End If
                Dim prevValue As FilterPopupModeExtended = FilterPopupMode
                filterPopupMode_Renamed = value
                OnChanged(New BaseOptionChangedEventArgs("FilterPopupMode", prevValue, FilterPopupMode))
            End Set
        End Property
    End Class
End Namespace
