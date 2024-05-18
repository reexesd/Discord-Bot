using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Discord_Bot.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Bot
{
    public class InteractionHandler(DiscordSocketClient client, InteractionService interactionService, IServiceProvider provider)
    {
        private readonly DiscordSocketClient _client = client;
        private readonly InteractionService _interactionService = interactionService;
        private readonly IServiceProvider _services = provider;

        public async Task InitAsync()
        {
            _client.Ready += RegisterCommandsAsync;
            _interactionService.Log += _services.GetRequiredService<IBotLogger>().LogAsync;
            _interactionService.InteractionExecuted += HandleInteractionExecuteAsync;

            await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

            _client.InteractionCreated += HandleInteractionCreatedAsync;
        }

        private async Task RegisterCommandsAsync()
        {
            await _interactionService.RegisterCommandsGloballyAsync();
        }

        private async Task HandleInteractionCreatedAsync(SocketInteraction interaction)
        {
            var context = new SocketInteractionContext(_client, interaction);

            var result = await _interactionService.ExecuteCommandAsync(context, _services);
        }

        private async Task HandleInteractionExecuteAsync(ICommandInfo commandInfo, IInteractionContext context, IResult result)
        {
            if (!result.IsSuccess && result is PreconditionResult res)
            {
                await context.Interaction.RespondAsync(res.ErrorReason, ephemeral: true);
            }
        }
    }
}
