using System.Collections.Generic;

namespace qyen.Pivot {
    public static class ListExtension {
        /// <summary>
        /// build pivot table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="rowHeaders"></param>
        /// <param name="colHeaders"></param>
        /// <param name="measures"></param>
        /// <returns></returns>
        public static PivotTable<T> ToPivotTable<T>(this IEnumerable<T> source, IList<PivotColumn<T>> rowHeaders, IList<PivotColumn<T>> colHeaders, IList<PivotMeasure<T>> measures) {
            return new PivotTable<T>(source, rowHeaders, colHeaders, measures);
        }
    }
    public enum PivotOrder { Ascending,Descending}
}
