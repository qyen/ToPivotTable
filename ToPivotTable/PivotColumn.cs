using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace qyen.Linq {
    /// <summary>
    /// Column(Row) definition
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PivotColumn<T> {

        /// <summary>
        /// Create PivotColumn list to aggregate by PropertyNames
        /// </summary>
        /// <param name="PropertyNames">Property Name of T</param>
        /// <returns></returns>
        public static List<PivotColumn<T>> Build(params string[] PropertyNames) => PropertyNames.Select(p => new PivotColumn<T>(p)).ToList();
        /// <summary>
        /// Property Name of T
        /// </summary>
        public string PropertyName { get; private set; }

        /// <summary>
        /// Column listing Order
        /// </summary>
        public PivotOrder Order { get; set; } = PivotOrder.Ascending;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="PropertyName"></param>
        /// <param name="titleGetter">delegate to Get title Action</param>
        /// <param name="valueGetter">delegate to Get value Action</param>
        public PivotColumn(string PropertyName, Func<T, string> titleGetter = null, Func<T, dynamic> valueGetter = null) {
            this.PropertyName = PropertyName;
            ValueGetter = valueGetter;
            TitleGetter = titleGetter;
        }
        
        internal List<PivotHeaderCell<T>> Cells { get; } = new List<PivotHeaderCell<T>>();

        /// <summary>
        /// All PivotHeaderCells at this level
        /// </summary>
        public IEnumerable<PivotHeaderCell<T>> Items => Cells.ToList();
        /// <summary>
        /// delegate get Value of T
        /// </summary>
        internal Func<T, dynamic> ValueGetter { get; set; }
        /// delegate get Title of T
        internal Func<T, string> TitleGetter { get; set; }

        /// <summary>
        /// Compare T of List and T of LeadObject
        /// </summary>
        internal bool EqualsExpression(T obj, T param) {
            return ValueGetter(obj) == ValueGetter(param);
        }
        /// <summary>
        /// Order/Group key getter
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal dynamic KeySelector(T obj) => ValueGetter(obj);
        /// <summary>
        /// Higher level column
        /// </summary>
        internal PivotColumn<T> HigherColumn { get; set; }
        /// <summary>
        /// lower level column
        /// </summary>
        internal PivotColumn<T> LowerColumn { get; set; }
    }

    /// <summary>
    /// pivot header cell
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PivotHeaderCell<T> {
        internal PivotHeaderCell(T leadObjet, PivotColumn<T> column) {
            LeadObject = leadObjet;
            Column = column;
        }
        /// <summary>
        /// cell value
        /// </summary>
        public string Value => Column.ValueGetter(LeadObject);
        /// <summary>
        /// cell display title
        /// </summary>
        public string Title => Column.TitleGetter(LeadObject);
        /// <summary>
        /// T contained in this cell
        /// </summary>
        internal T LeadObject { get; set; }
        /// <summary>
        /// Parent cell of Cell-Tree hierarchy
        /// </summary>
        internal PivotHeaderCell<T> Parent { get; set; }
        /// <summary>
        /// Child cells of Cell-Tree hierarchy
        /// </summary>
        internal List<PivotHeaderCell<T>> __Children { get; private set; } = new List<PivotHeaderCell<T>>();
        /// <summary>
        /// Child cells of Cell-Tree hierarchy
        /// </summary>
        public IEnumerable<PivotHeaderCell<T>> Children => __Children.ToList();
        /// <summary>
        /// Column what contain this cell
        /// </summary>
        internal PivotColumn<T> Column { get; set; }
        /// <summary>
        /// lowest level cells count of progeny
        /// </summary>
        public int CountLeaves {
            get {
                if (Column.LowerColumn == null) {
                    return 1;
                } else {
                    return __Children.Sum(c => c.CountLeaves);
                }
            }
        }
        /// <summary>
        /// Path of tree
        /// </summary>
        public IEnumerable<PivotHeaderCell<T>> Path{
            get {
                var path = new List<PivotHeaderCell<T>>();
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
