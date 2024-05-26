using Discord;
using Discord.Interactions;

namespace Discord_Bot
{
    public class RequiredThreadAttribute : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo commandInfo, IServiceProvider serviceProvider)
        {
            if (context.Channel is IThreadChannel)
                return Task.FromResult(PreconditionResult.FromSuccess());

            return Task.FromResult(PreconditionResult.FromError("Данная команда может быть применена только в ветках!"));
        }
    }
}
