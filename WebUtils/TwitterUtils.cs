using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqToTwitter;

namespace WebUtils
{
    public class TwitterUtils
    {
        public static int GetNumberOfMentions(TwitterContext twitterCtx)
        {
            var myMentions =
                from mention in twitterCtx.Status
                where mention.Type == StatusType.Mentions
                select mention;

            //    myMentions.ToList().ForEach(
            //        mention => Console.WriteLine(
            //            "Name: {0}, Tweet[{1}]: {2}\n",
            //            mention.User.Name, mention.StatusID, mention.Text));
            return myMentions.Count();
        }

        public static int ProcessSingleTweets(string ConsumerKey, string ConsumerSecret)
        {
            DataClasses1DataContext context = new DataClasses1DataContext();

            var now = DateTime.Now;
            var singleTweets = context.SingleTweets.Where(x => x.TimeToTweet < now).ToList();
            var tweetsProcessed = 0;
            foreach (var tweet in singleTweets)
            {
                ulong tweetId = 0;
                var site = context.Websites.FirstOrDefault(x => x.SiteId == tweet.SiteId);
                var auth = new SingleUserAuthorizer
                {
                    CredentialStore = new InMemoryCredentialStore
                    {
                        ConsumerKey = ConsumerKey,
                        ConsumerSecret = ConsumerSecret,
                        OAuthToken = site.TwitterOauthToken,
                        OAuthTokenSecret = site.TwitterAccessToken
                    }
                };
                using (var twitterCtx = new TwitterContext(auth))
                {
                    var task = twitterCtx.TweetAsync(tweet.Status);
                    task.Wait();
                    var tweetStatus = task.Result;
                    tweetId = tweetStatus.StatusID;
                }
                if (tweet.RetweetingSiteIds != null)
                {
                    foreach (var retweetingSiteId in tweet.RetweetingSiteIds.Split(','))
                    {
                        var retweetingSite = context.Websites.FirstOrDefault(x => x.SiteId == int.Parse(retweetingSiteId));
                        if (retweetingSite == null) continue;
                        var retweetingAuth = new SingleUserAuthorizer
                        {
                            CredentialStore = new InMemoryCredentialStore
                            {
                                ConsumerKey = ConsumerKey,
                                ConsumerSecret = ConsumerSecret,
                                OAuthToken = retweetingSite.TwitterOauthToken,
                                OAuthTokenSecret = retweetingSite.TwitterAccessToken
                            }
                        };
                        using (var twitterCtx = new TwitterContext(retweetingAuth))
                        {
                            var task = twitterCtx.RetweetAsync(tweetId);
                            task.Wait();
                        }
                    }
                }
                tweetsProcessed++;
            }
            context.SingleTweets.DeleteAllOnSubmit(singleTweets);
            context.SubmitChanges();

            return tweetsProcessed;
        }

        public static int ProcessRecurringTweets(string ConsumerKey, string ConsumerSecret)
        {
            DataClasses1DataContext context = new DataClasses1DataContext();

            var now = DateTime.Now;
            var recurringTweets = context.RecurringTweets.Where(x => x.TimeToTweet < now).ToList();
            var tweetsProcessed = 0;
            foreach (var tweet in recurringTweets)
            {
                ulong tweetId;
                var site = context.Websites.FirstOrDefault(x => x.SiteId == tweet.SiteId);
                var auth = new SingleUserAuthorizer
                {
                    CredentialStore = new InMemoryCredentialStore
                    {
                        ConsumerKey = ConsumerKey,
                        ConsumerSecret = ConsumerSecret,
                        OAuthToken = site.TwitterOauthToken,
                        OAuthTokenSecret = site.TwitterAccessToken
                    }
                };
                using (var twitterCtx = new TwitterContext(auth))
                {
                    var task = twitterCtx.TweetAsync(tweet.Status);
                    task.Wait();
                    var tweetStatus = task.Result;
                    tweetId = tweetStatus.StatusID;
                }
                if (tweet.RetweetingSiteIds != null)
                {
                    foreach (var retweetingSiteId in tweet.RetweetingSiteIds.Split(','))
                    {
                        var retweetingSite = context.Websites.FirstOrDefault(x => x.SiteId == int.Parse(retweetingSiteId));
                        if (retweetingSite == null) continue;
                        var retweetingAuth = new SingleUserAuthorizer
                        {
                            CredentialStore = new InMemoryCredentialStore
                            {
                                ConsumerKey = ConsumerKey,
                                ConsumerSecret = ConsumerSecret,
                                OAuthToken = retweetingSite.TwitterOauthToken,
                                OAuthTokenSecret = retweetingSite.TwitterAccessToken
                            }
                        };
                        using (var twitterCtx = new TwitterContext(retweetingAuth))
                        {
                            var task = twitterCtx.RetweetAsync(tweetId);
                            task.Wait();
                        }
                    }
                }
                tweetsProcessed++;
                if (tweet.PeriodInDays.HasValue)
                    tweet.TimeToTweet = tweet.TimeToTweet.AddDays(tweet.PeriodInDays.Value);
                else if (tweet.PeriodInMonths.HasValue)
                    tweet.TimeToTweet = tweet.TimeToTweet.AddMonths(tweet.PeriodInMonths.Value);
                else if (tweet.DayOfMonth.HasValue)
                // "4" means 4th (whatever day of week) of the month
                {
                    var nextDate = tweet.TimeToTweet.AddDays(28);
                    if ((nextDate.Month == tweet.TimeToTweet.Month) // hasn't made it to next month
                        || (nextDate.Day <= 7 * (tweet.DayOfMonth.Value - 1))) // falls a week too early
                        nextDate = nextDate.AddDays(7);
                    tweet.TimeToTweet = nextDate;
                }

                if (tweet.TimeToTweet > tweet.TimeToStopTweeting)
                    context.RecurringTweets.DeleteOnSubmit(tweet);
            }
            context.SubmitChanges();

            return tweetsProcessed;
        }

        public static void ScheduleSingleTweet(int siteId, string status, string retweetingSiteIds,
            DateTime timeToTweet)
        {
            DataClasses1DataContext context = new DataClasses1DataContext();

            var tweet = new SingleTweet()
            {
                SiteId = siteId,
                Status = status,
                RetweetingSiteIds = retweetingSiteIds,
                TimeToTweet = timeToTweet
            };

            context.SingleTweets.InsertOnSubmit(tweet);
            context.SubmitChanges();
        }

        
    }
}
