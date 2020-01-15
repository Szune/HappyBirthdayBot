using System;
using System.Collections.Generic;
using System.Threading;

namespace BirthdayBot
{
    class Program
    {
        /* config */
        public static BirthdayConfig Config;
        private static DateTime _lastResetDateLocalTime;
        private static TimeZoneInfo _tz;
        private static string _timeZoneId;
        /* birthdays */
        private static Birthdays _birthdays;
        /* apis */
        private static TelegramApi _messagingApi;
        /* exit handlers */
        private static readonly List<IExitHandler> _exitHandlers = new List<IExitHandler>();

        private static CancellationTokenSource _cts;

        /* constants */
        private const string ConfigFile = "config.json";

        static void Main(string[] args)
        {
            Load();
            PreStart();
            Start();
        }
        
        private static void Load()
        {
            Console.WriteLine("Dates are specified in the format MM-DD");
            LoadConfig();
            
            _messagingApi = new TelegramApi(Config.Token, Config.ChatId);
            Console.WriteLine($"Using {_messagingApi.GetType().Name}");
            
            _birthdays = Birthdays.Load(_messagingApi);
            var messageHandler = new TelegramMessageHandler(_messagingApi, _birthdays);
            _messagingApi.AddHandler(messageHandler);
            _exitHandlers.Add(_birthdays);
            _exitHandlers.Add(messageHandler);
        }

        private static void PreStart()
        {
            var date = DateTime.UtcNow.ToTimeZone(_tz);
            Console.WriteLine($"Last reset on {_lastResetDateLocalTime:yyyy-MM-dd}");
            ResetAlertsIfNewDay(date);
        }
        
        private static void Start()
        {
            _cts = new CancellationTokenSource();
            var token = _cts.Token;
            var thread = new Thread(() => MessageLoop(token));
            thread.Start();
            while (true)
            {
                Console.WriteLine("To add an admin enter admin add username");
                Console.WriteLine("To remove an admin enter admin delete username");
                Console.WriteLine("Enter q to quit");
                var text = Console.ReadLine()?.Trim() ?? "";

                if (text.StartsWith("admin"))
                {
                    ManageAdmins(text);
                    continue;
                }
                if (text.ToLower() != "q") 
                    continue;
                
                // quit
                OnExit();
                break;
            }
        }

        private static void ManageAdmins(string text)
        {
            var parts = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 3)
            {
                return;
            }

            if (parts[0].Trim().ToUpperInvariant() != "ADMIN")
            {
                return;
            }

            if (parts[1].Trim().ToUpperInvariant() == "ADD")
                Config.Admins.Add(parts[2].Trim().ToUpperInvariant());
            else if (parts[1].Trim().ToUpperInvariant() == "DELETE")
                Config.Admins.Remove(parts[2].Trim().ToUpperInvariant());
            else
                return;
            SaveConfig();
        }

        private static void MessageLoop(CancellationToken token)
        {
            try
            {
                while (true)
                {
                    token.ThrowIfCancellationRequested();
                    var date = DateTime.UtcNow.ToTimeZone(_tz);
                    ResetAlertsIfNewDay(date);
                    _birthdays.CongratulateTheUngratulated(date);
                    _messagingApi.Fetch();
                    token.WaitHandle.WaitOne(Config.MessageLoopSleepTimeMs);
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Exiting");
            }
            catch (ObjectDisposedException)
            {
                Console.WriteLine("Exiting");
            }
            Environment.Exit(0); // exit without waiting for the blocked ReadLine() in Start()
        }

        private static void ResetAlertsIfNewDay(DateTime dateLocalTime)
        {
            if (!IsResetTime(dateLocalTime, _lastResetDateLocalTime)) 
                return;
            Console.WriteLine("New day: resetting alerts");
            _birthdays.ResetAlerts();
            _lastResetDateLocalTime = dateLocalTime.Date;
            SaveConfig();
            _birthdays.Save();
        }

        private static bool IsResetTime(DateTime nowLocalTime, DateTime lastResetDateLocalTime)
        {
            return nowLocalTime.Day != lastResetDateLocalTime.Day;
        }


        private static void LoadConfig()
        {
            Config = JsonHelper.DeserializeFile(ConfigFile, new BirthdayConfig
            {
                MessageLoopSleepTimeMs = 5000,
                Token = "Unknown",
                ChatId = "Unknown",
                TimeZoneId = "Central European Standard Time",
                LastResetDateLocalTime = DateTime.MinValue,
                Admins = new List<string>()
            });
            _timeZoneId = Config.TimeZoneId;
            _tz = TimeZoneInfo.FindSystemTimeZoneById(Config.TimeZoneId);
            _lastResetDateLocalTime = Config.LastResetDateLocalTime;
        }


        private static void SaveConfig()
        {
            JsonHelper.SerializeToFile(ConfigFile,
                new BirthdayConfig
                {
                    MessageLoopSleepTimeMs = Config.MessageLoopSleepTimeMs,
                    Token = Config.Token,
                    ChatId = Config.ChatId,
                    TimeZoneId = _timeZoneId,
                    LastResetDateLocalTime = _lastResetDateLocalTime,
                    Admins = Config.Admins
                });
        }
        

        public static void OnExit()
        {
            foreach (var handler in _exitHandlers)
            {
                handler.OnExit();
            }
            
            SaveConfig();
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}