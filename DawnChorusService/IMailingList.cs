using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DawnChorusService
{
    public interface IMailingListMember
    {
        string Name { get; set; }
        string Email { get; set; }
        string VerifyCode { get; set; }
        bool Verified { get; set; }
        string SiteName { get; }
    }

    public partial class SnabusMailingList : IMailingListMember
    {
        public string SiteName { get { return "SNABUS"; } }
    }

    public partial class CCCafeMailingList : IMailingListMember
    {
        public string SiteName { get { return "CCCafe"; } }
    }

    public partial class JuliaHaywardMailingList : IMailingListMember
    {
        public string SiteName { get { return "JuliaHayward"; } }
    }
}
