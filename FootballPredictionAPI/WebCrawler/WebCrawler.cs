using System.Reflection;
using AutoMapper;
using FootballPredictionAPI.Models;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore.Cosmos.Storage.Internal;

namespace FootballPredictionAPI.WebCrawler;

public class WebCrawler
{

    private readonly string _seed = "https://www.laliga.com/en-NO/laliga-santander/results";
    const string _matchDay = "laliga-santander/results/2022-23/gameweek";
    private const string _baseUrl = "https://www.laliga.com";
    private MapperConfiguration config = new MapperConfiguration(cfg => {  });
    private IMapper _mapper;

    public WebCrawler()
    {
        //_mapper = new Mapper(config);
        _mapper = config.CreateMapper();
    }
    
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

    public HtmlDocument ReadDocFromUrl(string url)
    {
        HtmlDocument htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(url);
        return htmlDocument;
    }

    public FootballMatch ReadStatsForMatch(Match match)
    {
        HtmlWeb web = new HtmlWeb();
        HtmlDocument htmlDoc = web.Load(match.StatsUrl);
        var stats = htmlDoc.DocumentNode.SelectNodes("//div[@class='styled__TabContent-sc-165n2lv-0 bdnghW']")
            .LastOrDefault();
        var table = stats.SelectNodes(".//div[@class='styled__CellStyled-vl6wna-0 gpAwUe']").FirstOrDefault();
        var dict = new Dictionary<string, object>();
        var body = table.SelectNodes("div").Last();
        foreach (HtmlNode row in body.SelectNodes("div")) {
            
            var feat = row.SelectNodes("p").FirstOrDefault().InnerText;
            foreach (HtmlNode cell in row.SelectNodes("div"))
            {
                if (feat != "Penalties")
                {
                    var features = cell.SelectNodes("div").Select(f => f.InnerText).ToArray();

                    string home = "HT" + String.Join("", feat.Split(" "));
                    string away = "AT" + String.Join("", feat.Split(" "));

                    dict.Add(home, Convert.ToDouble(features[0].Replace("%", "").Replace(".", ",")));
                    dict.Add(away, Convert.ToDouble(features[2].Replace("%", "").Replace(".", ",")));

                }
            }
        }

        FootballMatch mp = _mapper.Map<FootballMatch>(dict);

        foreach (PropertyInfo property in typeof(Match).GetProperties().Where(p => p.CanWrite))
        {
            property.SetValue(mp, property.GetValue(match, null), null);
        }

        return mp;
    }
}