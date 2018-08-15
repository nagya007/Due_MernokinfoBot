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
        public DbSet<User> users;
        public DbSet<UserEnrolled> userEnrolleds;
        public DbService()
        {
            context = new BotDbContext();
            this.datas = context.Datas;
            this.users = context.Users;
            this.userEnrolleds = context.UserEnrolleds;
        }    
        public IQueryable<Data> GetDayByDate(DateTime date)
        {
            return this.datas.Where(data => data.StartDate.Year == date.Year && data.StartDate.Month == date.Month && data.StartDate.Day == date.Day);
        }
        public IQueryable<UserEnrolled> GetDayByDateAndSingUp(User user, Data data)
        {
            return this.userEnrolleds.Where(u => u.UserId == user.UserId && u.EventId == data.EventId);
        }
        public string GetSingUpEvent(User user)
        {
            var result = (from d in datas
                          join ur in userEnrolleds on d.EventId equals ur.EventId
                          join u in users on ur.UserId equals u.UserId
                          select new
                          {
                              d.EventId,
                              u.UserId,
                              d.StartDate,
                              d.EndDate,
                              d.SubjectCode,
                              d.ClassCode,
                              d.ZH
                          }).Where(z => z.UserId == user.UserId);
            string kiir = "";
            foreach (var item in result)
            {               
                kiir += $"{Writer.wSubjectcode}{item.SubjectCode}\n" +
                            $"{Writer.wClasscode}{item.ClassCode}\n" +
                            $"{Writer.wStartdate}\n{item.StartDate} \n " +
                            $"{Writer.wEnddate}\n{item.EndDate} \n " +
                            $"{Writer.wZh}{item.ZH}\n \n";
            }
            return kiir;
        }
        public string GetNextZhByUser(User user)
        {
            var result = (from d in datas
                          join ur in userEnrolleds on d.EventId equals ur.EventId
                          join u in users on ur.UserId equals u.UserId
                          select new
                          {
                              d.EventId,
                              u.UserId,
                              d.StartDate,
                              d.EndDate,
                              d.SubjectCode,
                              d.ClassCode,
                              d.ZH
                          }).Where(z => z.UserId == user.UserId).OrderBy(d => d.StartDate).FirstOrDefault(d => d.ZH == true);
            string kiir = "";
            if (result!=null)
            {
                kiir += $"{Writer.wSubjectcode}\n {result.SubjectCode}\n" +
              $"{Writer.wClasscode}\n {result.ClassCode}\n" +
              $"{Writer.wStartdate}\n{result.StartDate} \n " +
              $"{Writer.wEnddate}\n {result.EndDate} \n {Writer.wZh}{result.ZH}\n";
                return kiir;
            }
            return kiir;    
        }
        public string GetDayByUserByDate(User user, DateTime date)
        {
            var resultu = (from d in datas
                           join ur in userEnrolleds on d.EventId equals ur.EventId
                           join u in users on ur.UserId equals u.UserId
                           select new
                           {
                               d.EventId,
                               u.UserId,
                               d.StartDate,
                               d.EndDate,
                               d.SubjectCode,
                               d.ClassCode,
                               d.ZH
                           }).Where(z => z.UserId == user.UserId).Where(d => d.StartDate.Year == date.Year && d.StartDate.Month == date.Month && d.StartDate.Day == date.Day);

            string kiir = "";
            foreach (var item in resultu)
            {
                kiir += $"{Writer.wSubjectcode}\n {item.SubjectCode}\n" +
               $"{Writer.wClasscode}\n{item.ClassCode}\n" +
               $"{Writer.wStartdate}\n{item.StartDate} \n " +
               $"{Writer.wEnddate}\n{item.EndDate} \n " +
               $"{Writer.wZh}\n{item.ZH}\n";
            }
            return kiir;
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
        public bool IsUserExists(string username)
        {
            return this.users.Any(u => u.UserName == username);
        }
        public bool UpdateUserByAndUserName(string username, long chatid)
        {
            var result = this.users.SingleOrDefault(u => u.UserName == username);
            if (result != null)
            {
                result.ChatId = chatid;
                context.SaveChanges();
                return true;
            }
            return false;
        }             
        public User GetUserByChatId(long chatid)
        {
            return this.users.FirstOrDefault(u => u.ChatId == chatid);
        }
        public Data GetEventByName(string subjectCode)
        {
            return this.datas.FirstOrDefault(d => d.SubjectCode == subjectCode);
        }
        public bool IsUserSingUpEvent(User user, Data data)
        {
            if (this.userEnrolleds.Any(u => u.UserId == user.UserId) && this.userEnrolleds.Any(d => d.EventId == data.EventId))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public IQueryable<Data> GetAllData()
        {
           return this.datas.Select(s => s);        
        }            
    }
}