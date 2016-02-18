using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace FootballHedge
{
    class PWilliamHill : ParseBookies, IParseBookies
    {
        public override bool Parse(string page, ref List<ParsedDataFromPage> PD)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(page);
            if (doc.DocumentNode.SelectSingleNode("//tr[@class='rowOdd']") == null) { return false; }

            foreach (var node in doc.DocumentNode.SelectNodes("//tr[@class='rowOdd']") ?? Enumerable.Empty<HtmlNode>())
            {
                var teams = node.SelectSingleNode(".//span[contains(@id,'mkt_namespace')]");
                if (teams == null) continue;

                

                string[] buf = teams.InnerText.Replace("&nbsp;", "").Replace(" v ","-").Split('-');





                var v1 = node.SelectSingleNode(".//td[5]");
                var v0 = node.SelectSingleNode(".//td[6]");
                var v2 = node.SelectSingleNode(".//td[7]");

                if (v1 == null || v0 == null || v2 == null) continue;

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


                //string Team1 = buf[0].Trim();
                //string Team2 = buf[1].Trim();

                string Team1 = Regex.Replace(buf[0], RepPatter, "").ToUpper().Trim();
                string Team2 = Regex.Replace(buf[1], RepPatter, "").ToUpper().Trim(); 

               ParsedDataFromPage nob = new ParsedDataFromPage();
               nob.Team1 = Team1;
               nob.Team2 = Team2;
               nob.X[0] = x0;
               nob.X[1] = x1;
               nob.X[2] = x2;

               PD.Add(nob);



            }


            return true;
        }
    }
}
