using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebUtils
{
    public interface IMailingListMember
    {
        string Name { get; set; }
        string Email { get; set; }
        string VerifyCode { get; set; }
        bool Verified { get; set; }
        string SiteName { get; }
    }

    public partial class SnabusMailingListMember : IMailingListMember
    {
        public string SiteName { get { return "SNABUS"; } }
    }

    public partial class CCCafeMailingListMember : IMailingListMember
    {
        public string SiteName { get { return "CCCafe"; } }
    }

    public partial class JuliaHaywardMailingListMember : IMailingListMember
    {
        public string SiteName { get { return "JuliaHayward"; } }
    }
}
