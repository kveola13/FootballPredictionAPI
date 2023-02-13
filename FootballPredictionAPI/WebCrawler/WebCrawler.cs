using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore.Cosmos.Storage.Internal;

namespace FootballPredictionAPI.WebCrawler;

public class WebCrawler
{

    private readonly string _seed = "https://www.laliga.com/en-NO/laliga-santander/results";
    const string _matchDay = "laliga-santander/results/2022-23/gameweek";
    private const string _baseUrl = "https://www.laliga.com";
    
    public List<List<string>> GetMatchDayResults(string matchDayUrl)
    {
        HtmlWeb web = new HtmlWeb();
        HtmlDocument htmlDocument = web.Load(matchDayUrl);
        var res = GetTable(htmlDocument);
        return res;
    }

    private List<List<string>> GetTable(HtmlDocument day)
    {
        Console.WriteLine("WEB CRAWLER GETTABLE()");
        List<List<string>> rows = new List<List<string>>();

        foreach (HtmlNode table in day.DocumentNode.SelectNodes("//table/tbody")) {
            foreach (HtmlNode row in table.SelectNodes("tr")) {
                List<string> line = new List<string>();
                foreach (HtmlNode cell in row.SelectNodes("th|td")) {
                    var links = cell.SelectSingleNode("a");
                    if (links != null)
                    {
                        var link = links.GetAttributeValue("href", null);
                        if (link != null && link.Contains("match/temporada"))
                        {
                            string linkurl = _baseUrl + link;
                            line.Add(linkurl);
                        }
                    }
                    else
                    {
                        if (cell.InnerText != "")
                        {
                            var divs = cell.SelectNodes("div/div");
                            if (divs != null && divs.Count == 3)
                            {
                                foreach (var d in divs)
                                {
                                    line.Add(d.InnerText);
                                }
                            }
                            else
                            {
                                line.Add(cell.InnerText);
                            }
                        }
                    }
                }

                if (line.Count > 2)
                {
                    rows.Add(line);
                }
            }
        }
        
        return rows;
    }
    
    public List<string> GetMatchDays()
    {
        Console.WriteLine("GET MATCH DAYS CRAWLER");
        List<string> matchDays = new List<string>();
        HtmlWeb web = new HtmlWeb();
        HtmlDocument doc = web.Load(_seed);
        var linkedPages = doc.DocumentNode.Descendants("a")
            .Select(a => a.GetAttributeValue("href", null))
            .Where(u => !String.IsNullOrEmpty(u));

        Console.WriteLine("LINKED LInks" + linkedPages.Count());
        foreach (var page in linkedPages)
        {
            if (page.Contains(_matchDay))
            {
                string newPage = _baseUrl + page;
                Console.WriteLine(newPage);
                matchDays.Add(newPage);
            }
        }

        Console.WriteLine(matchDays.Count);
        return matchDays;
    }

    public object ReadDocFromUrl(string url)
    {
        HtmlDocument htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(url);
        return htmlDocument;
    }
}