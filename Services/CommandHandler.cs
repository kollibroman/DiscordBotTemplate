using System.Threading;
using System.Threading.Tasks;
using System;
using Discord;
using Discord.Addons.Hosting;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace DiscordBotTemplate.Services
{
    public class CommandHandler : DiscordClientService
    {
        private readonly IServiceProvider _provider;
        private readonly DiscordSocketClient _client;
        private readonly CommandService _service;
        private readonly IConfiguration _config;

        public CommandHandler(DiscordSocketClient client, ILogger<CommandHandler> logger, IServiceProvider provider, CommandService service, IConfiguration config) : base(client, logger)
        {
            _client = client;
            _provider = provider;
            _service = service;
           _config = config;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            //_client.Ready += OnReady;   //uncomment to use this event
            _client.MessageReceived += OnMessageReceived;
            _service.CommandExecuted += OnCommandExecuted;
            await _service.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);
        }

        private async Task OnMessageReceived(SocketMessage arg)
        {
            if (!(arg is SocketUserMessage message)) return;
            if (message.Source != MessageSource.User) return;

            var argPos = 0;
            if (!message.HasStringPrefix(_config["prefix"], ref argPos) && !message.HasMentionPrefix(_client.CurrentUser, ref argPos)) return;

            var context = new SocketCommandContext(_client, message);
            await _service.ExecuteAsync(context, argPos, _provider);
        }

        private async Task OnCommandExecuted(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            var embed = new EmbedBuilder()
                .WithColor(Color.DarkGreen)
                .AddField("This is not how ya dew it broo!", result)
                .Build();

            if (command.IsSpecified && !result.IsSuccess) await context.Channel.SendMessageAsync(embed: embed);
        }

       /* private async Task OnReady() This event is only useful when using Victoria Lavalink wrapper
        {

        }*/
    }
}