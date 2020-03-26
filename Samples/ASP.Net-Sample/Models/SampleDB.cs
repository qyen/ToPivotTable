using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ASP.Net_Sample.Models {
    public class SampleDB {
        private static List<MockData> data = null;
        public static IQueryable<MockData> Data {
            get {
                if (data == null) {
                    using (var reader = new StreamReader(HttpContext.Current.Server.MapPath("~/App_Data/MOCK_DATA.json"))) {
                        data = Jil.JSON.Deserialize<List<MockData>>(reader);
                    }
                }
                return data.AsQueryable();
            }
        }
    }
}