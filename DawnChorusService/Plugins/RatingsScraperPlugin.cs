using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using WebUtils;

namespace DawnChorusService.Plugins
{
    public class RatingsScraperPlugin : IPlugin
    {
        private Timer timer;

        public void Start()
        {
            timer = new Timer();
            timer.Interval = 24 * 60 * 60 * 1000;
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.Enabled = true;
        }


        public void Stop()
        {
            timer.Enabled = false;
        }


        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var results = ScrapeUkbgfRatings();
            var mine = new List<RatingsScraperPlugin.RatingEntry>(results)
                .First(x => x.Name == "Julia Hayward");


            var status = string.Format("Look what my human gets up to... only {0}th http://results.ukbgf.com/{1}",
                mine.Position, mine.Url);
            TwitterUtils.ScheduleSingleTweet(14, status, "", DateTime.UtcNow);
        }


        public class RatingEntry
        {
            public string Name { get; set; }
            public int Position { get; set; }
            public double Rating { get; set; }
            public string Url { get; set; }
        }


        public IEnumerable<RatingEntry> ScrapeUkbgfRatings()
        {
            var url = "http://results.ukbgf.com/ratings";

            var request = WebRequest.Create(url);

            var dom = new HtmlAgilityPack.HtmlDocument();
            dom.Load(request.GetResponse().GetResponseStream());
            var table = dom.GetElementbyId("ratings");
            var rows = table.Descendants("tr");
            bool header = true;
            foreach (var row in rows)
            {
                if (header) { header = false; continue; }

                var rating = new RatingEntry()
                {
                    Position = int.Parse(row.ChildNodes[0].InnerText),
                    Name = row.ChildNodes[1].ChildNodes[0].FirstChild.InnerText,
                    Url = row.ChildNodes[1].ChildNodes[0].Attributes["href"].Value,
                    Rating = double.Parse(row.ChildNodes[3].InnerText)
                };

                yield return rating;
            }
        }
    }
}
