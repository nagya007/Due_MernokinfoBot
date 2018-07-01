﻿using System;
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
        public IQueryable<UserEnrolled> GetDayByDateAndSingUp(User user, Data data)
        {
           return this.userEnrolleds.Where(u => u.UserId == user.UserId && u.EventId == data.EventId);
        }
        public IQueryable GetSingUpEvent(User user)
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
                          }).Where(z => z.UserId ==user.UserId);
                  return result;
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
        public User GetUserByChatId(long chatid)
        {
            return this.users.FirstOrDefault(u => u.ChatId == chatid);
        }
        public Data GetEventByName(string subjectCode)
        {
            return this.datas.FirstOrDefault(d => d.SubjectCode == subjectCode);
        }
        public IQueryable<Data> GetEventByUserSingUp()
        {
            return null;
            
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
    }
}