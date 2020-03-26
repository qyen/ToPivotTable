using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qyen.Pivot.Mvc5 {
    public class PivotTableHeaderRender<T> {
        private List<PivotTableColumnRender<T>> rootCells = new List<PivotTableColumnRender<T>>();
        private List<PivotColumn<T>> Headers { get; set; }
        public int MaxDepth => Headers.Count;
        public string HeaderTitle(int depth) => Headers[depth].PropertyName;

        public PivotTableHeaderRender(HeaderType headerType, IEnumerable<PivotColumn<T>> headers, PivotTableRenderOption<T> option) {
            Contract.Requires(headers != null);
            Contract.Requires(option != null);
            Headers = headers.ToList();

            var topLevelHeader = headers.First();
            var headerOption = option.Header[headerType];
            if (headerOption.RenderTotal && headerOption.TotalPosition == OutputPosition.Above) {
                rootCells.Add(new PivotTableTotalColumnRender<T>(null, option, headerOption));
            }
            foreach (var cell in topLevelHeader.Items) {
                rootCells.Add(new PivotTableColumnRender<T>(cell, option, headerOption));
            }
            if (headerOption.RenderTotal && headerOption.TotalPosition == OutputPosition.Below) {
                rootCells.Add(new PivotTableTotalColumnRender<T>(null, option, headerOption));
            }
        }
        public IEnumerable<PivotTableColumnRender<T>> this[int depth] {
            get {
                foreach (var cell in rootCells) {
                    foreach (var c in cell.ListByDepth(depth)) {
                        yield return c;
                    }
                }
            }
        }
        public IEnumerable<PivotTableColumnRender<T>> Leaves {
            get {
                foreach (var cell in rootCells) {
                    foreach (var c in cell.Leaf) {
                        yield return c;
                    }
                }

            }
        }
    }
}
