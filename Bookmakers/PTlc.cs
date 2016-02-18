using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace FootballHedge
{
    class PTlc: ParseBookies, IParseBookies
    {
        public override bool Parse(string page, ref List<ParsedDataFromPage> PD)
        {
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(page);


            if (doc.DocumentNode.SelectSingleNode("//div[@class='bets ml']") == null) { return false; }
  
            foreach (var node in doc.DocumentNode.SelectNodes("//div[@class='bets ml']") ?? Enumerable.Empty<HtmlNode>())
            {
                var teams = node.SelectSingleNode(".//dt[@class='team_betting ']");
                if (teams == null) continue;

                string Team1 = teams.SelectSingleNode(".//span[1]").InnerText;
                string Team2 = teams.SelectSingleNode(".//span[2]").InnerText;

                var odds = node.SelectSingleNode(".//dd");
                if (odds == null) continue;

                string catchstr = odds.SelectSingleNode(".//ul/li[1]").InnerText;
                double x1 = 1;
                if (catchstr != null && TryToParse.ParseDouble(catchstr, out x1)) { }

                catchstr = odds.SelectSingleNode(".//ul/li[2]").InnerText;
                double x0 = 1;
                if (catchstr != null && TryToParse.ParseDouble(catchstr, out x0)) { }

                catchstr = odds.SelectSingleNode(".//ul/li[3]").InnerText;
                double x2 = 1;
                if (catchstr != null && TryToParse.ParseDouble(catchstr, out x2)) { }


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
