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
        public OracleAdapter(string connectionString, string dbName)
        {
            ConnectionString = connectionString;
            DbName = dbName;
        }

        public string ConnectionString { get; }
        public string DbName { get; }



        public void CreateTables()
        {
            throw new NotImplementedException();
        }


        // One way of inserting bulk data to Oracle db
        public void FillVotesTable(Votes users)
        {
            string tableName = users.GetType().Name;

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


            try
            {
                using (var connection = new OracleConnection(ConnectionString))
                {
                    connection.Open();
                    using (var bulkCopy = new OracleBulkCopy(connection, OracleBulkCopyOptions.UseInternalTransaction))
                    {
                        bulkCopy.DestinationTableName = $"{DbName}.{tableName}";
                        bulkCopy.BulkCopyTimeout = 600;
                        bulkCopy.WriteToServer(dTable);
                    }
                }
            }
            catch (Exception ex)
            {
                ConsolePrinter.PrintLine($"Method {nameof(FillVotesTable)} failed when inserting bulp data to the oracle table {DbName}.{tableName}!", ConsoleColor.Red);
                return;
            }

        }

        // Second way of inserting bulk data to Oracledb
        public void FillVotesTable2(Votes users)
        {
            int[] ids = users.Rows.Select(r => r.Id).ToArray();
            int[] postIds = users.Rows.Select(r => r.PostId).ToArray();
            int[] voteTypeIds = users.Rows.Select(r => r.VoteTypeId).ToArray();
            DateTime[] creationDates = users.Rows.Select(r => r.CreationDate).ToArray();

            OracleParameter id = new OracleParameter();
            id.OracleDbType = OracleDbType.Int32;
            id.Value = ids;

            OracleParameter postId = new OracleParameter();
            postId.OracleDbType = OracleDbType.Int32;
            postId.Value = postIds;

            OracleParameter voteTypeId = new OracleParameter();
            voteTypeId.OracleDbType = OracleDbType.Int32;
            voteTypeId.Value = voteTypeIds;

            OracleParameter creationDate = new OracleParameter();
            creationDate.OracleDbType = OracleDbType.Date;
            creationDate.Value = creationDates;



            OracleConnection connection = new OracleConnection(ConnectionString);
            connection.Open();

            OracleCommand cmd = new OracleCommand();
            cmd.Connection = connection;
            cmd.CommandText = $"insert into {DbName}.Comments ( id, postId, voteTypeId, creationDate) " +
                                                     $"values ( :1, :2, :3, :4 )";
            cmd.ArrayBindCount = ids.Length;
            cmd.Parameters.Add(id);
            cmd.Parameters.Add(postId);
            cmd.Parameters.Add(voteTypeId);
            cmd.Parameters.Add(creationDate);

            cmd.ExecuteNonQuery();
        }

        public void FillVotesTable3(Votes users)
        {
        }



        /// /////////////////////////////////////////////////////
        public void FillPostsTable(Posts posts)
        {
           

        }
   




        public void FillUsersTable(Users users)
        {
           
        }

   

        public void FillCommentsTable(Comments users)
        {

        }
    }

}
