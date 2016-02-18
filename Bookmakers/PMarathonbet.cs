using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Text.RegularExpressions;


namespace FootballHedge
{
    class PMarathonbet:ParseBookies,IParseBookies
    {
        
        public override bool Parse(string page, ref List<ParsedDataFromPage> PD)
        {

          
            var doc = new HtmlDocument();
            doc.LoadHtml(page);
            if (doc.DocumentNode.SelectSingleNode("//tr[@class='event-header']") == null) { return false; }
     
            foreach (var node in doc.DocumentNode.SelectNodes("//tr[@class='event-header']") ?? Enumerable.Empty<HtmlNode>())
            {
                try
                {
                    var teams = node.SelectSingleNode(".//span[@class='command text-underline']");
                    if (teams == null) continue;

                    string Team1 = teams.SelectSingleNode(".//div[1]").InnerText;
                    string Team2 = teams.SelectSingleNode(".//div[2]").InnerText;


                    //string Team1 = Regex.Replace(teams.SelectSingleNode(".//div[1]").InnerText, BasicProperties.RepPatter, "").ToUpper().Trim();
                    //string Team2 = Regex.Replace(teams.SelectSingleNode(".//div[2]").InnerText, BasicProperties.RepPatter, "").ToUpper().Trim();


                    var v1 = node.SelectSingleNode(".//td[contains(@data-sel,'" + Team1 + "')]");
                    var v0 = node.SelectSingleNode(".//td[contains(@data-sel,'Draw')]");
                    var v2 = node.SelectSingleNode(".//td[contains(@data-sel,'" + Team2 + "')]");

                    if (v1 == null || v2 == null || v0 == null) continue;

                    string catchstr = v1.InnerText.Trim();
                    double x1 = 1;
                    if (catchstr.Contains('/'))
                    {
                        List<double> d = new List<double>(Array.ConvertAll(catchstr.Split('/'), double.Parse));
                        x1 = Math.Round(d[0] / d[1] + 1, 2);
                    }
                    else { if (catchstr != null && TryToParse.ParseDouble(catchstr, out x1)) { } }


                    catchstr = v0.InnerText.Trim();
                    double x0 = 1;
                    if (catchstr.Contains('/'))
                    {
                        List<double> d = new List<double>(Array.ConvertAll(catchstr.Split('/'), double.Parse));
                        x0 = Math.Round(d[0] / d[1] + 1, 2);
                    }
                    else { if (catchstr != null && TryToParse.ParseDouble(catchstr, out x0)) { } }

                    catchstr = v2.InnerText.Trim();
                    double x2 = 1;
                    if (catchstr.Contains('/'))
                    {
                        List<double> d = new List<double>(Array.ConvertAll(catchstr.Split('/'), double.Parse));
                        x2 = Math.Round(d[0] / d[1] + 1, 2);
                    }
                    else { if (catchstr != null && TryToParse.ParseDouble(catchstr, out x2)) { } }



                    Team1 = Regex.Replace(teams.SelectSingleNode(".//div[1]").InnerText, RepPatter, "").ToUpper().Trim();
                    Team2 = Regex.Replace(teams.SelectSingleNode(".//div[2]").InnerText, RepPatter, "").ToUpper().Trim();

                    ParsedDataFromPage nob = new ParsedDataFromPage();

                    nob.Team1 = Team1;
                    nob.Team2 = Team2;
                    nob.X[0] = x0;
                    nob.X[1] = x1;
                    nob.X[2] = x2;
                    PD.Add(nob);
                }
                catch (Exception)
                {

                    continue;
                }

                

            }
            return true;
        }
    }
}
