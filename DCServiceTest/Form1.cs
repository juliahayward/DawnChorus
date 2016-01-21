using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LinqToTwitter;
using WebUtils;

namespace DCServiceTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // This mimics the service's timer_Elapsed method
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

        private void Log(string text, EventLogEntryType type)
        {
            var sSource = "Dawn Chorus";
            if (!EventLog.SourceExists(sSource))
                EventLog.CreateEventSource(sSource, "Application");
            EventLog.WriteEntry(sSource, text, type);
        }
    }
}
