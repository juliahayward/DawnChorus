using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebUtils
{
    public class CCCafeMailingListUtils : IMailingList
    {
        public string Subscribe(string name, string email)
        {
            DataClasses1DataContext context = new DataClasses1DataContext();

            var ac = context.CCCafeMailingListMembers.Where(x => x.Email == email);
            if (!ac.Any())
            {
                CCCafeMailingListMember newc = new CCCafeMailingListMember();
                newc.Name = name;
                newc.Email = email;
                string guid = newc.VerifyCode = Guid.NewGuid().ToString();
                newc.Verified = false;
                context.CCCafeMailingListMembers.InsertOnSubmit(newc);
                context.SubmitChanges();
                return guid;
            }
            else
                return null;
        }

        public bool Confirm(string code)
        {
            DataClasses1DataContext context = new DataClasses1DataContext();

            var ac = context.CCCafeMailingListMembers.Where(c => c.VerifyCode == code);
            if (ac.Any())
            {
                foreach (var member in ac)
                {
                    member.Verified = true;
                }
                context.SubmitChanges();
                return true;
            }
            else
                return false;
        }

        public bool Unsubscribe(string code)
        {
            DataClasses1DataContext context = new DataClasses1DataContext();

            var ac = context.CCCafeMailingListMembers.Where(c => c.VerifyCode == code);

            if (ac.Any())
            {
                context.CCCafeMailingListMembers.DeleteOnSubmit(ac.First());
                context.SubmitChanges();
                return true;
            }
            else
                return false;
        }
    }
}
