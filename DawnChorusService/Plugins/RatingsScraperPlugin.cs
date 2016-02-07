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
        private readonly ITwitterUtils _twitterUtils;
        private Timer timer;

        public void Start()
        {
            timer = new Timer();
            timer.Interval = 60 * 60 * 1000;
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.Enabled = true;
        }

        public RatingsScraperPlugin(ITwitterUtils twitterUtils)
        {
            _twitterUtils = twitterUtils;
        }

        public void Stop()
        {
            timer.Enabled = false;
        }


        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (DateTime.UtcNow.Hour != 17) return;

            var results = ScrapeUkbgfRatings();

            TweetAboutFirst(results);

            //var mine = new List<RatingsScraperPlugin.RatingEntry>(results)
            //    .First(x => x.Name == "Julia Hayward");
            //// 14 is fiora
            //var status = string.Format("Look what my human gets up to... only {0}th http://results.ukbgf.com/{1}",
            //    mine.Position, mine.Url);
            //_twitterUtils.ScheduleSingleTweet(14, status, "", DateTime.UtcNow);
        }


        public class RatingEntry
        {
            public string Name { get; set; }
            public int Position { get; set; }
            public double Rating { get; set; }
            public string Url { get; set; }
            public int PlayerId { get; set; }
        }

        // UKBGF is 15 - Fiora is 14

        public void TweetAboutFirst(IEnumerable<RatingsScraperPlugin.RatingEntry> ratings)
        {
            DataClasses1DataContext context = new DataClasses1DataContext();

            var newTop = ratings.First();
            var top = context.AllTimeHighRatings.FirstOrDefault();
            if (top == null)
            {
                var ath = new AllTimeHighRating() { PlayerId = newTop.PlayerId, Rating = (float)newTop.Rating, PlayerName = newTop.Name };
                context.AllTimeHighRatings.InsertOnSubmit(ath);
            }
            else if (top.Rating + 0.01 < (float)newTop.Rating) 
            {
                top.PlayerId = newTop.PlayerId;
                top.PlayerName = newTop.Name;
                top.Rating = (float)newTop.Rating;

                var message = string.Format("{0} has reached the all-time highest rating of {1} - http://results.ukbgf.com/ratings #ratingsbot",
                        newTop.Name, newTop.Rating.ToString("0.00"));
                _twitterUtils.ScheduleSingleTweet(15, message, "", DateTime.UtcNow);
            }
            context.SubmitChanges();
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
                    PlayerId = int.Parse(row.ChildNodes[1].ChildNodes[0].Attributes["href"].Value.Replace("matchlog.php?id=", "")),
                    Rating = double.Parse(row.ChildNodes[3].InnerText)
                };

                yield return rating;
            }
        }
    }
}
