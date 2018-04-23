// Developer Express Code Central Example:
// How to add custom filter items into the DateTime filter popup window
// 
// This example demonstrates how to add the capability to enter "greater than",
// "less than", and "between" directly from the calendar popup filter list.
// 
// To
// provide this functionality, it is necessary to create a GridControl descendant
// with a custom MyGridView.
// 
// First, override the
// MyGridView.CreateDateFilterPopup and MyGridView.RaiseFilterPopupDate
// methods.
// 
// protected override DateFilterPopup CreateDateFilterPopup(GridColumn
// column, System.Windows.Forms.Control ownerControl, object creator)
// {  return
// new MyDateFilterPopup(this, column, ownerControl, creator);
// }
// 
// 
// In the
// MyGridView.RaiseFilterPopupDate method add required FilterCriteria.    protected
// override void RaiseFilterPopupDate(DateFilterPopup filterPopup,
// List<DevExpress.XtraEditors.FilterDateElement> list)    {      CriteriaOperator
// filter = new BinaryOperator(FocusedColumn.FieldName, DateTime.Today,
// BinaryOperatorType.Greater);      list.Add(new
// FilterDateElement(Localizer.Active.GetLocalizedString(StringId.FilterClauseGreater)
// ,"", filter));      filter = new BinaryOperator(FocusedColumn.FieldName,
// DateTime.Today, BinaryOperatorType.Less);      list.Add(new
// FilterDateElement(Localizer.Active.GetLocalizedString(StringId.FilterClauseLess)
// ,"", filter));      filter = new BetweenOperator(FocusedColumn.FieldName,
// DateTime.Today, DateTime.Today);      list.Add(new
// FilterDateElement(Localizer.Active.GetLocalizedString(StringId.FilterClauseBetween)
// ,"", filter));      base.RaiseFilterPopupDate(filterPopup, list);
// }
// 
// 
// Now, the CreateDateFilterPopup method returns the MyDateFilterPopup
// object instead of the DateFilterPopup one. Override the
// Popup.CreateRepositoryItem method to create additional controls and add handlers
// for them:    protected override RepositoryItemPopupBase CreateRepositoryItem()
// {      item = base.CreateRepositoryItem() as RepositoryItemPopupContainerEdit;
// if (DateFilterControl.Controls.Count > 0)      {        DateCalendar1 =
// CreateCalendar(DateCalendar1, DateCalendar.SelectionStart, DateCalendar.Top,
// DateCalendar.Left);        DateCalendar1.Visible = false;        DateCalendar2 =
// CreateCalendar(DateCalendar2, DateCalendar.SelectionStart, DateCalendar1.Top,
// DateCalendar.Left + DateCalendar1.Width);        DateCalendar2.Visible = false;
// Greater =
// GetCheckEditByName(Localizer.Active.GetLocalizedString(StringId.FilterClauseGreater));
// Less =
// GetCheckEditByName(Localizer.Active.GetLocalizedString(StringId.FilterClauseLess));
// Between =
// GetCheckEditByName(Localizer.Active.GetLocalizedString(StringId.FilterClauseBetween));
// Greater.CheckedChanged += CheckedChanged;        Less.CheckedChanged +=
// CheckedChanged;        Between.CheckedChanged += CheckedChanged;        foreach
// (Control ctrl in DateFilterControl.Controls)        {          if (ctrl is
// CheckEdit)            if (NotOurControl(ctrl as CheckEdit))              (ctrl
// as CheckEdit).CheckedChanged += OriginalDateFilterPopup_CheckedChanged;        }
// }      return item;    }
// 
// 
// 
// When a custom item's check state changes,
// update the popup window layout to display the calendar at a required place.
// void CheckedChanged(object sender, EventArgs e)    {      if ((sender as
// CheckEdit).Checked)      {        UpdateOurControlCheckedState((sender as
// CheckEdit).Text);        CalcControlsLocation((sender as CheckEdit).Text);
// }      else      {        if (DateCalendar1.Visible || DateCalendar2.Visible)
// {          ReturnOriginalView();          ReturnOriginalControlsLocation();
// }      }    }
// 
// You can find sample updates and versions for different programming languages here:
// http://www.devexpress.com/example=E4265

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
            myGridView1.Columns["Range Date"].OptionsFilter.FilterPopupMode = FilterPopupModeExtended.Date;
            myGridView1.Columns["Range Date"].OptionsFilter.UseFilterPopupRangeDateMode = true;
            myGridView1.OptionsFilter.ColumnFilterPopupMode = DevExpress.XtraGrid.Columns.ColumnFilterPopupMode.Classic;
        }
    }
}
