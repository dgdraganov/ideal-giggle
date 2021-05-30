using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ideal_giggle
{
    public class DataManager
    {
        public DataManager(string folderName, params string[] fileNames)
        {
            FolderName = folderName;
            FileNames = fileNames;
        }

        public string FolderName { get; }
        public string[] FileNames { get; }


        public bool CheckIfDataExists()
        {
            var dir = Path.Combine(Environment.CurrentDirectory, @$"..\..\..\..\..\{FolderName}");
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

    }
}
