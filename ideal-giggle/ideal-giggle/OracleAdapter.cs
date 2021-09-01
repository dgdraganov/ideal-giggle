using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Oracle.ManagedDataAccess.Client;

namespace ideal_giggle
{
    public class OracleAdapter : IDbAdapter
    {
        private string ConnectionString { get; }
        public OracleAdapter()
        {
            OracleConfiguration.OracleDataSources.Add("orclpdb1",
               "(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=ORCLPDB1)))");

            ConnectionString = "user id=NIKI; password=niki; data source=orclpdb1";
        }


        public void InsertToTable<T>(T table)
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

            BulkCopyToDb(typeof(T).Name, dTable);

        }
        private void BulkCopyToDb(string targetTable, DataTable dTable)
        {
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
                        bulkCopy.WriteToServer(dTable);
                    }
                }

                ConsolePrinter.PrintLine($"Successfully added {dTable.Rows.Count} records to table {targetTable}!", ConsoleColor.Green);
            }
            catch (Exception ex)
            {
                ConsolePrinter.PrintLine($"Method {nameof(BulkCopyToDb)} failed when inserting bulp data to the oracle table {targetTable}!", ConsoleColor.Red);
                ConsolePrinter.PrintLine($"{ex.Message}", ConsoleColor.DarkYellow);
                return;
            }
        }

     
    }

}
