using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qyen.Pivot {
    /// <summary>
    /// Pivot measure
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PivotMeasure<T> {
        /// <summary>
        /// Create a Measure list that calculates the total
        /// </summary>
        /// <param name="PropertyNames"></param>
        /// <returns></returns>
        public static List<PivotMeasure<T>> Build(params string[] PropertyNames) => PropertyNames.Select(p => new PivotMeasure<T>(p)).ToList();
        /// <summary>
        /// Create a measure that calculates the total
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="valueGetter"></param>
        /// <returns></returns>
        public static PivotMeasure<T> Sum(string propertyName, Func<T, decimal> valueGetter = null) => new PivotMeasure<T>(propertyName, valueGetter) { aggregate = aggregate_sum };
        /// <summary>
        /// Create a measure to calculate the count
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="valueGetter"></param>
        /// <returns></returns>
        public static PivotMeasure<T> Count(string propertyName, Func<T, decimal> valueGetter = null) => new PivotMeasure<T>(propertyName, valueGetter) { aggregate = aggregate_count };
        /// <summary>
        /// Create a measure to calculate the average
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="valueGetter"></param>
        /// <returns></returns>
        public static PivotMeasure<T> Average(string propertyName, Func<T, decimal> valueGetter = null) => new PivotMeasure<T>(propertyName, valueGetter) { aggregate = aggregate_average };
        /// <summary>
        /// Create a measure to calculate the maximum
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="valueGetter"></param>
        /// <returns></returns>
        public static PivotMeasure<T> Max(string propertyName, Func<T, decimal> valueGetter = null) => new PivotMeasure<T>(propertyName, valueGetter) { aggregate = aggregate_max };
        /// <summary>
        /// Create a measure to calculate the minimum
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="valueGetter"></param>
        /// <returns></returns>
        public static PivotMeasure<T> Min(string propertyName, Func<T, decimal> valueGetter = null) => new PivotMeasure<T>(propertyName, valueGetter) { aggregate = aggregate_min };
        /// <summary>
        /// Property Name of T
        /// </summary>
        public string PropertyName { get; private set; }
        /// <summary>
        /// delegate get value of T
        /// </summary>
        internal Func<T, decimal> ValueGetter { get; set; }

        public PivotMeasure(string propertyName, Func<T, decimal> valueGetter = null) {
            this.PropertyName = propertyName;
            ValueGetter = valueGetter;
        }
        /// <summary>
        /// delegate aggregate 
        /// </summary>
        public Func<IEnumerable<decimal>, decimal> aggregate { get; protected set; } = aggregate_sum;

        #region aggregate functions
        private static Func<IEnumerable<decimal>, decimal> aggregate_sum = (list) => list.Any()?list.Sum():0;
        private static Func<IEnumerable<decimal>, decimal> aggregate_count = (list) => list.Any() ? list.Count():0;
        private static Func<IEnumerable<decimal>, decimal> aggregate_average = (list) => list.Any() ? list.Average():0;
        private static Func<IEnumerable<decimal>, decimal> aggregate_max = (list) => list.Any() ? list.Max():0;
        private static Func<IEnumerable<decimal>, decimal> aggregate_min = (list) => list.Any() ? list.Min():0;
        #endregion
    }
    public class ComplexPivotMeasure<T> : PivotMeasure<T> {
        public ComplexPivotMeasure(string propertyName ,Func<decimal[],decimal> aggregate_function ,params PivotMeasure<T>[] measures) : base(propertyName,(b)=>0) {
            values_aggregate = aggregate_function;
            this.measures = measures;
        }
        public PivotMeasure<T>[] measures { get; private set; }
        public Func<decimal[], decimal> values_aggregate { get; protected set; } = (values) => 0; 
    }

}
