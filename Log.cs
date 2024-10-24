using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mbNES
{
    public static class Log
    {
        public static Queue<string> mbNesLog;
        private const int _maxEntries = 100;
        static Log()
        {
            mbNesLog = new Queue<string>();
        }

        public static void AddToLog(string logEntry)
        {
            mbNesLog.Enqueue(logEntry);
            if (mbNesLog.Count > _maxEntries)                   // If the log is too big
            {
                mbNesLog.Dequeue();
            }
        }

        public static string GetLog()
        {
            string logContents = "";

            foreach(string logEntry in mbNesLog)                // Put the log entries in a string to be returned
            {
                logContents += logEntry;
            }

            return logContents;
        }

    }
}
