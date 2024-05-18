using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord_Bot.ChatModule;
using Discord_Bot.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Bot
{
    public class MessageHandler(DiscordSocketClient client)
    {
        private readonly DiscordSocketClient _client = client;

        public event Func<IUserMessage, Task>? ChatRequestReceived;

        public Task HandleAsync(IMessage message)
        {
            if (message is IUserMessage userMessage && !userMessage.Author.IsBot)
            {
                if (userMessage.Channel is IThreadChannel thread)
                {
                    int argPos = 0;
                    if (userMessage.HasMentionPrefix(_client.CurrentUser, ref argPos))
                    {
                        ChatRequestReceived?.Invoke(userMessage);
                    }
                }
            }
            return Task.CompletedTask;
        }
    }
}
