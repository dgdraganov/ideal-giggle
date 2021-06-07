using System;
using System.Diagnostics;
using System.IO;

namespace ideal_giggle
{
    class Program
    {
        static void Main(string[] args)
        {
         
            var dir = Path.Combine(Environment.CurrentDirectory, @$"..\..\..\..\..\DbData");

            DataManager dm = new DataManager("Users", "Posts", "Comments", "Votes");

            var succ = dm.CheckIfDataExists(dir);

            if (!succ)
                return;

            // All files are present and the data loading may begin
            var fileNamePosts =     @$"{dir}\Posts.xml";
            var fileNameUsers =     @$"{dir}\Users.xml";
            var fileNameVotes =     @$"{dir}\Votes.xml";
            var fileNameComments =  @$"{dir}\Comments.xml";

            OracleAdapter adapter = 
                new OracleAdapter("...fakeConnectionString...", "...KurecDb...");
        
            adapter.FillPostsTable(dm.DeserializeToObject<Posts>(fileNamePosts));
            adapter.FillUsersTable(dm.DeserializeToObject<Users>(fileNameUsers));
            adapter.FillVotesTable(dm.DeserializeToObject<Votes>(fileNameVotes));
            adapter.FillCommentsTable(dm.DeserializeToObject<Comments>(fileNameComments));

       


        }
    }
}
