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
                new OracleAdapter("... fakeConnectionString ...", "... KurecDb ...");

            Posts posts = dm.DeserializeToObject<Posts>(fileNamePosts);
            adapter.FillPostsTable(posts);

            Users users = dm.DeserializeToObject<Users>(fileNameUsers);
            adapter.FillUsersTable(users);

            Votes votes = dm.DeserializeToObject<Votes>(fileNameVotes);
            adapter.FillVotesTable(votes);

            Comments comments = dm.DeserializeToObject<Comments>(fileNameComments);
            adapter.FillCommentsTable(comments);

       


        }
    }
}
