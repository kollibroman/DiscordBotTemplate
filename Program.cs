using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Discord;
//using Discord.Addons.Interactive;  Uncomment this to use this library
using Discord.Commands;
using Discord.WebSocket;
using Discord.Addons.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using DiscordBotTemplate.Services;

namespace Discordify
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
                    builder.SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", false, true)
                        .AddJsonFile("appsettings.development.json", true, true)
                        .AddEnvironmentVariables();

            var host = new HostBuilder()
                    .ConfigureAppConfiguration(x =>
                    {
                        x.AddConfiguration(builder.Build());
                    })
                    .ConfigureLogging(x => 
                    {
                        Log.Logger = new LoggerConfiguration()
                                .ReadFrom.Configuration(builder.Build())
                                .Enrich.FromLogContext()
                                .WriteTo.Console()
                                .CreateLogger();

                       x.AddSerilog(Log.Logger);
                       x.SetMinimumLevel(LogLevel.Trace);
                    })   
                    .ConfigureDiscordHost((context, config) =>
                    {
                        config.SocketConfig = new DiscordSocketConfig
                        {
                            LogLevel = LogSeverity.Debug,
                            AlwaysDownloadUsers = true,
                            MessageCacheSize = 200
                        };

                        config.Token = context.Configuration["token"];
                    })
                    .UseCommandService((context, config) =>
                    {
                        config = new CommandServiceConfig()
                        {
                            DefaultRunMode = RunMode.Async,
                            CaseSensitiveCommands = false,
                            LogLevel = LogSeverity.Debug
                        };
                    })
                    .ConfigureServices((context, services) =>
                    {
                        services.AddHostedService<CommandHandler>();
                        //services.AddScoped<InteractiveService>();  uncomment to be able to use InteractiveBase<> as a Inherited module
                    })
                    .UseSerilog()
                    .UseConsoleLifetime();
                   
                   var _host = host.Build();
                   using(_host)
                   {
                       await _host.RunAsync();
                   }
        }
    }
}