using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Warehouse.Controllers
{
    [Route("api/[controller]")]
    public class WarehouseController : Controller
    {
        // GET api/warehouse
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "test", "test value" };
        }

        // GET api/warehouse/partitionid/year
        [HttpGet("{partitionId}/{year}")]
        public JObject Get(string partitionId, int year)
        {
            if (partitionId == null || partitionId.Length == 0)
            {
                dynamic retVal = new JObject();
                retVal.partitionId = partitionId;
                retVal.yaer = year;

                dynamic err = new JObject();
                err.message = "PartitionId argument not supplied!";
                retVal.error = err;
                return retVal;

            }
            else {
                dynamic retVal = Warehouse.services.WarehouseService.getMontlyTotalsPerYear(partitionId, year);
                return retVal;
            }
           
            //var s = new StringBuilder();
            //s.Append("{\"partitionId\":\"").Append(id).Append("\", \"total\":{\"attendanceCount\":1, \"reservationCount\": 10},");
            //s.Append("\"jan\":{\"attendanceCount\":2, \"reservationCount\": 20},");
            //s.Append("\"feb\":{\"attendanceCount\":2, \"reservationCount\": 20},");
            //s.Append("\"mar\":{\"attendanceCount\":2, \"reservationCount\": 20},");
            //s.Append("\"apr\":{\"attendanceCount\":2, \"reservationCount\": 20},");
            //s.Append("\"may\":{\"attendanceCount\":2, \"reservationCount\": 20},");
            //s.Append("\"jun\":{\"attendanceCount\":2, \"reservationCount\": 20},");
            //s.Append("\"jul\":{\"attendanceCount\":2, \"reservationCount\": 20},");
            //s.Append("\"aug\":{\"attendanceCount\":2, \"reservationCount\": 20},");
            //s.Append("\"sep\":{\"attendanceCount\":2, \"reservationCount\": 20},");
            //s.Append("\"oct\":{\"attendanceCount\":2, \"reservationCount\": 20},");
            //s.Append("\"nov\":{\"attendanceCount\":2, \"reservationCount\": 20},");
            //s.Append("\"dec\":{\"attendanceCount\":2, \"reservationCount\": 20}}");

            ////var result = new JsonResult(JsonConvert.DeserializeObject(s.ToString()));
            ////Console.WriteLine(result.ToString());
            //////return new JsonResult().
            //return s.ToString();
        }

        // POST api/warehouse
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/warehouse/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/warehouse/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
