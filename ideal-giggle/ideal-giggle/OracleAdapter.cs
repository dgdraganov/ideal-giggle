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


        public void PartiallyProcess()
        {

        }


        /// <summary>
        /// FillCommentsTable uses plain old instert for the whole data at once
        /// </summary>
        /// <param name="tableRows"></param>
        public void FillCommentsTable(XmlNodeList tableRows)
        {
            var tasks = new List<Task>();

            int[] ids = new int[tableRows.Count];
            int[] postIds = new int[tableRows.Count];
            int[] userIds = new int[tableRows.Count];
            int[] scores = new int[tableRows.Count];
            string[] texts = new string[tableRows.Count];
            string[] contentLicenses = new string[tableRows.Count];
            // CreationDate ? data type

            // ======================================================================
            int chunk = tableRows.Count / 40;
            int from = 0;
            int to = chunk;


            while (true)
            {
                int fromLocal = from;
                int toLocal = to;

                Action action = () =>
                {

                    for (int i = fromLocal; i < toLocal; i++)
                    {
                        ids[i] = Convert.ToInt32(tableRows[i].Attributes["Id"].Value);
                        postIds[i] = Convert.ToInt32(tableRows[i].Attributes["PostId"].Value);
                        userIds[i] = Convert.ToInt32(tableRows[i].Attributes["UserId"]?.Value);
                        scores[i] = Convert.ToInt32(tableRows[i].Attributes["Score"].Value);
                        texts[i] = tableRows[i].Attributes["Text"].Value;
                        contentLicenses[i] = tableRows[i].Attributes["ContentLicense"].Value;
                        // CreationDate ? data type
                    }

                    //ConsolePrinter.PrintLine($"Done from {fromLocal} to {toLocal}", ConsoleColor.Green);
                };

                tasks.Add(Task.Factory.StartNew(action));

                from = to;
                to += chunk;

                if (to > tableRows.Count)
                {
                    to = tableRows.Count;
                }
                if (from >= tableRows.Count)
                {
                    break;
                }
            }

            Task.WaitAll(tasks.ToArray());



            OracleParameter id = new OracleParameter();
            id.OracleDbType = OracleDbType.Int32;
            id.Value = ids;

            OracleParameter postId = new OracleParameter();
            postId.OracleDbType = OracleDbType.Int32;
            postId.Value = postIds;

            OracleParameter userId = new OracleParameter();
            userId.OracleDbType = OracleDbType.Int32;
            userId.Value = userIds;

            OracleParameter score = new OracleParameter();
            score.OracleDbType = OracleDbType.Int32;
            score.Value = scores;

            OracleParameter text = new OracleParameter();
            text.OracleDbType = OracleDbType.NVarchar2;
            text.Value = texts;

            OracleParameter contentLicense = new OracleParameter();
            contentLicense.OracleDbType = OracleDbType.NVarchar2;
            contentLicense.Value = contentLicenses;

            // CreationDate ? data type

            // ===============================

            OracleConnection connection = new OracleConnection(ConnectionString);
            connection.Open();

            OracleCommand cmd = new OracleCommand();
            cmd.Connection = connection;
            cmd.CommandText = $"insert into {DbName}.Comments ( id, postId, score, userId, text, contentLicense) " +
                                                     $"values (:1, :2, :3, :4, :5, :6 )";
            cmd.ArrayBindCount = ids.Length;
            cmd.Parameters.Add(id);
            cmd.Parameters.Add(postIds);
            cmd.Parameters.Add(scores);
            cmd.Parameters.Add(userIds);
            cmd.Parameters.Add(texts);
            cmd.Parameters.Add(contentLicenses);
            cmd.ExecuteNonQuery();
        }


        /// <summary>
        /// FillUsersTable uses the Oracle Bulk copy functionality
        /// </summary>
        /// <param name="tableRows"></param>
        public void FillUsersTable(XmlNodeList tableRows)
        {

            DataTable dt = new DataTable();
            dt.Columns.Add("Id");
            dt.Columns.Add("Reputation");
            //CreationDate
            //LastAccessDate
            dt.Columns.Add("DisplayName");
            dt.Columns.Add("WebsiteUrl");
            dt.Columns.Add("Location");
            dt.Columns.Add("AboutMe");
            dt.Columns.Add("Views");
            dt.Columns.Add("UpVotes");
            dt.Columns.Add("DownVotes");
            dt.Columns.Add("AccountId");
            dt.Columns.Add("ProfileImageUrl");


            for (int i = 0; i < tableRows.Count; i++)
            {
                DataRow dr = dt.NewRow();
                dr["Id"] = Convert.ToInt32(tableRows[i].Attributes["Id"].Value);
                dr["Reputation"] = Convert.ToInt32(tableRows[i].Attributes["Reputation"].Value);
                dr["DisplayName"] = tableRows[i].Attributes["DisplayName"].Value;
                dr["WebsiteUrl"] = tableRows[i].Attributes["WebsiteUrl"].Value;
                dr["Location"] = tableRows[i].Attributes["Location"].Value;
                dr["AboutMe"] = tableRows[i].Attributes["AboutMe"].Value;
                dr["Views"] = Convert.ToInt32(tableRows[i].Attributes["Views"].Value);
                dr["UpVotes"] = Convert.ToInt32(tableRows[i].Attributes["UpVotes"].Value);
                dr["DownVotes"] = Convert.ToInt32(tableRows[i].Attributes["DownVotes"].Value);
                dr["AccountId"] = Convert.ToInt32(tableRows[i].Attributes["AccountId"].Value);
                dr["ProfileImageUrl"] = tableRows[i].Attributes["ProfileImageUrl"].Value;
                //CreationDate
                //LastAccessDate

                dt.Rows.Add(dr);
            }

            try
            {
                using (var connection = new OracleConnection(ConnectionString))
                {
                    connection.Open();
                    using (var bulkCopy = new OracleBulkCopy(connection, OracleBulkCopyOptions.UseInternalTransaction))
                    {
                        bulkCopy.DestinationTableName = $"{DbName}.Users";
                        bulkCopy.BulkCopyTimeout = 600;
                        bulkCopy.WriteToServer(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                ConsolePrinter.PrintLine($"Method {nameof(FillUsersTable)} failed when inserting bulp to the table {DbName}.Users!", ConsoleColor.Red);
                return;
            }

        }



        public void FillPostsTable(string fileName)
        {
            XmlRootAttribute xRoot = new XmlRootAttribute();
            xRoot.ElementName = "posts";
            xRoot.IsNullable = false;

            XmlSerializer serializer =
                    new XmlSerializer(typeof(Posts), xRoot);

            Posts posts;

            using (Stream reader = new FileStream(fileName, FileMode.Open))
            {
                // Call the Deserialize method to restore the object's state.
                posts = (Posts)serializer.Deserialize(reader);
            }



        }

        public void FillVotesTable(XmlNodeList table)
        {

        }
    }

    [XmlRoot("posts")]
    public class Posts
    {
        [XmlElement("row")]
        public List<Row> Rows { get; set; }
    }

    public class Row
    {
        [XmlAttribute("Id")]
        public int Id { get; set; }

        [XmlAttribute("PostTypeId")]
        public int PostTypeId { get; set; }

        [XmlAttribute("AcceptedAnswerId")]
        public int AcceptedAnswerId { get; set; }

        [XmlAttribute("Score")]
        public int Score { get; set; }

        [XmlAttribute("ViewCount")]
        public int ViewCount { get; set; }

        [XmlAttribute("Body")]
        public string Body { get; set; }

        [XmlAttribute("OwnerUserId")]
        public int OwnerUserId { get; set; }

        [XmlAttribute("LastEditorUserId")]
        public int LastEditorUserId { get; set; }

        [XmlAttribute("Title")]
        public string Title { get; set; }

        [XmlAttribute("AnswerCount")]
        public int AnswerCount { get; set; }

        [XmlAttribute("CommentCount")]
        public int CommentCount { get; set; }

        [XmlAttribute("FavoriteCount")]
        public int FavoriteCount { get; set; }

        [XmlAttribute("ContentLicense")]
        public string ContentLicense { get; set; }
    }
}
