using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ideal_giggle
{
    class Program
    {
        static void Main(string[] args)
        {
         
            var dir = Path.Combine(Environment.CurrentDirectory, @$"..\..\..\..\..\DbData");

            string[] tableNames = new string[] { "Users", "Posts", "Comments", "Votes" };
            DataManager dm = new DataManager(tableNames);

            var succ = dm.CheckIfDataExists(dir);

            if (!succ)
                return;

            
            Dictionary<string, string> fileNames = tableNames.ToDictionary(x => x, x => $"{dir}\\{x}.xml");



            // Adding to oracle tables
            OracleAdapter adapter =
            new OracleAdapter();


            //Posts posts = dm.DeserializeToObject<Posts>(fileNamePosts);
            //adapter.FillPostsTable(posts);

            //Users users = dm.DeserializeToObject<Users>(fileNameUsers);
            //adapter.FillUsersTable(users);

            Votes votes = dm.DeserializeToObject<Votes>(fileNames[nameof(Votes)]);
            adapter.FillVotesTable(votes);

            //Comments comments = dm.DeserializeToObject<Comments>(fileNameComments);
            //adapter.FillCommentsTable(comments);





            // Adding to mongo tables
            //MongoAdapter ma = new MongoAdapter("mongodb://127.0.0.1:27017");
            //ma.AddVotes(dm.DeserializeToObject<Votes>(fileNameVotes));


        }
    }
}
