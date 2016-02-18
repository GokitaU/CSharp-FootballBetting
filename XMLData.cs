using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;

namespace FootballHedge
{
    class XMLData
    {
        public static string pathAltTeamNames = @"AltTeamNames.xml";
        public static string pathAllMatches = @"foot.xml";
        public static string pathSettings = @"Settings.xml";

        public static void DeleteFiles()
        {
            File.Delete(pathAllMatches);
        }
        #region Saving / Loading App Settings
        public static void SaveSettings(MainWindow MainF)
        {
            XmlTextWriter xmlfile = new XmlTextWriter(pathSettings, Encoding.UTF8);
            xmlfile.Formatting = Formatting.Indented;
            xmlfile.WriteStartElement("Settings");
            xmlfile.WriteStartElement("Bookmakers");
            foreach (BasicBrokerData item in MainF.Bookmakers)
            {
                xmlfile.WriteStartElement("BookMaker");

                xmlfile.WriteStartElement("Name");
                xmlfile.WriteString(item.Name);
                xmlfile.WriteEndElement();

                xmlfile.WriteStartElement("Limit");
                xmlfile.WriteString(item.Limit.ToString());
                xmlfile.WriteEndElement();

                xmlfile.WriteStartElement("InUse");
                xmlfile.WriteString(item.InUse.ToString());
                xmlfile.WriteEndElement();

                xmlfile.WriteStartElement("Email");
                xmlfile.WriteString(item.EMailing.ToString());
                xmlfile.WriteEndElement();

                xmlfile.WriteStartElement("Dutch3Way");
                xmlfile.WriteString(item.Dutch3Way.ToString());
                xmlfile.WriteEndElement();

                xmlfile.WriteStartElement("EmailPLlimit");
                xmlfile.WriteString(item.PLlimit.ToString());
                xmlfile.WriteEndElement();

                xmlfile.WriteEndElement();

            }
            xmlfile.WriteEndElement();

            xmlfile.WriteStartElement("Leagues");
            foreach (LeagueState item in MainF.Leagues)
            {
                xmlfile.WriteStartElement("League");

                xmlfile.WriteStartElement("LeagueName");
                xmlfile.WriteString(item.Name);
                xmlfile.WriteEndElement();

                xmlfile.WriteStartElement("Value");
                xmlfile.WriteString(item.State.ToString());
                xmlfile.WriteEndElement();

                xmlfile.WriteEndElement();
            }
            xmlfile.WriteEndElement();
            xmlfile.WriteStartElement("SuspendedMatches");
            foreach (string str in MainF.SuspendedMatches)
            {
                xmlfile.WriteStartElement("Match");
                xmlfile.WriteString(str);
                xmlfile.WriteEndElement();
            }
           
            xmlfile.WriteEndElement();
            xmlfile.WriteEndElement();
            xmlfile.Close();
            
        }
        public static void LoadSettings(MainWindow MainF)
        {
            MainF.DefaultSettings();
            if (File.Exists(pathSettings))
            {
                XmlDocument xd = new XmlDocument();
                xd.Load(pathSettings);

                foreach (XmlNode item in xd.SelectNodes("Settings/Bookmakers/BookMaker"))
                {
                    var c =  item.SelectSingleNode(".//Name");
                    if(c==null) continue;

                    string name  = c.InnerText;
                    int index = MainF.Bookmakers.FindIndex(it => it.Name == name);

                    if (index < 0) continue;

                    c = item.SelectSingleNode(".//Limit");
                    string strbuf = "1";
                    if(c!=null) strbuf = c.InnerText;
                    double limit = 1;
                    if (strbuf != null && TryToParse.ParseDouble(strbuf, out limit)) { }


                    MainF.Bookmakers[index].Limit = limit;
                   
                    c = item.SelectSingleNode(".//InUse");
                    if (c != null) MainF.Bookmakers[index].InUse = Convert.ToBoolean(c.InnerText);

                    c = item.SelectSingleNode(".//Email");
                    if (c != null) MainF.Bookmakers[index].EMailing = Convert.ToBoolean(c.InnerText);

                    c = item.SelectSingleNode(".//Dutch3Way");
                    if (c != null) MainF.Bookmakers[index].Dutch3Way = Convert.ToBoolean(c.InnerText);

                    c = item.SelectSingleNode(".//EmailPLlimit");
                    if (c != null)
                    {
                        strbuf = c.InnerText;
                        if (strbuf != null && TryToParse.ParseDouble(strbuf, out limit) && limit > FootballHedge.Properties.Settings.Default.MailplLimit) 
                        {
                             MainF.Bookmakers[index].PLlimit = limit; 
                        }
                    }
                    
                }
                foreach (XmlNode item in xd.SelectNodes("Settings/Leagues/League"))
                {
                    string league = item.SelectSingleNode(".//LeagueName").InnerText;
                    bool value = Convert.ToBoolean(item.SelectSingleNode(".//Value").InnerText);
                    int index = MainF.Leagues.FindIndex(i => i.Name == league);
                    if(index != -1 ) MainF.Leagues[index].State = value;
                    else MainF.Leagues.Add(new LeagueState() { Name = league, State = value });
                }
                foreach (XmlNode item in xd.SelectNodes("Settings/SuspendedMatches/Match"))
                {
                    MainF.SuspendedMatches.Add(item.InnerText);
                }
            }

        }
        #endregion


        public static string ReturnAltTeamName(string team)
        {

            if (File.Exists(pathAltTeamNames))
            {

                using (XmlTextReader reader = new XmlTextReader(pathAltTeamNames))
                {
                    if (reader.ReadToFollowing("Team_" + team.Replace(" ", "_"))) return reader.ReadInnerXml();
                }
                AddNewTeam(team);
                return team;
            }
            else CreteAltTeamNamesFile();
            return "";
        }
        public static void UpdateAltTeamName(string team, string value)
        {

            if (File.Exists(pathAltTeamNames))
            {
                XmlDocument xd = new XmlDocument();
                xd.Load(pathAltTeamNames);

                XmlNode item = xd.SelectSingleNode("Teams/Team_" + team.Replace(" ", "_"));
                item.InnerText = value;

                xd.Save(pathAltTeamNames);
            }
            else CreteAltTeamNamesFile();
           
        }

        static void CreteAltTeamNamesFile()
        {
            XmlTextWriter xmlfile = new XmlTextWriter(pathAllMatches, Encoding.UTF8);
            xmlfile.Formatting = Formatting.Indented;
            xmlfile.WriteStartElement("Teams");

            xmlfile.WriteEndElement();
            xmlfile.Close();
        }

        static void AddNewTeam(string team)
        {
            XmlDocument xmlfile = new XmlDocument();
            xmlfile.Load(pathAltTeamNames);
            XmlNode Team = xmlfile.CreateElement("Team_"+team.Replace(" ","_"));
            Team.InnerText = team;

            xmlfile.DocumentElement.AppendChild(Team);
            xmlfile.Save(pathAltTeamNames);
        }

        public static void UpdateMainDataList(List<FData> FootData)
        {
            for (int id = FootData.Count - 1; id >= 0; id--)
            {
                FData it = FootData[id];
                string RepPatter = FootballHedge.Properties.Settings.Default.RepPattern;

                it.Match = Regex.Replace(ReturnAltTeamName(it.Team1) + "," + ReturnAltTeamName(it.Team2), RepPatter, "").ToUpper();
            }
        }

        public void Save(MainWindow MainF, List<FData> FootData)
        {
            XmlTextWriter xmlfile = new XmlTextWriter(pathAllMatches, Encoding.UTF8);
            xmlfile.Formatting = Formatting.Indented;
            xmlfile.WriteStartElement("Data");
            for (int i = FootData.Count - 1; i >= 0; i--)
            {
                FData item = FootData[i];
                xmlfile.WriteStartElement("Match");

                xmlfile.WriteStartElement("League");
                xmlfile.WriteString(item.League);
                xmlfile.WriteEndElement();

                xmlfile.WriteStartElement("Team1");
                xmlfile.WriteString(item.Team1);
                xmlfile.WriteEndElement();

                xmlfile.WriteStartElement("Team2");
                xmlfile.WriteString(item.Team2);
                xmlfile.WriteEndElement();

                xmlfile.WriteStartElement("Time");
                xmlfile.WriteString(item.time.ToString());
                xmlfile.WriteEndElement();

                xmlfile.WriteStartElement("Match");
                xmlfile.WriteString(item.Match);
                xmlfile.WriteEndElement();
                xmlfile.WriteStartElement("BookieOdds");

                foreach (KeyValuePair<int, X> it in FootData[i].XKoef)
                {
                   

                    BasicBrokerData data = MainF.Bookmakers[it.Key];
                    xmlfile.WriteStartElement("B_" + data.Name);

                    xmlfile.WriteStartElement("LastUpdate");
                    xmlfile.WriteString(it.Value.LastUpdate);
                    xmlfile.WriteEndElement();

                    xmlfile.WriteStartElement("X1");
                    xmlfile.WriteString(it.Value.Koef[1].ToString());
                    xmlfile.WriteEndElement();

                    xmlfile.WriteStartElement("X0");
                    xmlfile.WriteString(it.Value.Koef[0].ToString());
                    xmlfile.WriteEndElement();

                    xmlfile.WriteStartElement("X2");
                    xmlfile.WriteString(it.Value.Koef[2].ToString());
                    xmlfile.WriteEndElement();



                    if (MainF.Bookmakers[it.Key].Type == BrokerType.LAY)
                    {
                        xmlfile.WriteStartElement("LX1");
                        xmlfile.WriteString(item.Lay[1].ToString());
                        xmlfile.WriteEndElement();

                        xmlfile.WriteStartElement("LX0");
                        xmlfile.WriteString(item.Lay[0].ToString());
                        xmlfile.WriteEndElement();

                        xmlfile.WriteStartElement("LX2");
                        xmlfile.WriteString(item.Lay[2].ToString());
                        xmlfile.WriteEndElement();
                    }



                    xmlfile.WriteEndElement();
                }
/*
                for (int id = 0; id < MainF.Bookmakers.Count; id++)
                {
                    if (item.Koef[id, 1] == 0) continue;
                    BasicBrokerData data = MainF.Bookmakers[id];
                    xmlfile.WriteStartElement("B_" + data.Name);

                    xmlfile.WriteStartElement("LastUpdate");
                    xmlfile.WriteString(item.LastUpdate[id]);
                    xmlfile.WriteEndElement();

                    xmlfile.WriteStartElement("X1");
                    xmlfile.WriteString(item.Koef[id, 1].ToString());
                    xmlfile.WriteEndElement();

                    xmlfile.WriteStartElement("X0");
                    xmlfile.WriteString(item.Koef[id, 0].ToString());
                    xmlfile.WriteEndElement();

                    xmlfile.WriteStartElement("X2");
                    xmlfile.WriteString(item.Koef[id, 2].ToString());
                    xmlfile.WriteEndElement();

                    xmlfile.WriteEndElement();
                }
                */

                xmlfile.WriteEndElement();
                xmlfile.WriteEndElement();
            }
            xmlfile.WriteEndElement();
            xmlfile.Close();
        }

        public void Load(List<FData> FootData, List<BasicBrokerData> Bookmakers)
        {

            if (File.Exists(pathAllMatches))
            {
                XmlDocument xd = new XmlDocument();
                xd.Load(pathAllMatches);
                FootData.Clear();
                foreach (XmlNode item in xd.SelectNodes("Data/Match"))
                {
                    FData f = new FData();



                    if (item.SelectSingleNode(".//Team1") == null) continue;
                    f.Team1 = item.SelectSingleNode(".//Team1").InnerText;

                    if (item.SelectSingleNode(".//Team2") == null) continue;
                    f.Team2 = item.SelectSingleNode(".//Team2").InnerText;


                    if (item.SelectSingleNode(".//Match") == null) continue;
                    f.Match = item.SelectSingleNode(".//Match").InnerText;

                    if (item.SelectSingleNode(".//League") == null) continue;
                    f.League = item.SelectSingleNode(".//League").InnerText;

                   
                    if( item.SelectSingleNode(".//Time") != null)
                    {
                        string time = time = item.SelectSingleNode(".//Time").InnerText;
                        try
                        {
                            DateTime buftime;
                            if (TryToParse.ParseDateTime(time, out buftime))
                                f.time = buftime;
                            
                        }
                        catch (FormatException)
                        {
                            f.time = DateTime.Today;                       
                        }
                       
                    }

                    for (int i = 0; i < Bookmakers.Count; i++)
                    {
                        BasicBrokerData data = Bookmakers[i];
                        if (item.SelectSingleNode(".//BookieOdds/B_" + data.Name + "/X0") == null) continue;
                        f.AddNewItenToDictionary(i);
                        if (item.SelectSingleNode(".//BookieOdds/B_" + data.Name + "/LastUpdate") != null)
                            f.XKoef[i].LastUpdate = item.SelectSingleNode(".//BookieOdds/B_" + data.Name + "/LastUpdate").InnerText;

                        double value = 1;
                        var x = item.SelectSingleNode(".//BookieOdds/B_" + data.Name + "/X0");
                       
                        if(x != null && TryToParse.ParseDouble(x.InnerText, out value)) { }
                        f.XKoef[i].Koef[0] = value;
                        
                        value = 1;
                        x = item.SelectSingleNode(".//BookieOdds/B_" + data.Name + "/X1");
                        if(x != null && TryToParse.ParseDouble(x.InnerText, out value)) { }
                        f.XKoef[i].Koef[1] = value;

                        value = 1;
                        x = item.SelectSingleNode(".//BookieOdds/B_" + data.Name + "/X2");
                        if(x != null && TryToParse.ParseDouble(x.InnerText, out value)) { }
                        f.XKoef[i].Koef[2] = value;

                        if(Bookmakers[i].Type == BrokerType.LAY)
                        {
                            value = 1;
                            x = item.SelectSingleNode(".//BookieOdds/B_" + data.Name + "/LX0");
                       
                            if(x != null && TryToParse.ParseDouble(x.InnerText, out value)) { }
                            f.Lay[0] = value;
                        
                            value = 1;
                            x = item.SelectSingleNode(".//BookieOdds/B_" + data.Name + "/LX1");
                            if(x != null && TryToParse.ParseDouble(x.InnerText, out value)) { }
                            f.Lay[1] = value;

                            value = 1;
                            x = item.SelectSingleNode(".//BookieOdds/B_" + data.Name + "/LX2");
                            if(x != null && TryToParse.ParseDouble(x.InnerText, out value)) { }
                            f.Lay[2] = value;
                        }

                    }

                    FootData.Add(f);
                }

            }

        }




    }
}
