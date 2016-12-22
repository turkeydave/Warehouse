using Microsoft.AnalysisServices.AdomdClient;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse.services
{
    public static class WarehouseService
    {
        public static JObject getMontlyTotalsPerYear(String partitionId, int year) {
            
            dynamic retVal = new JObject();
            retVal.partitionId = partitionId;
            retVal.year = year;

            // todo: external config
            String serverUrl = "http://cube.office.zen.corp";
            String catalogName = "ZenPlanner-CubeOne";
            String userName = "ZENOFFICE\\FRODO";
            String password = "ush@ll!p@55"; // Michael ... nice dude!  Took me a sec...
            StringBuilder connStr = new StringBuilder();
            connStr.Append("Data Source=").Append(serverUrl).Append("/olap/msmdpump.dll;Catalog=");
            connStr.Append(catalogName).Append("; UID=").Append(userName).Append("; PWD=").Append(password);

            //Connect to the local server
            //String connStr = "jdbc:xmla:Server=http://cube.office.zen.corp/olap/msmdpump.dll;Catalog=ZenPlanner-CubeOne;USER=XXXX;PASSWORD=XXXXXX";
            //"Data Source=http://cube.office.zen.corp/olap/msmdpump.dll;Catalog=ZenPlanner-CubeOne; UID=ZENOFFICE\\DAVID; PWD=XXXXX"
            using (AdomdConnection conn = new AdomdConnection(connStr.ToString()))
            {
                conn.Open();

                //Create a command, using this connection
                AdomdCommand cmd = conn.CreateCommand();
                StringBuilder s = new StringBuilder();
                s.Append("SELECT {[Measures].[Fact Attendance Count],[Measures].[Fact Reservation Count] } ");
                s.Append(" ON COLUMNS ,");
                s.Append(" NON EMPTY Hierarchize({DrilldownLevel({[Dim Date].[Month Of Year Name].[All]})})");
                s.Append(" ON ROWS");
                s.Append(" FROM [ZenPlanner-DataWarehouse]");
                s.Append(" WHERE ([Dim Business].[Business Native ID].&[{").Append(partitionId).Append("}],[Dim Date].[Year Name].&[");
                s.Append(year.ToString()).Append("])");

                //cmd.CommandText = @"
                //        SELECT 
                //        {[Measures].[Fact Attendance Count],[Measures].[Fact Reservation Count]}
                //         ON COLUMNS ,
                //         NON EMPTY Hierarchize({DrilldownLevel({[Dim Date].[Month Of Year Name].[All]})})
                //         ON ROWS
                //         FROM [ZenPlanner-DataWarehouse]
                //         WHERE ([Dim Business].[Business Native ID].&[{EE0DB82E-F1C3-4FC6-9976-8852F3F52D33}],[Dim Date].[Year Name].&[2016])";

                cmd.CommandText = s.ToString();
                

                //Execute the query, returning a cellset
                CellSet cs = cmd.ExecuteCellSet();

                List<string> colNames = new List<string>();

                //Output the column captions from the first axis
                //Note that this procedure assumes a single member exists per column.
                TupleCollection tuplesOnColumns = cs.Axes[0].Set.Tuples;
                foreach (Microsoft.AnalysisServices.AdomdClient.Tuple column in tuplesOnColumns)
                {
                    string col = column.Members[0].Caption.Replace(" ", "_");
                    //result.Append(column.Members[0].Caption + "\t");
                    colNames.Add(col);
                }

                //Output the row captions from the second axis and cell data
                //Note that this procedure assumes a two-dimensional cellset
                TupleCollection tuplesOnRows = cs.Axes[1].Set.Tuples;
                for (int row = 0; row < tuplesOnRows.Count; row++)
                {
                    string rowName = tuplesOnRows[row].Members[0].Caption.Replace(" ", "_");
                    dynamic valueSet = new JObject();
                    
                    for (int col = 0; col < tuplesOnColumns.Count; col++)
                    {
                        // .FormattedValue is either "", or "int"
                        // .Value will be int or null
                        int val = cs.Cells[col, row].FormattedValue != "" ? int.Parse(cs.Cells[col, row].FormattedValue) : 0;
                        valueSet[colNames[col]] = val;
                    }

                    retVal[rowName] = valueSet;
                }
                conn.Close();

                //return result.ToString();
            } // using connection

            //dynamic tot = new JObject();
            //tot.attendanceCount = 1;
            //tot.reservationCount = 10;
            //retVal.total = tot;

            //dynamic jan = new JObject();
            //jan.attendanceCount = 1;
            //jan.reservationCount = 10;
            //retVal.jan = jan;

            return retVal;

        } // getMontlyTotalsPerYear
    }
}
