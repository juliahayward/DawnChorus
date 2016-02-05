using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebUtils
{
    public class HitManager
    {
        public int GetHitCount(string site)
        {
            DataClasses1DataContext context = new DataClasses1DataContext();

            IQueryable<AccessCount> ac = null;

            ac = from c in context.AccessCounts
                 where (c.Site == site)
                 select c;

            if (ac.Count<AccessCount>() == 0)
            {
                AccessCount newc = new AccessCount();
                newc.nSessions = 0;
                newc.Site = site;
                context.AccessCounts.InsertOnSubmit(newc);
                context.SubmitChanges();
                return 0;
            }
            else
                return ac.First<AccessCount>().nSessions;
        }

        public void IncrementHitCount(string site)
        {
            DataClasses1DataContext context = new DataClasses1DataContext();

            IQueryable<AccessCount> ac = null;

            ac = from c in context.AccessCounts
                 where (c.Site == site)
                 select c;

            AccessCount fc = ac.First<AccessCount>();
            fc.nSessions = fc.nSessions + 1;

            context.SubmitChanges();
        }
    }
}
