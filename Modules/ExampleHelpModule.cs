using System.Threading.Tasks;
using Discord.Commands;

namespace DiscordBotTemplate.Modules
{
    public class ExampleHelpModule : ModuleBase<SocketCommandContext>
    {
        [Command("help")]
        private async Task help()
        {
            await ReplyAsync("Example help message");
        }
    }
}