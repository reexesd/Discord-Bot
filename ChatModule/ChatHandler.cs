using Discord;
using Discord.WebSocket;
using Discord_Bot.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Discord_Bot.ChatModule
{
    public class ChatHandler(DiscordSocketClient client, ChatGPTService chatGPTService, FileManager fileManager)
    {
        private readonly int _mentionLength = client.CurrentUser.Mention.Length;
        private readonly ChatGPTService _chatService = chatGPTService;
        private readonly FileManager _fileManager = fileManager;

        public async Task HandleRequestAsync(IUserMessage message)
        {
            string request = message.Content.Remove(0, _mentionLength);

            if (message.Channel is not IThreadChannel chat)
                return;

            using (chat.EnterTypingState())
            {
                var answer = await GetResponseFromAI(chat, request);

                if (answer.IsHugeAnswer && answer.CodeExist)
                    await _fileManager.SendMessageWithCodeAttachmentsAsync(answer.ContentWOCode, answer.CodeBlocks, chat);
                else if (answer.IsHugeAnswer)
                    foreach (var part in answer.PartsOfAnswer)
                        await chat.SendMessageAsync(part);
                else
                    await chat.SendMessageAsync(answer.Content);
            }
        }

        public Task HandleThreadDeleting(Cacheable<SocketThreadChannel, ulong> thread)
        {
            if(_chatService.IsChatExists(thread.Value))
                _chatService.DeleteChat(thread.Value);

            return Task.CompletedTask;
        }

        public async Task<AnswerFromAI> GetResponseFromAI(IThreadChannel chat, string request)
        {
            AnswerFromAI answer;

            if (_chatService.IsChatExists(chat))
                answer = await _chatService.SentRequestAsync(chat, request);
            else
                answer = await _chatService.SentRequestAsync(request);

            return answer;
        }
    }
}
