using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Helpers;
using System.Collections.Generic;
using DevExpress.XtraEditors;
using DevExpress.Data.Filtering;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Popup;


namespace DateRange {
    public class MyDateFilterPopup : DateFilterPopup {
        public MyDateFilterPopup(ColumnView view, GridColumn column, Control ownerControl, object creator)
            : base(view, column, ownerControl, creator) {

        }
        const int DateCalendarBorders = 10;
        int BetweenWidth;
        DateControlEx dateCalendar;
        DateControlEx DateCalendar1;
        DateControlEx DateCalendar2;
        CheckEdit Greater;
        CheckEdit Less;
        CheckEdit Between;

        DateControlEx DateCalendar {
            get {
                if(dateCalendar == null) SetDateCalendar(DateFilterControl);
                return dateCalendar;
            }
        }

        PopupOutlookDateFilterControl dateFilterControl;
        PopupOutlookDateFilterControl DateFilterControl {
            get {
                if(dateFilterControl == null) SetDateFilterControl(item);
                return dateFilterControl;
            }
        }




        #region Creators
        RepositoryItemPopupContainerEdit item;
        protected override RepositoryItemPopupBase CreateRepositoryItem() {
            item = base.CreateRepositoryItem() as RepositoryItemPopupContainerEdit;
            if(DateFilterControl.Controls.Count > 0) {
                DateCalendar1 = CreateCalendar(DateCalendar1, DateCalendar.SelectionStart, DateCalendar.Top, DateCalendar.Left);
                DateCalendar1.Visible = false;
                DateCalendar2 = CreateCalendar(DateCalendar2, DateCalendar.SelectionStart, DateCalendar1.Top, DateCalendar.Left + DateCalendar1.Width);
                DateCalendar2.Visible = false;

                Greater = GetCheckEditByName(Localizer.Active.GetLocalizedString(StringId.FilterClauseGreater));
                Less = GetCheckEditByName(Localizer.Active.GetLocalizedString(StringId.FilterClauseLess));
                Between = GetCheckEditByName(Localizer.Active.GetLocalizedString(StringId.FilterClauseBetween));

                Greater.CheckedChanged += CheckedChanged;
                Less.CheckedChanged += CheckedChanged;
                Between.CheckedChanged += CheckedChanged;
                foreach(Control ctrl in DateFilterControl.Controls) {

                    if(ctrl is CheckEdit)
                        if(NotOurControl(ctrl as CheckEdit))
                            (ctrl as CheckEdit).CheckedChanged += OriginalDateFilterPopup_CheckedChanged;
                }
            }
            return item;
        }


        private Control FindOwnerForm(Control Owner) {
            if(Owner.Parent is PopupContainerForm) { return Owner.Parent; }
            else return FindOwnerForm(Owner.Parent);
        }


        public Point OriginalLocation;
        protected virtual void CreateRepositoryItemForOurFilterCriteria(string Name) {
            DateCalendar.Visible = false;
            DateCalendar1.Visible = true;
            Point Location = FindOwnerForm(dateFilterControl).Location;
            OriginalLocation = Location;
            BetweenWidth = FindOwnerForm(DateFilterControl).Width;
            if(Name == Localizer.Active.GetLocalizedString(StringId.FilterClauseBetween)) {

                FindOwnerForm(DateFilterControl).Width = DateCalendar1.Width + DateCalendar2.Width + DateCalendarBorders * 4;
                DateCalendar2.Visible = true;
                if(!Screen.PrimaryScreen.Bounds.Contains(FindOwnerForm(DateFilterControl).Bounds))
                    Location.X -= FindOwnerForm(dateFilterControl).Width / 2 - DateCalendarBorders * 2;
                FindOwnerForm(dateFilterControl).Location = Location;
            }

        }

        private void ReturnOriginalView() {
            DateCalendar1.Visible = false;
            DateCalendar2.Visible = false;
            dateCalendar.Visible = true;
            FindOwnerForm(DateFilterControl).Location = OriginalLocation;
            if(BetweenWidth < DateFilterControl.Parent.Parent.Width)
                DateFilterControl.Parent.Parent.Width = BetweenWidth;

        }


        private void CalcControlsLocation(string Name) {
            CheckEdit FirstCheckEdit = DateFilterControl.Controls[0] as CheckEdit;
            int ChechEditControls = DateFilterControl.Controls.Count;
            int n = 0;
            for(int i = 0; i < ChechEditControls; i++) {
                Control ctrl = DateFilterControl.Controls[i];
                if(ctrl is CheckEdit) {
                    ctrl.Location = new Point(FirstCheckEdit.Location.X, FirstCheckEdit.Location.Y + FirstCheckEdit.Size.Height * (i - n));
                    if(ctrl.Text == Name) {
                        Point newLoc = ctrl.Location;
                        newLoc.Y += ctrl.Size.Height;
                        DateCalendar1.Location = newLoc;
                        newLoc.X += DateCalendar1.Width + DateCalendarBorders * 2;
                        DateCalendar2.Location = newLoc;
                        i = DateFilterControl.Controls.Count - 1;

                    }
                }
                else n++;

            }
            CreateRepositoryItemForOurFilterCriteria(Name);
        }
        DateControlEx CreateCalendar(DateControlEx calendar, DateTime dateTime, int top, int left) {
            calendar = new DateControlEx();
            calendar.DateTime = dateTime;
            calendar.Top = top;
            calendar.Left = left;
            calendar.EditDateModified += DateCalendar_SelectionChanged;
            DateFilterControl.Controls.Add(calendar);
            return calendar;
        }
        #endregion

        #region Handlers&HandlersHelpers




        void CheckedChanged(object sender, EventArgs e) {
            if((sender as CheckEdit).Checked) {
                UpdateOurControlCheckedState((sender as CheckEdit).Text);
                CalcControlsLocation((sender as CheckEdit).Text);
                this.View.ActiveFilterCriteria = GetFilterCriteriaByControlState();
            }
            else {
                if(DateCalendar1.Visible || DateCalendar2.Visible) {
                    ReturnOriginalView();
                    ReturnOriginalControlsLocation();
                }
            }

        }
        protected virtual void UpdateOurControlCheckedState(string Name) {
            if(Name == Localizer.Active.GetLocalizedString(StringId.FilterClauseBetween)) {
                Less.Checked = false;
                Greater.Checked = false;
            }
            if(Name == Localizer.Active.GetLocalizedString(StringId.FilterClauseGreater)) {
                Less.Checked = false;
                Between.Checked = false;
            }
            if(Name == Localizer.Active.GetLocalizedString(StringId.FilterClauseLess)) {
                Greater.Checked = false;
                Between.Checked = false;
            }
            foreach(Control ctrl in DateFilterControl.Controls) {
                if(ctrl is CheckEdit)
                    if(NotOurControl(ctrl as CheckEdit))
                        (ctrl as CheckEdit).Checked = false;
            }
        }

        void OriginalDateFilterPopup_CheckedChanged(object sender, EventArgs e) {
            if((sender as CheckEdit).Checked)
                if(NotOurControl(sender as CheckEdit)) {
                    Greater.Checked = false;
                    Less.Checked = false;
                    Between.Checked = false;
                }

        }
        private void ReturnOriginalControlsLocation() {
            for(int i = 0; i < DateFilterControl.Controls.Count - 1; i++) {
                Control ctrl = DateFilterControl.Controls[i + 1];
                Point NewLocation = DateFilterControl.Controls[i].Location;
                NewLocation.Y += DateFilterControl.Controls[i].Height;
                ctrl.Location = NewLocation;
            }
        }

        void DateCalendar_SelectionChanged(object sender, EventArgs e) {
            this.View.ActiveFilterCriteria = GetFilterCriteriaByControlState();
            if(!Greater.Checked && !Less.Checked & !Between.Checked)
                dateCalendar.DateTime = DateCalendar1.DateTime;
        }
        #endregion


        private CriteriaOperator GetFilterCriteriaByControlState() {
            if(Greater.Checked)
                return GetBinaryOperatorByName(Localizer.Active.GetLocalizedString(StringId.FilterClauseGreater));

            if(Less.Checked)
                return GetBinaryOperatorByName(Localizer.Active.GetLocalizedString(StringId.FilterClauseLess));
            if(Between.Checked) {
                return GetBetweenOperatorByName(Localizer.Active.GetLocalizedString(StringId.FilterClauseBetween));
            }
            return null;
        }
        protected new MyGridView View {
            get { return base.View as MyGridView; }
        }

        #region ControlHelpers

        public CheckEdit GetCheckEditByName(String Name) {

            foreach(Control ctrl in DateFilterControl.Controls) {
                if(ctrl.Text == Name)
                    return ctrl as CheckEdit;

            }
            return null;

        }

        private void SetDateFilterControl(RepositoryItemPopupContainerEdit item) {
            foreach(Control ctrl in item.PopupControl.Controls)
                if(ctrl is PopupOutlookDateFilterControl) {
                    dateFilterControl = ctrl as PopupOutlookDateFilterControl;
                    break;
                }
        }

        private void SetDateCalendar(PopupOutlookDateFilterControl dateFilterControl) {
            foreach(Control c in dateFilterControl.Controls)
                if(c is DateControlEx) {
                    dateCalendar = c as DateControlEx;
                    break;
                }
        }

        protected virtual BinaryOperator GetBinaryOperatorByName(string Name) {
            if(DateCalendar1 != null) {
                if(Name == Localizer.Active.GetLocalizedString(StringId.FilterClauseGreater)) { return new BinaryOperator(this.Column.FieldName, DateCalendar1.DateTime, BinaryOperatorType.Greater); }
                if(Name == Localizer.Active.GetLocalizedString(StringId.FilterClauseLess)) { return new BinaryOperator(this.Column.FieldName, DateCalendar1.DateTime, BinaryOperatorType.Less); }
            }
            return null;
        }
        protected virtual BetweenOperator GetBetweenOperatorByName(string Name) {
            if(DateCalendar2 != null)
                if(Name == Localizer.Active.GetLocalizedString(StringId.FilterClauseBetween)) { return new BetweenOperator(this.Column.FieldName, DateCalendar1.DateTime, DateCalendar2.DateTime); }
            return null;

        }
        #endregion

        protected virtual bool NotOurControl(CheckEdit ctrl) {
            if(ctrl.Text != Localizer.Active.GetLocalizedString(StringId.FilterClauseGreater)
                && ctrl.Text != Localizer.Active.GetLocalizedString(StringId.FilterClauseLess)
                   && ctrl.Text != Localizer.Active.GetLocalizedString(StringId.FilterClauseBetween))
                return true;

            return false;
        }

        public override void Dispose() {
            if(DateCalendar1 != null)
                DateCalendar1.EditDateModified -= DateCalendar_SelectionChanged;
            if(DateCalendar2 != null)
                DateCalendar2.EditDateModified -= DateCalendar_SelectionChanged;
            foreach(Control ctrl in DateFilterControl.Controls) {
                if(ctrl is CheckEdit)
                    if(NotOurControl(ctrl as CheckEdit))
                        (ctrl as CheckEdit).CheckedChanged -= OriginalDateFilterPopup_CheckedChanged;
            }
            try {
                this.View.ActiveFilterString = GetFilterCriteriaByControlState().ToString();
            }
            catch {
            }
            DateCalendar1.Dispose();
            DateCalendar2.Dispose();
            base.Dispose();
            if(dateCalendar != null) {
                dateCalendar.Dispose();
                dateCalendar = null;
            }
            if(Greater != null) {
                Greater.Dispose();
                Greater = null;
            }
            if(Less != null) {
                Less.Dispose();
                Less = null;
            }
            if(Between != null) {
                Between.Dispose();
                Between = null;
            }
            if(dateFilterControl != null) {
                dateFilterControl.Dispose();
                dateFilterControl = null;
            }
            if(item != null) {
                item.Dispose();
                item = null;
            }
        }
    }
}