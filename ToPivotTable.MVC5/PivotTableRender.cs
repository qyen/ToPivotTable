using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace qyen.Pivot.Mvc5 {
    public static class PivotTableRender {
        public static MvcHtmlString Run<TModel>(PivotTable<TModel> pivotTable, PivotTableRenderOption<TModel> option) {
            Contract.Requires(pivotTable != null);
            Contract.Requires(option != null);

            return MvcHtmlString.Create(new PivotTableRender<TModel>(pivotTable, option).Run());
        }
    }
    public class PivotTableRender<T> {
        private PivotTable<T> Table { get; set; }
        private PivotTableRenderOption<T> Option { get; set; }
        private bool IsVertical => Option.MeasureArrangement == MeasureArrangementType.Vertical;

        /// <summary>
        /// Header Row Rendering Count
        /// </summary>
        private int ColHeaderRowCount => RowRender.MaxDepth + (IsVertical ? 0 : 1);

        /// <summary>
        /// Header Col Rendering Count
        /// </summary>
        private int RowHeaderColCount => ColRender.MaxDepth + (IsVertical ? 1 : 0);

        private int HorisontalMeasureRatio => IsVertical ? 1 : Table.Measures.Count();
        private int VerticalMeasureRatio => IsVertical ? Table.Measures.Count() : 1;

        /// <summary>
        /// Header Row Rendering Engine
        /// </summary>
        private PivotTableHeaderRender<T> RowRender { get; set; }
        /// <summary>
        /// Header Col Rendering Engine
        /// </summary>
        private PivotTableHeaderRender<T> ColRender { get; set; }
        internal PivotTableRender(PivotTable<T> pivotTable, PivotTableRenderOption<T> option) {
            Table = pivotTable;
            Option = option;
            RowRender = new PivotTableHeaderRender<T>(HeaderType.Row, pivotTable.ColHeaders, option);
            ColRender = new PivotTableHeaderRender<T>(HeaderType.Column, pivotTable.RowHeaders, option);


        }
        public string Run() {
            return RenderTable(RenderHeaderRows() + RenderRows());
        }
        /// <summary>
        /// Rendering Table
        /// </summary>
        /// <param name="Contents"></param>
        /// <returns></returns>
        private string RenderTable(string Contents) {
            return $"<div class='{Option.TableCssClass}-host'>" +
                $"<table class=\"{Option.TableCssClass}\" " +
                $"data-hRows=\"{ColHeaderRowCount}\"" +
                $"data-hCols=\"{RowHeaderColCount}\">" +
                $"{Contents}</table></div>";
        }
        /// <summary>
        /// Rendering Header Rows
        /// </summary>
        /// <returns>html string (thead>tr>th)</returns>
        private string RenderHeaderRows() {
            var sb = new StringBuilder();
            var cornerRows = ColHeaderRowCount - (Option.RenderHeaderTitles ? 1 : 0);
            var cornerColumns = RowHeaderColCount - (Option.RenderHeaderTitles ? 1 : 0);

            for (var i = 0; i < RowRender.MaxDepth; i++) {
                var row = new StringBuilder();
                if (i == 0) {
                    row.Append($"<th class=\"ch-0 rh-0 header cornerHeader\" colspan=\"{cornerColumns}\" rowspan=\"{cornerRows}\"></th>");
                }
                if (Option.RenderHeaderTitles) {
                    row.Append($"<th class=\"coltitle rh-{i} ch-{cornerColumns}\">{RowRender.HeaderTitle(i)}</th>");
                }
                foreach (var cell in RowRender[i]) {
                    var leafCount = cell.Leaf.Count() * HorisontalMeasureRatio;
                    var isTotal = (cell is PivotTableTotalColumnRender<T>);
                    var rowSpan = (isTotal ? RowRender.MaxDepth - cell.depth : 1);
                    var cellClass = $"{cell.CssClass} rh-{i} " + (isTotal ? "total" : "");
                    row.Append($"<th class=\"{cellClass}\" colspan=\"{leafCount}\" rowspan=\"{rowSpan}\">{cell.Title}</th>");
                }

                sb.Append($"<tr class=\"headerRow\">{row}</tr>");
            }
            if (!IsVertical) {
                var row = new StringBuilder();
                if (Option.RenderHeaderTitles) {
                    for (int i = 0; i < ColRender.MaxDepth; i++) {
                        row.Append($"<th class=\"rowtitle rh-{cornerRows} ch-{i}\">{ColRender.HeaderTitle(i)}</th>");
                    }
                }
                foreach (var cell in RowRender.Leaves) {
                    foreach (var measure in Table.Measures) {
                        row.Append(RenderMeasureTitleCell(measure));
                    }
                }

                sb.Append($"<tr class=\"headerRow\">{row}</tr>");
            }
            return $"<thead class='heaerRows'>{sb.ToString()}</thead>";
        }
        private string RenderMeasureTitleCell(PivotMeasure<T> measure) {
            return $"<th class=\"{Option.MeasureTitleCssClass}\">{ measure.PropertyName}</th>";
        }
        private string RenderRows() {
            var sb = new StringBuilder();
            foreach (var leaf in ColRender.Leaves) {
                var row = new StringBuilder();
                var path = leaf.Path;
                var newLeaf = path.LastOrDefault(p => !p.IsFirstChild);
                bool levelFound = (newLeaf == null);
                row.Append($"<tr class=\"{Option.RowCssClass}\">");
                foreach (var cell in path) {
                    if (!levelFound && cell != newLeaf)
                        continue;
                    levelFound = true;

                    var rowSpan = cell.Leaf.Count() * VerticalMeasureRatio;
                    var isTotal = (cell is PivotTableTotalColumnRender<T>);
                    var colSpan = (isTotal ? ColRender.MaxDepth - cell.depth : 1);
                    row.Append($"<th class=\"{cell.CssClass} ch-{cell.depth}\" colspan=\"{colSpan}\" rowspan=\"{rowSpan}\">{cell.Title}</th>");
                }

                if (IsVertical) {
                    foreach (var measure in Table.Measures) {
                        row.Append(RenderMeasureTitleCell(measure));

                        foreach (var colHeaderCell in RowRender.Leaves) {
                            row.Append(Option.RenderMeasureCell(Table, leaf.Cell, colHeaderCell.Cell, measure));
                        }
                        if (measure != Table.Measures.Last()) {
                            row.Append($"</tr><tr class=\"{Option.RowCssClass}\">");
                        }
                    }

                } else {
                    foreach (var colHeaderCell in RowRender.Leaves) {
                        foreach (var measure in Table.Measures) {
                            row.Append(Option.RenderMeasureCell(Table, leaf.Cell, colHeaderCell.Cell, measure));
                        }
                    }
                }

                row.Append($"</tr>");
                sb.Append(row);
            }
            return sb.ToString();
        }


    }
}
