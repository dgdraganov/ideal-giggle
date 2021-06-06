using System;
using System.IO;

namespace ideal_giggle
{
    class Program
    {
        static void Main(string[] args)
        {
            DataManager dm = new DataManager("DbData", "Users", "Posts", "Comments", "Votes");

            var succ = dm.CheckIfDataExists();

            if (!succ)
                return;

            // All files are present and the data loading may begin
            var fileNameComments = @"C:\Users\draga\Desktop\csProj\ideal-giggle\DbData\Comments.xml";
            var fileNamePosts = @"C:\Users\draga\Desktop\csProj\ideal-giggle\DbData\Posts.xml";
            var xmlRowsComments = dm.GetTableRows(fileNameComments);
            var xmlRowsPosts = dm.GetTableRows(fileNamePosts);

            OracleAdapter adapter = new OracleAdapter("fakeConnectionString", "KurecDb");

            adapter.FillPostsTable(fileNamePosts);
            adapter.FillCommentsTable(xmlRowsComments);
            adapter.FillUsersTable(xmlRowsPosts);


        }
    }
}
