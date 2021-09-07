using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ideal_giggle
{
    public class InsertionManager
    {
        public InsertionManager(string filesDirectory, Logger logger)
        {
            Logger = logger;
            DataManager = new DataManager(filesDirectory, Logger);
            DbAdapters = new HashSet<IDbAdapter>();
            Measurements = new Dictionary<string, Dictionary<string, long>>();
        }
        private Logger Logger { get; }

        public DataManager DataManager { get; }
        public ICollection<IDbAdapter> DbAdapters { get; }
        public Dictionary<string, Dictionary<string, long>> Measurements { get; }

        public void PrintStatistics()
        {
            var allFilePaths = DataManager.FilesPaths;
            ConsolePrinter.PrintLine("\tFile sizes:");
            foreach (var file in allFilePaths)
            {
                var sizeInKB = new FileInfo(file.Value).Length / 1000;
                ConsolePrinter.PrintLine($"\t\t{file.Key}.xml\t\t  {sizeInKB} KB");
            }
            
            foreach (var adapter in Measurements.Keys)
            {
                ConsolePrinter.PrintLine($"\t{adapter}:", ConsoleColor.DarkYellow);
                long totalTime = 0;
                foreach (var table in Measurements[adapter])
                {
                    ConsolePrinter.PrintLine($"\t\t{table.Key} - \t\t{TimeSpan.FromMilliseconds(table.Value)}", ConsoleColor.Green);
                    totalTime += table.Value;
                }
                ConsolePrinter.PrintLine($"\t\tTotal time: \t\t{TimeSpan.FromMilliseconds(totalTime)}", ConsoleColor.Cyan);
            }

        }

        public void AddAdapter(IDbAdapter adapter)
        {
            DbAdapters.Add(adapter);

            Measurements[adapter.Name] = new Dictionary<string, long>();
            foreach (var table in DataManager.TableNames)
                Measurements[adapter.Name][table] = 0;
        }

        public void FillDatabases()
        {
            var filePaths = DataManager.FilesPaths;

            var insertDataMethod = typeof(InsertionManager)
                                                    .GetMethod("InsertData");

            foreach (var kvp in filePaths)
            {
                ConsolePrinter.PrintLine($"================ {kvp.Key} ================");
                var typeOfTable = Assembly.GetExecutingAssembly().GetType($"ideal_giggle.{kvp.Key}", true);
                var filePath = kvp.Value;
                insertDataMethod.MakeGenericMethod(new[] { typeOfTable })
                                 .Invoke(this, new[] { filePath });

            }

            PrintStatistics();
        }


        public void InsertData<T>(string filePath)
        {

            const int BYTES_TO_READ =       300_000_000;  // 300 MB
            const int TOTAL_ROWS_TO_READ = 100_000;
            const int MAX_CHUNK_ROWS =      25_000;

            int totalRowsRead = 0;

            while (totalRowsRead < TOTAL_ROWS_TO_READ)
            {
                T data = DataManager.DeserializeByChunks<T>(filePath,
                                                             totalRowsRead,
                                                             BYTES_TO_READ,
                                                             MAX_CHUNK_ROWS);
                if (data == null)
                    return;
                
                // If nothing to process - break
                var rowsRead = (data as dynamic).Rows.Count;
                if (rowsRead == 0)
                    break;

                totalRowsRead += rowsRead;

                foreach (var adapter in DbAdapters)
                {
                    var insertTime = adapter.InsertToTable<T>(data);

                    var adapterName = adapter.Name;
                    var tableName = typeof(T).Name;

                    Measurements[adapterName][tableName] += insertTime;
                }
                ConsolePrinter.PrintLine("-----------------------------");
            }
        }


    }
}