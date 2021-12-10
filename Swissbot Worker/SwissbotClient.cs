using Discord.WebSocket;
using Newtonsoft.Json;
using Swissbot_Worker.MessageTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Swissbot_Worker
{
    class SwissbotClient
    {
        private DiscordSocketClient client;
        public static ClientWebSocket socket = new ClientWebSocket();
        public event EventHandler<WebsocketMessageEventArgs> WebsocketMessageEvent;
        private RequestHandler handler;
        public static bool isConnected 
            => socket.State == WebSocketState.Open;
        public static async Task SendMessage(ISendable Message)
        {
            string content = JsonConvert.SerializeObject(Message);

            if (isConnected)
            {
                await socket.SendAsync(Encoding.UTF8.GetBytes(content), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            else
            {
                Program.CloseWorker();
            }
        }

        public SwissbotClient(DiscordSocketClient c)
        {
            client = c;
            handler = new RequestHandler(c, this);
            startClient().GetAwaiter().GetResult();
        }
        private async Task startClient()
        {
            // Start the websocket connection
            try
            {
                await socket.ConnectAsync(new Uri("ws://localhost:3000/apprentice/v1/socket"), CancellationToken.None);
            }
            catch(Exception x)
            {
                Program.CloseWorker(x);
            }

            // Handle the recieve loop
            Task.Run(() => RecieveLoop().ConfigureAwait(false));

            // Send the handshake
            await SendMessage(new Handshake());
        }

        CancellationToken cancellationToken = new CancellationToken();
        private async Task RecieveLoop()
        {
            cancellationToken.Register(() => Program.CloseWorker());
            while (socket.State == WebSocketState.Open)
            {
                try
                {
                    byte[] _buff = new byte[1024];
                    WebSocketReceiveResult r;
                    try
                    {
                        r = await socket.ReceiveAsync(_buff, cancellationToken);
                    }
                    catch(Exception x)
                    {
                        Program.CloseWorker(x);
                        return;
                    }
                    WebsocketMessageEvent?.Invoke(null, new WebsocketMessageEventArgs()
                    {
                        data = _buff,
                        result = r,
                        socket = socket
                    });
                }
                catch (Exception x)
                {
                    Console.Error.WriteLine(x);
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, $"Bad socket", CancellationToken.None);
                    Program.CloseWorker();
                }
            }
        }
    }
    public class WebsocketMessageEventArgs
    {
        public byte[] data;
        public WebSocketReceiveResult result;
        public WebSocket socket;
    }
    
}
