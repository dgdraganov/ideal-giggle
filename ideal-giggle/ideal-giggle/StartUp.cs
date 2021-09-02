using System;
using System.IO;
using System.Linq;

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

            var logDirectory = @".\log.txt";
            var logger = new Logger(logDirectory);

            InsertionManager iManager = new InsertionManager(filesDirectory, logger);
            OracleAdapter oAdapt = new OracleAdapter(logger);
            MongoAdapter mAdapt = new MongoAdapter(logger);

            iManager.AddAdapter(oAdapt);
            iManager.AddAdapter(mAdapt);

            iManager.FillDatabases();

        }
    }
}
