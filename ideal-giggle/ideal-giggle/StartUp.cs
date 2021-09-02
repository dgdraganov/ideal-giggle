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
            if (!Directory.Exists(filesDirectory))
            {
                var dirInfo = Directory.CreateDirectory(filesDirectory);
                ConsolePrinter.PrintLine($"Directory '{dirInfo.FullName}' was missing and it has been created! Make sure the following files are present in the directory before restarting the program:", ConsoleColor.Red);
                return;
            }
            if (!Directory.GetFiles(filesDirectory).Any())
            {
                ConsolePrinter.PrintLine("No files in the DbData directory!", ConsoleColor.Red);
                return;
            }

            InsertionManager iManager = new InsertionManager(filesDirectory);

            OracleAdapter oAdapt = new OracleAdapter();
            MongoAdapter mAdapt = new MongoAdapter();

            iManager.AddAdapter(oAdapt);
            iManager.AddAdapter(mAdapt);

            iManager.FillDatabases();

        }


    }
}
