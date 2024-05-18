using System;
using Discord;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Bot.Logger
{
    public class Logger(ITimeService time) : IBotLogger
    {
        private readonly ITimeService _time = time;

        public async Task LogAsync(LogMessage message)
        {
            switch (message.Severity)
            {
                case LogSeverity.Info:
                    await LogWithColor(message, ConsoleColor.Blue);
                    break;

                case LogSeverity.Warning:
                    await LogWithColor(message, ConsoleColor.DarkYellow);
                    break;

                case LogSeverity.Critical:
                    await LogWithColor(message, ConsoleColor.DarkRed);
                    break;

                case LogSeverity.Debug:
                    await LogWithColor(message, ConsoleColor.Green);
                    break;

                case LogSeverity.Error:
                    await LogWithColor(message, ConsoleColor.Red);
                    break;
            }
        }

        public async Task LogAsync(Exception exception)
        {
            await LogWithColor(exception, ConsoleColor.Red);
        }

        private Task LogWithColor(LogMessage message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write($"{message.Severity} from {message.Source} at {_time.HomeTime}");
            Console.ForegroundColor = ConsoleColor.White;

            if (message.Exception == null)
                Console.WriteLine($": {message.Message}");
            else
                Console.WriteLine($": {message.Exception.Message} Details: {message.Exception.InnerException?.Message}");

            return Task.CompletedTask;
        }

        private Task LogWithColor(Exception ex, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write($"Exception from {ex.Source} at {_time.HomeTime}");
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine($": {ex.Message} Details: {ex.InnerException?.Message}");

            return Task.CompletedTask;
        }
    }
}
