using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;

namespace FootballHedge
{
    class DataBase
    {
        
        public bool changecolor = false;
        MainWindow MainF;
        public DataBase(MainWindow MainF)
        {
            this.MainF = MainF;
        }
        protected double CalcLayPL(double x1, double x2)
        {
            if (x2 == 0) return 0;
            double ls = x2 < 2 ? x1 / (1.01 * x2 - 0.02) : x1 / x2;
          //  double rez = 0.99 * x1 / x2;
            return ls-ls*0.01;
        }
        protected double CalcHandicapPL(double x1, double x2)
        {
            return x1 - x1 / x2;

        }
        protected double Calc3WayDucth(double x0, double x1, double x2)
        {
            //double v0 = t0 == BrokerType.LAY ? 0.01 : 0;
            //double v1 = t1 == BrokerType.LAY ? 0.01 : 0;
            //double v2 = t2 == BrokerType.LAY ? 0.01 : 0;

            //double s1 = (x0 - v0 + v0 * 2) / (x1 - v1 + v1 * 2);
            //double s2 = (x0 - v0 + v0 * 2) / (x2 - v2 + v2 * 2);
            double s1 = x0 / x1;
            double s2 = x0 / x2;

            return 100 * (x0 - s1 - s2 - 1) / (1 + s1 + s2 );
        }
        protected string ChangeTimeToString(DateTime time)
        {
            if (time.Day == DateTime.Now.Day) return time.ToString("HH:mm") + " Today";
            return time.ToString("MMM-dd HH:mm");
        }
        public void UpLay(List<FData> FootData , List<ParsedDataFromPage>  PD, int id)
        {
               double MLimit = FootballHedge.Properties.Settings.Default.MailplLimit;

            foreach (ParsedDataFromPage item in PD)
            {
                int i;
                for (i = FootData.Count - 1; i >= 0; i--)
                {
                    if(FootData[i].Team1 == item.Team1 && FootData[i].Team2 == item.Team2)
                    {
                        if (item.delete)  FootData.RemoveAt(i);
                        else
                        {
                            FootData[i].SizeX[0] = item.Size[0];
                            FootData[i].SizeX[1] = item.Size[1];
                            FootData[i].SizeX[2] = item.Size[2];
                            MainF.LastMatchbookUpdate();
                            foreach (KeyValuePair<int,X> it in FootData[i].XKoef)
                            {
                                if (MainF.Bookmakers[it.Key].Type != BrokerType.BACK) continue;

                                for (int q = 0; q < 3; q++)
                                {
                                    if (it.Value.Koef[q] >= MainF.Bookmakers[it.Key].Limit && FootData[i].Lay[q] > item.Lay[q])
                                    {
                                        double p0 = CalcLayPL(it.Value.Koef[q], item.Lay[q]);
                                        if (p0 >= 1 && MainF.Bookmakers[it.Key].InUse) 
                                        {
                                            WriteBreakToFile.LogSave("Lay  " + MainF.Bookmakers[it.Key].Name + " | " + item.Team1 + "-" + item.Team2 + "  [" + q.ToString() + "] " + FootData[i].Lay[q].ToString() + " -> " + item.Lay[q].ToString()); 
                                            changecolor = true; 
                                        }

                                        if (p0 >= MLimit && p0 >= MainF.Bookmakers[it.Key].PLlimit &&  MainF.Bookmakers[it.Key].EMailing  )
                                        {
                                            StringBuilder selection = new StringBuilder();
                                            switch(q)
                                            {
                                                case 0: selection.Append("Draw"); break;
                                                case 1: selection.Append(item.Team1); break;
                                                case 2: selection.Append(item.Team2); break;
                                                default: selection.Append("uknown"); break;
                                            }
                                            PrepareMessage(item.Team1 + "-" + item.Team2, selection.ToString(), it.Value.Koef[q], item.Lay[q], MainF.Bookmakers[it.Key].Name, it.Value.LastUpdate, FootData[i].time);  
                                        }
                                    }
                                }


                            }

                            FootData[i].XKoef[id].Koef[0] = item.X[0];
                            FootData[i].XKoef[id].Koef[1] = item.X[1];
                            FootData[i].XKoef[id].Koef[2] = item.X[2];

                            FootData[i].Lay[0] = item.Lay[0];
                            FootData[i].Lay[1] = item.Lay[1];
                            FootData[i].Lay[2] = item.Lay[2];

                            
                        }
                        break; 
                    }

                }
                 if(i<0)
                {
                    if (item.delete) continue;

                    if (MainF.SuspendedMatches.Count > 0 && MainF.SuspendedMatches.FindIndex(str => str == item.Team1 + " - " + item.Team2) != -1) continue;
                    string RepPatter = FootballHedge.Properties.Settings.Default.RepPattern;

                    DateTime dt = DateTime.Parse(item.time, System.Globalization.CultureInfo.CurrentCulture);


                     FData NI = new FData(id);
                     NI.Team1 = item.Team1;
                     NI.Team2 = item.Team2;
                     NI.Match = Regex.Replace(XMLData.ReturnAltTeamName(item.Team1) + "|" + XMLData.ReturnAltTeamName(item.Team2), RepPatter, "").ToUpper();
                     NI.League = item.League;
                     NI.SizeX = item.Size;
                     //NI.Koef[id, 0] = item.X[0];
                     //NI.Koef[id, 1] = item.X[1];
                     //NI.Koef[id, 2] = item.X[2];
                     NI.XKoef[id].Koef[0] = item.X[0];
                     NI.XKoef[id].Koef[1] = item.X[1];
                     NI.XKoef[id].Koef[2] = item.X[2];
                     NI.Lay[0] = item.Lay[0];
                     NI.Lay[1] = item.Lay[1];
                     NI.Lay[2] = item.Lay[2];
                     NI.time = dt;
                     FootData.Add(NI);
                     MainF.LastMatchbookUpdate();
                     if (item.League != null) { MainF.CheckAndUpdateEventsCombobox(item.League); }


                }
            }

            

        }
        public void UpBack(List<FData> FootData , List<ParsedDataFromPage>  PD, int id)
        {
            double MLimit = FootballHedge.Properties.Settings.Default.MailplLimit;

            foreach (ParsedDataFromPage obj in PD)
            {
                for (int j = FootData.Count - 1; j >= 0; j--)
                {
                    FData item = FootData[j];

                    int del = item.Match.IndexOf("|");
                    if (del == -1) continue;

                    if (item.Match.IndexOf(obj.Team1, 0, del) == -1) continue;

                    if (item.Match.IndexOf(obj.Team2, del) == -1) continue;

                    if (!item.XKoef.ContainsKey(id)) { item.AddNewItenToDictionary(id); }

                    for (int q = 0; q < 3; q++)
                    {
                        if(item.XKoef[id].Koef[q] >= MainF.Bookmakers[id].Limit && item.XKoef[id].Koef[q] < obj.X[q])
                        {
                            double p0 = CalcLayPL(obj.X[q], item.Lay[q]);
                            if (p0 >= 1 && MainF.Bookmakers[id].InUse) 
                            {
                                WriteBreakToFile.LogSave("Back " + MainF.Bookmakers[id].Name + " | " + item.Team1 + "-" + item.Team2 + "  [" + q.ToString() + "] " + item.XKoef[id].Koef[q].ToString() + " -> " + obj.X[q].ToString()); 
                                changecolor = true;            
                            }
                            if (p0 >= MLimit && p0 >= MainF.Bookmakers[id].PLlimit && MainF.Bookmakers[id].EMailing) 
                            {
                                StringBuilder selection = new StringBuilder();
                                switch(q)
                                {
                                    case 0: selection.Append("Draw"); break;
                                    case 1: selection.Append(item.Team1); break;
                                    case 2: selection.Append(item.Team2); break;
                                    default: selection.Append("uknown");  break;
                                }
                                PrepareMessage(item.Team1 + "-" + item.Team2, selection.ToString(), obj.X[q], item.Lay[q], MainF.Bookmakers[id].Name, obj.time,item.time);
                                WriteBreakToFile.LogSave("EMAIL Back " + MainF.Bookmakers[id].Name + " | " + item.Team1 + "-" + item.Team2 + "  [" + q.ToString() + "] " + item.XKoef[id].Koef[q].ToString() + " -> " + obj.X[q].ToString()); 
                               
                            }
                        }
                    }
                        
                    item.XKoef[id].Koef[0] = obj.X[0];
                    item.XKoef[id].Koef[1] = obj.X[1];
                    item.XKoef[id].Koef[2] = obj.X[2];
                    item.XKoef[id].LastUpdate = DateTime.Now.ToString("HH:mm");

                    
                }
            }

        }
        public void UpHand(List<FData> FootData , List<ParsedDataFromPage>  PD, int id)
        {
            double MLimit = FootballHedge.Properties.Settings.Default.MailplLimit;

            foreach (ParsedDataFromPage obj in PD)
            {
                if (obj.League != "0.5") continue;
                for (int j = FootData.Count - 1; j >= 0; j--)
                {
                    FData item = FootData[j];
                    if (!item.XKoef.ContainsKey(id)) { item.AddNewItenToDictionary(id); }
                    if (item.Match.Contains(obj.Team1) && item.Match.Contains(obj.Team2))
                    {

                        foreach (KeyValuePair<int, X> it in FootData[j].XKoef)
                        {
                            if (MainF.Bookmakers[it.Key].Type != BrokerType.BACK) continue;

                            if (it.Value.Koef[1] < it.Value.Koef[2] && it.Value.Koef[1] >= MainF.Bookmakers[it.Key].Limit && it.Value.Koef[2] < obj.X[2] )
                            {
                                double p1 = CalcHandicapPL(it.Value.Koef[1], obj.X[2]);
                                if (p1 >= 1 && MainF.Bookmakers[id].InUse)  changecolor = true;
                                if (p1 >= MLimit && p1 >= MainF.Bookmakers[it.Key].PLlimit) PrepareMessage(item.Team1 + "-" + item.Team2, item.Team1, obj.X[2], it.Value.Koef[1], MainF.Bookmakers[it.Key].Name, obj.time,item.time);
                            }

                            if (it.Value.Koef[1] > it.Value.Koef[2] && it.Value.Koef[2] >= MainF.Bookmakers[it.Key].Limit && it.Value.Koef[1] < obj.X[1])
                            {
                                double p2 = CalcHandicapPL(it.Value.Koef[2], obj.X[1]);
                                if (p2 >= 1 && MainF.Bookmakers[id].InUse)  changecolor = true;
                                if (p2 >= MLimit && p2 >= MainF.Bookmakers[it.Key].PLlimit) PrepareMessage(item.Team1 + "-" + item.Team2, item.Team1, obj.X[1], it.Value.Koef[2], MainF.Bookmakers[it.Key].Name + " handicap", obj.time,item.time);
                            }




                        }

                        //for (int q = 0; q < MainF.Bookmakers.Count; q++)
                        //{
                        //    if(MainF.Bookmakers[q].Type != BrokerType.BACK) continue;


                        //    if(item.Koef[q,1] == 0) continue;

                        //    if (item.Koef[q, 1] >= MainF.Bookmakers[q].Limit && item.Koef[id, 2] < obj.X[2])
                        //    {
                        //        double p1 = CalcHandicapPL(item.Koef[q,1], obj.X[2]);
                        //        if (p1 >= 1 && MainF.Bookmakers[id].InUse) changecolor = true;
                        //        if (p1 >= MLimit) PrepareMessage(item.Team1 + "-" + item.Team2, item.Team1, obj.X[2], item.Koef[q, 1], MainF.Bookmakers[q].Name, obj.time);
                        //    }

                        //    if (item.Koef[q, 2] >= MainF.Bookmakers[q].Limit && item.Koef[id, 1] < obj.X[1])
                        //    {
                        //        double p2 = CalcHandicapPL(item.Koef[q, 2], obj.X[1]);
                        //        if (p2 >= 1 && MainF.Bookmakers[id].InUse) changecolor = true;
                        //        if (p2 >= MLimit) PrepareMessage(item.Team1 + "-" + item.Team2, item.Team1, obj.X[1], item.Koef[q, 2], MainF.Bookmakers[q].Name+" handicap", obj.time);
                        //    }

                        //}
                        //item.Koef[id, 1] = obj.X[1];
                        //item.Koef[id, 2] = obj.X[2];
                        //item.LastUpdate[id] = DateTime.Now.ToString("HH:mm");
                        item.XKoef[id].Koef[1] = obj.X[1];
                        item.XKoef[id].Koef[2] = obj.X[2];
                        item.XKoef[id].LastUpdate = DateTime.Now.ToString("HH:mm");
                    }

                }
            }

        }

        public void Calchedge(ref List<LayData> datalist, List<FData> FootData , CalcParameters cpar)
        {

            if (FootData.Count == 0) return;

            for (int id = FootData.Count - 1; id >= 0; id--)
            {
                FData item = FootData[id];
                if (cpar.today && item.time.Day != DateTime.Now.Day) continue;


                foreach (KeyValuePair<int, X> it in item.XKoef)
                {
                    BasicBrokerData bookie = MainF.Bookmakers[it.Key];
                    if (cpar.preferedbookie != "All" && cpar.preferedbookie != bookie.Name) continue;
                    if (!cpar.bookielimit.Contains(bookie.Name)) continue;

                    if (bookie.Type == BrokerType.BACK)
                    {
                        for (int q = 0; q < 3; q++)
                        {
                            if (it.Value.Koef[q] != 0 && item.Lay[q] != 0 && it.Value.Koef[q] >= bookie.Limit && CalcLayPL(it.Value.Koef[q], item.Lay[q]) > cpar.limit)
                            {
                                StringBuilder selection = new StringBuilder();
                                switch (q)
                                {
                                    case 0: selection.Append("Draw"); break;
                                    case 1: selection.Append(item.Team1); break;
                                    case 2: selection.Append(item.Team2); break;
                                    default: selection.Append("Uknown"); break;
                                        
                                }

                                LayData ld = new LayData();
                                ld.Match = item.Team1 + " - " + item.Team2;
                                ld.Selection = selection.ToString();
                                ld.Lay = item.Lay[q];
                                ld.Back = it.Value.Koef[q];
                                ld.Profit = Math.Round(CalcLayPL(it.Value.Koef[q], item.Lay[q]), 3);
                                ld.Size = item.SizeX[0];
                                ld.Bookie = bookie.Name;
                                ld.League = item.League;
                                ld.Time =  ChangeTimeToString(item.time);
                                ld.LastUpdate = it.Value.LastUpdate;
                                datalist.Add(ld);
                            }

                        }

                    } // end type.Back
                    if (bookie.Type == BrokerType.HANDICAP)
                    {
                        foreach (KeyValuePair<int, X> hi in item.XKoef)
                        {
                            if (MainF.Bookmakers[hi.Key].Type != BrokerType.BACK) continue;
                            if (!cpar.bookielimit.Contains(MainF.Bookmakers[hi.Key].Name)) continue;
                            if (cpar.preferedbookie != "All" && cpar.preferedbookie != MainF.Bookmakers[hi.Key].Name) continue;

                            if (hi.Value.Koef[1] != 0 && hi.Value.Koef[1] < hi.Value.Koef[2] && hi.Value.Koef[1] >= MainF.Bookmakers[hi.Key].Limit && CalcHandicapPL(hi.Value.Koef[1], it.Value.Koef[2]) > cpar.limit)
                            {
                                LayData ld = new LayData();
                                ld.Match = item.Team1 + " - " + item.Team2;
                                ld.Selection = item.Team1;
                                ld.Lay = it.Value.Koef[2];
                                ld.Back = hi.Value.Koef[1];
                                ld.Profit = Math.Round(CalcHandicapPL(ld.Back, ld.Lay), 2);
                                ld.Bookie = MainF.Bookmakers[hi.Key].Name;
                                ld.League = item.League;
                                ld.Size = -1;
                                ld.Time = ChangeTimeToString(item.time);
                                ld.LastUpdate = hi.Value.LastUpdate + " Hand";
                                datalist.Add(ld);
                            }
                            if (hi.Value.Koef[2] != 0 && hi.Value.Koef[1] > hi.Value.Koef[2] && hi.Value.Koef[2] >= MainF.Bookmakers[hi.Key].Limit && CalcHandicapPL(hi.Value.Koef[2], it.Value.Koef[1]) > cpar.limit)
                            {
                                LayData ld = new LayData();
                                ld.Match = item.Team1 + " - " + item.Team2;
                                ld.Selection = item.Team2;
                                ld.Lay = it.Value.Koef[1];
                                ld.Back = hi.Value.Koef[2];
                                ld.Profit = Math.Round(CalcHandicapPL(ld.Back, ld.Lay), 2);
                                ld.Bookie = MainF.Bookmakers[hi.Key].Name;
                                ld.League = item.League;
                                ld.Size = -1;
                                ld.Time = ChangeTimeToString(item.time);
                                ld.LastUpdate = hi.Value.LastUpdate + " Hand";
                                datalist.Add(ld);
                            }


                        }
                    }


                }
            }
           
        }

        public void Calc3WayDutch(List<ListViewData3WayDutch> datalist, List<FData> FootData)
        {
            if (FootData.Count == 0) return;
            double limit = FootballHedge.Properties.Settings.Default.plLimit;
            for (int id = FootData.Count - 1; id >= 0; id--)
            {
                FData item = FootData[id];

                foreach (KeyValuePair<int, X> x0 in item.XKoef)
                {
                    if (x0.Value.Koef[0] == 0 || !MainF.Bookmakers[x0.Key].Dutch3Way) continue;
                    foreach (KeyValuePair<int, X> x1 in item.XKoef)
                    {
                        if (x0.Key == x1.Key) continue;
                        if (x1.Value.Koef[1] == 0 || !MainF.Bookmakers[x1.Key].Dutch3Way) continue;
                        foreach (KeyValuePair<int, X> x2 in item.XKoef)
                        {
                            if (x0.Key == x2.Key || x1.Key == x2.Key) continue;
                            if (x2.Value.Koef[2] == 0 || !MainF.Bookmakers[x2.Key].Dutch3Way) continue;
                            double p = Calc3WayDucth(x0.Value.Koef[0], x1.Value.Koef[1], x2.Value.Koef[2]);
                            if(p >= limit)
                            {
                                ListViewData3WayDutch dd = new ListViewData3WayDutch();
                                dd.Match = item.Team1 + " - " + item.Team2;
                                dd.League = item.League;
                                dd.Bookie0 = MainF.Bookmakers[x0.Key].Name;
                                dd.Bookie1 = MainF.Bookmakers[x1.Key].Name;
                                dd.Bookie2 = MainF.Bookmakers[x2.Key].Name;
                                dd.X0 = x0.Value.Koef[0];
                                dd.X1 = x1.Value.Koef[1];
                                dd.X2 = x2.Value.Koef[2];
                                dd.Profit = Math.Round(p,2);
                                datalist.Add(dd);
                                
                            }
                        }
                    }


                }

            }

        }

        public void PrepareMessage(string match, string selection, double x1, double x2, string b1, string tt, DateTime eventdate)
        {
            
            Task.Factory.StartNew(() =>
            {
                //int index = MainF.Bookmakers.FindIndex(item => item.Name == b1);
                //if (index == -1) return;
                //if (!MainF.Bookmakers[index].EMailing) return;

              //  double profit = CalcLayPL(x1, x2);
                if (!Mail.CheckIfEmailsAllowed() || !Mail.CheckIfSingleMailIsAllowed()) return;

                string tytle = eventdate.Day == DateTime.Now.Day? String.Format("{0}  [{1}] |  {2}/{3} ({4})  {5}", match, selection, x1, x2, b1, tt):
                                                                 String.Format("!{0}  [{1}] |  {2}/{3} ({4})  {5} {6}", match, selection, x1, x2, b1, tt,ChangeTimeToString(eventdate));
                                   
                WriteBreakToFile.LogSave(tytle); 
                                         
                Mail.SendMail(tytle, tytle);
            });
        }
    }
}
