using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting.Contexts;
using System.Data.Entity;
namespace DUE_Mernokinfo_Bot
{
    class DbService
    {
        public BotDbContext context;
        public DbSet<Data> datas;
        public DbService()
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
        public IQueryable<Data> GetDayByDate(DateTime date)
        {
            return this.datas.Where(data => data.StartDate.Year == date.Year && data.StartDate.Month == date.Month && data.StartDate.Day == date.Day);
        }
        public IQueryable<Data> GetHourByDate(DateTime date)
        {
            return this.datas.Where(data => data.StartDate.Year == date.Year && data.StartDate.Month == date.Month && data.StartDate.Day == date.Day && data.StartDate.Hour == date.Hour && data.StartDate.Minute == date.Minute && data.StartDate.Second == date.Second);
        }
        public Data GetNextZh()
        {
            return this.datas.OrderBy(data => data.StartDate).FirstOrDefault(data => data.ZH == true);

        }
        public Data GetNext()
        {
            return this.datas.OrderBy(data => data.StartDate).First();
        }
        public bool GetChatIdByUserName(string user/*/Username/*/)
        {
            if (null =="2")
            {
            }
            else
            {
                return false;
            }
        }

    }
}