using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace DUE_Mernokinfo_Bot
{
    class SubscriptionHandler
    {
        public BotDbContext context;
        public DbSet<UserEnrolled> userEnrolleds;
        public SubscriptionHandler()
        {
            context = new BotDbContext();
            this.userEnrolleds = context.UserEnrolleds;
        }
        public bool SingOutEvent(UserEnrolled userEnrolled)
        {
            var result = this.userEnrolleds.FirstOrDefault(ue => ue.EventId == userEnrolled.EventId && ue.UserId == userEnrolled.UserId);
            if (result != null)
            {
                this.userEnrolleds.Remove(result);
                context.SaveChanges();
                return true;
            }
            return false;
        }
        public bool SingUpEvent(UserEnrolled userEnrolled)
        {
            if (this.userEnrolleds.Add(userEnrolled).Equals(userEnrolled))
            {
                this.userEnrolleds.Add(userEnrolled);
                context.SaveChanges();
                return true;
            }
            return false;
        }
    }
}
