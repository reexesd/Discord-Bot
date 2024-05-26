using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System.Reflection;
using Discord_Bot.Logger;
using OpenAI_API;
using Discord_Bot.ChatModule;

namespace Discord_Bot
{
    public class Program
    {
        private static ServiceProvider? _services;

        private static readonly IConfigurationRoot _appConfig = new ConfigurationBuilder().AddUserSecrets(Assembly.GetExecutingAssembly()).Build();

        private static readonly InteractionServiceConfig _interactionServiceConfig = new()
        {
            DefaultRunMode = RunMode.Async,
            UseCompiledLambda = true,
            LogLevel = LogSeverity.Debug
        };

        private static readonly DiscordSocketConfig _discordSocketConfig = new()
        {
            GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
        };

        public static async Task Main()
        {
            _services = ConfigureServiceProvider();

            await _services.GetRequiredService<InteractionHandler>().InitAsync();

            var client = _services.GetRequiredService<DiscordSocketClient>();

            client.Log += _services.GetRequiredService<IBotLogger>().LogAsync;
            client.MessageReceived += _services.GetRequiredService<MessageHandler>().HandleAsync;
            client.Ready += FinishSetup;

            await client.LoginAsync(TokenType.Bot, _appConfig["Token"]);
            await client.StartAsync();


            await Task.Delay(Timeout.Infinite);
        }

        private static Task FinishSetup()
        {
            var chatHandler = _services!.GetRequiredService<ChatHandler>();
            _services!.GetRequiredService<MessageHandler>().ChatRequestReceived += chatHandler.HandleRequestAsync;
            _services!.GetRequiredService<DiscordSocketClient>().ThreadDeleted += chatHandler.HandleThreadDeleting;
            return Task.CompletedTask;
        }

        private static ServiceProvider ConfigureServiceProvider()
        {
            var services = new ServiceCollection()
                .AddSingleton(_appConfig)
                .AddSingleton(new OpenAIAPI(_appConfig["AIAPI"]))
                .AddSingleton<ChatGPTService>()
                .AddSingleton<ChatHandler>()
                .AddSingleton<IBotLogger, Logger.Logger>()
                .AddSingleton<ITimeService, TimeService>()
                .AddSingleton(_discordSocketConfig)
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>(), _interactionServiceConfig))
                .AddSingleton<InteractionHandler>()
                .AddSingleton<MessageHandler>()
                .AddSingleton<FileManager>()
                .BuildServiceProvider();

            return services;
        }
    }
}