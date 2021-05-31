using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
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

        }

        public void FillCommentsTable(XmlNodeList tableRows)
        {
            OracleConnection connection = new OracleConnection(ConnectionString);
            connection.Open();

            int[] ids = new int[tableRows.Count];
            int[] postIds = new int[tableRows.Count];
            int[] userIds = new int[tableRows.Count];
            int[] scores = new int[tableRows.Count];
            string[] texts = new string[tableRows.Count];

            // CreationDate ? data type

            string[] contentLicenses = new string[tableRows.Count];


            for (int i = 0; i < tableRows.Count; i++)
            {
                ids[i] = Convert.ToInt32(tableRows[i].Attributes["Id"].Value);
                postIds[i] = Convert.ToInt32(tableRows[i].Attributes["PostId"].Value);
                userIds[i] = Convert.ToInt32(tableRows[i].Attributes["UserId"].Value);
                scores[i] = Convert.ToInt32(tableRows[i].Attributes["Score"].Value);
                texts[i] = tableRows[i].Attributes["Text"].Value;
                contentLicenses[i] = tableRows[i].Attributes["ContentLicense"].Value;
                // CreationDate ? data type
            }

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
        public void FillUsersTable(XmlNodeList table)
        {
            
        }
        public void FillPostsTable(XmlNodeList table)
        {

        }
        public void FillVotesTable(XmlNodeList table)
        {

        }
    }
}
