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
            string filesDirectory = 
                Path.Combine(Environment.CurrentDirectory, @$"..\..\..\..\..\DbData");
            InsertionManager iManager = new InsertionManager(filesDirectory);

            OracleAdapter oAdapt = new OracleAdapter();
            MongoAdapter mAdapt = new MongoAdapter();

            iManager.AddAdapter(oAdapt);
            iManager.AddAdapter(mAdapt);

            iManager.FillDatabases();

        }


    }
}
