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
        public InsertionManager()
        {
            DataManager = new DataManager();
            DbAdapters = new HashSet<IDbAdapter>();
            Measurements = new Dictionary<string, Dictionary<string, long>>();
            Initialize();
        }

        public DataManager DataManager { get; }
        public ICollection<IDbAdapter> DbAdapters { get; }
        public Dictionary<string, Dictionary<string, long>> Measurements { get; }

        public void PrintStatistics()
        {
            foreach (var adapter in Measurements.Keys)
            {
                ConsolePrinter.PrintLine($"\t{adapter}:", ConsoleColor.DarkYellow);
                foreach (var table in Measurements[adapter])
                {
                    ConsolePrinter.PrintLine($"\t\t{table.Key} - {TimeSpan.FromMilliseconds(table.Value)}", ConsoleColor.Green);
                }
            }
        }

        public void AddAdapter(IDbAdapter adapter)
        {
            DbAdapters.Add(adapter);
        }

        public void FillDatabases()
        {
            var fileNames = DataManager.FilesPaths;

            var insertDataMethod = typeof(InsertionManager)
                                                    .GetMethod("InsertData");

            foreach (var table in fileNames.Keys)
            {
                var typeOfTable = Assembly.GetExecutingAssembly().GetType($"ideal_giggle.{table}", true);
                var filePath = fileNames[table];
                insertDataMethod.MakeGenericMethod(new[] { typeOfTable })
                                 .Invoke(this, new[] { filePath });

            }

            PrintStatistics();
        }


        public void InsertData<T>(string filePath)
        {
            const int SIZE_OF_CHUNK = 1_000_000;
            const int MAX_LINES_TO_INSERT = 10_000_000;

            int totalRowsRead = 0;
            while (totalRowsRead < MAX_LINES_TO_INSERT)
            {
                T data = DataManager.DeserializeByChunks<T>(filePath,
                                                             totalRowsRead,
                                                             SIZE_OF_CHUNK);
                
                // If nothing to process - break
                var rowsRead = (data as dynamic).Rows.Count;
                if (rowsRead == 0)
                    break;

                foreach (var adapter in DbAdapters)
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    adapter.InsertToTable<T>(data);
                    sw.Stop();

                    var adapterName = adapter.GetType().Name;
                    var tableName = typeof(T).Name;

                    Measurements[adapterName][tableName] += sw.ElapsedMilliseconds;
                }

                totalRowsRead += rowsRead;
            }
        }


        private void Initialize()
        {
            var succ = DataManager.CheckIfDataExists();
            if (!succ)
                return;

          
            foreach (var adapt in DbAdapters)
            {
                //var adapterTypeName = adapt.GetType().Name;
                Measurements[adapt.Name] = new Dictionary<string, long>();
                foreach (var table in DataManager.TableNames)
                {
                    Measurements[adapt.Name][table] = 0;
                }
            }
        }

    }
}