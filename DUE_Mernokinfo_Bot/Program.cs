using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using System.Timers;
using System.Data.Entity;
using System.Data;
using System.Runtime.Remoting.Contexts;
using System.Threading;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.InlineQueryResults;


namespace DUE_Mernokinfo_Bot
{
    class Program
    {
        private static readonly TelegramBotClient Bot = new TelegramBotClient("506745618:AAGWmuVwrFYxJZcYeaC3tXX9TDyQn3LbDzo");
        public DbService dbService = new DbService();
        public BotDbContext context = new BotDbContext();
        public static string username;
        private string inlineKeyBoardData;
        static void Main(string[] args)
        {
            Thread printer = new Thread(new ThreadStart(InvokeMethod));
            printer.Start();
            Bot.OnMessage += On_Message;
            Bot.OnMessageEdited += On_Message;
            Bot.OnCallbackQuery += Bot_OnCallbackQuery;
            Bot.StartReceiving();
            Console.ReadLine();
            Bot.StopReceiving();
            Console.ReadLine();
        }
        private static async void Bot_OnCallbackQuery(object sender, CallbackQueryEventArgs e)
        {
            var callbackQuery = e.CallbackQuery;
            //  var callbackQuery = callbackQueryEventArgs.CallbackQuery;

            await Bot.AnswerCallbackQueryAsync(
                callbackQuery.Id,
                $"Received {callbackQuery.Data}");

            //await Bot.SendTextMessageAsync(
            //    callbackQuery.Message.Chat.Id,
            //    $"Received {callbackQuery.Data}");

            

            if (callbackQuery.Data == "Emlékeztess!")
            {
                DbService dbService = new DbService();
                await Bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, string.Format("{0}", callbackQuery.Message.Text), replyMarkup: new ReplyKeyboardRemove());
                UserEnrolled userenrolled = new UserEnrolled();
                User suser = dbService.GetUserByChatId(callbackQuery.Message.Chat.Id);
                Data data = dbService.GetEventByName(callbackQuery.Message.Text);
                if (!dbService.IsUserSingUpEvent(suser, data))
                {
                    userenrolled.UserId = suser.UserId;
                    userenrolled.EventId = data.EventId;
                    dbService.SingUpEvent(userenrolled);
                    Console.WriteLine("Add event!");                  
                }
                else
                {
                  await Bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, $"Már feliratkoztál az eseményre. {data.StartDate} kor kezdődik!");
                }
            }

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
            Writer w = new Writer();
            DbService dbService = new DbService();
            dbService.RemoveDataByEndDate();
            Console.WriteLine(DateTime.Now.ToString());
            DateTime onehure = DateTime.Now.AddHours(1);
            IQueryable<Data> ringonehour = dbService.GetHourByDate(onehure);
            string ringone = "";
            string ringten = "";
            foreach (var item in ringonehour)
            {
                ringone += $"Egy óra múlva kezdődik! \n {w.WSubjectCode}: \n{item.SubjectCode},\n {w.WClassCode}: \n  {item.ClassCode}, \n {w.WStartDate}: \n {item.StartDate}, \n {w.WEndDate}: \n {item.EndDate}, \n {w.WZh}: \n {item.ZH}";
                Console.WriteLine($" {item.SubjectCode}, {item.ClassCode}, {item.StartDate}, {item.EndDate}, {item.ZH}");
            }
            Bot.SendTextMessageAsync(-279234619, ringone);
            DateTime tenminutedate = DateTime.Now.AddMinutes(10);
            IQueryable<Data> ringtenminute = dbService.GetHourByDate(tenminutedate);
            foreach (var item in ringtenminute)
            {
                ringten += $"10 perc múlva kezdődik!  \n {w.WSubjectCode}: \n{item.SubjectCode},\n {w.WClassCode}: \n  {item.ClassCode}, \n {w.WStartDate}: \n {item.StartDate}, \n {w.WEndDate}: \n {item.EndDate}, \n {w.WZh}: \n {item.ZH}";
                Console.WriteLine($"  {item.SubjectCode}, {item.ClassCode}, {item.StartDate}, {item.EndDate}, {item.ZH}  ");
            }
            Bot.SendTextMessageAsync(-279234619, ringten);

        }
        public static void On_Message(object sender, MessageEventArgs e)
        {
            DbService dbService = new DbService();
            if (e.Message.Chat.Id == -279234619 || e.Message.Chat.Id == -1001105059482 || e.Message.Chat.Id == -277596717)
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
                                Bot.SendTextMessageAsync(72204263, $"Hiba az id parancsal");
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
                                Writer w = new Writer();
                                DateTime tudaydate = DateTime.Now;
                                IQueryable<Data> today = null;
                                today = dbService.GetDayByDate(tudaydate);
                                if (today.Any())
                                {
                                    foreach (var item in today)
                                    {
                                        Bot.SendTextMessageAsync(e.Message.Chat.Id, $"{w.WSubjectCode}:/n {item.SubjectCode},/n {w.WClassCode}: /n{item.ClassCode},/n {w.WStartDate}: /n {item.StartDate},/n {w.WEndDate}: /n{item.EndDate},/n {w.WZh}: /n {item.ZH}");
                                        Console.WriteLine($" {item.SubjectCode}, {item.ClassCode}, {item.StartDate}, {item.EndDate}, {item.ZH}");
                                    }
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
                                Bot.SendTextMessageAsync(72204263, $"Hiba a today parancsal");
                                break;
                            }
                        case "/nextday":
                            try
                            {
                                Writer w = new Writer();
                                DateTime nextdaydate = DateTime.Now.AddDays(1);
                                var nextday = dbService.GetDayByDate(nextdaydate);
                                if (nextday.Any())
                                {
                                    foreach (var item in nextday)
                                    {
                                        Bot.SendTextMessageAsync(e.Message.Chat.Id, $"{w.WSubjectCode}: \n{item.SubjectCode},\n {w.WClassCode}: \n  {item.ClassCode}, \n {w.WStartDate}: \n {item.StartDate}, \n {w.WEndDate}: \n {item.EndDate}, \n {w.WZh}: \n {item.ZH}");
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
                                Writer w = new Writer();
                                var nextzh = dbService.GetNextZh();
                                if (nextzh != null)
                                {
                                    Bot.SendTextMessageAsync(e.Message.Chat.Id, $"{w.WStartDate}: /n {nextzh.SubjectCode}/n {w.WClassCode}: /n {nextzh.ClassCode} /n {w.WStartDate}: /n {nextzh.StartDate} /n  {w.WEndDate}: /n {nextzh.EndDate}");
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
                                Bot.SendTextMessageAsync(e.Message.Chat.Id, $"Elérhatő utasítások: \n addzh - [StartDate][EndDate][SubjectCode][ClassCode][Zh(true, false)] \n nextzh - következő zh \n nextday - következő napi teendők(zh - k) \n today - mai teendők(zh - k) \n help - elérhető utasítások \n donate - ...");
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
                                Writer w = new Writer();
                                Data next = dbService.GetNext();
                                if (next.IsEmpty())
                                {
                                    Bot.SendTextMessageAsync(e.Message.Chat.Id, $"{w.WSubjectCode}:/n {next.SubjectCode}/n {w.WClassCode}:/n {next.ClassCode} /n {w.WStartDate}: /n {next.StartDate}/n {w.WEndDate}: /n {next.EndDate}");
                                    break;
                                }
                                else
                                {
                                    Bot.SendTextMessageAsync(e.Message.Chat.Id, $"Nincs következő esemény!");
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
                            Bot.SendTextMessageAsync(72204263, $"Hiba a myid paramcsal");
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
                            string mytoday;
                            User mydayuser = dbService.GetUserByChatId(e.Message.Chat.Id);
                            mytoday = dbService.GetDayByUserByDate(mydayuser, tudaydate);
                            if (mytoday.Any())
                            {
                                Bot.SendTextMessageAsync(e.Message.Chat.Id, $"{mytoday}");
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
                            string mynextday;
                            User nextdayuser = dbService.GetUserByChatId(e.Message.Chat.Id);
                            mynextday = dbService.GetDayByUserByDate(nextdayuser, nextdaydate);
                            if (mynextday.Any())
                            {
                                Bot.SendTextMessageAsync(e.Message.Chat.Id, $"{mynextday}");
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
                    case "/mynextzh":
                        try
                        {
                            Writer w = new Writer();
                            User nextuser = dbService.GetUserByChatId(e.Message.Chat.Id);
                            var nextzh = dbService.GetNextZhByUser(nextuser);
                            if (nextzh.Any())
                            {
                                Bot.SendTextMessageAsync(e.Message.Chat.Id, $"{nextzh}s");
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
                            Bot.SendTextMessageAsync(e.Message.Chat.Id, $"Elérhatő utasítások: \n addzh - [StartDate][EndDate][SubjectCode][ClassCode][Zh(true, false)] \n nextzh - következő zh \n nextday - következő napi teendők(zh - k) \n today - mai teendők(zh - k) \n help - elérhető utasítások \n donate - ...");
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
                            var allDatas = dbService.GetAllData();
                            foreach (var item in allDatas)
                            {
                                var inlineKeyboard = new InlineKeyboardMarkup(new[]
                                {
                                    new[]
                                    {
                                      InlineKeyboardButton.WithCallbackData("Emlékeztess!"),
                                      InlineKeyboardButton.WithCallbackData("Ne emlékeztes!"),
                                    }

                                });
                                Bot.SendTextMessageAsync(e.Message.Chat.Id, $"{item.SubjectCode}", replyMarkup: inlineKeyboard);
                                
                            }
                            Bot.SendTextMessageAsync(e.Message.Chat.Id, "Done");
                            break;
                         
                            //UserEnrolled userenrolled = new UserEnrolled();
                            //User suser = dbService.GetUserByChatId(e.Message.Chat.Id);
                            //Data data = dbService.GetEventByName(messages[1].ToString());
                            //if (!dbService.IsUserSingUpEvent(suser, data))
                            //{
                            //    userenrolled.UserId = suser.UserId;
                            //    userenrolled.EventId = data.EventId;
                            //    dbService.SingUpEvent(userenrolled);
                            //    Console.WriteLine("Add event!");
                            //    break;
                            //}
                            //else
                            //{
                            //    Bot.SendTextMessageAsync(e.Message.Chat.Id, $"Már feliratkoztál az eseményre. {data.StartDate} kor kezdődik!");

                            //    break;
                            //}
                            //var inlineKeyboard = new InlineKeyboardMarkup(new[]
                            //   {
                            //        new[]
                            //        {
                            //          InlineKeyboardButton.WithCallbackData("1"),
                            //          InlineKeyboardButton.WithCallbackData("11"),
                            //          InlineKeyboardButton.WithCallbackData("220")
                            //        },
                            //        new[]
                            //        {
                            //          InlineKeyboardButton.WithCallbackData("Mukodik")
                            //        }
                            //  });
                            //Bot.SendTextMessageAsync(e.Message.Chat.Id, "OK", replyMarkup: inlineKeyboard);
                            //break;
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
                    case "/calender":
                        try
                        {
                            var inlineKeyboard = new InlineKeyboardMarkup(new[]
                               {
                                    new[]
                                    {
                                      InlineKeyboardButton.WithCallbackData("1"),
                                      InlineKeyboardButton.WithCallbackData("2"),
                                      InlineKeyboardButton.WithCallbackData("3"),
                                      InlineKeyboardButton.WithCallbackData("4"),
                                      InlineKeyboardButton.WithCallbackData("5"),
                                      InlineKeyboardButton.WithCallbackData("5"),
                                      InlineKeyboardButton.WithCallbackData("7"),

                                    },
                                    new[]
                                    {
                                      InlineKeyboardButton.WithCallbackData("8"),
                                      InlineKeyboardButton.WithCallbackData("9"),
                                      InlineKeyboardButton.WithCallbackData("10"),
                                       InlineKeyboardButton.WithCallbackData("11"),
                                        InlineKeyboardButton.WithCallbackData("12"),
                                         InlineKeyboardButton.WithCallbackData("13"),
                                          InlineKeyboardButton.WithCallbackData("14"),
                                    },
                                    new[]
                                    {
                                      InlineKeyboardButton.WithCallbackData("15"),
                                      InlineKeyboardButton.WithCallbackData("16"),
                                      InlineKeyboardButton.WithCallbackData("17"),
                                       InlineKeyboardButton.WithCallbackData("18"),
                                        InlineKeyboardButton.WithCallbackData("19"),
                                         InlineKeyboardButton.WithCallbackData("20"),
                                          InlineKeyboardButton.WithCallbackData("21"),
                                    },
                                    new[]
                                    {
                                      InlineKeyboardButton.WithCallbackData("22"),
                                      InlineKeyboardButton.WithCallbackData("23"),
                                      InlineKeyboardButton.WithCallbackData("24"),
                                       InlineKeyboardButton.WithCallbackData("25"),
                                        InlineKeyboardButton.WithCallbackData("26"),
                                         InlineKeyboardButton.WithCallbackData("27"),
                                          InlineKeyboardButton.WithCallbackData("28"),
                                    },
                                    new[]
                                    {
                                         InlineKeyboardButton.WithCallbackData("29"),
                                          InlineKeyboardButton.WithCallbackData("30"),
                                           InlineKeyboardButton.WithCallbackData("31"),
                                    },

                                });
                            Bot.SendTextMessageAsync(e.Message.Chat.Id, $"Nap", replyMarkup: inlineKeyboard);

                            break;
                        }
                        catch (Exception)
                        {

                            throw;
                        }
                }
            }
            else
            {
                Bot.SendTextMessageAsync(e.Message.Chat.Id, $"Jelenleg nem elérhrtő számodra a bot!");
                Bot.SendTextMessageAsync(72204263, e.Message.Chat.Id.ToString());
            }
        }

    }
}