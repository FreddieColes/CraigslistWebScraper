using CraigslistWebScraper.Builders;
using CraigslistWebScraper.Data;
using CraigslistWebScraper.Workers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CraigslistWebScraper
{
    class Program
    {
        private const string Method = "search";
        static void Main(string[] args)
        {
            try
            {

                Console.WriteLine("Please enter which city you would like to scrape information from:");
                var craigslistCity = Console.ReadLine() ?? string.Empty;

                Console.WriteLine("Please enter the CraigsList category that you would like to scrape:");
                var craigslistCategoryName = Console.ReadLine() ?? string.Empty;

                using (WebClient client = new WebClient())
                {
                    //Replacing any empty parts of the input with nothing (filling in the gaps)
                    string content = client.DownloadString($"http://{craigslistCity.Replace(" ", string.Empty)}.craigslist.org/{Method}/{craigslistCategoryName}");

                    ScrapeCriteria scrapeCriteria = new ScrapeCriteriaBuilder()
                        .WithData(content)
                        .WithRegex(@"<a href=\""(.*?)\"" data-id=\""(.*?)\"" class=\""result-title hdrlnk\"">(.*?)</a>")
                        .WithRegexOption(RegexOptions.ExplicitCapture)
                        .WithPart(new ScrapeCriteriaPartBuilder()
                            .WithRegex(@">(.*?)</a>")
                            .WithRegexOptions(RegexOptions.Singleline)
                            .Build())
                        .WithPart(new ScrapeCriteriaPartBuilder()
                            .WithRegex(@"href=\""(.*?)\""")
                            .WithRegexOptions(RegexOptions.Singleline)
                            .Build())
                        .Build();

                    Scraper scraper = new Scraper();

                    var scrappedElements = scraper.Scrape(scrapeCriteria);

                    if (scrappedElements.Any())
                    {
                        foreach (var scrappedElement in scrappedElements) Console.WriteLine(scrappedElement);
                        Console.Read();
                    }
                    else
                    {
                        Console.WriteLine("There were no matches for the specified scrape criteria.");
                        Console.Read();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
