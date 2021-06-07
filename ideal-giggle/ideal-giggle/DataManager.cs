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
        public DataManager(params string[] fileNames)
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


        public T DeserializeToObject<T>(string fileNamePosts) where T : class
        {
            XmlSerializer serializer =
                   new XmlSerializer(typeof(T));

            T obj;
            using (Stream reader = File.OpenRead(fileNamePosts))
            {
                obj = (T)serializer.Deserialize(reader);
            }
            return obj;
        }

    

    }
}
