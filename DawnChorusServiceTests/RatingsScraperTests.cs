using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DawnChorusService.Plugins;
using System.Collections.Generic;
using WebUtils;
using Rhino.Mocks;

namespace DawnChorusServiceTests
{
    [TestClass]
    public class RatingsScraperTests
    {
        [TestMethod]
        public void Test_ScraperWorks()
        {
            var mockTU = MockRepository.GenerateMock<ITwitterUtils>();
            var scraper = new RatingsScraperPlugin(mockTU);
            var results = scraper.ScrapeUkbgfRatings();

            // Didn't throw - great
            var mine = new List<RatingsScraperPlugin.RatingEntry>(results)
                .First(x => x.Name == "Julia Hayward");

            Assert.AreEqual(267, mine.Position);
            Assert.AreEqual("matchlog.php?id=369", mine.Url);
            // translates to http://results.ukbgf.com/matchlog?id=369
        }

        [TestMethod]
        public void Scraper_TweetAboutFirst_TweetsOnce()
        {
            var mockTU = MockRepository.GenerateMock<ITwitterUtils>();
            mockTU.Expect(x => x.ScheduleSingleTweet(14, "", "", DateTime.Now)).IgnoreArguments();
            var scraper = new RatingsScraperPlugin(mockTU);
            var results = scraper.ScrapeUkbgfRatings();

            scraper.TweetAboutFirst(results);

            mockTU.VerifyAllExpectations();
        }

    }
}
