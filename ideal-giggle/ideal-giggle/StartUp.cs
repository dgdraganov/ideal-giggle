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

        }


    }
}
