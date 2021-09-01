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
        public DataManager()
        {
            FilesDirectory = Path.Combine(Environment.CurrentDirectory, @$"..\..\..\..\..\DbData");
            TableNames = Directory.GetFiles(FilesDirectory).Select(f => f.Split('\\').Last().Split('.')[0]).ToArray();
            FilesPaths = TableNames.ToDictionary(x => x, x => $"{FilesDirectory}\\{x}.xml");
        }

        public string FilesDirectory { get; }
        public string[] TableNames { get; }
        public IDictionary<string, string> FilesPaths { get; }


        public bool CheckIfDataExists()
        {

            if (!Directory.Exists(FilesDirectory))
            {
                var dirInfo = Directory.CreateDirectory(FilesDirectory);

                ConsolePrinter.PrintLine($"Directory '{dirInfo.FullName}' was missing and it has been created! Make sure the following files are present in the directory before restarting the program:", ConsoleColor.Red);
                ConsolePrinter.PrintLine($"{string.Join(", ", TableNames)}");
                return false;
            }

            var files = Directory.GetFiles(FilesDirectory).Select(f => f.Split("\\").Last()).ToArray();

            foreach (var fileName in TableNames)
            {
                var currentFile = string.Concat(fileName, ".xml");
                if (!files.Contains(currentFile))
                {
                    ConsolePrinter.PrintLine($"{currentFile} is missing!\nMake sure the following files are present in the directory before restarting the program:", ConsoleColor.Red);
                    ConsolePrinter.PrintLine($"{string.Join(", ", TableNames)}");
                    return false;
                }
            }

            return true;
        }

        public T DeserializeByChunks<T>(string fileName,
                                            int linesToSkip,
                                            int linesToRead)
        {
            XmlSerializer serializer =
                  new XmlSerializer(typeof(List<T>));
            T obj;

            using (StreamReader xmlReader = new StreamReader(File.OpenRead(fileName)))
            {
                var tableName = typeof(T).Name.ToLower();
                var openingTag = $"<{tableName}>";
                var closingTag = $"</{tableName}>";

                // Skip lines that are already read
                for (int i = 0; i < linesToSkip + 2; i++)
                    xmlReader.ReadLine();
                

                StringBuilder sb = new StringBuilder();
                sb.AppendLine(openingTag);

                var linesRead = 0;
                string row = null;

                while ((row = xmlReader.ReadLine()) != null && 
                            linesRead < linesToRead)
                {
                    sb.Append(row);
                    linesRead++;
                }
               
                if (row != null && row != closingTag)
                {
                    sb.Append(closingTag);
                }

                var resultString = sb.ToString();
                byte[] byteArray = Encoding.ASCII.GetBytes(resultString);
             
                using (StreamReader reader = new StreamReader(new MemoryStream(byteArray)))
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(T)/*, new XmlRootAttribute(tableName)*/);
                    var parsedLines = (T)deserializer.Deserialize(reader);

                    return parsedLines;
                }

            }

        }




        public T DeserializeToObject<T>(string fileName) where T : class
        {
            XmlSerializer serializer =
                   new XmlSerializer(typeof(T));

            T obj;
            using (Stream reader = File.OpenRead(fileName))
            {
                obj = (T)serializer.Deserialize(reader);
            }
            return obj;
        }



    }
}
