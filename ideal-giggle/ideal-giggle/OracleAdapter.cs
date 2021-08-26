using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Oracle.ManagedDataAccess.Client;

namespace ideal_giggle
{
    public class OracleAdapter
    {


        // One way of inserting bulk data to Oracle db
        public void FillVotesTable(Votes users)
        {
            string targetTable = nameof(Votes);

            DataTable dTable = new DataTable();
            dTable.Columns.Add("Id");
            dTable.Columns.Add("PostId");
            dTable.Columns.Add("VoteTypeId");
            dTable.Columns.Add("CreationDate");


            var votes = users.Rows;
            foreach (var vote in votes)
            {
                DataRow dRow = dTable.NewRow();
                dRow["Id"] = vote.Id;
                dRow["PostId"] = vote.PostId;
                dRow["VoteTypeId"] = vote.VoteTypeId;
                dRow["CreationDate"] = vote.CreationDate;

                dTable.Rows.Add(dRow);
            }

            OracleConfiguration.OracleDataSources.Add("orclpdb1", 
                "(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=ORCLPDB1)))");


            try
            {
                using (var connection = new OracleConnection("user id=NIKI; password=niki; data source=orclpdb1"))
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
            }
            catch (Exception ex)
            {
                ConsolePrinter.PrintLine($"Method {nameof(FillVotesTable)} failed when inserting bulp data to the oracle table {targetTable}!", ConsoleColor.Red);
                return;
            }

        }

        // Second way of inserting bulk data to Oracledb
        //public void FillVotesTable2(Votes users)
        //{
        //    int[] ids = users.Rows.Select(r => r.Id).ToArray();
        //    int[] postIds = users.Rows.Select(r => r.PostId).ToArray();
        //    int[] voteTypeIds = users.Rows.Select(r => r.VoteTypeId).ToArray();
        //    DateTime[] creationDates = users.Rows.Select(r => r.CreationDate).ToArray();

        //    OracleParameter id = new OracleParameter();
        //    id.OracleDbType = OracleDbType.Int32;
        //    id.Value = ids;

        //    OracleParameter postId = new OracleParameter();
        //    postId.OracleDbType = OracleDbType.Int32;
        //    postId.Value = postIds;

        //    OracleParameter voteTypeId = new OracleParameter();
        //    voteTypeId.OracleDbType = OracleDbType.Int32;
        //    voteTypeId.Value = voteTypeIds;

        //    OracleParameter creationDate = new OracleParameter();
        //    creationDate.OracleDbType = OracleDbType.TimeStamp;
        //    creationDate.Value = creationDates;



        //    OracleConnection connection = new OracleConnection(ConnectionString);
        //    connection.Open();

        //    OracleCommand cmd = new OracleCommand();
        //    cmd.Connection = connection;
        //    cmd.CommandText = $"insert into {DbName}.Comments ( ID, POSTID, VOTETYPEID, CREATIONDATE) " +
        //                                             $"values ( :1, :2, :3, :4 )";
        //    cmd.ArrayBindCount = ids.Length;
        //    cmd.Parameters.Add(id);
        //    cmd.Parameters.Add(postId);
        //    cmd.Parameters.Add(voteTypeId);
        //    cmd.Parameters.Add(creationDate);

        //    cmd.ExecuteNonQuery();
        //}


        //public void FillVotesTable3(Votes users)
        //{
        //}



        public void FillPostsTable(Posts posts)
        {
            string targetTable = nameof(Posts);




        }





        public void FillUsersTable(Users users)
        {
            string targetTable = nameof(Users);

        }



        public void FillCommentsTable(Comments users)
        {
            string targetTable = nameof(Comments);



        }
    }

}
