using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ideal_giggle
{
    public class Logger
    {
        public string LogDirectory { get; }
        public Logger(string logDir)
        {
            LogDirectory = logDir;
        }
        public void Log(LogLevel state, string message)
        {
            if (!File.Exists(LogDirectory))
                File.Create(LogDirectory);

            using (var streamWriter = new StreamWriter(LogDirectory))
            {
                streamWriter.WriteLine($"{DateTime.Now}[{state.ToString().ToUpper()}]:\t {message}");
            }
        }

        public void Log(LogLevel state, string message, Exception ex)
        {
            var logMessage = $"{message} \n {ex.Message}";
            Log(state, logMessage);
        }
    }

    public enum LogLevel
    {
        Error,
        Info
    }
}
