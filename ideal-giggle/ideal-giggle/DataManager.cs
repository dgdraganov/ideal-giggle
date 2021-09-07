using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ideal_giggle
{
    public class DataManager
    {

        public DataManager(string filesDirectory, Logger logger)
        {
            Logger = logger;
            FilesDirectory = filesDirectory;
            TableNames = Directory.GetFiles(FilesDirectory).Select(f => f.Split('\\').Last().Split('.')[0]).ToArray();
            FilesPaths = TableNames.ToDictionary(x => x, x => $"{FilesDirectory}\\{x}.xml");
        }
        private Logger Logger { get; }
        public string FilesDirectory { get; }
        public string[] TableNames { get; }
        public IDictionary<string, string> FilesPaths { get; }

        public T DeserializeByChunks<T>(string fileName,
                                            int linesToSkip,
                                            int maxChunkMemory,
                                            int maxChunkRows)
        {

            XmlSerializer serializer =
                  new XmlSerializer(typeof(List<T>));
            T obj;

            var tableName = typeof(T).Name.ToLower();
            var openingTag = $"<{tableName}>";
            var closingTag = $"</{tableName}>";

            using (var xmlReader = 
                         new StreamReader(File.OpenRead(fileName)))
            {
                // Skip lines that are already read
                for (int i = 0; i < linesToSkip + 2; i++)
                    xmlReader.ReadLine();
                
                var sb = new StringBuilder();

                // Start with opening xml tag
                sb.AppendLine(openingTag);

                var volumeRead = 0;
                string row = null;
                var rowsRead = 0;

                while ((row = xmlReader.ReadLine()) != null
                           && volumeRead < maxChunkMemory
                           && rowsRead < maxChunkRows)
                {
                    sb.Append(row);
                    volumeRead += Encoding.ASCII.GetByteCount(row);
                }
               
                // If reading stopped and the end tag is not 
                // reached - add closing tag before serialization
                if (row != null && row != closingTag)
                    sb.Append(closingTag);
                
                var resultString = sb.ToString();
                var byteArray = Encoding.ASCII.GetBytes(resultString);

                try
                {
                    // Make a stream reader out of the result string
                    // and deserialize is to object T
                    using (var reader = new StreamReader(new MemoryStream(byteArray)))
                    {
                        XmlSerializer deserializer = new XmlSerializer(typeof(T));
                        var resultObject = (T)deserializer.Deserialize(reader);
                        return resultObject;
                    }
                }
                catch (Exception ex)
                {
                    ConsolePrinter.PrintLine($"Error when deserializing data from table {typeof(T).Name}!");
                    Logger.Log(LogLevel.Error, $"Exception has been thrown by {nameof(DeserializeByChunks)} method!", ex);

                    return default(T);
                }
              
            }
        }
    }
}
