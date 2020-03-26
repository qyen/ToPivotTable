using qyen.Pivot;
using qyen.Pivot.Mvc5;
using System.Diagnostics.Contracts;

namespace System.Web.Mvc {
    public static class ToPivotExtensions {
        public static MvcHtmlString RenderPivotTable<TModel, T>(this HtmlHelper<TModel> htmlHelper, PivotTable<T> pivotTable, PivotTableRenderOption<T> option = null) {
            Contract.Requires(htmlHelper != null);
            Contract.Requires(pivotTable != null);
            if (option == null)
                option = new PivotTableRenderOption<T>();

            return PivotTableRender.Run(pivotTable, option);
        }
    }
}
