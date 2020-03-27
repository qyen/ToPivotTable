using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using qyen.Pivot;
using qyen.Pivot.Mvc5;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;

namespace ToPivotTable.MVC.Core {
    public static class ToPivotExtensions {
        public static HtmlString RenderPivotTable<TModel, T>(this IHtmlHelper<TModel> htmlHelper, PivotTable<T> pivotTable, PivotTableRenderOption<T> option = null) {
            Contract.Requires(htmlHelper != null);
            Contract.Requires(pivotTable != null);
            if (option == null)
                option = new PivotTableRenderOption<T>();

            return new HtmlString(new PivotTableRender<T>(pivotTable, option).Run());
        }
    }
}
