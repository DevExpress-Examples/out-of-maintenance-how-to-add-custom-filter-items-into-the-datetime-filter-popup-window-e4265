Imports System
Imports System.Drawing
Imports System.Windows.Forms
Imports DevExpress.XtraGrid.Columns
Imports DevExpress.XtraGrid.Views.Base
Imports DevExpress.XtraEditors.Repository
Imports DevExpress.XtraEditors.Helpers
Imports DevExpress.XtraEditors
Imports DevExpress.Data.Filtering
Imports DevExpress.XtraEditors.Controls
Imports DevExpress.XtraEditors.Popup

Namespace DateRange

    Public Class MyDateFilterPopup
        Inherits DateFilterPopup

        Public Sub New(ByVal view As ColumnView, ByVal column As GridColumn, ByVal ownerControl As Control, ByVal creator As Object)
            MyBase.New(view, column, ownerControl, creator)
        End Sub

        Const DateCalendarBorders As Integer = 10

        Private BetweenWidth As Integer

        Private dateCalendarField As DateControlEx

        Private DateCalendar1 As DateControlEx

        Private DateCalendar2 As DateControlEx

        Private Greater As CheckEdit

        Private Less As CheckEdit

        Private Between As CheckEdit

        Private ReadOnly Property DateCalendar As DateControlEx
            Get
                If dateCalendarField Is Nothing Then SetDateCalendar(DateFilterControl)
                Return dateCalendarField
            End Get
        End Property

        Private dateFilterControlField As PopupOutlookDateFilterControl

        Private ReadOnly Property DateFilterControl As PopupOutlookDateFilterControl
            Get
                If dateFilterControlField Is Nothing Then SetDateFilterControl(item)
                Return dateFilterControlField
            End Get
        End Property

#Region "Creators"
        Private item As RepositoryItemPopupContainerEdit

        Protected Overrides Function CreateRepositoryItem() As RepositoryItemPopupBase
            item = TryCast(MyBase.CreateRepositoryItem(), RepositoryItemPopupContainerEdit)
            If DateFilterControl.Controls.Count > 0 Then
                DateCalendar1 = CreateCalendar(DateCalendar1, DateCalendar.SelectionStart, DateCalendar.Top, DateCalendar.Left)
                DateCalendar1.Visible = False
                DateCalendar2 = CreateCalendar(DateCalendar2, DateCalendar.SelectionStart, DateCalendar1.Top, DateCalendar.Left + DateCalendar1.Width)
                DateCalendar2.Visible = False
                Greater = GetCheckEditByName(Localizer.Active.GetLocalizedString(StringId.FilterClauseGreater))
                Less = GetCheckEditByName(Localizer.Active.GetLocalizedString(StringId.FilterClauseLess))
                Between = GetCheckEditByName(Localizer.Active.GetLocalizedString(StringId.FilterClauseBetween))
                AddHandler Greater.CheckedChanged, AddressOf CheckedChanged
                AddHandler Less.CheckedChanged, AddressOf CheckedChanged
                AddHandler Between.CheckedChanged, AddressOf CheckedChanged
                For Each ctrl As Control In DateFilterControl.Controls
                    If TypeOf ctrl Is CheckEdit Then
                        If NotOurControl(TryCast(ctrl, CheckEdit)) Then AddHandler TryCast(ctrl, CheckEdit).CheckedChanged, AddressOf OriginalDateFilterPopup_CheckedChanged
                    End If
                Next
            End If

            Return item
        End Function

        Private Function FindOwnerForm(ByVal Owner As Control) As Control
            If TypeOf Owner.Parent Is PopupContainerForm Then
                Return Owner.Parent
            Else
                Return FindOwnerForm(Owner.Parent)
            End If
        End Function

        Public OriginalLocation As Point

        Protected Overridable Sub CreateRepositoryItemForOurFilterCriteria(ByVal Name As String)
            DateCalendar.Visible = False
            DateCalendar1.Visible = True
            Dim Location As Point = FindOwnerForm(dateFilterControlField).Location
            OriginalLocation = Location
            BetweenWidth = FindOwnerForm(DateFilterControl).Width
            If Equals(Name, Localizer.Active.GetLocalizedString(StringId.FilterClauseBetween)) Then
                FindOwnerForm(DateFilterControl).Width = DateCalendar1.Width + DateCalendar2.Width + DateCalendarBorders * 4
                DateCalendar2.Visible = True
                If Not Screen.PrimaryScreen.Bounds.Contains(FindOwnerForm(DateFilterControl).Bounds) Then Location.X -= FindOwnerForm(dateFilterControlField).Width \ 2 - DateCalendarBorders * 2
                FindOwnerForm(dateFilterControlField).Location = Location
            End If
        End Sub

        Private Sub ReturnOriginalView()
            DateCalendar1.Visible = False
            DateCalendar2.Visible = False
            dateCalendarField.Visible = True
            FindOwnerForm(DateFilterControl).Location = OriginalLocation
            If BetweenWidth < DateFilterControl.Parent.Parent.Width Then DateFilterControl.Parent.Parent.Width = BetweenWidth
        End Sub

        Private Sub CalcControlsLocation(ByVal Name As String)
            Dim FirstCheckEdit As CheckEdit = TryCast(DateFilterControl.Controls(0), CheckEdit)
            Dim ChechEditControls As Integer = DateFilterControl.Controls.Count
            Dim n As Integer = 0
            For i As Integer = 0 To ChechEditControls - 1
                Dim ctrl As Control = DateFilterControl.Controls(i)
                If TypeOf ctrl Is CheckEdit Then
                    ctrl.Location = New Point(FirstCheckEdit.Location.X, FirstCheckEdit.Location.Y + FirstCheckEdit.Size.Height * (i - n))
                    If Equals(ctrl.Text, Name) Then
                        Dim newLoc As Point = ctrl.Location
                        newLoc.Y += ctrl.Size.Height
                        DateCalendar1.Location = newLoc
                        newLoc.X += DateCalendar1.Width + DateCalendarBorders * 2
                        DateCalendar2.Location = newLoc
                        i = DateFilterControl.Controls.Count - 1
                    End If
                Else
                    n += 1
                End If
            Next

            CreateRepositoryItemForOurFilterCriteria(Name)
        End Sub

        Private Function CreateCalendar(ByVal calendar As DateControlEx, ByVal dateTime As Date, ByVal top As Integer, ByVal left As Integer) As DateControlEx
            calendar = New DateControlEx()
            calendar.DateTime = dateTime
            calendar.Top = top
            calendar.Left = left
            AddHandler calendar.EditDateModified, AddressOf DateCalendar_SelectionChanged
            DateFilterControl.Controls.Add(calendar)
            Return calendar
        End Function

#End Region
#Region "Handlers&HandlersHelpers"
        Private Sub CheckedChanged(ByVal sender As Object, ByVal e As EventArgs)
            If TryCast(sender, CheckEdit).Checked Then
                UpdateOurControlCheckedState(TryCast(sender, CheckEdit).Text)
                CalcControlsLocation(TryCast(sender, CheckEdit).Text)
                View.ActiveFilterCriteria = GetFilterCriteriaByControlState()
            Else
                If DateCalendar1.Visible OrElse DateCalendar2.Visible Then
                    ReturnOriginalView()
                    ReturnOriginalControlsLocation()
                End If
            End If
        End Sub

        Protected Overridable Sub UpdateOurControlCheckedState(ByVal Name As String)
            If Equals(Name, Localizer.Active.GetLocalizedString(StringId.FilterClauseBetween)) Then
                Less.Checked = False
                Greater.Checked = False
            End If

            If Equals(Name, Localizer.Active.GetLocalizedString(StringId.FilterClauseGreater)) Then
                Less.Checked = False
                Between.Checked = False
            End If

            If Equals(Name, Localizer.Active.GetLocalizedString(StringId.FilterClauseLess)) Then
                Greater.Checked = False
                Between.Checked = False
            End If

            For Each ctrl As Control In DateFilterControl.Controls
                If TypeOf ctrl Is CheckEdit Then
                    If NotOurControl(TryCast(ctrl, CheckEdit)) Then TryCast(ctrl, CheckEdit).Checked = False
                End If
            Next
        End Sub

        Private Sub OriginalDateFilterPopup_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs)
            If TryCast(sender, CheckEdit).Checked Then
                If NotOurControl(TryCast(sender, CheckEdit)) Then
                    Greater.Checked = False
                    Less.Checked = False
                    Between.Checked = False
                End If
            End If
        End Sub

        Private Sub ReturnOriginalControlsLocation()
            For i As Integer = 0 To DateFilterControl.Controls.Count - 1 - 1
                Dim ctrl As Control = DateFilterControl.Controls(i + 1)
                Dim NewLocation As Point = DateFilterControl.Controls(i).Location
                NewLocation.Y += DateFilterControl.Controls(i).Height
                ctrl.Location = NewLocation
            Next
        End Sub

        Private Sub DateCalendar_SelectionChanged(ByVal sender As Object, ByVal e As EventArgs)
            View.ActiveFilterCriteria = GetFilterCriteriaByControlState()
            If Not Greater.Checked AndAlso Not Less.Checked And Not Between.Checked Then dateCalendarField.DateTime = DateCalendar1.DateTime
        End Sub

#End Region
        Private Function GetFilterCriteriaByControlState() As CriteriaOperator
            If Greater.Checked Then Return GetBinaryOperatorByName(Localizer.Active.GetLocalizedString(StringId.FilterClauseGreater))
            If Less.Checked Then Return GetBinaryOperatorByName(Localizer.Active.GetLocalizedString(StringId.FilterClauseLess))
            If Between.Checked Then
                Return GetBetweenOperatorByName(Localizer.Active.GetLocalizedString(StringId.FilterClauseBetween))
            End If

            Return Nothing
        End Function

        Protected Overloads ReadOnly Property View As MyGridView
            Get
                Return TryCast(MyBase.View, MyGridView)
            End Get
        End Property

#Region "ControlHelpers"
        Public Function GetCheckEditByName(ByVal Name As String) As CheckEdit
            For Each ctrl As Control In DateFilterControl.Controls
                If Equals(ctrl.Text, Name) Then Return TryCast(ctrl, CheckEdit)
            Next

            Return Nothing
        End Function

        Private Sub SetDateFilterControl(ByVal item As RepositoryItemPopupContainerEdit)
            For Each ctrl As Control In item.PopupControl.Controls
                If TypeOf ctrl Is PopupOutlookDateFilterControl Then
                    dateFilterControlField = TryCast(ctrl, PopupOutlookDateFilterControl)
                    Exit For
                End If
            Next
        End Sub

        Private Sub SetDateCalendar(ByVal dateFilterControl As PopupOutlookDateFilterControl)
            For Each c As Control In dateFilterControl.Controls
                If TypeOf c Is DateControlEx Then
                    dateCalendarField = TryCast(c, DateControlEx)
                    Exit For
                End If
            Next
        End Sub

        Protected Overridable Function GetBinaryOperatorByName(ByVal Name As String) As BinaryOperator
            If DateCalendar1 IsNot Nothing Then
                If Equals(Name, Localizer.Active.GetLocalizedString(StringId.FilterClauseGreater)) Then
                    Return New BinaryOperator(Column.FieldName, DateCalendar1.DateTime, BinaryOperatorType.Greater)
                End If

                If Equals(Name, Localizer.Active.GetLocalizedString(StringId.FilterClauseLess)) Then
                    Return New BinaryOperator(Column.FieldName, DateCalendar1.DateTime, BinaryOperatorType.Less)
                End If
            End If

            Return Nothing
        End Function

        Protected Overridable Function GetBetweenOperatorByName(ByVal Name As String) As BetweenOperator
            If DateCalendar2 IsNot Nothing Then
                If Equals(Name, Localizer.Active.GetLocalizedString(StringId.FilterClauseBetween)) Then
                    Return New BetweenOperator(Column.FieldName, DateCalendar1.DateTime, DateCalendar2.DateTime)
                End If
            End If

            Return Nothing
        End Function

#End Region
        Protected Overridable Function NotOurControl(ByVal ctrl As CheckEdit) As Boolean
            If Not Equals(ctrl.Text, Localizer.Active.GetLocalizedString(StringId.FilterClauseGreater)) AndAlso Not Equals(ctrl.Text, Localizer.Active.GetLocalizedString(StringId.FilterClauseLess)) AndAlso Not Equals(ctrl.Text, Localizer.Active.GetLocalizedString(StringId.FilterClauseBetween)) Then Return True
            Return False
        End Function

        Public Overrides Sub Dispose()
            If DateCalendar1 IsNot Nothing Then RemoveHandler DateCalendar1.EditDateModified, AddressOf DateCalendar_SelectionChanged
            If DateCalendar2 IsNot Nothing Then RemoveHandler DateCalendar2.EditDateModified, AddressOf DateCalendar_SelectionChanged
            For Each ctrl As Control In DateFilterControl.Controls
                If TypeOf ctrl Is CheckEdit Then
                    If NotOurControl(TryCast(ctrl, CheckEdit)) Then RemoveHandler TryCast(ctrl, CheckEdit).CheckedChanged, AddressOf OriginalDateFilterPopup_CheckedChanged
                End If
            Next

            Try
                View.ActiveFilterString = GetFilterCriteriaByControlState().ToString()
            Catch
            End Try

            DateCalendar1.Dispose()
            DateCalendar2.Dispose()
            MyBase.Dispose()
            If dateCalendarField IsNot Nothing Then
                dateCalendarField.Dispose()
                dateCalendarField = Nothing
            End If

            If Greater IsNot Nothing Then
                Greater.Dispose()
                Greater = Nothing
            End If

            If Less IsNot Nothing Then
                Less.Dispose()
                Less = Nothing
            End If

            If Between IsNot Nothing Then
                Between.Dispose()
                Between = Nothing
            End If

            If dateFilterControlField IsNot Nothing Then
                dateFilterControlField.Dispose()
                dateFilterControlField = Nothing
            End If

            If item IsNot Nothing Then
                item.Dispose()
                item = Nothing
            End If
        End Sub
    End Class
End Namespace
