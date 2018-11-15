<!-- default file list -->
*Files to look at*:

* [Form1.cs](./CS/RangeDate/Form1.cs) (VB: [Form1.vb](./VB/RangeDate/Form1.vb))
* [MyDateFilterPopup.cs](./CS/RangeDate/MyGridControl/MyDateFilterPopup.cs) (VB: [MyDateFilterPopup.vb](./VB/RangeDate/MyGridControl/MyDateFilterPopup.vb))
* [MyGridColumn.cs](./CS/RangeDate/MyGridControl/MyGridColumn.cs) (VB: [MyGridColumn.vb](./VB/RangeDate/MyGridControl/MyGridColumn.vb))
* [MyGridControl.cs](./CS/RangeDate/MyGridControl/MyGridControl.cs) (VB: [MyGridControl.vb](./VB/RangeDate/MyGridControl/MyGridControl.vb))
* [MyGridView.cs](./CS/RangeDate/MyGridControl/MyGridView.cs) (VB: [MyGridView.vb](./VB/RangeDate/MyGridControl/MyGridView.vb))
* [MyOptionsColumnFilter.cs](./CS/RangeDate/MyGridControl/MyOptionsColumnFilter.cs) (VB: [MyOptionsColumnFilter.vb](./VB/RangeDate/MyGridControl/MyOptionsColumnFilter.vb))
* [Program.cs](./CS/RangeDate/Program.cs) (VB: [Program.vb](./VB/RangeDate/Program.vb))
<!-- default file list end -->
# How to add custom filter items into the DateTime filter popup window 


<p>In the latest versions, you can use the <a href="https://documentation.devexpress.com/WindowsForms/DevExpressXtraGridViewsBaseColumnView_ShowFilterPopupDatetopic.aspx">ShowFilterPopupDate</a> event to add a custom filter item to the filter dropdown. See the <a href="https://documentation.devexpress.com/WindowsForms/DevExpressXtraGridViewsBaseColumnView_ShowFilterPopupDatetopic.aspx">ShowFilterPopupDate</a> topic for additional information and code snippet.</p>
<p><br>------------------------------------------------------------------------<br>This example demonstrates how to add the capability to enter "greater than", "less than", and "between" directly from the calendar popup filter list.</p>
<p>To provide this functionality, it is necessary to create a GridControl descendant with a custom MyGridView.</p>
<p>First, override the <strong>MyGridView.CreateDateFilterPopup</strong> and<strong> MyGridView.RaiseFilterPopupDate</strong> methods.</p>


```cs
protected override DateFilterPopup CreateDateFilterPopup(GridColumn column, System.Windows.Forms.Control ownerControl, object creator)
{
   return new MyDateFilterPopup(this, column, ownerControl, creator);
}



```


<p>In the <strong>MyGridView.RaiseFilterPopupDate</strong> method add required FilterCriteria.</p>


```cs
protected override void RaiseFilterPopupDate(DateFilterPopup filterPopup, List<FilterDateElement> list) {
    CriteriaOperator filter = new BinaryOperator(filterPopup.Column.FieldName, DateTime.Today, BinaryOperatorType.Greater);

    list.Add(new FilterDateElement(Localizer.Active.GetLocalizedString(StringId.FilterClauseGreater)
        , "", filter));
    filter = new BinaryOperator(filterPopup.Column.FieldName, DateTime.Today, BinaryOperatorType.Less);
    list.Add(new FilterDateElement(Localizer.Active.GetLocalizedString(StringId.FilterClauseLess)
        , "", filter));
    filter = new BetweenOperator(filterPopup.Column.FieldName, DateTime.Today, DateTime.Today);
    list.Add(new FilterDateElement(Localizer.Active.GetLocalizedString(StringId.FilterClauseBetween), "", filter));
    base.RaiseFilterPopupDate(filterPopup, list);
}
```


<p>Now, the <strong>CreateDateFilterPopu</strong>p method returns the MyDateFilterPopup object instead of the DateFilterPopup one.</p>
<p>Override the<strong> Popup.CreateRepositoryItem</strong> method to create additional controls and add handlers for them:</p>


```cs
        protected override RepositoryItemPopupBase CreateRepositoryItem()
        {
            item = base.CreateRepositoryItem() as RepositoryItemPopupContainerEdit;
            if (DateFilterControl.Controls.Count > 0)
            {
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
                foreach (Control ctrl in DateFilterControl.Controls)
                {

                    if (ctrl is CheckEdit)
                        if (NotOurControl(ctrl as CheckEdit))
                            (ctrl as CheckEdit).CheckedChanged += OriginalDateFilterPopup_CheckedChanged;
                }
            }
            return item;
        }



```


<p>When a custom item's check state changes, update the popup window layout to display the calendar at a required place.</p>


```cs
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
```


<p> </p>

<br/>


