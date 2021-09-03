using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ideal_giggle
{

    class Program
    {
        static void Main(string[] args)
        {
            string filesDirectory = 
                Path.Combine(Environment.CurrentDirectory, @$"..\..\..\..\..\DbData");

            bool ok = CheckFilesDirectory(filesDirectory);
            if (!ok)
                return;

            var logDirectory = @".\log.txt";
            var logger = new Logger(logDirectory);

            InsertionManager iManager = new InsertionManager(filesDirectory, logger);
            OracleAdapter oAdapt = new OracleAdapter(logger);
            MongoAdapter mAdapt = new MongoAdapter(logger);

            iManager.AddAdapter(oAdapt);
            iManager.AddAdapter(mAdapt);

            iManager.FillDatabases();
        }

        private static bool CheckFilesDirectory(string filesDirectory)
        {
            if (!Directory.Exists(filesDirectory))
            {
                var dirInfo = Directory.CreateDirectory(filesDirectory);
                ConsolePrinter.PrintLine($"Directory '{dirInfo.FullName}' was missing and it has been created! Make sure the following files are present in the directory before restarting the program:", ConsoleColor.Red);
                return false;
            }
            if (!Directory.GetFiles(filesDirectory).Any())
            {
                ConsolePrinter.PrintLine("No files in the DbData directory!", ConsoleColor.Red);
                return false;
            }
            return true;
        }
    }
}
