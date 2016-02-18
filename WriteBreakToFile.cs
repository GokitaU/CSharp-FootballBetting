using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootballHedge
{
    static class WriteBreakToFile
    {
        public static void LogSave(string line)
        {
            try
            {
                if (!FootballHedge.Properties.Settings.Default.WriteLog) return;
                string LogFilePath = @"log.txt";
                StreamWriter filewrite = new StreamWriter(LogFilePath, true);
                filewrite.WriteLine(DateTime.Now.ToString("HH:mm") + "  " + line);
                filewrite.Close();
            }
            catch (Exception)
            {
                return;
            }

        }
        public static void LogParseSave(string line)
        {
            try
            {
                if (!FootballHedge.Properties.Settings.Default.WriteLog) return;
                string LogFilePath = @"Parselog.txt";
                StreamWriter filewrite = new StreamWriter(LogFilePath, true);
                filewrite.WriteLine(DateTime.Now.ToString("HH:mm") + "  " + line);
                filewrite.Close();
            }
            catch (Exception)
            {
                return;
            }

        }
        public static void ClearFile()
        {
            string LogFilePath = @"log.txt";
            StreamWriter filewrite = new StreamWriter(LogFilePath, false);
            filewrite.Close();
            LogFilePath = @"Parselog.txt";
            StreamWriter filewritelog = new StreamWriter(LogFilePath, false);
            filewritelog.Close();
        }
    }
}
