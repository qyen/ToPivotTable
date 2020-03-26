using ASP.Net_Sample.Models;
using qyen.Pivot;
using qyen.Pivot.Mvc5;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace ASP.Net_Sample.Controllers {
    public class HomeController : Controller {
        public ActionResult Index(IndexViewModel model) {
            model.pivot = SampleDB.Data.ToPivotTable(
                                PivotColumn<MockData>.Build("country", "gender"),
                                PivotColumn<MockData>.Build("stock_market"),
                                PivotMeasure<MockData>.Build("stock")
                          );
            return View(model);
        }
        public ActionResult Total(IndexViewModel model) {
            model.pivot = SampleDB.Data.ToPivotTable(
                                PivotColumn<MockData>.Build("country", "gender"),
                                PivotColumn<MockData>.Build("stock_market"),
                                PivotMeasure<MockData>.Build("stock")
                          );
            model.Option.Header[HeaderType.Row].RenderTotal = true;
            model.Option.HeaderCellOption[model.pivot.ColumnByName("country")].RenderTotal = true;
            return View(model);
        }
        public ActionResult Measure(IndexViewModel model) {
            model.pivot = SampleDB.Data.ToPivotTable(
                                PivotColumn<MockData>.Build("country", "gender"),
                                PivotColumn<MockData>.Build("stock_market"),
                                new List<PivotMeasure<MockData>>() {
                                    PivotMeasure<MockData>.Sum("Sum",(t)=>t.stock),
                                    PivotMeasure<MockData>.Average("Avg",(t)=>t.stock),
                                    PivotMeasure<MockData>.Min("Min",(t)=>t.stock),
                                    PivotMeasure<MockData>.Max("Max",(t)=>t.stock),
                                }
                          );
            return View(model);
        }
        public ActionResult CustomColumn(IndexViewModel model) {
            var DB = SampleDB.Data.Where(mock => mock.Job.StartsWith("A"));
            model.pivot = DB.ToPivotTable(
                                new List<PivotColumn<MockData>>() {
                                    new PivotColumn<MockData>("initial",(t)=>t.car.Substring(0,1),(t)=>t.car.Substring(0,1)){
                                        Order=PivotOrder.Descending,
                                    },
                                    new PivotColumn<MockData>("car"),
                                },
                                new List<PivotColumn<MockData>>() {
                                    new PivotColumn<MockData>("category",(t)=>t.Job.Split(' ').First(),(t)=>t.Job.Split(' ').First()),
                                    new PivotColumn<MockData>("Job",(t)=>string.Join(" ",t.Job.Split(' ').Skip(1))),
                                },
                                new List<PivotMeasure<MockData>>() {
                                    PivotMeasure<MockData>.Average("Avg.Cash",(t)=>t.cash),
                                }
                          );
            return View(model);
        }

        public ActionResult FixedHeader(IndexViewModel model) {
            var DB = SampleDB.Data.Where(mock => mock.Job.StartsWith("A"));
            model.pivot = DB.ToPivotTable(
                                new List<PivotColumn<MockData>>() {
                                    new PivotColumn<MockData>("initial",(t)=>t.car.Substring(0,1),(t)=>t.car.Substring(0,1)){
                                        Order=PivotOrder.Descending,
                                    },
                                    new PivotColumn<MockData>("car"),
                                },
                                new List<PivotColumn<MockData>>() {
                                    new PivotColumn<MockData>("category",(t)=>t.Job.Split(' ').First(),(t)=>t.Job.Split(' ').First()),
                                    new PivotColumn<MockData>("Job",(t)=>string.Join(" ",t.Job.Split(' ').Skip(1))),
                                },
                                new List<PivotMeasure<MockData>>() {
                                    PivotMeasure<MockData>.Average("Avg.Cash",(t)=>t.cash),
                                }
                          );
            return View(model);
        }
        public ActionResult Raw() {
            return View(SampleDB.Data.Take(50));
        }

        public class IndexViewModel {
            public string country { get; set; }

            public PivotTable<MockData> pivot { get; set; }
            public PivotTableRenderOption<MockData> Option { get; set; } = new PivotTableRenderOption<MockData>();
        }

    }
}