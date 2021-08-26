using System;
using System.Collections.Generic;
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

            // Inserting to Oracle DB
            OracleAdapter adapter =
            new OracleAdapter();

            Posts posts = dm.DeserializeToObject<Posts>(fileNames[nameof(Posts)]);
            adapter.FillPostsTable(posts);

            Users users = dm.DeserializeToObject<Users>(fileNames[nameof(Users)]);
            adapter.FillUsersTable(users);

            Votes votes = dm.DeserializeToObject<Votes>(fileNames[nameof(Votes)]);
            adapter.FillVotesTable(votes);

            Comments comments = dm.DeserializeToObject<Comments>(fileNames[nameof(Comments)]);
            adapter.FillCommentsTable(comments);



            // Adding to mongo tables
            MongoAdapter ma = new MongoAdapter();
            ma.FillVotesTable(dm.DeserializeToObject<Votes>(fileNames[nameof(Votes)]));
            ma.FillUsersTable(dm.DeserializeToObject<Users>(fileNames[nameof(Users)]));

            ma.FillPostsTable(dm.DeserializeToObject<Posts>(fileNames[nameof(Posts)]));
            ma.FillVotesTable(dm.DeserializeToObject<Votes>(fileNames[nameof(Votes)]));


        }
    }
}
