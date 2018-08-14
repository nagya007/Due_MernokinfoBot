using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;


namespace DUE_Mernokinfo_Bot
{
    class EventHandler
    {
        public BotDbContext context;
        public DbSet<Data> datas;
        public EventHandler()
        {
            context = new BotDbContext();
            this.datas = context.Datas;
        }
        public bool AddData(Data data)
        {
            if (this.datas.Add(data).Equals(data))
            {
                this.datas.Add(data);
                context.SaveChanges();
                return true;
            }
            return false;
        }
        public bool RemoveDataByEndDate()
        {
            var result = this.datas.SingleOrDefault(datas => datas.EndDate <= DateTime.Now);
            if (result != null)
            {
                datas.Remove(result);
                context.SaveChanges();
                return true;
            }
            return false;
        }

    }
}
