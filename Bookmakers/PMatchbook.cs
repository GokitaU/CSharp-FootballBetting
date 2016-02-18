using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Windows;
using System.Diagnostics;

namespace FootballHedge
{
    class PMatchbook : ParseBookies, IParseBookies
    {
        MainWindow MainF;
        public PMatchbook(MainWindow MainF)
        {
            this.MainF = MainF;
        }
      public override bool Parse(string page, ref List<ParsedDataFromPage> PD)
        {
            if (page.Contains("\"offset\""))
            {
                int poz = page.IndexOf("]},{");
                if (poz == -1) return false;
                poz = 0;
                while (poz!=-1)
                {
                    
                    int poznext = page.IndexOf("]},{", poz + 1);
                    if (poznext == -1) poznext = page.Length - 1;

                    int aa = page.IndexOf("\"Match Odds\"",poz);
                    if (aa > poznext || aa ==-1 ) { poz = page.IndexOf("]},{", poz + 1); continue; }

                    StringBuilder country = new StringBuilder();

                    int p1 = poz;
                    while (true)
                    {
                        var q1 = page.IndexOf("\"COUNTRY\"", p1, poznext - p1 - 2);
                        if (q1 == -1) break;
                        else country.Append(page.Substring(q1 + 22, page.IndexOf("\"", q1 + 23) - q1 - 22)).Append(" ");
                        p1 = q1 + 1;
                    }

                    p1 = poz;
                    StringBuilder competition = new StringBuilder();
                    while (true)
                    {
                        var q1 = page.IndexOf("\"COMPETITION\"", p1, poznext - p1 - 2);
                        if (q1 == -1) break;
                        else
                        {
                            string buf = page.Substring(q1 + 26, page.IndexOf("\"", q1 + 27) - q1 - 26).Trim();
                            if (!buf.Contains("live-betting")) competition.Append(buf + " ");

                            
                        }
                        p1 = q1 + 1;
                    }
                    if (competition.Length == 0) { poz = page.IndexOf("]},{", poz + 1); continue; }
                    string league = (country.ToString() + competition.ToString().Trim()).Replace("-", " ").ToUpper();







                    int index = MainF.Leagues.FindIndex(i => i.Name == league);
                    if (index != -1)
                    {
                        if(!MainF.Leagues[index].State) { poz = page.IndexOf("]},{", poz + 1); continue; }
                    }
                    else {  MainF.Leagues.Add(new LeagueState() { Name = league, State = true }); }

                    var n1 = page.IndexOf("\"name\"", aa);
                    var n2 = page.IndexOf("\"name\"", n1 + 1);
                    var n0 = page.IndexOf("\"name\"", n2 + 1);



                    if (n1 == -1 || n2 == -1 || n0==-1) { poz = page.IndexOf("]},{", poz + 1); continue; }
                    //-----------------------------------------------------------------
                    var b1 = page.IndexOf("\"back\"", n1, n2 - n1);
                    double backodds1 = 0;
                    if (b1 != -1)
                    {
                        var d1 = page.IndexOf("\"decimal-odds\"", b1);
                        string back = page.Substring(d1 + 15, page.IndexOf(",", d1) - d1 - 15);
                        if (back != null && TryToParse.ParseDouble(back, out backodds1)) { }
                    }

                    var l1 = page.IndexOf("\"lay\"", n1,n2-n1);

                    double layodds1 = 0;
                    double available1 = 0;
                    if(l1 != -1)
                    {
                        var d1 = page.IndexOf("\"decimal-odds\"", l1);
                        string laysize = page.Substring(l1 + 25, page.IndexOf(",", l1 + 10) - l1 - 25);
                        if (laysize != null && TryToParse.ParseDouble(laysize, out available1)) { }
                        string lay1 = page.Substring(d1 + 15, page.IndexOf(",", d1) - d1 - 15);
                        if (lay1 != null && TryToParse.ParseDouble(lay1, out layodds1)) { }
   
                    }
                    //-----------------------------------------------------------------
                    var b2 = page.IndexOf("\"back\"", n2, n0 - n2);
                    double backodds2 = 0;
                    if (b2 != -1)
                    {
                        var d2 = page.IndexOf("\"decimal-odds\"", b2);
                        string back = page.Substring(d2 + 15, page.IndexOf(",", d2) - d2 - 15);
                        if (back != null && TryToParse.ParseDouble(back, out backodds2)) { }

                    }

                    var l2 = page.IndexOf("\"lay\"", n2,n0-n2);

                    double layodds2 = 0;
                    double available2 = 0;
                    if(l2!=-1)
                    {
                        var d2 = page.IndexOf("\"decimal-odds\"", l2);
                        string laysize = page.Substring(l2 + 25, page.IndexOf(",", l2 + 10) - l2 - 25);
                        if (laysize != null && TryToParse.ParseDouble(laysize, out available2)) { }
                        string lay2 = page.Substring(d2 + 15, page.IndexOf(",", d2) - d2 - 15);
                        if (lay2 != null && TryToParse.ParseDouble(lay2, out layodds2)) { }

                    }
                    //-----------------------------------------------------------------
                    var b0 = page.IndexOf("\"back\"", n0, poznext - n0 - 1);
                    double backodds0 = 0;
                    if (b0 != -1)
                    {
                        var d0 = page.IndexOf("\"decimal-odds\"", b0);
                        string back = page.Substring(d0 + 15, page.IndexOf(",", d0) - d0 - 15);
                        if (back != null && TryToParse.ParseDouble(back, out backodds0)) { }

                    }

                    var l0 = page.IndexOf("\"lay\"", n0, poznext-n0-1);

                    double layodds0 = 0;
                    double available0 = 0;

                    if(l0!=-1)
                    {
                        var d0 = page.IndexOf("\"decimal-odds\"", l0);
                        string laysize = page.Substring(l0 + 25, page.IndexOf(",", l0 + 10) - l0 - 25);
                        if (laysize != null && TryToParse.ParseDouble(laysize, out available0)) { }
                        string lay0 = page.Substring(d0 + 15, page.IndexOf(",", d0) - d0 - 15);
                        if (lay0 != null && TryToParse.ParseDouble(lay0, out layodds0)) { }

                    }
                    //-----------------------------------------------------------------
     

                    var s = page.IndexOf("\"start\"", poz, poznext - poz - 1);
                    var f = page.IndexOf("\"in-running-flag\"", poz, poznext - poz - 2);

                    string starttime = "";
                    if (s != -1) starttime = page.Substring(s + 9, page.IndexOf(",", s + 10) - s - 10);

                    string flag = "false";
                    if (f != -1) flag = page.Substring(f + 18, page.IndexOf(",", f + 10) - f - 18);

                    poz = poznext;

                    ParsedDataFromPage nob = new ParsedDataFromPage();
                    
                    nob.time = starttime;
                    nob.League = league.ToUpper().Trim();
                    nob.Team1 = page.Substring(n1 + 8, page.IndexOf(",", n1) - n1 - 9);
                    nob.Team2 = page.Substring(n2 + 8, page.IndexOf(",", n2) - n2 - 9);

                 

                    nob.Lay[0] = Math.Round(layodds0, 2);
                    nob.Lay[1] = Math.Round(layodds1, 2);
                    nob.Lay[2] = Math.Round(layodds2, 2);


                    nob.X[0] = Math.Round(backodds0, 2);
                    nob.X[1] = Math.Round(backodds1, 2);
                    nob.X[2] = Math.Round(backodds2, 2);

                    nob.Size[0] = Math.Round(available0,0);
                    nob.Size[1] = Math.Round(available1,0);
                    nob.Size[2] = Math.Round(available2,0);
                    nob.delete = Convert.ToBoolean(flag);
                    PD.Add(nob);


                //    System.Windows.Forms.MessageBox.Show(nob.Team1 + "  " + nob.Team2 + "  " + available0 + "  " + available1 + "  " + available2);


                   }
            }

            else
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(page);

                if (doc.DocumentNode.SelectSingleNode("//li[contains(@class,'event')]") == null) return false;

                var check = doc.DocumentNode.SelectSingleNode("//a[@class='breadcrumb ']/span");
                string league = "Unknown";
                if (check != null) league = check.InnerText.Trim();

                string catchstr;
                foreach (var node in doc.DocumentNode.SelectNodes("//li[contains(@class,'event')]") ?? Enumerable.Empty<HtmlNode>())
                {
                    var checkevent = node.SelectSingleNode(".//a[contains(@class,'details')]");
                    if (checkevent != null)
                    {
                        if (!checkevent.InnerText.ToUpper().Contains(" VS ")) { continue; }
                        if (checkevent.Attributes["class"].Value.Contains("in_play"))
                        {

                            string[] d = checkevent.SelectSingleNode(".//h2").InnerText.Replace(" vs ", "-").Split('-');

                            ParsedDataFromPage nobdel = new ParsedDataFromPage();

                            nobdel.Team1 = d[0].Trim();
                            nobdel.Team2 = d[1].Trim();
                            nobdel.delete = true;
                            PD.Add(nobdel);
                            continue;
                        }
                    }

                    var a = node.SelectSingleNode(".//div[@class='runners']/ul/li[2]");
                    if (a == null) continue;
                    var a1 = a.SelectSingleNode(".//h4");
                    if (a1 == null) continue;

                    string Team1 = a1.InnerText.Trim();

                    var a2 = a.SelectSingleNode(".//ol[@class='lay']").SelectSingleNode(".//span[@class='odds']");
                    if (a2 == null) continue;
                    catchstr = a2.InnerText.Trim();
                    double layodds1 = 1;
                    if (catchstr != null && TryToParse.ParseDouble(catchstr, out layodds1)) { }

                    a2 = a.SelectSingleNode(".//ol[@class='lay']").SelectSingleNode(".//span[@class='amount']");
                    if (a2 == null) continue;
                    string laysize1 = a2.InnerText.Trim();


                    var b = node.SelectSingleNode(".//div[@class='runners']/ul/li[3]");

                    if (b == null) continue;
                    a1 = b.SelectSingleNode(".//h4");
                    if (a1 == null) continue;

                    string Team2 = a1.InnerText.Trim();


                    a2 = b.SelectSingleNode(".//ol[@class='lay']").SelectSingleNode(".//span[@class='odds']");
                    if (a2 == null) continue;
                    catchstr = a2.InnerText.Trim();
                    double layodds2 = 1;
                    if (catchstr != null && TryToParse.ParseDouble(catchstr, out layodds2)) { }

                    a2 = b.SelectSingleNode(".//ol[@class='lay']").SelectSingleNode(".//span[@class='amount']");
                    if (a2 == null) continue;
                    string laysize2 = a2.InnerText.Trim();


                    var c = node.SelectSingleNode(".//div[@class='runners']/ul/li[4]");

                    if (c == null) continue;
                    a1 = c.SelectSingleNode(".//h4");
                    if (a1 == null) continue;

                    string t3 = a1.InnerText.Trim();

                    a2 = c.SelectSingleNode(".//ol[@class='lay']").SelectSingleNode(".//span[@class='odds']");
                    if (a2 == null) continue;
                    double layodds0 = 1;
                    catchstr = a2.InnerText.Trim();
                    if (catchstr != null && TryToParse.ParseDouble(catchstr, out layodds0)) { }

                    a2 = c.SelectSingleNode(".//ol[@class='lay']").SelectSingleNode(".//span[@class='amount']");
                    if (a2 == null) continue;
                    string laysize0 = a2.InnerText.Trim();

                    var time = node.SelectSingleNode(".//div[@class='time']");

                    string t = "";
                    var hour = time.SelectSingleNode(".//span[@class='hour']");
                    if (hour.InnerText != "")
                    {
                        var min = time.SelectSingleNode(".//span[@class='minute']");
                        t = hour.InnerText.Trim() + ":" + min.InnerText.Trim() + " Today";
                    }
                    else
                    {
                        t = time.SelectSingleNode(".//div[@class='date']").InnerText.Trim();
                    }


                    

                    ParsedDataFromPage nob = new ParsedDataFromPage();
                    if (league.Length < 1) league = "Empty";
                    nob.time = t;
                    nob.League = league;
                    nob.Team1 = Team1;
                    nob.Team2 = Team2;
                    nob.X[0] = layodds0;
                    nob.X[1] = layodds1;
                    nob.X[2] = layodds2;
  
                    PD.Add(nob);

                }
            }
            return true;
        }




    }
}
