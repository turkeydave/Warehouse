using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Warehouse;
using Newtonsoft.Json.Linq;

namespace ConsoleApp1
{
    [TestClass]
    public class WarehouseTests
    {
        [TestMethod]
        public void testGetYearlyCounts() {
            string partitionId = "EE0DB82E-F1C3-4FC6-9976-8852F3F52D33";
            int year = 2016;
            JObject testVal = Warehouse.services.WarehouseService.getMontlyTotalsPerYear(partitionId, year);
            Assert.IsNotNull(testVal);
        }
    }
}
