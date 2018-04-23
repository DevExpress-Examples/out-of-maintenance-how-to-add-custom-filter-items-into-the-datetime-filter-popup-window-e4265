using System;
using System.Linq;
using DevExpress.XtraGrid.Columns;
using System.Collections.Generic;
using DevExpress.Utils.Controls;

namespace DateRange {
    public class MyOptionsColumnFilter : OptionsColumnFilter {
        protected internal bool UseFilterPopupRangeDateMode = false;
        FilterPopupModeExtended filterPopupMode;

        public MyOptionsColumnFilter(GridColumn column) : base(column)
        {
        }

        public new FilterPopupModeExtended FilterPopupMode {
            get { return filterPopupMode; }
            set {
                if(FilterPopupMode == value) return;
                FilterPopupModeExtended prevValue = FilterPopupMode;
                filterPopupMode = value;
                OnChanged(new BaseOptionChangedEventArgs("FilterPopupMode", prevValue, FilterPopupMode));
            }
        }
    }
}
