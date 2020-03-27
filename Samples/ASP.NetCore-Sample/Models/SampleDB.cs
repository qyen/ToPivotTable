using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Web;

namespace ASP.NetCore_Sample.Models {
    public class SampleDB {
        public static void init(string JsonPath) { 
            if (data == null) {
                var jsonString = File.ReadAllText(JsonPath);
                data =  JsonSerializer.Deserialize<List<MockData>>(jsonString);
            }
        }

        private static List<MockData> data = null;
        public static IQueryable<MockData> Data => data.AsQueryable();
    }
}