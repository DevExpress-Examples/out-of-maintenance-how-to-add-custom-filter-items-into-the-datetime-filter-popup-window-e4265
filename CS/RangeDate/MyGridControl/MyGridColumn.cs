using System;
using System.Linq;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using System.Collections.Generic;
using DevExpress.XtraGrid.Views.Base;

namespace DateRange {
    [System.ComponentModel.DesignerCategory("")]
    public class MyGridColumn : GridColumn {
        public new MyOptionsColumnFilter OptionsFilter { get { return base.OptionsFilter as MyOptionsColumnFilter; } }
        protected override OptionsColumnFilter CreateOptionsFilter() { return new MyOptionsColumnFilter(this); }
        protected override FilterPopupMode GetFilterPopupMode() {
            FilterPopupModeExtended modeExtended = OptionsFilter.FilterPopupMode;
            FilterPopupMode mode = FilterPopupMode.List;
            if(modeExtended == FilterPopupModeExtended.Default
                && (ColumnType.Equals(typeof(DateTime)) || ColumnType.Equals(typeof(DateTime?)))
                && FilterMode != ColumnFilterMode.DisplayText) modeExtended = FilterPopupModeExtended.DateSmart;
            if(modeExtended == FilterPopupModeExtended.Default) modeExtended = FilterPopupModeExtended.List;
            OptionsFilter.UseFilterPopupRangeDateMode = false;
            switch(modeExtended.ToString()) {
                case "Default": { mode = FilterPopupMode.Default; break; }
                case "List": { mode = FilterPopupMode.List; break; }
                case "CheckedList": { mode = FilterPopupMode.CheckedList; break; }
                case "Date": { mode = FilterPopupMode.Date; break; }
                case "DateSmart": { mode = FilterPopupMode.DateSmart; break; }
                case "DateAlt": { mode = FilterPopupMode.DateAlt; break; }
                case "DateRange": { mode = FilterPopupMode.Date; OptionsFilter.UseFilterPopupRangeDateMode = true; break; }
                default:
                    break;
            }
            return mode;
        }
    }

    public enum FilterPopupModeExtended { Default, List, CheckedList, Date, DateSmart, DateAlt, DateRange }

    public class MyGridColumnCollection : GridColumnCollection {
        public MyGridColumnCollection(ColumnView view) : base(view) { }
        protected override GridColumn CreateColumn() {
            return new MyGridColumn();
        }
        public new MyGridColumn this[string fieldName] { get { return ColumnByFieldName(fieldName) as MyGridColumn; } }
        public new MyGridColumn this[int index] { get { return (MyGridColumn)List[index]; } }
    }
}
