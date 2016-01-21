using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace DawnChorusService
{
    [ServiceContract(Namespace = "http://WebsiteManager")]
    public interface ISiteCounters
    {
        [OperationContract]
        IEnumerable<WebsiteVm> GetCounts();

        [OperationContract]
        int InitiateMailing(string site);

        [OperationContract]
        int ScheduleSingleTweet(int siteId, string status, string retweetingSiteIds, DateTime timeToTweet);

        [OperationContract]
        void AddToSnabusMailingList(string name, string email, bool autoVerify);

        [OperationContract]
        IEnumerable<ScheduledTweetVm> GetScheduledTweets(int siteId);
    }
}
