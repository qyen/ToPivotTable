using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace qyen.Pivot {
    public class PivotTable<T> {
        private IList<PivotColumn<T>> __RowHeaders;
        private IList<PivotColumn<T>> __ColHeaders;
        private IList<PivotMeasure<T>> __Measures;
        private IEnumerable<T> __source;
        public IEnumerable<PivotColumn<T>> RowHeaders => __RowHeaders.ToList();
        public IEnumerable<PivotColumn<T>> ColHeaders => __ColHeaders.ToList();
        /// <summary>
        /// Enumerate All(Column and Row) Headers
        /// </summary>
        public IEnumerable<PivotColumn<T>> AllHeaders {
            get {
                foreach (var c in __RowHeaders)
                    yield return c;
                foreach (var c in __ColHeaders)
                    yield return c;
            }
        }
        public PivotColumn<T> ColumnByName(string name) => AllHeaders.FirstOrDefault(c => c.PropertyName == name);
        public IEnumerable<PivotMeasure<T>> Measures => __Measures.ToList();

        internal PivotTable(IEnumerable<T> source, IList<PivotColumn<T>> rowHeaders, IList<PivotColumn<T>> colHeaders, IList<PivotMeasure<T>> measures) {
            __RowHeaders = rowHeaders.ToList();
            __ColHeaders = colHeaders.ToList();
            __Measures = measures.ToList();
            __source = source;
            BuildHeader(source);
        }
        private PivotTable() { }

        public PivotTable<T> flip() {
            return new PivotTable<T>() {
                __RowHeaders = this.__ColHeaders,
                __ColHeaders = this.__RowHeaders,
                __Measures = this.__Measures,
                __source = this.__source,
            };
        }

        private void BuildHeader(IEnumerable<T> source) {
            BuildColumnExpressions(__RowHeaders);
            BuildColumnExpressions(__ColHeaders);
            BuildMeasureExpressions();
            BuildHeaderCells(source, __RowHeaders);
            BuildHeaderCells(source, __ColHeaders);
        }
        /// <summary>
        /// build headers expressions
        /// </summary>
        /// <param name="columns"></param>
        private void BuildColumnExpressions(IList<PivotColumn<T>> columns) {
            var EntityType = typeof(T);
            var listParameterExpression = Expression.Parameter(EntityType, "obj");
            PivotColumn<T> previousColumn = null;
            foreach (var column in columns) {
                if (previousColumn != null) {
                    previousColumn.LowerColumn = column;
                    column.HigherColumn = previousColumn;
                }
                previousColumn = column;
                if (column.ValueGetter == null) {
                    var propertyExpression = Expression.Property(listParameterExpression, column.PropertyName);
                    column.ValueGetter = Expression.Lambda<Func<T, dynamic>>(
                        Expression.Convert(propertyExpression, typeof(object)),
                        listParameterExpression).Compile();
                }
                if (column.TitleGetter == null) {
                    column.TitleGetter = (obj) => Convert.ToString(column.ValueGetter(obj));
                }
            }
        }
        /// <summary>
        /// build header cell tree
        /// </summary>
        /// <param name="source"></param>
        /// <param name="columns">Row or Column Header List</param>
        private void BuildHeaderCells(IEnumerable<T> source, IList<PivotColumn<T>> columns) {
            BuildHeaderCells(source, columns.First());
        }

        /// <summary>
        /// Build Header Cell recursively
        /// </summary>
        /// <param name="source"></param>
        /// <param name="column"></param>
        /// <param name="parent"></param>
        private void BuildHeaderCells(IEnumerable<T> source, PivotColumn<T> column, PivotHeaderCell<T> parent = null) {
            var list = column.Order == PivotOrder.Ascending
                            ? source.GroupBy(column.KeySelector).OrderBy(grp => grp.Key)
                            : source.GroupBy(column.KeySelector).OrderByDescending(grp => grp.Key);
            foreach (var group in list) {
                var cell = new PivotHeaderCell<T>(group.First(), column);
                column.Cells.Add(cell);
                if (parent != null)
                    parent.__Children.Add(cell);
                cell.Parent = parent;
                if (column.LowerColumn != null) {
                    BuildHeaderCells(group, column.LowerColumn, cell);
                }
            }
        }
        /// <summary>
        /// build Measure Getter Expression
        /// </summary>
        /// <param name="columns"></param>
        private void BuildMeasureExpressions() {
            var EntityType = typeof(T);
            var listParameterExpression = Expression.Parameter(EntityType, "obj");
            var ConvertToDecimalMethod = typeof(Convert).GetMethod("ToDecimal", new Type[] { typeof(object) });
            foreach (var measure in __Measures) {
                if (measure.ValueGetter == null) {
                    var propertyExpression = Expression.Property(listParameterExpression, measure.PropertyName);
                    measure.ValueGetter = Expression.Lambda<Func<T, decimal>>(
                        Expression.Call(null, ConvertToDecimalMethod, Expression.Convert(propertyExpression, typeof(object))),
                        listParameterExpression).Compile();
                }
            }
        }

        /// <summary>
        /// Get Measure Value
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="measure"></param>
        /// <returns></returns>
        public decimal this[PivotHeaderCell<T> row, PivotHeaderCell<T> col, PivotMeasure<T> measure] {
            get {
                return GetValue(row, col, measure);
            }
        }
        /// <summary>
        /// Get Measure Value
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="measure"></param>
        /// <returns></returns>
        private decimal ComplexGetValue(PivotHeaderCell<T> row, PivotHeaderCell<T> col, ComplexPivotMeasure<T> measure) {
            var values = new List<decimal>();
            foreach (var ms in measure.measures) {
                values.Add(GetValue(row, col, ms));
            }
            return measure.values_aggregate(values.ToArray());
        }

        /// <summary>
        /// Get Measure Value
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="measure"></param>
        /// <returns></returns>
        public decimal GetValue(PivotHeaderCell<T> row, PivotHeaderCell<T> col, PivotMeasure<T> measure) {
            if (measure is ComplexPivotMeasure<T>) {
                return ComplexGetValue(row, col, measure as ComplexPivotMeasure<T>);
            }
            var query = __source.AsQueryable();
            var paramCell = new List<PivotHeaderCell<T>>();
            if (row != null) {
                var current = row;
                while (current != null) {
                    paramCell.Add(current);
                    current = current.Parent;
                }
            }
            if (col != null) {
                var current = col;
                while (current != null) {
                    paramCell.Add(current);
                    current = current.Parent;
                }
            }
            foreach (var item in paramCell) {
                query = query.Where(t => item.Column.EqualsExpression(t, item.LeadObject));
            }
            return measure.aggregate(query.Select(t => measure.ValueGetter(t)));
        }
        public bool EqualsExpression(T obj, PivotHeaderCell<T> current) {
            var col = current.Column;
            var param = current.LeadObject;
            return col.ValueGetter(obj) == col.ValueGetter(param);
        }

    }
}
