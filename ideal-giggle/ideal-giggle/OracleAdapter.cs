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
            dTable.Columns.Add("Id", typeof(int));
            dTable.Columns.Add("PostId", typeof(int));
            dTable.Columns.Add("VoteTypeId", typeof(int));
            dTable.Columns.Add("CreationDate", typeof(DateTime));


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

        public void FillUsersBadgesTable(UsersBadges usersBadgesTable)
        {
            DataTable dTable = new DataTable();
            dTable.Columns.Add("Id", typeof(int));
            dTable.Columns.Add("UserId", typeof(int));
            dTable.Columns.Add("BadgeId", typeof(int));
            dTable.Columns.Add("Date", typeof(DateTime));

            var usersBadges = usersBadgesTable.Rows;
            foreach (var userBadge in usersBadges)
            {
                DataRow dRow = dTable.NewRow();
                dRow["Id"] = userBadge.Id;
                dRow["UserId"] = userBadge.UserId;
                dRow["BadgeId"] = userBadge.BadgeId;
                dRow["Date"] = userBadge.Date;

                dTable.Rows.Add(dRow);
            }

            BulkCopyToDb(nameof(UsersBadges), dTable);
        }

        public void FillBadgesTable(Badges badgesTable)
        {
            DataTable dTable = new DataTable();
            dTable.Columns.Add("Id", typeof(int));
            dTable.Columns.Add("Name", typeof(string));

            var badges = badgesTable.Rows;
            foreach (var badge in badges)
            {
                DataRow dRow = dTable.NewRow();
                dRow["Id"] = badge.Id;
                dRow["Name"] = badge.Name;

                dTable.Rows.Add(dRow);
            }

            BulkCopyToDb(nameof(Badges), dTable);
        }

        public void FillPostsTable(Posts postsTable)
        {
            DataTable dTable = new DataTable();
            dTable.Columns.Add("Id", typeof(int));
            dTable.Columns.Add("PostTypeId", typeof(int));
            dTable.Columns.Add("AcceptedAnswerId", typeof(int));
            dTable.Columns.Add("Score", typeof(int));
            dTable.Columns.Add("ViewCount", typeof(int));
            dTable.Columns.Add("Body", typeof(string));
            dTable.Columns.Add("OwnerUserId", typeof(int));
            dTable.Columns.Add("LastEditorUserId", typeof(int));
            dTable.Columns.Add("Title", typeof(string));
            dTable.Columns.Add("Tags", typeof(string));
            dTable.Columns.Add("AnswerCount", typeof(int));
            dTable.Columns.Add("CommentCount", typeof(int));
            dTable.Columns.Add("FavoriteCount", typeof(int));
            dTable.Columns.Add("ContentLicense", typeof(string));
            dTable.Columns.Add("CreationDate", typeof(DateTime));
            dTable.Columns.Add("LastActivityDate", typeof(DateTime));
            dTable.Columns.Add("LastEditDate", typeof(DateTime));


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
            dTable.Columns.Add("Id", typeof(int));
            dTable.Columns.Add("Reputation", typeof(int));
            dTable.Columns.Add("CreationDate", typeof(DateTime));
            dTable.Columns.Add("DisplayName", typeof(string));
            dTable.Columns.Add("LastAccessDate", typeof(DateTime));
            dTable.Columns.Add("WebsiteUrl", typeof(string));
            dTable.Columns.Add("Location", typeof(string));
            dTable.Columns.Add("AboutMe", typeof(string));
            dTable.Columns.Add("Views", typeof(int));
            dTable.Columns.Add("UpVotes", typeof(int));
            dTable.Columns.Add("DownVotes", typeof(int));
            dTable.Columns.Add("AccountId", typeof(int));

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
            dTable.Columns.Add("Id", typeof(int));
            dTable.Columns.Add("PostId", typeof(int));
            dTable.Columns.Add("Score", typeof(int));
            dTable.Columns.Add("Text", typeof(string));
            dTable.Columns.Add("CreationDate", typeof(DateTime));
            dTable.Columns.Add("UserId", typeof(int));
            dTable.Columns.Add("ContentLicense", typeof(string));

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

            BulkCopyToDb(nameof(Comments), dTable);
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
                ConsolePrinter.PrintLine($"Method {nameof(FillVotesTable)} failed when inserting bulp data to the oracle table {targetTable}!", ConsoleColor.Red);
                ConsolePrinter.PrintLine($"{ex.Message}", ConsoleColor.DarkYellow);
                return;
            }
        }
    }

}
