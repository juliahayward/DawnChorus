using System;
using System.Diagnostics;
using System.Configuration;
using System.Timers;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceProcess;
using LinqToTwitter;
using WebUtils;
using DawnChorusService.Plugins;

namespace DawnChorusService
{
    class DawnChorusService : ServiceBase
    {
        private ServiceHost selfHost;
        private Timer timer;
        private readonly RatingsScraperPlugin plugin = new RatingsScraperPlugin();

        /// <summary>
        /// Public Constructor for WindowsService.
        /// - Put all of your Initialization code here.
        /// </summary>
        public DawnChorusService()
        {
            this.ServiceName = "Dawn Chorus Service";
            this.EventLog.Log = "Application";

            // These Flags set whether or not to handle that specific
            //  type of event. Set to true if you need it, false otherwise.
            this.CanHandlePowerEvent = true;
            this.CanHandleSessionChangeEvent = true;
            this.CanPauseAndContinue = true;
            this.CanShutdown = true;
            this.CanStop = true;
        }

        /// <summary>
        /// The Main Thread: This is where your Service is Run.
        /// </summary>
        static void Main()
        {
            ServiceBase.Run(new DawnChorusService());
        }

        /// <summary>
        /// Dispose of objects that need it here.
        /// </summary>
        /// <param name="disposing">Whether
        ///    or not disposing is going on.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        /// <summary>
        /// OnStart(): Put startup code here
        ///  - Start threads, get inital data, etc.
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            base.OnStart(args);
            plugin.Start();

            timer = new Timer();
            timer.Interval = 5 * 60 * 1000;
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.Enabled = true;

            Uri baseAddress = new Uri("http://localhost:8000/WebsiteManager/Service"); // localhost to debug

            selfHost = new ServiceHost(typeof(SiteCounters), baseAddress);

            try
            {
                selfHost.AddServiceEndpoint(typeof(ISiteCounters), new WSHttpBinding(), "CounterService"); // basicHttpBinding to debug

                ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
                smb.HttpGetEnabled = true;
                selfHost.Description.Behaviors.Add(smb);

                selfHost.Open();
            }
            catch (CommunicationException ce)
            {
                Log("Aborting: " + ce.Message, EventLogEntryType.Error);
                selfHost.Abort();
            }
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

        private void Log(string text, EventLogEntryType type)
        {
            var sSource = "Dawn Chorus";
            if (!EventLog.SourceExists(sSource))
                EventLog.CreateEventSource(sSource, "Application");
            EventLog.WriteEntry(sSource, text, type);
        }

        

        /// <summary>
        /// OnStop(): Put your stop code here
        /// - Stop threads, set final data, etc.
        /// </summary>
        protected override void OnStop()
        {
            base.OnStop();

            plugin.Stop();

            // Close the ServiceHostBase to shutdown the WCF service.
            selfHost.Close();
        }

        /// <summary>
        /// OnPause: Put your pause code here
        /// - Pause working threads, etc.
        /// </summary>
        protected override void OnPause()
        {
            base.OnPause();
        }

        /// <summary>
        /// OnContinue(): Put your continue code here
        /// - Un-pause working threads, etc.
        /// </summary>
        protected override void OnContinue()
        {
            base.OnContinue();
        }

        /// <summary>
        /// OnShutdown(): Called when the System is shutting down
        /// - Put code here when you need special handling
        ///   of code that deals with a system shutdown, such
        ///   as saving special data before shutdown.
        /// </summary>
        protected override void OnShutdown()
        {
            base.OnShutdown();
        }

        /// <summary>
        /// OnCustomCommand(): If you need to send a command to your
        ///   service without the need for Remoting or Sockets, use
        ///   this method to do custom methods.
        /// </summary>
        /// <param name="command">Arbitrary Integer between 128 & 256</param>
        protected override void OnCustomCommand(int command)
        {
            //  A custom command can be sent to a service by using this method:
            //#  int command = 128; //Some Arbitrary number between 128 & 256
            //#  ServiceController sc = new ServiceController("NameOfService");
            //#  sc.ExecuteCommand(command);

            base.OnCustomCommand(command);
        }

        /// <summary>
        /// OnPowerEvent(): Useful for detecting power status changes,
        ///   such as going into Suspend mode or Low Battery for laptops.
        /// </summary>
        /// <param name="powerStatus">The Power Broadcast Status
        /// (BatteryLow, Suspend, etc.)</param>
        protected override bool OnPowerEvent(PowerBroadcastStatus powerStatus)
        {
            return base.OnPowerEvent(powerStatus);
        }

        /// <summary>
        /// OnSessionChange(): To handle a change event
        ///   from a Terminal Server session.
        ///   Useful if you need to determine
        ///   when a user logs in remotely or logs off,
        ///   or when someone logs into the console.
        /// </summary>
        /// <param name="changeDescription">The Session Change
        /// Event that occured.</param>
        protected override void OnSessionChange(
                  SessionChangeDescription changeDescription)
        {
            base.OnSessionChange(changeDescription);
        }
    }
}