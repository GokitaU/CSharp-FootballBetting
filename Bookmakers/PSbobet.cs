using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Windows;

namespace FootballHedge
{
    class PSbobet:ParseBookies,IParseBookies
    {
        public override bool Parse(string page, ref List<ParsedDataFromPage> PD)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(page);

            if (doc.DocumentNode.SelectSingleNode(".//tr[contains(@id,'bu:od:or')]") == null) { return false; }
            foreach (var node in doc.DocumentNode.SelectNodes(".//tr[contains(@id,'bu:od:or')]") ?? Enumerable.Empty<HtmlNode>())
            {
                var t1 = node.SelectSingleNode(".//td[3]");
                var t0 = node.SelectSingleNode(".//td[4]");
                var t2 = node.SelectSingleNode(".//td[5]");

                if (t1 == null || t0 == null || t2 == null ) continue;

                var te1 = t1.SelectSingleNode(".//span[@class='OddsL']");
                var te2 = t2.SelectSingleNode(".//span[@class='OddsL']");

                if (te1 == null || te2 == null) continue;

                double x1 = 1;
                double x0 = 1;
                double x2 = 1;

                var v1 = t1.SelectSingleNode(".//span[@class='OddsR']");

                if(v1 != null)
                {
                    string catchstr = v1.InnerText;
                    if (catchstr != null && TryToParse.ParseDouble(catchstr, out x1)) { }
                }

                var v0 = t0.SelectSingleNode(".//span[@class='OddsR']");
                if(v0 != null)
                {
                    string catchstr = v0.InnerText;
                    if (catchstr != null && TryToParse.ParseDouble(catchstr, out x0)) { }
                }
                var v2 = t2.SelectSingleNode(".//span[@class='OddsR']");
                if(v2 != null)
                {
                    string catchstr = v2.InnerText;
                    if (catchstr != null && TryToParse.ParseDouble(catchstr, out x2)) { }
                }



                ParsedDataFromPage nob = new ParsedDataFromPage();

                nob.Team1 = Regex.Replace(te1.InnerText.Replace("-", " "), RepPatter, "").ToUpper().Trim();
                nob.Team2 = Regex.Replace(te2.InnerText.Replace("-", " "), RepPatter, "").ToUpper().Trim(); 
                nob.X[0] = Math.Round(x0, 2);
                nob.X[1] = Math.Round(x1, 2);
                nob.X[2] = Math.Round(x2, 2);
                PD.Add(nob);

                //System.Windows.Forms.MessageBox.Show(nob.Team1+"  "+nob.Team2);

            }




            return true;
        }
        /*
        public override bool Parse(string page, ref List<ParsedDataFromPage> PD)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(page);

            if (doc.DocumentNode.SelectSingleNode(".//tr[contains(@id,'bu:od:or')]") == null) { return false; }


            foreach (var node in doc.DocumentNode.SelectNodes(".//tr[contains(@id,'bu:od:or')]") ?? Enumerable.Empty<HtmlNode>())
            {
                var t1 = node.SelectSingleNode(".//td[3]");
                var t2 = node.SelectSingleNode(".//td[4]");

                if (t1 == null || t2 == null) continue;

                string ev = t1.SelectSingleNode(".//span[@class='OddsM']").InnerText;

                string Team1 = t1.SelectSingleNode(".//span[@class='OddsL']").InnerText;
                double x1 = 0;

                if (ev == "+0.50")
                {
                    string catchstr = t1.SelectSingleNode(".//span[@class='OddsR']").InnerText;
                    if (catchstr != null && TryToParse.ParseDouble(catchstr, out x1)) { }
                }

                ev = t2.SelectSingleNode(".//span[@class='OddsM']").InnerText;
                string Team2 = t2.SelectSingleNode(".//span[@class='OddsL']").InnerText;
                double x2 = 0;
                if (ev == "+0.50")
                {
                    string catchstr = t2.SelectSingleNode(".//span[@class='OddsR']").InnerText;
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
         * */
    }
}
