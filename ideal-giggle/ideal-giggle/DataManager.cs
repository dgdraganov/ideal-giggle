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
        public DataManager(string[] fileNames)
        {
            FileNames = fileNames;
        }

        public string[] FileNames { get; }


        public bool CheckIfDataExists(string dir)
        {

            if (!Directory.Exists(dir))
            {
                var dirInfo = Directory.CreateDirectory(dir);

                ConsolePrinter.PrintLine($"Directory '{dirInfo.FullName}' was missing and it has been created! Make sure the following files are present in the directory before restarting the program:", ConsoleColor.Red);
                ConsolePrinter.PrintLine($"{string.Join(", ", FileNames)}");
                return false;
            }

            var files = Directory.GetFiles(dir).Select(f => f.Split("\\").Last()).ToArray();

            foreach (var fileName in FileNames)
            {
                var currentFile = string.Concat(fileName, ".xml");
                if (!files.Contains(currentFile))
                {
                    ConsolePrinter.PrintLine($"{currentFile} is missing!\nMake sure the following files are present in the directory before restarting the program:", ConsoleColor.Red);
                    ConsolePrinter.PrintLine($"{string.Join(", ", FileNames)}");
                    return false;
                }
            }

            return true;
        }

        public T DeserializeByChunks<T>(string fileName,
                                            int linesToSkip)
        {
            const int LINES_TO_READ = 10_000;

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
                            linesRead < LINES_TO_READ)
                {
                    sb.Append(row);
                    linesRead++;
                }
               
                if (row != null && row != closingTag)
                {
                    sb.Append(closingTag);
                }

                /*     I M P O R T A N T
                    
                Check empty case: 
                    <votes>
                    </votes>
                 
                 */

                var resultString = sb.ToString();
                byte[] byteArray = Encoding.ASCII.GetBytes(resultString);
             

                // convert stream to string
                
                using (StreamReader reader = new StreamReader(new MemoryStream(byteArray)))
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(T)/*, new XmlRootAttribute(tableName)*/);
                    var parsedLines = (T)deserializer.Deserialize(reader);

                    return parsedLines;
                }
                //parsedLines = list;

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
