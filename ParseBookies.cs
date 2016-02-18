using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using HtmlAgilityPack;

namespace FootballHedge
{
    interface IParseBookies
    {
        bool Parse(string page, ref List<ParsedDataFromPage> PD);
    }
    public class ParseBookies : IParseBookies
    {

        string LogFilePath = @"D:\zt\Rddd.txt";
        public string RepPatter = FootballHedge.Properties.Settings.Default.RepPattern;
        public virtual bool Parse(string page, ref List<ParsedDataFromPage> PD)
        {

            return true;
        }


        protected void LogSave(string line)
        {
            StreamWriter filewrite = new StreamWriter(LogFilePath, true);
            filewrite.WriteLine(line);
            filewrite.Close();
        }
        protected void DeleteLogFile()
        {
            if (File.Exists(LogFilePath)) File.Delete(LogFilePath);
        }
        StreamWriter filewrite;
        protected void CreateLogFile()
        {
            filewrite = new StreamWriter(LogFilePath);
        }

        public void WriteListToLogFile(List<ParsedDataFromPage> PDL)
        {
            filewrite = new StreamWriter(LogFilePath);
            foreach (ParsedDataFromPage item in PDL)
            {
                filewrite.WriteLine(string.Format("{0}-{1}  {2} {3} {4}", item.Team1, item.Team2, item.X[0], item.X[1], item.X[2]));
            }

            filewrite.Close();
        }

        protected void SaveToFile(string str)
        {
            filewrite.WriteLine(str);


        }
        protected void LogFileClose()
        {
            filewrite.Close();

        }

    }


}
