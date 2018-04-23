using System;
using System.Linq;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.Utils.Serializing;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Data.Filtering;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;


namespace DateRange {
    [System.ComponentModel.DesignerCategory("")]
    public class MyGridView : GridView {
        public MyGridView() : this(null) { }
        public MyGridView(GridControl grid)
            : base(grid) {

        }
        protected override GridColumnCollection CreateColumnCollection() {
            return new MyGridColumnCollection(this);
        }
        [Browsable(false)]
        [XtraSerializableProperty(XtraSerializationVisibility.Collection, true, true, true, 0, XtraSerializationFlags.DefaultValue)]
        [XtraSerializablePropertyId(2)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public new MyGridColumnCollection Columns {
            get { return base.Columns as MyGridColumnCollection; }
        }

        protected override DateFilterPopup CreateDateFilterPopup(GridColumn column, System.Windows.Forms.Control ownerControl, object creator) {
            return new MyDateFilterPopup(this, column, ownerControl, creator);
        }

        enum some { Greater, DateRange }

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
    }
}