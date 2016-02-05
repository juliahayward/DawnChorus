using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebUtils
{
    interface IMailingList
    {
        string Subscribe(string name, string email);
        bool Confirm(string verificationCode);
        bool Unsubscribe(string verificationCode);
    }
}
