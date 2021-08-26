using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace ideal_giggle
{
    public class OracleAdapter
    {
        private string ConnectionString { get; }
        public OracleAdapter()
        {
            OracleConfiguration.OracleDataSources.Add("orclpdb1",
               "(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=ORCLPDB1)))");

            ConnectionString = "user id=NIKI; password=niki; data source=orclpdb1";
        }

        public void FillVotesTable(Votes votesTable)
        {
            DataTable dTable = new DataTable();
            dTable.Columns.Add("Id");
            dTable.Columns.Add("PostId");
            dTable.Columns.Add("VoteTypeId");
            dTable.Columns.Add("CreationDate");


            var votes = votesTable.Rows;
            foreach (var vote in votes)
            {
                DataRow dRow = dTable.NewRow();
                dRow["Id"] = vote.Id;
                dRow["PostId"] = vote.PostId;
                dRow["VoteTypeId"] = vote.VoteTypeId;
                dRow["CreationDate"] = vote.CreationDate;

                dTable.Rows.Add(dRow);
            }

            BulkCopyToDb(nameof(Votes), dTable);

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


        public void FillPostsTable(Posts postsTable)
        {

            DataTable dTable = new DataTable();
            dTable.Columns.Add("Id");
            dTable.Columns.Add("PostTypeId");
            dTable.Columns.Add("AcceptedAnswerId");
            dTable.Columns.Add("Score");
            dTable.Columns.Add("ViewCount");
            dTable.Columns.Add("Body");
            dTable.Columns.Add("OwnerUserId");
            dTable.Columns.Add("LastEditorUserId");
            dTable.Columns.Add("Title");
            dTable.Columns.Add("Tags");
            dTable.Columns.Add("AnswerCount");
            dTable.Columns.Add("CommentCount");
            dTable.Columns.Add("FavoriteCount");
            dTable.Columns.Add("ContentLicense");
            dTable.Columns.Add("CreationDate");
            dTable.Columns.Add("LastActivityDate");
            dTable.Columns.Add("LastEditDate");


            var posts = postsTable.Rows;
            foreach (var post in posts)
            {
                DataRow dRow = dTable.NewRow();
                dRow["Id"] = post.Id;
                dRow["PostTypeId"] = post.PostTypeId;
                dRow["AcceptedAnswerId"] = post.AcceptedAnswerId;
                dRow["Score"] = post.Score;
                dRow["ViewCount"] = post.ViewCount;
                dRow["Body"] = post.Body;
                dRow["OwnerUserId"] = post.OwnerUserId;
                dRow["LastEditorUserId"] = post.LastEditorUserId;
                dRow["Title"] = post.Title;
                dRow["Tags"] = post.Tags;
                dRow["AnswerCount"] = post.AnswerCount;
                dRow["CommentCount"] = post.CommentCount;
                dRow["FavoriteCount"] = post.FavoriteCount;
                dRow["ContentLicense"] = post.ContentLicense;
                dRow["CreationDate"] = post.CreationDate;
                dRow["LastActivityDate"] = post.LastActivityDate;
                dRow["LastEditDate"] = post.LastEditDate;

                dTable.Rows.Add(dRow);
            }

            BulkCopyToDb(nameof(Posts), dTable);

        }
        public void FillUsersTable(Users usersTable)
        {

            DataTable dTable = new DataTable();
            dTable.Columns.Add("Id");
            dTable.Columns.Add("Reputation");
            dTable.Columns.Add("CreationDate");
            dTable.Columns.Add("DisplayName");
            dTable.Columns.Add("LastAccessDate");
            dTable.Columns.Add("WebsiteUrl");
            dTable.Columns.Add("Location");
            dTable.Columns.Add("AboutMe");
            dTable.Columns.Add("Views");
            dTable.Columns.Add("UpVotes");
            dTable.Columns.Add("DownVotes");
            dTable.Columns.Add("AccountId");

            var users = usersTable.Rows;
            foreach (var user in users)
            {
                DataRow dRow = dTable.NewRow();
                dRow["Id"] = user.Id;
                dRow["Reputation"] = user.Reputation;
                dRow["CreationDate"] = user.CreationDate;
                dRow["DisplayName"] = user.DisplayName;
                dRow["LastAccessDate"] = user.LastAccessDate;
                dRow["WebsiteUrl"] = user.WebsiteUrl;
                dRow["Location"] = user.Location;
                dRow["AboutMe"] = user.AboutMe;
                dRow["Views"] = user.Views;
                dRow["UpVotes"] = user.UpVotes;
                dRow["DownVotes"] = user.DownVotes;
                dRow["AccountId"] = user.AccountId;

                dTable.Rows.Add(dRow);
            }

            BulkCopyToDb(nameof(Users), dTable);
        }



        public void FillCommentsTable(Comments commentsTable)
        {
            DataTable dTable = new DataTable();
            dTable.Columns.Add("Id");
            dTable.Columns.Add("PostId");
            dTable.Columns.Add("Score");
            dTable.Columns.Add("Text");
            dTable.Columns.Add("CreationDate");
            dTable.Columns.Add("UserId");
            dTable.Columns.Add("ContentLicense");

            var comments = commentsTable.Rows;
            foreach (var comment in comments)
            {
                DataRow dRow = dTable.NewRow();
                dRow["Id"] = comment.Id;
                dRow["PostId"] = comment.PostId;
                dRow["Score"] = comment.Score;
                dRow["Text"] = comment.Text;
                dRow["CreationDate"] = comment.CreationDate;
                dRow["UserId"] = comment.UserId;
                dRow["ContentLicense"] = comment.ContentLicense;

                dTable.Rows.Add(dRow);
            }
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
            }
            catch (Exception ex)
            {
                ConsolePrinter.PrintLine($"Method {nameof(FillVotesTable)} failed when inserting bulp data to the oracle table {targetTable}!", ConsoleColor.Red);
                ConsolePrinter.PrintLine($"{ex.Message}", ConsoleColor.DarkYellow);
                return;
            }
        }
    }

}
