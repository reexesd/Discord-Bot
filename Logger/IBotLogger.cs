using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Bot.Logger
{
    public interface IBotLogger
    {
        public Task LogAsync(LogMessage message);

        public Task LogAsync(Exception exception);
    }
}
