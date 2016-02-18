using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace FootballHedge
{
    class PBwin: ParseBookies,IParseBookies
    {
        
        public override bool Parse(string page, ref List<ParsedDataFromPage> PD)
        {
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(page);

            if (doc.DocumentNode.SelectSingleNode("//tr[@class='col3 three-way']") == null) { return false; }

            foreach (var node in doc.DocumentNode.SelectNodes("//tr[@class='col3 three-way']") ?? Enumerable.Empty<HtmlNode>())
            {
                var a1 = node.SelectSingleNode(".//td[1]");
                var a0 = node.SelectSingleNode(".//td[2]");
                var a2 = node.SelectSingleNode(".//td[3]");

                if (a1 == null || a0 == null || a2 == null || a1.SelectSingleNode(".//span[@class='option-name']") == null || a2.SelectSingleNode(".//span[@class='option-name']") == null) continue;

                var t1 = a1.SelectSingleNode(".//span[@class='odds']");
                double x1 = 1;
                if(t1 != null)
                {
                    string catchstr = t1.InnerText;
                    if (catchstr.Contains('/'))
                    {
                        List<double> d = new List<double>(Array.ConvertAll(catchstr.Split('/'), double.Parse));
                        x1 = Math.Round(d[0] / d[1] + 1, 2);
                    }
                    else { if (catchstr != null && TryToParse.ParseDouble(catchstr, out x1)) { } }
                }

                var t0 = a0.SelectSingleNode(".//span[@class='odds']");
                double x0 = 1;
                if(t0 != null)
                {
                    string catchstr = t0.InnerText;
                    if (catchstr.Contains('/'))
                    {
                        List<double> d = new List<double>(Array.ConvertAll(catchstr.Split('/'), double.Parse));
                        x0 = Math.Round(d[0] / d[1] + 1, 2);
                    }
                    else { if (catchstr != null && TryToParse.ParseDouble(catchstr, out x0)) { } }
                }

                var t2 =  a2.SelectSingleNode(".//span[@class='odds']");
                double x2 = 1;
                if(t2 != null)
                {
                    string catchstr = t2.InnerText;
                    if (catchstr.Contains('/'))
                    {
                        List<double> d = new List<double>(Array.ConvertAll(catchstr.Split('/'), double.Parse));
                        x2 = Math.Round(d[0] / d[1] + 1, 2);
                    }
                    else { if (catchstr != null && TryToParse.ParseDouble(catchstr, out x2)) { } }
                }



                //string Team1 = a1.SelectSingleNode(".//span[@class='option-name']").InnerText;
                //string Team2 = a2.SelectSingleNode(".//span[@class='option-name']").InnerText;

                string Team1 = Regex.Replace(a1.SelectSingleNode(".//span[@class='option-name']").InnerText, RepPatter, "").ToUpper().Trim();
                string Team2 = Regex.Replace(a2.SelectSingleNode(".//span[@class='option-name']").InnerText, RepPatter, "").ToUpper().Trim(); 

                ParsedDataFromPage nob = new ParsedDataFromPage();
                nob.Team1 = Team1;
                nob.Team2 = Team2;
                nob.X[0] = x0;
                nob.X[1] = x1;
                nob.X[2] = x2;
                PD.Add(nob);
             //   System.Windows.Forms.MessageBox.Show(nob.Team1 + "  " + nob.Team2 + "  " + nob.X[1].ToString() + "  " + nob.X[0].ToString() + "  " + nob.X[2].ToString());
            }

            return true;
        }
        
    }
}
