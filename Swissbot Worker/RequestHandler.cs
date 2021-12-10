using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using Swissbot_Worker.MessageTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Swissbot_Worker
{
    class RequestHandler
    {
        private DiscordSocketClient discordClient;
        private SwissbotClient swissbotClient;
        private SocketGuild guild
            => discordClient.GetGuild(592458779006730264);
        public RequestHandler(DiscordSocketClient c, SwissbotClient sc)
        {
            discordClient = c;
            swissbotClient = sc;

            // Add the event
            sc.WebsocketMessageEvent += (object s, WebsocketMessageEventArgs d) => 
            {
                Task.Run(() => Sc_WebsocketMessageEvent(d).ConfigureAwait(false));
            };
        }

        private async Task Sc_WebsocketMessageEvent(WebsocketMessageEventArgs e)
        {
            if(e.result.MessageType == System.Net.WebSockets.WebSocketMessageType.Close)
            {
                Console.WriteLine($"Recieved a close: {e.result.CloseStatus}, {e.result.CloseStatusDescription}");
                Program.CloseWorker();
            }

            string content = Encoding.UTF8.GetString(e.data);

            IRecieveable message;
            try
            {
                message = JsonConvert.DeserializeObject<RawRecieveable>(content);
            }
            catch(Exception x)
            {
                await e.socket.CloseAsync(System.Net.WebSockets.WebSocketCloseStatus.InvalidMessageType, "Invalid IRecieveable", CancellationToken.None);
                Program.CloseWorker();
                return;
            }

            Console.WriteLine("Got new message: " + content);

            switch (message.Type)
            {
                case "MuteUsers":
                    // Switch our packet type
                    Console.WriteLine("Running mute users");
                    MuteUsers packet = JsonConvert.DeserializeObject<MuteUsers>(content);
                    foreach(var user in packet.Users)
                    {
                        var dUser = discordClient.GetGuild(592458779006730264).GetUser(user);

                        if (packet.Action == "Mute")
                            await dUser.ModifyAsync(x => x.Mute = packet.Value);
                        else if (packet.Action == "Deafen")
                            await dUser.ModifyAsync(x => x.Deaf = packet.Value);

                        Console.WriteLine($"{packet.Action}: {user}");
                    }
                    break;
                case "RemoveRoles" or "AddRoles":
                    Console.WriteLine("Starting role updates");
                    RolesUpdate rolesPacket = JsonConvert.DeserializeObject<RolesUpdate>(content);


                    await guild.DownloadUsersAsync();
                    var targetUser = guild.GetUser(rolesPacket.User);
                    
                    if (targetUser == null)
                        return;

                    List<IRole> roles = new List<IRole>();
                    foreach (var r in rolesPacket.Roles)
                        roles.Add(guild.GetRole(r));

                    switch (rolesPacket.Action)
                    {
                        case "add":
                            await targetUser.AddRolesAsync(roles);
                            break;
                        case "remove":
                            await targetUser.RemoveRolesAsync(roles);
                            break;
                    }
                    break;
                case "handshake_accept":
                    Console.WriteLine("Worker Authed");
                    break;
            }
        }
    }
}
