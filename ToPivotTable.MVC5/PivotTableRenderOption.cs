using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace qyen.Pivot.Mvc5{
    public enum HeaderType { Row, Column }
    public enum MeasureArrangementType { Horisontal, Vertical }
    public enum OutputPosition { Above, Below }
    public class PivotTableRenderOption<T> {
        public MeasureArrangementType MeasureArrangement { get; set; } = MeasureArrangementType.Horisontal;
        public string TableCssClass { get; set; } = "pivot";
        public string HeaderRowCssClass { get; set; } = "headerRow";
        public string RowCssClass { get; set; } = "dataRow";
        public string MeasureTitleCssClass { get; set; } = "measureTitle";
        public bool RenderHeaderTitles { get; set; } = true;

        public Dictionary<HeaderType, PivotAxisRenderOption> Header { get; } = new Dictionary<HeaderType, PivotAxisRenderOption>() {
            { HeaderType.Row , new PivotAxisRenderOption() {} },
            { HeaderType.Column , new PivotAxisRenderOption() {} },
        };

        private Dictionary<PivotMeasure<T>, Func<PivotTable<T>, PivotHeaderCell<T>, PivotHeaderCell<T>, PivotMeasure<T>, string>> measureFormatterDictionary
            = new Dictionary<PivotMeasure<T>, Func<PivotTable<T>, PivotHeaderCell<T>, PivotHeaderCell<T>, PivotMeasure<T>, string>>();
        private static Func<PivotTable<T>, PivotHeaderCell<T>, PivotHeaderCell<T>, PivotMeasure<T>, string> defaultCellRender
            = (pivot, row, col, measure) => $"<td class=\"number\">{string.Format("{0:#,0}", (pivot[row, col, measure]))}</td>";
        public PivotTableRenderOption<T> SetMeasureFormatter(PivotMeasure<T> measure, Func<PivotTable<T>, PivotHeaderCell<T>, PivotHeaderCell<T>, PivotMeasure<T>, string> formatFunction) {
            if (measureFormatterDictionary.ContainsKey(measure)) {
                measureFormatterDictionary[measure] = formatFunction;
            } else {
                measureFormatterDictionary.Add(measure, formatFunction);
            }
            return this;
        }
        internal string RenderMeasureCell(PivotTable<T> pivot, PivotHeaderCell<T> row, PivotHeaderCell<T> col, PivotMeasure<T> measure) {
            if (measureFormatterDictionary.ContainsKey(measure)) {
                return measureFormatterDictionary[measure](pivot, row, col, measure);
            } else {
                return defaultCellRender(pivot, row, col, measure);
            }
        }
        public PivotAxisHeaderRenderOptions<T> HeaderCellOption { get; private set; } = new PivotAxisHeaderRenderOptions<T>();
    }

    public class PivotAxisRenderOption {
        public bool RenderTotal { get; set; } = false;
        public string TotalCssClass { get; set; } = Properties.Resources.GrandTotalCellTitle;
        public string TotalTitle { get; set; } = Properties.Resources.GrandTotalCellTitle;
        public OutputPosition TotalPosition { get; set; } = OutputPosition.Below;
    }
    public class PivotAxisHeaderRenderOptions<T> {
        private Dictionary<PivotColumn<T>, PivotAxisRenderOption> options = new Dictionary<PivotColumn<T>, PivotAxisRenderOption>();
#pragma warning disable CA1043 // インデクサーには整数または文字列引数を使用します
        public PivotAxisRenderOption this[PivotColumn<T> index] {
#pragma warning restore CA1043 
            get {
                if (!options.ContainsKey(index)) {
                    options.Add(index, new PivotAxisRenderOption() {
                        TotalCssClass = Properties.Resources.TotalCellTitle,
                        TotalTitle = Properties.Resources.TotalCellTitle,
                    });
                }
                return options[index];
            }
        }
    }

}