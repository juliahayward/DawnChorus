using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DawnChorusService.Plugins;
using System.Collections.Generic;
using WebUtils;

namespace DawnChorusServiceTests
{
    [TestClass]
    public class RatingsScraperTests
    {
        [TestMethod]
        public void Test_ScraperWorks()
        {
            var scraper = new RatingsScraperPlugin();
            var results = scraper.ScrapeUkbgfRatings();

            // Didn't throw - great
            var mine = new List<RatingsScraperPlugin.RatingEntry>(results)
                .First(x => x.Name == "Julia Hayward");

            Assert.AreEqual(267, mine.Position);
            Assert.AreEqual("matchlog.php?id=369", mine.Url);
            // translates to http://results.ukbgf.com/matchlog?id=369
        }
    }
}
