using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

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

            Posts posts =       dm.DeserializeToObject<Posts>(fileNames[nameof(Posts)]);
            Users users =       dm.DeserializeToObject<Users>(fileNames[nameof(Users)]);
            Votes votes =       dm.DeserializeToObject<Votes>(fileNames[nameof(Votes)]);
            Comments comments = dm.DeserializeToObject<Comments>(fileNames[nameof(Comments)]);


            Stopwatch sw = new Stopwatch();


            // Inserting to Oracle DB
            OracleAdapter adapter =
            new OracleAdapter();

            sw.Start();
            adapter.FillPostsTable(posts);
            adapter.FillUsersTable(users);
            adapter.FillVotesTable(votes);
            adapter.FillCommentsTable(comments);

            sw.Stop();
            ConsolePrinter.PrintLine($"All data filled to the Oracle DB! Time required for all data to be inserted: {sw.Elapsed}");
            sw.Reset();



            // Inserting to Мongo DB
            MongoAdapter ma = new MongoAdapter();

            sw.Start();
            ma.FillVotesTable(dm.DeserializeToObject<Votes>(fileNames[nameof(Votes)]));
            ma.FillUsersTable(dm.DeserializeToObject<Users>(fileNames[nameof(Users)]));
            ma.FillPostsTable(dm.DeserializeToObject<Posts>(fileNames[nameof(Posts)]));
            ma.FillCommentsTable(dm.DeserializeToObject<Comments>(fileNames[nameof(Comments)]));
            sw.Stop();

            ConsolePrinter.PrintLine($"All data filled to the Mongo DB! Time required for all data to be inserted: {sw.Elapsed}");


        }
    }
}
