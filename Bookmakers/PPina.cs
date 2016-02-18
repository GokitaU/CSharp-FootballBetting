using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace FootballHedge
{
    class PPina: ParseBookies,IParseBookies
    {
        public override bool Parse(string page, ref List<ParsedDataFromPage> PD)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(page);

            if (doc.DocumentNode.SelectSingleNode("//tbody[contains(@ng-repeat,'event in events')]") == null) { return false; }

            foreach (var node in doc.DocumentNode.SelectNodes("//tbody[contains(@ng-repeat,'event in events')]") ?? Enumerable.Empty<HtmlNode>())
            {
                var l1 = node.SelectSingleNode(".//tr[1]");
                var l2 = node.SelectSingleNode(".//tr[2]");
                var l0 = node.SelectSingleNode(".//tr[3]");
                if (l1 == null || l2 == null) continue;

                var v1 = l1.SelectSingleNode(".//span[contains(@ng-if,'participant.MoneyLine')]");
                if(v1 ==null ) continue;

                var v2 = l2.SelectSingleNode(".//span[contains(@ng-if,'participant.MoneyLine')]");
                if(v2 ==null ) continue;

                var v0 = l0.SelectSingleNode(".//span[contains(@ng-if,'participant.MoneyLine')]");
                if (v0 == null) continue;

                string catchstr = v1.InnerText;
                double x1 = 1;
                if (catchstr != null && TryToParse.ParseDouble(catchstr, out x1)) { }

                catchstr = v2.InnerText;
                double x2 = 1;
                if (catchstr != null && TryToParse.ParseDouble(catchstr, out x2)) { }

                catchstr = v0.InnerText;
                double x0 = 1;
                if (catchstr != null && TryToParse.ParseDouble(catchstr, out x0)) { }

                var t1 = l1.SelectSingleNode(".//span[contains(@ng-if,'participant.Name')]");
                var t2 = l2.SelectSingleNode(".//span[contains(@ng-if,'participant.Name')]");

                if(t1 == null || t2 == null) continue;

 

                //var h1 = l1.SelectSingleNode(".//span[@class='spread ng-binding']");
                //var h2 = l2.SelectSingleNode(".//span[@class='spread ng-binding']");

                //if (h1 == null || h2 == null) continue;
                //if (h1.InnerText != "+0.5" && h2.InnerText != "+0.5") continue;

                //double x1 = 0;
                //if (h1.InnerText == "+0.5")
                //{
                //    string catchstr = l1.SelectSingleNode(".//span[@class='price ng-binding']").InnerText;
                //    if (catchstr != null && TryToParse.ParseDouble(catchstr, out x1)) { }
                //}
                //double x2 = 0;
                //if (h2.InnerText == "+0.5")
                //{
                //    string catchstr = l2.SelectSingleNode(".//span[@class='price ng-binding']").InnerText;
                //    if (catchstr != null && TryToParse.ParseDouble(catchstr, out x2)) { }
                //}

                ParsedDataFromPage nob = new ParsedDataFromPage();

                nob.Team1 = Regex.Replace(t1.InnerText, RepPatter, "").ToUpper().Trim();
                nob.Team2 = Regex.Replace(t2.InnerText, RepPatter, "").ToUpper().Trim();
                nob.X[0] = x0;
                nob.X[1] = x1;
                nob.X[2] = x2;
                PD.Add(nob);


            }


            return true;
        }
    }
}
