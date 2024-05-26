using Discord;

namespace Discord_Bot.Logger
{
    public interface IBotLogger
    {
        public Task LogAsync(LogMessage message);

        public Task LogAsync(Exception exception);
    }
}
