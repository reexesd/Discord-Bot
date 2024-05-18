using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Resources;
using OpenAI_API.Chat;
using OpenAI_API;
using OpenAI_API.Models;
using Discord_Bot.Properties;

namespace Discord_Bot.ChatModule
{
    public class ChatGPTService(OpenAIAPI openAI)
    {
        private readonly OpenAIAPI _openAI = openAI;
        private readonly string _systemPrompt = Resources.SystemPrompt;
        private readonly Dictionary<IThreadChannel, Conversation> _registredChats = [];

        public async Task<string> RegisterNewChatAsync(IThreadChannel channel)
        {
            var chat = _openAI.Chat.CreateConversation();
            chat.AppendSystemMessage(_systemPrompt);
            _registredChats.Add(channel, chat);
            return await chat.GetResponseFromChatbotAsync();
        }

        public void DeleteChat(IThreadChannel channel) =>
            _registredChats.Remove(channel);

        public bool IsChatExists(IThreadChannel channel) =>
            _registredChats.ContainsKey(channel);

        public async Task<AnswerFromAI> SentRequestAsync(IThreadChannel channel, string content)
        {
            var chat = _registredChats[channel];
            chat.AppendUserInput(content);
            var result = await chat.GetResponseFromChatbotAsync();

            var answer = new AnswerFromAI(result);
            return answer;
        }

        public async Task<AnswerFromAI> SentRequestAsync(string content)
        {
            var request = new ChatRequest()
            {
                Model = Model.ChatGPTTurbo,
                Messages = [new(ChatMessageRole.System, _systemPrompt), new(ChatMessageRole.User, content)]
            };
            var result = await _openAI.Chat.CreateChatCompletionAsync(request);

            var answer = new AnswerFromAI(result.ToString());
            return answer;
        }
    }
}
