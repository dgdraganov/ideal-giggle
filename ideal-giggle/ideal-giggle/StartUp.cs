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
            // Inserting to Oracle DB
            OracleAdapter oAdapt =
                new OracleAdapter();

            // Inserting to Мongo DB
            MongoAdapter mAdapt = 
                new MongoAdapter();

            string[] tableNames = new string[] { /*"Posts",*/ "Comments", "Votes", "Users", "UsersBadges", "Badges", "Tags" };

            InsertionManager im = new InsertionManager(tableNames, oAdapt, mAdapt);
            im.FillDatabases();

            //Posts posts =               dm.DeserializeToObject<Posts>(fileNames[nameof(Posts)]);
            //Users users = dm.DeserializeToObject<Users>(fileNames[nameof(Users)]);
            //Votes votes = dm.DeserializeToObject<Votes>(fileNames[nameof(Votes)]);
            //Comments comments = dm.DeserializeToObject<Comments>(fileNames[nameof(Comments)]);
            //UsersBadges usersBadges = dm.DeserializeToObject<UsersBadges>(fileNames[nameof(UsersBadges)]);
            //Badges badges = dm.DeserializeToObject<Badges>(fileNames[nameof(Badges)]);
            //Tags tags = dm.DeserializeToObject<Tags>(fileNames[nameof(Tags)]);

            //Stopwatch sw = new Stopwatch();

            //sw.Start();
            //adapter.FillPostsTable(posts);
            //adapter.FillUsersTable(users);
            //adapter.FillVotesTable(votes);
            //adapter.FillCommentsTable(comments);
            //adapter.FillUsersBadgesTable(usersBadges);
            //adapter.FillBadgesTable(badges);
            //adapter.FillTagsTable(tags);

            //sw.Stop();
            //ConsolePrinter.PrintLine($"All data filled to the Oracle DB! Time required for all data to be inserted: {sw.Elapsed}");
            //sw.Reset();



            //sw.Start();
            ////ma.FillPostsTable(dm.DeserializeToObject<Posts>(fileNames[nameof(Posts)]));
            //mAdapter.FillVotesTable(votes);
            //mAdapter.FillUsersTable(users);
            //mAdapter.FillCommentsTable(comments);
            //mAdapter.FillBadgesTable(badges);
            //mAdapter.FillUsersBadgesTable(usersBadges);
            //mAdapter.FillTagsTable(tags);

            //sw.Stop();

            //ConsolePrinter.PrintLine($"All data filled to the Mongo DB! Time required for all data to be inserted: {sw.Elapsed}");

        }


    }
}
