using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using System.Timers;
using System.Data.Entity;
using System.Data;
using System.Runtime.Remoting.Contexts;
using System.Threading;
namespace DUE_Mernokinfo_Bot
{
    class Program
    {
        private static readonly TelegramBotClient Bot = new TelegramBotClient("506745618:AAGWmuVwrFYxJZcYeaC3tXX9TDyQn3LbDzo");
        public DbService dbService = new DbService();
        public BotDbContext context = new BotDbContext();
        public static string username;
        
        // public static long chatid;
        static void Main(string[] args)
        {
            Thread printer = new Thread(new ThreadStart(InvokeMethod));
            printer.Start();
            Bot.OnMessage += On_Message;
            Bot.OnMessageEdited += On_Message;
            Bot.StartReceiving();
            Console.ReadLine();
            Bot.StopReceiving();
            Console.ReadLine();
        }
        static void InvokeMethod()
        {
            while (true)
            {
                PrintTime();
                Thread.Sleep(1000);
            }
        }
        public static void PrintTime()
        {
            DbService dbService = new DbService();
            dbService.RemoveDataByEndDate();
            Console.WriteLine(DateTime.Now.ToString());
            DateTime onehure = DateTime.Now.AddHours(1);
            IQueryable<Data> ringonehour = dbService.GetHourByDate(onehure);
            string ringone = "";
            string ringten = "";
            foreach (var item in ringonehour)
            {
                ringone += $"Egy óra múlva kezdődik! \n {item.SubjectCode}, {item.ClassCode}, {item.StartDate}, {item.EndDate}, {item.ZH}";
                Console.WriteLine($" {item.SubjectCode}, {item.ClassCode}, {item.StartDate}, {item.EndDate}, {item.ZH}");
            }
            Bot.SendTextMessageAsync(72204263, ringone);

            DateTime tenminutedate = DateTime.Now.AddMinutes(10);
            IQueryable<Data> ringtenminute = dbService.GetHourByDate(tenminutedate);
            foreach (var item in ringtenminute)
            {
                ringten += $"10 perc múlva kezdődik! \n {item.SubjectCode}, {item.ClassCode}, {item.StartDate}, {item.EndDate}, {item.ZH}";
                Console.WriteLine($"  {item.SubjectCode}, {item.ClassCode}, {item.StartDate}, {item.EndDate}, {item.ZH}  ");
            }
            Bot.SendTextMessageAsync(72204263, ringten);

        }
        public static void On_Message(object sender, MessageEventArgs e)
        {
            DbService dbService = new DbService();
            if (e.Message.Chat.Id == -261290206)
            {
                if (e.Message.Type == MessageType.Text)
                {
                    string[] messages = e.Message.Text.Split(' ');
                    if (messages.Length == 0)
                        return;
                    string command = messages[0].Replace("@DueMernokinfoBot", string.Empty).ToLower();
                    switch (command)
                    {
                        case "/id":
                            try
                            {
                                Bot.SendTextMessageAsync(e.Message.Chat.Id, e.Message.Chat.Id.ToString());
                                Console.WriteLine(e.Message.Chat.Id.ToString());
                                break;
                            }
                            catch (Exception)
                            {
                                Bot.SendTextMessageAsync(e.Message.Chat.Id, $"Valami hiba lépett fel kérlek probáld később!");
                                Bot.SendTextMessageAsync(72204263, $"Hiba a today paramcsal");
                                break;
                            }
                        case "/addzh":
                            try
                            {
                                Data newdata = new Data();
                                newdata.StartDate = Convert.ToDateTime(messages[1]);
                                newdata.EndDate = Convert.ToDateTime(messages[2]);
                                newdata.SubjectCode = messages[3];
                                newdata.ClassCode = messages[4];
                                newdata.ZH = Convert.ToBoolean(messages[5]);
                                dbService.AddData(newdata);
                                Bot.SendTextMessageAsync(e.Message.Chat.Id, "Sikeresen hozzáadtad az eseményt!");
                                break;
                            }
                            catch (Exception)
                            {
                                Bot.SendTextMessageAsync(e.Message.Chat.Id, $"Hiba az adatok felvitele küzben probálja újra figyeljen a formára!");
                                break;
                            }
                        case "/today":
                            try
                            {
                                DateTime tudaydate = DateTime.Now;
                                IQueryable<Data> today = null;
                                today = dbService.GetDayByDate(tudaydate);
                                if (today != null)
                                {
                                    foreach (var item in today)
                                    {
                                        Bot.SendTextMessageAsync(e.Message.Chat.Id, $"{item.SubjectCode}, {item.ClassCode}, {item.StartDate}, {item.EndDate}, {item.ZH}");
                                        Console.WriteLine($" {item.SubjectCode}, {item.ClassCode}, {item.StartDate}, {item.EndDate}, {item.ZH}");
                                    }
                                    break;
                                }
                                else
                                {
                                    Bot.SendTextMessageAsync(e.Message.Chat.Id, $"Nincs a mai napon esemény!");
                                    break;
                                }
                            }
                            catch (Exception)
                            {
                                Bot.SendTextMessageAsync(e.Message.Chat.Id, $"Valami hiba lépett fel kérlek probáld később!");
                                Bot.SendTextMessageAsync(72204263, $"Hiba a today parancsal");
                                break;
                            }
                        case "/nextday":
                            try
                            {
                                DateTime nextdaydate = DateTime.Now.AddDays(1);
                                IQueryable<Data> nextday = null;

                                nextday = dbService.GetDayByDate(nextdaydate);
                                if (nextday != null)
                                {
                                    foreach (var item in nextday)
                                    {
                                        Bot.SendTextMessageAsync(e.Message.Chat.Id, $"{item.SubjectCode}, {item.ClassCode}, {item.StartDate}, {item.EndDate}, {item.ZH}");
                                        Console.WriteLine($" {item.SubjectCode}, {item.ClassCode}, {item.StartDate}, {item.EndDate}, {item.ZH}");
                                    }
                                    break;
                                }
                                else
                                {
                                    Bot.SendTextMessageAsync(e.Message.Chat.Id, $"Nincs a holnapi napon esemény!");
                                    break;
                                }

                            }
                            catch (Exception)
                            {
                                Bot.SendTextMessageAsync(e.Message.Chat.Id, $"Valami hiba lépett fel kérlek probáld később!");
                                Bot.SendTextMessageAsync(72204263, $"Hiba a nextday parancsal");
                                break;
                            }
                        case "/nextzh":
                            try
                            {
                                var nextzh = dbService.GetNextZh();
                                if (nextzh != null)
                                {
                                    Bot.SendTextMessageAsync(e.Message.Chat.Id, $"{nextzh.SubjectCode} {nextzh.ClassCode} {nextzh.StartDate} {nextzh.EndDate}   ");
                                    break;
                                }
                                else
                                {
                                    Bot.SendTextMessageAsync(e.Message.Chat.Id, $"Nincs zh!");
                                    break;
                                }
                            }
                            catch (Exception)
                            {
                                Bot.SendTextMessageAsync(e.Message.Chat.Id, $"Valami hiba lépett fel kérlek probáld később!");
                                Bot.SendTextMessageAsync(72204263, $"Hiba a nextzh parancsal");
                                break;
                            }
                        case "/help":
                            try
                            {
                                Bot.SendTextMessageAsync(e.Message.Chat.Id, $" fuck Elérhatő utasítások: addzh - [StartDate][EndDate][SubjectCode][ClassCode][Zh(true, false)] nextzh - következő zh nextday - következő napi teendők(zh - k) today - mai teendők(zh - k) help - elérhető utasítások donate - ...");
                                break;
                            }
                            catch (Exception)
                            {
                                Bot.SendTextMessageAsync(e.Message.Chat.Id, $"Valami hiba lépett fel kérlek probáld később!");
                                Bot.SendTextMessageAsync(72204263, $"Hiba a help parancsal");
                                break;
                            }
                        case "/singupevent":
                            try
                            {
                                UserEnrolled userenrolled = new UserEnrolled();
                                User suser = dbService.GetUserByChatId(e.Message.Chat.Id);
                                Data data = dbService.GetEventByName(messages[1].ToString());
                                if (!dbService.IsUserSingUpEvent(suser, data))
                                {
                                    userenrolled.UserId = suser.UserId;
                                    userenrolled.EventId = data.EventId;
                                    dbService.SingUpEvent(userenrolled);
                                    Console.WriteLine("Add event!");
                                    break;
                                }
                                else
                                {
                                    Bot.SendTextMessageAsync(e.Message.Chat.Id, $"Már feliratkoztál az eseményre. {data.StartDate} kor kezdődik!");
                                    break;
                                }
                            }
                            catch (Exception)
                            {
                                Bot.SendTextMessageAsync(e.Message.Chat.Id, $"Valami probléma történt nem sikerült feliratkozni az eseményre figyelj a név megadására!");
                                break;
                            }
                        case "/start":
                            try
                            {
                                if (dbService.UpdateUserByAndUserName(e.Message.Chat.Username, e.Message.Chat.Id))
                                {
                                    Console.WriteLine($"Sikeres Hozzáadás");
                                }
                                else
                                {
                                    Console.WriteLine("Sikertelen!");
                                }
                                Bot.SendTextMessageAsync(e.Message.Chat.Id, $"Ez a bot a Dunaújvárosi Mérnökinfós hallgatók eseményeinek nyilvántartására és jelzésére jött létre.");
                                Bot.SendTextMessageAsync(e.Message.Chat.Id, e.Message.Chat.Id.ToString());
                                username = e.Message.Chat.Username;
                                Console.WriteLine($"azt írta {username} hogy start");
                                break;
                            }
                            catch (Exception)
                            {
                                Bot.SendTextMessageAsync(e.Message.Chat.Id, $"Valami hiba lépett fel kérlek probáld később!");
                                Bot.SendTextMessageAsync(72204263, $"Hiba a help parancsal");
                                break;
                            }
                        case "/next":
                            try
                            {
                                var next = dbService.GetNext();
                                if (next != null)
                                {
                                    Bot.SendTextMessageAsync(e.Message.From.Username, $"{next.SubjectCode} {next.ClassCode} {next.StartDate} {next.EndDate}");
                                    break;
                                }
                                else
                                {
                                    Bot.SendTextMessageAsync(e.Message.Chat.Id, $"Nincs esemény!");
                                    break;
                                }
                            }
                            catch (Exception)
                            {
                                Bot.SendTextMessageAsync(e.Message.Chat.Id, $"Valami hiba lépett fel kérlek probáld később!");
                                Bot.SendTextMessageAsync(72204263, $"Hiba a next parancsal");
                                break;
                            }
                        case "/donate":
                            try
                            {
                                Bot.SendTextMessageAsync(e.Message.Chat.Id, $"Tudtam hogy valaki kipróbálja {e.Message.Chat.Username}");
                                Bot.SendTextMessageAsync(72204263, $"Támogatni akart {e.Message.Chat.Username}");
                                Bot.SendTextMessageAsync(e.Message.Chat.Id, $"paypal.me/nagya007");
                                break;
                            }
                            catch (Exception)
                            {
                                Bot.SendTextMessageAsync(e.Message.Chat.Id, $"Valami hiba lépett fel kérlek probáld később!");
                                Bot.SendTextMessageAsync(72204263, $"Hiba a donate parancsal");
                                break;
                            }
                        case "/test":
                            try
                            {
                                User user = dbService.GetUserByChatId(e.Message.Chat.Id);
                                string result = dbService.GetSingUpEvent(user);
                                Bot.SendTextMessageAsync(e.Message.Chat.Id, result);
                                break;
                            }
                            catch (Exception)
                            {
                                Bot.SendTextMessageAsync(e.Message.Chat.Id, $"Valami nem sikerült kérlek próbáld később!");
                                break;
                            }
                    }
                }
            }
            else if (dbService.IsUserExists(e.Message.Chat.Username))
            {
                string[] messages = e.Message.Text.Split(' ');
                if (messages.Length == 0)
                    return;
                string command = messages[0].Replace("@DueMernokinfoBot", string.Empty).ToLower();
                switch (command)
                {
                    case "/myid":
                        try
                        {
                            Bot.SendTextMessageAsync(e.Message.Chat.Id, e.Message.Chat.Id.ToString());
                            Console.WriteLine(e.Message.Chat.Id.ToString());
                            break;
                        }
                        catch (Exception)
                        {
                            Bot.SendTextMessageAsync(e.Message.Chat.Id, $"Valami hiba lépett fel kérlek probáld később!");
                            Bot.SendTextMessageAsync(72204263, $"Hiba a today paramcsal");
                            break;
                        }
                    case "/addzh":
                        try
                        {
                            Data newdata = new Data();
                            newdata.StartDate = Convert.ToDateTime(messages[1]);
                            newdata.EndDate = Convert.ToDateTime(messages[2]);
                            newdata.SubjectCode = messages[3];
                            newdata.ClassCode = messages[4];
                            newdata.ZH = Convert.ToBoolean(messages[5]);
                            dbService.AddData(newdata);
                            Bot.SendTextMessageAsync(e.Message.Chat.Id, "Sikeresen hozzáadtad az eseményt!");
                            break;
                        }
                        catch (Exception)
                        {
                            Bot.SendTextMessageAsync(e.Message.Chat.Id, $"Hiba az adatok felvitele küzben probálja újra figyeljen a formára!");
                            break;
                        }
                    case "/mytoday":
                        try
                        {
                            DateTime tudaydate = DateTime.Now;
                            IQueryable<Data> today = null;
                            today = dbService.GetDayByDate(tudaydate);
                            if (today != null)
                            {
                                foreach (var item in today)
                                {
                                    Bot.SendTextMessageAsync(e.Message.Chat.Id, $"{item.SubjectCode}, {item.ClassCode}, {item.StartDate}, {item.EndDate}, {item.ZH}");
                                    Console.WriteLine($" {item.SubjectCode}, {item.ClassCode}, {item.StartDate}, {item.EndDate}, {item.ZH}");
                                }
                                break;
                            }
                            else
                            {
                                Bot.SendTextMessageAsync(e.Message.Chat.Id, $"Nincs a mai napon esemény!");
                                break;
                            }

                        }
                        catch (Exception)
                        {
                            Bot.SendTextMessageAsync(e.Message.Chat.Id, $"Valami hiba lépett fel kérlek probáld később!");
                            Bot.SendTextMessageAsync(72204263, $"Hiba a today parancsal");
                            break;
                        }
                    case "/mynextday":
                        try
                        {
                            DateTime nextdaydate = DateTime.Now.AddDays(1);
                            IQueryable<Data> nextday = null;
                            nextday = dbService.GetDayByDate(nextdaydate);
                            if (nextday != null)
                            {
                                foreach (var item in nextday)
                                {
                                    Bot.SendTextMessageAsync(e.Message.Chat.Id, $"{item.SubjectCode}, {item.ClassCode}, {item.StartDate}, {item.EndDate}, {item.ZH}");
                                    Console.WriteLine($" {item.SubjectCode}, {item.ClassCode}, {item.StartDate}, {item.EndDate}, {item.ZH}");
                                }
                                break;
                            }
                            else
                            {
                                Bot.SendTextMessageAsync(e.Message.Chat.Id, $"Nincs a holnapi napon esemény!");
                                break;
                            }

                        }
                        catch (Exception)
                        {
                            Bot.SendTextMessageAsync(e.Message.Chat.Id, $"Valami hiba lépett fel kérlek probáld később!");
                            Bot.SendTextMessageAsync(72204263, $"Hiba a nextday parancsal");
                            break;
                        }
                    case "/nextmyday":
                        try
                        {
                            break;
                        }
                        catch (Exception)
                        {
                            break;
                        }
                    case "/mynextzh":
                        try
                        {
                            Writer w = new Writer();
                            User nextuser = dbService.GetUserByChatId(e.Message.Chat.Id);
                            dbService.GetNextZhByUser(nextuser);
                            //if (w.WSubjectCode!=null)
                            //{
                                Bot.SendTextMessageAsync(e.Message.Chat.Id, $"{w.WEventId} {w.WUserId} {w.WSubjectCode} {w.WStartDate} {w.WEndDate} {w.WClassCode} {w.WZh}");
                                break;
                            //}
                            //else
                            //{
                            //    Bot.SendTextMessageAsync(e.Message.Chat.Id, $"Nincs zh!");
                            //    break;
                            //}
                        }
                        catch (Exception)
                        {
                            Bot.SendTextMessageAsync(e.Message.Chat.Id, $"Valami hiba lépett fel kérlek probáld később!");
                            Bot.SendTextMessageAsync(72204263, $"Hiba a nextzh parancsal");
                            break;
                        }
                    case "/help":
                        try
                        {
                            Bot.SendTextMessageAsync(e.Message.Chat.Id, $" Elérhatő utasítások: addzh - [StartDate][EndDate][SubjectCode][ClassCode][Zh(true, false)] nextzh - következő zh nextday - következő napi teendők(zh - k) today - mai teendők(zh - k) help - elérhető utasítások donate - ...");
                            break;
                        }
                        catch (Exception)
                        {
                            Bot.SendTextMessageAsync(e.Message.Chat.Id, $"Valami hiba lépett fel kérlek probáld később!");
                            Bot.SendTextMessageAsync(72204263, $"Hiba a help parancsal");
                            break;
                        }
                    case "/singupevent":
                        try
                        {
                            UserEnrolled userenrolled = new UserEnrolled();
                            User suser = dbService.GetUserByChatId(e.Message.Chat.Id);
                            Data data = dbService.GetEventByName(messages[1].ToString());
                            if (!dbService.IsUserSingUpEvent(suser, data))
                            {
                                userenrolled.UserId = suser.UserId;
                                userenrolled.EventId = data.EventId;
                                dbService.SingUpEvent(userenrolled);
                                Console.WriteLine("Add event!");
                                break;
                            }
                            else
                            {
                                Bot.SendTextMessageAsync(e.Message.Chat.Id, $"Már feliratkoztál az eseményre. {data.StartDate} kor kezdődik!");
                                break;
                            }
                        }
                        catch (Exception)
                        {
                            Bot.SendTextMessageAsync(e.Message.Chat.Id, $"Valami probléma történt nem sikerült feliratkozni az eseményre figyelj a név megadására!");
                            break;
                        }
                    case "/start":
                        try
                        {
                            if (dbService.UpdateUserByAndUserName(e.Message.Chat.Username, e.Message.Chat.Id))
                            {
                                Console.WriteLine($"Sikeres Hozzáadás");
                            }
                            else
                            {
                                Console.WriteLine("Sikertelen!");
                            }
                            Bot.SendTextMessageAsync(e.Message.Chat.Id, $"Ez a bot a Dunaújvárosi Mérnökinfós hallgatók eseményeinek nyilvántartására és jelzésére jött létre.");
                            Bot.SendTextMessageAsync(e.Message.Chat.Id, e.Message.Chat.Id.ToString());
                            username = e.Message.Chat.Username;
                            Console.WriteLine($"azt írta {username} hogy start");
                            break;
                        }
                        catch (Exception)
                        {
                            Bot.SendTextMessageAsync(e.Message.Chat.Id, $"Valami hiba lépett fel kérlek probáld később!");
                            Bot.SendTextMessageAsync(72204263, $"Hiba a help parancsal");
                            break;
                        }
                    case "/donate":
                        try
                        {
                            Bot.SendTextMessageAsync(e.Message.Chat.Id, $"Tudtam hogy valaki kipróbálja {e.Message.Chat.Username}");
                            Bot.SendTextMessageAsync(72204263, $"Támogatni akart {e.Message.Chat.Username}");
                            Bot.SendTextMessageAsync(e.Message.Chat.Id, $"paypal.me/nagya007");
                            break;
                        }
                        catch (Exception)
                        {
                            Bot.SendTextMessageAsync(e.Message.Chat.Id, $"Valami hiba lépett fel kérlek probáld később!");
                            Bot.SendTextMessageAsync(72204263, $"Hiba a donate parancsal");
                            break;
                        }
                    case "/mytest":
                        try
                        {
                            User user = dbService.GetUserByChatId(e.Message.Chat.Id);
                            string result = dbService.GetSingUpEvent(user);
                            Bot.SendTextMessageAsync(e.Message.Chat.Id, result);
                            break;
                        }
                        catch (Exception)
                        {
                            Bot.SendTextMessageAsync(e.Message.Chat.Id, $"Valami nem sikerült kérlek próbáld később!");
                            break;
                        }
                }
            }
            else
            {
                Bot.SendTextMessageAsync(e.Message.Chat.Id, $"Jelenleg nem elérhrtő számodra a bot!");
            }
        }
    }
}