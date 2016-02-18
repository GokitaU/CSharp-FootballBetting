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
    class ParsedData
    {

        private MainWindow MainF;
        public ParsedData(MainWindow MainF)
        {
            this.MainF = MainF;
        }
        private bool BUSY;
        public bool busy { set { this.BUSY = value; } get { return this.BUSY; } }
        public void CheckForNewFiles()
        {

            if (FootballHedge.Properties.Settings.Default.Dir1) CheckDirectory(FootballHedge.Properties.Settings.Default.Dir1Path);
            if (FootballHedge.Properties.Settings.Default.Dir2) CheckDirectory(FootballHedge.Properties.Settings.Default.Dir2Path);
            busy = false;
        }

        private void CheckDirectory(string path)
        {
            string[] files;
            try
            {
                files = Directory.GetFiles(path);
            }
            catch (Exception)
            {
                if (path == FootballHedge.Properties.Settings.Default.Dir1Path) FootballHedge.Properties.Settings.Default.Dir1 = false;
                if (path == FootballHedge.Properties.Settings.Default.Dir2Path) FootballHedge.Properties.Settings.Default.Dir2 = false;
                MainF.ErrorLabel(path); 
                return;
            }

            foreach (string fname in files)
            {
                StreamReader ReadFile;
                try { ReadFile = new StreamReader(fname); }
                catch (Exception) { continue; }


                string str = ReadFile.ReadToEnd();

                ReadFile.Close();

                if (fname.Contains(".mht"))
                {
                    str = str.Replace("=\r\n", "");
                    str = str.Replace("=3D", "=");
                }
   
                

               if( ParseSource(str, fname))
               {
                   try
                   {
                       File.Delete(fname);
                   }
                   catch (Exception)
                   {
                       WriteBreakToFile.LogParseSave("Can't delete file " + fname);
                       //MessageBox.Show("Can't delete " + fname);
                   }
               }
               else
               {
                   try
                   {
                       //string destination = fname.Replace(@"\Football", @"\New Folder");
                       //File.Move(fname, destination);
                       File.Delete(fname);
                   }
                   catch (Exception)
                   {
                       WriteBreakToFile.LogParseSave("Can't delete file " + fname);
                      // WriteBreakToFile.LogParseSave("File.Move Error " + fname);
                   }
               }
            }
        }

        public bool ParseSource(string str, string fname)
        {
            int key = MainF.Bookmakers.FindIndex(item => fname.ToUpper().Contains(item.Name.ToUpper()));

            if (key == -1) { MainF.ErrorLabel("Unidentified "+fname); return false; }

            ParseBookies book = MainF.Bookmakers[key].Book;
            MainF.LastBrokerTestLabel(MainF.Bookmakers[key].Name);

            List<ParsedDataFromPage> ParsDataList = new List<ParsedDataFromPage>();

            if (book.Parse(str, ref ParsDataList))
            {
              MainF.UpdateDataBase(ParsDataList, key);
            }
            else { MainF.ErrorLabel(fname); WriteBreakToFile.LogParseSave("Error Parsing " + fname); return false; }
            return true;
        }
    }
}
