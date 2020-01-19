using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BirthdayBot.Extensions;
using BirthdayBot.Interfaces;
using BirthdayBot.IO;
using BirthdayBot.Telegram;

namespace BirthdayBot
{
    class Program
    {
        /* config */
        public static BirthdayConfig Config;
        private static DateTime _lastResetDateLocalTime;
        private static TimeZoneInfo _tz;

        private static string _timeZoneId;
        private static Commands _commands;
        
        /* admins */
        private static IAdminManager _adminManager;

        /* birthdays */
        private static Birthdays _birthdays;

        /* apis */
        private static TelegramApi _messagingApi;

        /* exit handlers */
        private static readonly List<IExitHandler> ExitHandlers = new List<IExitHandler>();
        private static readonly object Lock = new object();
        private static bool _onExitCalled;
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
            var messageHandler = new TelegramMessageHandler(_messagingApi, _birthdays, _adminManager);
            _messagingApi.AddHandler(messageHandler);
            ExitHandlers.Add(_birthdays);
            ExitHandlers.Add(messageHandler);
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
                Console.WriteLine("-----------------------------------------");
                Console.WriteLine("Console commands: admin add/delete username, timezone list/file, q [close the bot]");
                Console.WriteLine("-----------------------------------------");
                var text = Console.ReadLine()?.Trim() ?? "";

                if (DispatchCommand(text))
                    continue;
                if (text.ToLower() != "q")
                    continue;

                // quit
                OnExit();
                break;
            }
        }

        private static bool DispatchCommand(string text)
        {
            var strings = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var result = strings.DispatchCommand(
                ("timezone list", _commands.TimeZoneIdsList), 
                ("timezone file", _commands.TimeZoneIdsWriteToFile),
                ("admin add", _commands.AdminAdd),
                ("admin delete", _commands.AdminDelete),
                ("admin list", _commands.AdminList));
            return result;
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

            OnExit();
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
                Token = "Enter your token here",
                ChatId = "Enter your chat id here",
                TimeZoneId = "Central European Standard Time",
                LastResetDateLocalTime = DateTime.MinValue,
                Admins = new List<string>()
            });
            _timeZoneId = Config.TimeZoneId;
            _tz = TimeZoneInfo.FindSystemTimeZoneById(Config.TimeZoneId);
            _lastResetDateLocalTime = Config.LastResetDateLocalTime;
            _adminManager = new AdminManager(Config.Admins);
            _commands = new Commands(_adminManager);
        }


        public static void SaveConfig()
        {
            JsonHelper.SerializeToFile(ConfigFile,
                new BirthdayConfig
                {
                    MessageLoopSleepTimeMs = Config.MessageLoopSleepTimeMs,
                    Token = Config.Token,
                    ChatId = Config.ChatId,
                    TimeZoneId = _timeZoneId,
                    LastResetDateLocalTime = _lastResetDateLocalTime,
                    Admins = _adminManager.List().ToList()
                });
        }


        public static void OnExit()
        {
            lock (Lock)
            {
                if (_onExitCalled)
                {
                    return;
                }

                _onExitCalled = true;
            }
            foreach (var handler in ExitHandlers)
            {
                handler.OnExit();
            }

            SaveConfig();
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}