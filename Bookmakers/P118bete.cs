using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace FootballHedge
{
    class P118bete:ParseBookies, IParseBookies
    {
        public override bool Parse(string page, ref List<ParsedDataFromPage> PD)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(page);

            if (doc.DocumentNode.SelectSingleNode(".//tbody[contains(@class,'evt')]") == null) { return false; }

         

            foreach (var node in doc.DocumentNode.SelectNodes(".//tbody[contains(@class,'evt')]") ?? Enumerable.Empty<HtmlNode>())
            {
                var t1 = node.SelectSingleNode(".//tr[1]");
                var t2 = node.SelectSingleNode(".//tr[2]");
                if (t1 == null || t2 == null) continue;

                string Team1 = t1.SelectSingleNode(".//span[contains(@class,'selection-txt')]").InnerText;
                string Team2 = t2.SelectSingleNode(".//span[contains(@class,'selection-txt')]").InnerText;

                string h1 = t1.SelectSingleNode(".//div[@class='hdp']").InnerText;
                string h2 = t2.SelectSingleNode(".//div[@class='hdp']").InnerText;
                double x1 = 0;
                if (h1 == "+0.5")
                {
                    string catchstr = t1.SelectSingleNode(".//td[@class='col-hdp-odds']/span").InnerText;
                    if (catchstr != null && TryToParse.ParseDouble(catchstr, out x1)) { }
                }

                double x2 = 0;
                if (h2 == "+0.5")
                {
                    string catchstr = t2.SelectSingleNode(".//td[@class='col-hdp-odds']/span").InnerText;
                    if (catchstr != null && TryToParse.ParseDouble(catchstr, out x2)) { }
                }
                if (x1 == 0 && x2 == 0) continue;

                ParsedDataFromPage nob = new ParsedDataFromPage();

                nob.Team1 = Team1;
                nob.Team2 = Team2;
                nob.X[1] = x1;
                nob.X[2] = x2;
                PD.Add(nob);
            }
            return true;
        }

    }
}
