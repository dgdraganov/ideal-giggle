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
        public InsertionManager(string[] tables,
                                params IDbAdapter[] adapters)
        {
            DataManager = new DataManager(tables);
            DbAdapters = adapters;
            Measurements = new Dictionary<string, Dictionary<string, long>>();
            Initialize();
        }

        public DataManager DataManager { get; }
        public IDbAdapter[] DbAdapters { get; }
        public Dictionary<string, Dictionary<string, long>> Measurements { get; }

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
        }


        public void InsertData<T>(string filePath)
        {
            int totalRowsRead = 0;
            while (true)
            {
                T data = DataManager.DeserializeByChunks<T>(filePath,
                                                             totalRowsRead);
                
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
                var adapterTypeName = adapt.GetType().Name;
                Measurements[adapterTypeName] = new Dictionary<string, long>();
                foreach (var table in DataManager.TableNames)
                {
                    Measurements[adapterTypeName][table] = 0;
                }
            }
        }

    }
}