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


            // ---------------

            var dir = Path.Combine(Environment.CurrentDirectory, @$"..\..\..\..\..\DbData");

            string[] tableNames = new string[] { /*"Posts",*/ "Comments", "Votes", "Users", /*"UsersBadges", "Badges", "Tags"*/ };

            DataManager dm = new DataManager(tableNames);

            var succ = dm.CheckIfDataExists(dir);

            if (!succ)
                return;

            Dictionary<string, string> fileNames = tableNames
                                                    .ToDictionary(x => x, x => $"{dir}\\{x}.xml");


            // Inserting to Oracle DB
            OracleAdapter adapter =
            new OracleAdapter();


            //var rowsRead = 0;
            //IEnumerable<Votes.Row> voteRows = null;
            int totalRowsRead = 0;
            while (true)
            {
                //    USE     DYNAMIC !!!!

                Votes data = dm.DeserializeByChunks<Votes>(@"C:\Users\draga\Desktop\csProj\ideal-giggle\DbData\Votes.xml",
                                                                 totalRowsRead);

                adapter.FillGenericTable<Votes>(data);

                // If nothing to process - break
                if (data.Rows.Count == 0)
                    break;

                Console.WriteLine(data.Rows.Last().Id);
                Console.WriteLine("Processing...");
                totalRowsRead += data.Rows.Count;
            }




            //Posts posts =               dm.DeserializeToObject<Posts>(fileNames[nameof(Posts)]);
            Users users = dm.DeserializeToObject<Users>(fileNames[nameof(Users)]);
            Votes votes = dm.DeserializeToObject<Votes>(fileNames[nameof(Votes)]);
            Comments comments = dm.DeserializeToObject<Comments>(fileNames[nameof(Comments)]);
            UsersBadges usersBadges = dm.DeserializeToObject<UsersBadges>(fileNames[nameof(UsersBadges)]);
            Badges badges = dm.DeserializeToObject<Badges>(fileNames[nameof(Badges)]);
            Tags tags = dm.DeserializeToObject<Tags>(fileNames[nameof(Tags)]);

            Stopwatch sw = new Stopwatch();


         

            sw.Start();
            //adapter.FillPostsTable(posts);
            adapter.FillUsersTable(users);
            adapter.FillVotesTable(votes);
            adapter.FillCommentsTable(comments);
            adapter.FillUsersBadgesTable(usersBadges);
            adapter.FillBadgesTable(badges);
            adapter.FillTagsTable(tags);

            sw.Stop();
            ConsolePrinter.PrintLine($"All data filled to the Oracle DB! Time required for all data to be inserted: {sw.Elapsed}");
            sw.Reset();

            // Inserting to Мongo DB
            MongoAdapter ma = new MongoAdapter();

            sw.Start();
            //ma.FillPostsTable(dm.DeserializeToObject<Posts>(fileNames[nameof(Posts)]));
            ma.FillVotesTable(votes);
            ma.FillUsersTable(users);
            ma.FillCommentsTable(comments);
            ma.FillBadgesTable(badges);
            ma.FillUsersBadgesTable(usersBadges);
            ma.FillTagsTable(tags);

            sw.Stop();

            ConsolePrinter.PrintLine($"All data filled to the Mongo DB! Time required for all data to be inserted: {sw.Elapsed}");

        }


        public static void InsertData<T>(DataManager dMngr, OracleAdapter oAdapt)
        {

            //int totalRowsRead = 0;
            //while (true)
            //{
            //    T data = dMngr.DeserializeByChunks<T>(@"C:\Users\draga\Desktop\csProj\ideal-giggle\DbData\Votes.xml",
            //                                                     totalRowsRead);
            //    // If nothing to process - break
            //    if (data.Rows.Count == 0)
            //        break;

            //    Console.WriteLine(data.Rows.Last().Id);
            //    Console.WriteLine("Processing...");
            //    totalRowsRead += data.Rows.Count;
            //}

        }
    }
}
