using Discord.WebSocket;
using Swissbot_Worker.MessageTypes;
using System;
using System.Threading.Tasks;

namespace Swissbot_Worker
{
    class Program
    {
        public static string SwissbotAuth = "";
        public static int WorkerId = 0;
        static void Main(string[] args)
        {
            // Auth key is passed in thru args
            SwissbotAuth = args[0];
            string token = args[1];
            WorkerId = int.Parse(args[2]);

            // Start the bot
            new Program().Start(token).GetAwaiter().GetResult();
        }
        public static DiscordSocketClient client;
        private SwissbotClient swissbotClient;
        public async Task Start(string token)
        {
            client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                LogLevel = Discord.LogSeverity.Debug,
                AlwaysDownloadUsers = true
            });

            client.Log += HandleLogs;

            await client.LoginAsync(Discord.TokenType.Bot, token);

            await client.StartAsync();

            await client.SetStatusAsync(Discord.UserStatus.Invisible);

            swissbotClient = new SwissbotClient(client);

            await Task.Delay(-1);
        }

        public static void CloseWorker(Exception x = null)
        {
#if DEBUG
            Console.WriteLine($"Worker closed: {x}");
#endif

            client.StopAsync();
            client.Dispose();

            Environment.Exit(0);
        }

        private async Task HandleLogs(Discord.LogMessage arg)
        {
            if (arg.Message == null)
                return;
            if (arg.Message.StartsWith("Received Dispatch"))
                return;

            string formatted = $"[ Worker {WorkerId}: {arg.Severity}-{arg.Source} ]: {arg.Message} {(arg.Exception != null ? arg.Exception.ToString() : "")}";
            Console.WriteLine(formatted);

            if (SwissbotClient.isConnected)
            {
                await SwissbotClient.SendMessage(new Log()
                {
                    message = formatted,
                    workerId = WorkerId
                });
            }
        }
    }
}
