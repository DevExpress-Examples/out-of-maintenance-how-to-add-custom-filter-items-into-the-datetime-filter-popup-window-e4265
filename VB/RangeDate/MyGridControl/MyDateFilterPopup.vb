Imports System
Imports System.Drawing
Imports System.Windows.Forms
Imports System.Linq
Imports DevExpress.XtraGrid.Columns
Imports DevExpress.XtraGrid.Views.Base
Imports DevExpress.XtraEditors.Repository
Imports DevExpress.XtraEditors.Helpers
Imports System.Collections.Generic
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
        Private Const DateCalendarBorders As Integer = 10
        Private BetweenWidth As Integer

        Private dateCalendar_Renamed As DateControlEx
        Private DateCalendar1 As DateControlEx
        Private DateCalendar2 As DateControlEx
        Private Greater As CheckEdit
        Private Less As CheckEdit
        Private Between As CheckEdit

        Private ReadOnly Property DateCalendar() As DateControlEx
            Get
                If dateCalendar_Renamed Is Nothing Then
                    SetDateCalendar(DateFilterControl)
                End If
                Return dateCalendar_Renamed
            End Get
        End Property


        Private dateFilterControl_Renamed As PopupOutlookDateFilterControl
        Private ReadOnly Property DateFilterControl() As PopupOutlookDateFilterControl
            Get
                If dateFilterControl_Renamed Is Nothing Then
                    SetDateFilterControl(item)
                End If
                Return dateFilterControl_Renamed
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
                        If NotOurControl(TryCast(ctrl, CheckEdit)) Then
                            AddHandler TryCast(ctrl, CheckEdit).CheckedChanged, AddressOf OriginalDateFilterPopup_CheckedChanged
                        End If
                    End If
                Next ctrl
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
            Dim Location As Point = FindOwnerForm(dateFilterControl_Renamed).Location
            OriginalLocation = Location
            BetweenWidth = FindOwnerForm(DateFilterControl).Width
            If Name = Localizer.Active.GetLocalizedString(StringId.FilterClauseBetween) Then

                FindOwnerForm(DateFilterControl).Width = DateCalendar1.Width + DateCalendar2.Width + DateCalendarBorders * 4
                DateCalendar2.Visible = True
                If Not Screen.PrimaryScreen.Bounds.Contains(FindOwnerForm(DateFilterControl).Bounds) Then
                    Location.X -= FindOwnerForm(dateFilterControl_Renamed).Width \ 2 - DateCalendarBorders * 2
                End If
                FindOwnerForm(dateFilterControl_Renamed).Location = Location
            End If

        End Sub

        Private Sub ReturnOriginalView()
            DateCalendar1.Visible = False
            DateCalendar2.Visible = False
            dateCalendar_Renamed.Visible = True
            FindOwnerForm(DateFilterControl).Location = OriginalLocation
            If BetweenWidth < DateFilterControl.Parent.Parent.Width Then
                DateFilterControl.Parent.Parent.Width = BetweenWidth
            End If

        End Sub


        Private Sub CalcControlsLocation(ByVal Name As String)
            Dim FirstCheckEdit As CheckEdit = TryCast(DateFilterControl.Controls(0), CheckEdit)
            Dim ChechEditControls As Integer = DateFilterControl.Controls.Count
            Dim n As Integer = 0
            For i As Integer = 0 To ChechEditControls - 1
                Dim ctrl As Control = DateFilterControl.Controls(i)
                If TypeOf ctrl Is CheckEdit Then
                    ctrl.Location = New Point(FirstCheckEdit.Location.X, FirstCheckEdit.Location.Y + FirstCheckEdit.Size.Height * (i - n))
                    If ctrl.Text = Name Then
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

            Next i
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
            If (TryCast(sender, CheckEdit)).Checked Then
                UpdateOurControlCheckedState((TryCast(sender, CheckEdit)).Text)
                CalcControlsLocation((TryCast(sender, CheckEdit)).Text)
                Me.View.ActiveFilterCriteria = GetFilterCriteriaByControlState()
            Else
                If DateCalendar1.Visible OrElse DateCalendar2.Visible Then
                    ReturnOriginalView()
                    ReturnOriginalControlsLocation()
                End If
            End If

        End Sub
        Protected Overridable Sub UpdateOurControlCheckedState(ByVal Name As String)
            If Name = Localizer.Active.GetLocalizedString(StringId.FilterClauseBetween) Then
                Less.Checked = False
                Greater.Checked = False
            End If
            If Name = Localizer.Active.GetLocalizedString(StringId.FilterClauseGreater) Then
                Less.Checked = False
                Between.Checked = False
            End If
            If Name = Localizer.Active.GetLocalizedString(StringId.FilterClauseLess) Then
                Greater.Checked = False
                Between.Checked = False
            End If
            For Each ctrl As Control In DateFilterControl.Controls
                If TypeOf ctrl Is CheckEdit Then
                    If NotOurControl(TryCast(ctrl, CheckEdit)) Then
                        TryCast(ctrl, CheckEdit).Checked = False
                    End If
                End If
            Next ctrl
        End Sub

        Private Sub OriginalDateFilterPopup_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs)
            If (TryCast(sender, CheckEdit)).Checked Then
                If NotOurControl(TryCast(sender, CheckEdit)) Then
                    Greater.Checked = False
                    Less.Checked = False
                    Between.Checked = False
                End If
            End If

        End Sub
        Private Sub ReturnOriginalControlsLocation()
            For i As Integer = 0 To DateFilterControl.Controls.Count - 2
                Dim ctrl As Control = DateFilterControl.Controls(i + 1)
                Dim NewLocation As Point = DateFilterControl.Controls(i).Location
                NewLocation.Y += DateFilterControl.Controls(i).Height
                ctrl.Location = NewLocation
            Next i
        End Sub

        Private Sub DateCalendar_SelectionChanged(ByVal sender As Object, ByVal e As EventArgs)
            Me.View.ActiveFilterCriteria = GetFilterCriteriaByControlState()
            If Not Greater.Checked AndAlso Not Less.Checked And Not Between.Checked Then
                dateCalendar_Renamed.DateTime = DateCalendar1.DateTime
            End If
        End Sub
        #End Region


        Private Function GetFilterCriteriaByControlState() As CriteriaOperator
            If Greater.Checked Then
                Return GetBinaryOperatorByName(Localizer.Active.GetLocalizedString(StringId.FilterClauseGreater))
            End If

            If Less.Checked Then
                Return GetBinaryOperatorByName(Localizer.Active.GetLocalizedString(StringId.FilterClauseLess))
            End If
            If Between.Checked Then
                Return GetBetweenOperatorByName(Localizer.Active.GetLocalizedString(StringId.FilterClauseBetween))
            End If
            Return Nothing
        End Function
        Protected Shadows ReadOnly Property View() As MyGridView
            Get
                Return TryCast(MyBase.View, MyGridView)
            End Get
        End Property

        #Region "ControlHelpers"

        Public Function GetCheckEditByName(ByVal Name As String) As CheckEdit

            For Each ctrl As Control In DateFilterControl.Controls
                If ctrl.Text = Name Then
                    Return TryCast(ctrl, CheckEdit)
                End If

            Next ctrl
            Return Nothing

        End Function

        Private Sub SetDateFilterControl(ByVal item As RepositoryItemPopupContainerEdit)
            For Each ctrl As Control In item.PopupControl.Controls
                If TypeOf ctrl Is PopupOutlookDateFilterControl Then
                    dateFilterControl_Renamed = TryCast(ctrl, PopupOutlookDateFilterControl)
                    Exit For
                End If
            Next ctrl
        End Sub

        Private Sub SetDateCalendar(ByVal dateFilterControl As PopupOutlookDateFilterControl)
            For Each c As Control In dateFilterControl.Controls
                If TypeOf c Is DateControlEx Then
                    dateCalendar_Renamed = TryCast(c, DateControlEx)
                    Exit For
                End If
            Next c
        End Sub

        Protected Overridable Function GetBinaryOperatorByName(ByVal Name As String) As BinaryOperator
            If DateCalendar1 IsNot Nothing Then
                If Name = Localizer.Active.GetLocalizedString(StringId.FilterClauseGreater) Then
                    Return New BinaryOperator(Me.Column.FieldName, DateCalendar1.DateTime, BinaryOperatorType.Greater)
                End If
                If Name = Localizer.Active.GetLocalizedString(StringId.FilterClauseLess) Then
                    Return New BinaryOperator(Me.Column.FieldName, DateCalendar1.DateTime, BinaryOperatorType.Less)
                End If
            End If
            Return Nothing
        End Function
        Protected Overridable Function GetBetweenOperatorByName(ByVal Name As String) As BetweenOperator
            If DateCalendar2 IsNot Nothing Then
                If Name = Localizer.Active.GetLocalizedString(StringId.FilterClauseBetween) Then
                Return New BetweenOperator(Me.Column.FieldName, DateCalendar1.DateTime, DateCalendar2.DateTime)
                End If
            End If
            Return Nothing

        End Function
        #End Region

        Protected Overridable Function NotOurControl(ByVal ctrl As CheckEdit) As Boolean
            If ctrl.Text <> Localizer.Active.GetLocalizedString(StringId.FilterClauseGreater) AndAlso ctrl.Text <> Localizer.Active.GetLocalizedString(StringId.FilterClauseLess) AndAlso ctrl.Text <> Localizer.Active.GetLocalizedString(StringId.FilterClauseBetween) Then
                Return True
            End If

            Return False
        End Function

        Public Overrides Sub Dispose()
            If DateCalendar1 IsNot Nothing Then
                RemoveHandler DateCalendar1.EditDateModified, AddressOf DateCalendar_SelectionChanged
            End If
            If DateCalendar2 IsNot Nothing Then
                RemoveHandler DateCalendar2.EditDateModified, AddressOf DateCalendar_SelectionChanged
            End If
            For Each ctrl As Control In DateFilterControl.Controls
                If TypeOf ctrl Is CheckEdit Then
                    If NotOurControl(TryCast(ctrl, CheckEdit)) Then
                        RemoveHandler TryCast(ctrl, CheckEdit).CheckedChanged, AddressOf OriginalDateFilterPopup_CheckedChanged
                    End If
                End If
            Next ctrl
            Try
                Me.View.ActiveFilterString = GetFilterCriteriaByControlState().ToString()
            Catch
            End Try
            DateCalendar1.Dispose()
            DateCalendar2.Dispose()
            MyBase.Dispose()
            If dateCalendar_Renamed IsNot Nothing Then
                dateCalendar_Renamed.Dispose()
                dateCalendar_Renamed = Nothing
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
            If dateFilterControl_Renamed IsNot Nothing Then
                dateFilterControl_Renamed.Dispose()
                dateFilterControl_Renamed = Nothing
            End If
            If item IsNot Nothing Then
                item.Dispose()
                item = Nothing
            End If
        End Sub
    End Class
End Namespace