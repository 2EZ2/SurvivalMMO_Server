using DarkRift;
using DarkRift.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalMMO_PlayerPlugin
{
    internal class ServerPlayerManager : Plugin
    {
        public override bool ThreadSafe => true;

        public override Version Version => new Version(1,0,0);

        public ServerPlayerManager(PluginLoadData pluginLoadData) : base(pluginLoadData)
        {
            //Subscribe for notification when a new client connects
            ClientManager.ClientConnected += ClientManager_ClientConnected;

            //Subscribe for notifications when a new client disconnects
            ClientManager.ClientDisconnected += ClientManager_ClientDisconnected;
        }


        Dictionary<IClient, RiftView> players = new Dictionary<IClient, RiftView>();

        void ClientManager_ClientConnected(object sender, ClientConnectedEventArgs e)
        {
            //Subscribe to when this client sends messages
            e.Client.MessageReceived += Client_PlayerMessageReceivedEvent;

            RiftView newConnection = new RiftView(e.Client.ID, e.Client.ID);

            lock (players)
                players.Add(e.Client, newConnection);

            using (Message message = Message.Create(MessageTag.PlayerConnected, newConnection))
            {
                foreach (IClient sendTo in ClientManager.GetAllClients().Except(new IClient[] { e.Client }))
                {
                    e.Client.SendMessage(message, SendMode.Reliable);
                }
            }
        }

        private void ClientManager_ClientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {
            RiftView player = new RiftView(e.Client.ID, e.Client.ID);

            lock (players)
                players.Remove(e.Client);

            using (Message message = Message.Create(MessageTag.PlayerDisconnected, player))
            {
                foreach (IClient sendTo in ClientManager.GetAllClients().Except(new IClient[] { e.Client }))
                {
                    e.Client.SendMessage(message, SendMode.Reliable);
                }
            }
        }

        void Client_PlayerMessageReceivedEvent(object sender, MessageReceivedEventArgs e)
        {

        }
    }
}
