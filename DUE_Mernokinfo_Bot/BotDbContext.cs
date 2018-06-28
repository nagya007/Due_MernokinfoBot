using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DUE_Mernokinfo_Bot
{
   public class BotDbContext : DbContext
    {
        public DbSet<Data> Datas { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserEnrolled> UserEnrolleds { get; set; }
    }
}
