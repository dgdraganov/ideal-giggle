using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Oracle.ManagedDataAccess.Client;

namespace ideal_giggle
{
    public class OracleAdapter : IDbAdapter
    {
      
        public OracleAdapter()
        {
            OracleConfiguration.OracleDataSources.Add("orclpdb1",
               "(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=ORCLPDB1)))");

            ConnectionString = "user id=NIKI; password=niki; data source=orclpdb1";
            Name = "Oracle Adapter";
        }

        public string Name { get; }

        private string ConnectionString { get; }


        public long InsertToTable<T>(T table)
        {
            var members = typeof(T).GetProperty("Rows", BindingFlags.Public | BindingFlags.Instance);
            var rowType = typeof(T).GetNestedType("Row");
            var rowTypeMembers = rowType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var listOfRows = (IEnumerable<object>)members.GetValue(table);

            // Add dTable columns
            DataTable dTable = new DataTable();
            foreach (var member in rowTypeMembers)
                dTable.Columns.Add(member.Name, member.PropertyType);


            foreach (var row in listOfRows)
            {
                DataRow dRow = dTable.NewRow();

                foreach (var member in rowTypeMembers)
                {
                    dRow[member.Name] = member.GetValue(row);
                }

                dTable.Rows.Add(dRow);
            }

            return BulkCopyToDb(typeof(T).Name, dTable);

        }
        private long BulkCopyToDb(string targetTable, DataTable dTable)
        {
            Stopwatch sw = new Stopwatch();

            try
            {
                using (var connection = new OracleConnection(ConnectionString))
                {
                    connection.Open();
                    using (var bulkCopy = new OracleBulkCopy(connection, OracleBulkCopyOptions.UseInternalTransaction))
                    {
                        bulkCopy.DestinationTableName = targetTable;
                        bulkCopy.BulkCopyTimeout = 600;
                        foreach (DataColumn dtColumn in dTable.Columns)
                        {
                            bulkCopy.ColumnMappings.Add(dtColumn.ColumnName, dtColumn.ColumnName.ToUpper());
                        }

                        sw.Start();
                        bulkCopy.WriteToServer(dTable);
                        sw.Stop();
                    }
                }

                ConsolePrinter.PrintLine($"Oracle DB -> Successfully added {dTable.Rows.Count} records to table {targetTable}!", ConsoleColor.Green);
                return sw.ElapsedMilliseconds;

            }
            catch (Exception ex)
            {
                ConsolePrinter.PrintLine($"Method {nameof(BulkCopyToDb)} failed when inserting bulp data to the oracle table {targetTable}!", ConsoleColor.Red);
                ConsolePrinter.PrintLine($"{ex.Message}", ConsoleColor.DarkYellow);
                return 0;
            }

        }

     
    }

}
