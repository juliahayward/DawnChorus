using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using WebUtils;

namespace DawnChorusService.Plugins
{
    public class ScheduledTweetSenderPlugin : IPlugin
    {
        private Timer timer;

        public void Start()
        {
            timer = new Timer();
            timer.Interval = 5 * 60 * 1000;
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.Enabled = true;
        }


        public void Stop()
        {
            timer.Enabled = false;
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                var ConsumerKey = ConfigurationManager.AppSettings["twitterConsumerKey"];
                var ConsumerSecret = ConfigurationManager.AppSettings["twitterConsumerSecret"];
                var tweets = TwitterUtils.ProcessSingleTweets(ConsumerKey, ConsumerSecret);
                Log("Tweeted " + tweets + " singletons at " + DateTime.Now.ToShortTimeString(), EventLogEntryType.Information);
                tweets = TwitterUtils.ProcessRecurringTweets(ConsumerKey, ConsumerSecret);
                Log("Tweeted " + tweets + " recurring at " + DateTime.Now.ToShortTimeString(), EventLogEntryType.Information);
            }
            catch (Exception ex)
            {
                Log(ex.Message, EventLogEntryType.Error);
            }
        }

        // Warning - duplicated code
        private void Log(string text, EventLogEntryType type)
        {
            var sSource = "Dawn Chorus";
            if (!EventLog.SourceExists(sSource))
                EventLog.CreateEventSource(sSource, "Application");
            EventLog.WriteEntry(sSource, text, type);
        }

    }
}
