using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qyen.Pivot.Mvc5 {
    public class PivotTableColumnRender<T> {
        public PivotHeaderCell<T> Cell { get; private set; }
        internal PivotTableColumnRender<T> Parent { get; private set; }
        internal int depth { get; set; }

        public string Title { get; protected set; }
        public string CssClass { get; protected set; } = "cell";

        internal List<PivotTableColumnRender<T>> Children { get; private set; } = new List<PivotTableColumnRender<T>>();
        public PivotTableColumnRender(PivotHeaderCell<T> current, PivotTableRenderOption<T> option, PivotAxisRenderOption headerOption, PivotTableColumnRender<T> parent = null) {
            Contract.Requires(option != null);
            Contract.Requires(headerOption != null);

            Cell = current;
            Parent = parent;
            Title = Cell == null ? headerOption.TotalTitle : Cell.Title;
            depth = Parent == null ? 0 : Parent.depth + 1;

            if (Cell == null)
                return;
            // if total cell , no child
            if (this is PivotTableTotalColumnRender<T>)
                return;

            var currentLevelOption = option.HeaderCellOption[Cell.Column];
            if (currentLevelOption.RenderTotal && currentLevelOption.TotalPosition == OutputPosition.Above) {
                Children.Add(new PivotTableTotalColumnRender<T>(Cell, option, currentLevelOption, this));
            }
            foreach (var child in Cell.Children) {
                Children.Add(new PivotTableColumnRender<T>(child, option, currentLevelOption, this));
            }
            if (currentLevelOption.RenderTotal && currentLevelOption.TotalPosition == OutputPosition.Below) {
                Children.Add(new PivotTableTotalColumnRender<T>(Cell, option, currentLevelOption, this));
            }
        }
        public IEnumerable<PivotTableColumnRender<T>> ListByDepth(int depth) {
            if (this.depth > depth) {
                yield break;
            } else if (depth == this.depth) {
                yield return this;
            } else {
                foreach (var child in Children) {
                    foreach (var c in child.ListByDepth(depth)) {
                        yield return c;
                    }
                }
            }
        }
        public IEnumerable<PivotTableColumnRender<T>> Leaf {
            get {
                if (Children.Any()) {
                    foreach (var child in Children) {
                        foreach (var c in child.Leaf) {
                            yield return c;
                        }
                    }
                } else {
                    yield return this;
                }
            }
        }
        /// <summary>
        /// Path of tree
        /// </summary>
        public IEnumerable<PivotTableColumnRender<T>> Path {
            get {
                var path = new List<PivotTableColumnRender<T>>();
                var current = this;
                while (current != null) {
                    path.Add(current);
                    current = current.Parent;
                }
                path.Reverse();
                return path;
            }
        }
        /// <summary>
        /// Is First Child Cell in Parent
        /// </summary>
        public bool IsFirstChild => Parent == null ? true : Parent.Children.First() == this;
        /// <summary>
        /// Is Last Child Cell in Parent
        /// </summary>
        public bool IsLastChild => Parent == null ? true : Parent.Children.Last() == this;

    }
}
