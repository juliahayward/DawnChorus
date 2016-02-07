using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading;
using WebUtils;

namespace DawnChorusService
{
    public class SiteCounters : ISiteCounters
    {
        public IEnumerable<WebsiteVm> GetCounts()
        {
            WebUtils.DataClasses1DataContext context = new WebUtils.DataClasses1DataContext();

            return context.Websites.Select(x => new WebsiteVm()
                {
                    Id = x.SiteId,
                    Name = x.SiteName,
                    HitCount = GetCounts(context, x.SiteId),
                    FilesInFTP = FilesInFtp(x.SiteName),
                    SupportsTwitter = (x.TwitterId != null)
                });
        }

        private int GetCounts(WebUtils.DataClasses1DataContext context, int siteId)
        {
            var count = context.AccessCounts.FirstOrDefault(x => x.SiteId == siteId);
            return (count != null) ? count.nSessions : 0;
        }

        private int FilesInFtp(string siteName)
        {
            try
            {
                var files = Directory.GetFiles(@"C:\inetpub\ftproot\" + siteName);
                return files.Count();
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public int InitiateMailing(string sitename)
        {
            if (sitename == "JuliaHayward")
                return InitiateMailingJulia();
            else if (sitename == "SNABUS")
                return InitiateMailingSnabus();
            else
                return InitiateMailingCCCafe();
        }

        public int InitiateMailingJulia()
        {
            var sent = 0;
            try
            {
                DataClasses1DataContext context = new DataClasses1DataContext();

                foreach (var recipient in context.JuliaHaywardMailingLists.Where(x => x.Verified))
                {
                    using (MailMessage message = new MailMessage(
                        new MailAddress("julia.hayward@btconnect.com", "Julia's mailing list"),
                        new MailAddress(recipient.Email)))
                    {

                        message.Body = GetMessage(recipient);
                        message.ReplyToList.Clear();
                        message.ReplyToList.Add(new MailAddress("mailinglist@juliahayward.com"));
                        message.Subject = "Test";

                        string attFolder = @"C:\inetpub\ftproot\JuliaHayward\attachments";
                        foreach (var file in Directory.GetFiles(attFolder))
                            message.Attachments.Add(new Attachment(file));

                        System.Net.Mail.SmtpClient client = new SmtpClient();
                        client.Host = "smtp.outlook.com";
                        client.UseDefaultCredentials = false;
                        client.EnableSsl = true;
                        client.Credentials = new NetworkCredential("julia.hayward@btconnect.com",
                            "bluemeanie1");
                        try
                        {
                            client.Send(message);
                            sent++;
                            Thread.Sleep(2000);
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("derp 1");
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return sent;
        }

        public int InitiateMailingCCCafe()
        {
            var sent = 0;
            try
            {
                DataClasses1DataContext context = new DataClasses1DataContext();

                foreach (var recipient in context.CCCafeMailingLists.Where(x => x.Verified))
                {
                    using (MailMessage message = new MailMessage(
                        new MailAddress("julia.hayward@btconnect.com", "Community Coaching Cafe"),
                        new MailAddress(recipient.Email)))
                    {

                        message.Body = GetMessage(recipient);
                        message.ReplyToList.Clear();
                        message.ReplyToList.Add(new MailAddress("christine@communitycoachingcafe.org.uk"));
                        message.Subject = "Community Coaching Cafe update";

                        string attFolder = @"C:\inetpub\ftproot\CCCafe\attachments";
                        foreach (var file in Directory.GetFiles(attFolder))
                            message.Attachments.Add(new Attachment(file));

                        System.Net.Mail.SmtpClient client = new SmtpClient();
                        client.Host = "smtp.outlook.com";
                        client.UseDefaultCredentials = false;
                        client.EnableSsl = true;
                        client.Credentials = new NetworkCredential("julia.hayward@btconnect.com",
                            "bluemeanie1");
                        try
                        {
                            client.Send(message);
                            sent++;
                            Thread.Sleep(2000);
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("derp 2");
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return sent;
        }

        public void AddToSnabusMailingList(string name, string email, bool autoVerify)
        {
            try
            {
                DataClasses1DataContext context = new DataClasses1DataContext();

                SnabusMailingList newc = new SnabusMailingList();
                newc.Name = name;
                newc.Email = email;
                string guid = newc.VerifyCode = Guid.NewGuid().ToString();
                newc.Verified = true;
                context.SnabusMailingLists.InsertOnSubmit(newc);
                context.SubmitChanges();
            }
            catch (Exception ex)
            {
                FileStream fs = new FileStream("C:\\temp\\dcerror.txt", FileMode.Append, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine(ex.Message);
                sw.Close();
            }
        }

        public int InitiateMailingSnabus()
        {
            var sent = 0;
            try
            {
                DataClasses1DataContext context = new DataClasses1DataContext();

                foreach (var recipient in context.SnabusMailingLists.Where(x => x.Verified))
                {
                    using (MailMessage message = new MailMessage(
                        new MailAddress("julia.hayward@btconnect.com", "St Neots Area Bus Users Society"),
                        new MailAddress(recipient.Email)))
                    {

                        message.Body = GetMessage(recipient);
                        message.ReplyToList.Clear();
                        message.ReplyToList.Add(new MailAddress("info@snabus.org.uk"));
                        message.Subject = "St Neots Area Bus Users Society newsletter";

                        string attFolder = @"C:\inetpub\ftproot\Snabus\attachments";
                        foreach (var file in Directory.GetFiles(attFolder))
                            message.Attachments.Add(new Attachment(file));

                        System.Net.Mail.SmtpClient client = new SmtpClient();
                        client.Host = "smtp.outlook.com";
                        client.UseDefaultCredentials = false;
                        client.EnableSsl = true;
                        client.Credentials = new NetworkCredential("julia.hayward@btconnect.com",
                            "bluemeanie1");
                        try
                        {
                            client.Send(message);
                            sent++;
                            Thread.Sleep(10000);
                        }
                        catch (Exception ex)
                        {
                            FileStream fs = new FileStream("C:\\temp\\dcerror.txt", FileMode.Append, FileAccess.Write);
                            StreamWriter sw = new StreamWriter(fs);
                            sw.WriteLine(recipient.Email + " : " + ex.Message);
                            sw.Close();
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return sent;
        }

        private string GetMessage(IMailingListMember recipient)
        {
            var sb = new StringBuilder();
            string header = @"C:\Program Files\Julia\WebResources\" + recipient.SiteName + @"\header.txt";
            using (var reader = new StreamReader(new FileStream(header, FileMode.Open, FileAccess.Read)))
            {
                while (!reader.EndOfStream)
                {
                    sb.Append(reader.ReadLine().Replace("{name}", recipient.Name.Trim()).Replace("{key}", recipient.VerifyCode));
                    sb.Append(Environment.NewLine);
                }
            }
            string body = @"C:\inetpub\ftproot\" + recipient.SiteName + @"\message.txt";
            using (var reader = new StreamReader(new FileStream(body, FileMode.Open, FileAccess.Read)))
            {
                while (!reader.EndOfStream)
                {
                    sb.Append(reader.ReadLine().Replace("{name}", recipient.Name.Trim()).Replace("{key}", recipient.VerifyCode));
                    sb.Append(Environment.NewLine);
                }
            }
            string footer = @"C:\Program Files\Julia\WebResources\" + recipient.SiteName + @"\footer.txt";
            using (var reader = new StreamReader(new FileStream(footer, FileMode.Open, FileAccess.Read)))
            {
                while (!reader.EndOfStream)
                {
                    sb.Append(reader.ReadLine().Replace("{name}", recipient.Name.Trim()).Replace("{key}", recipient.VerifyCode));
                    sb.Append(Environment.NewLine);
                }
            }
            return sb.ToString();
        }


        public IEnumerable<ScheduledTweetVm> GetScheduledTweets(int siteId)
        {
            WebUtils.DataClasses1DataContext context = new WebUtils.DataClasses1DataContext();

            return context.SingleTweets.Where(x => x.SiteId == siteId)
                .Select(x => new ScheduledTweetVm()
            {
                TimeToTweet = x.TimeToTweet,
                Status = x.Status
            }).OrderBy(x => x.TimeToTweet);
        }

        public int ScheduleSingleTweet(int siteId, string status, string retweetingSiteIds, DateTime timeToTweet)
        {
            var twitterUtils = new TwitterUtils();
            twitterUtils.ScheduleSingleTweet(siteId, status, retweetingSiteIds, timeToTweet);
            return 0;
        }
    }
}
