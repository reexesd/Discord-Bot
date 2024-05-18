using Discord;
using Discord.Interactions;
using Discord.Rest;
using Discord.WebSocket;
using Discord_Bot.ChatModule;
using OpenAI_API;
using System.Runtime.CompilerServices;

namespace Discord_Bot.Modules
{
    public class CommandModule(ChatGPTService chatService) : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly ChatGPTService _chatService = chatService;

        public InteractionService? Service { get; set; }

        [SlashCommand("start-new-chat", "Начинает новую ветку (чат) с ботом")]
        private async Task StartThreadWithAI([Summary(description:"Уровень доступа к чату")]ThreadAccessLevels accessLevel, [Summary(description:"Имя чата")]string name)
        {
            var currentMessageChannel = Context.Channel;
            
            IThreadChannel createdThread;

            if (currentMessageChannel is not ITextChannel || currentMessageChannel is IVoiceChannel)
            {
                await RespondAsync("Command can be executed only in text channel", ephemeral: true);
                return;
            }

            var currentTextChannel = currentMessageChannel as ITextChannel;

            createdThread = await currentTextChannel!.CreateThreadAsync(name, (ThreadType)(int)accessLevel);

            await createdThread.JoinAsync();
            
            await DeferAsync(ephemeral: true);

            string greeting = await _chatService.RegisterNewChatAsync(createdThread);

            await createdThread.SendMessageAsync($"{Context.User.Mention}:wave:\n{greeting}");

            await ModifyOriginalResponseAsync((prop) => prop.Content = "Chat created!");
        }

        
        [SlashCommand("delete", "Удаляет ветку (чат) с ботом"), RequiredThread]
        private async Task CloseThreadWithAI()
        {
            var currThread = Context.Channel as IThreadChannel;

            if (!_chatService.IsChatExists(currThread!))
            {
                await RespondAsync("Данная команда может быть применена только в ветке, созданной через `/start-new-chat`", ephemeral: true);
                return;
            }

            await currThread!.DeleteAsync();
            _chatService.DeleteChat(currThread);
            
            await RespondAsync();
        }
    }
}