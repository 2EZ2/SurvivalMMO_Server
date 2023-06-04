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

        public ushort lastrObjIDNumber = 100;

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
                //Spawn players on new client
                foreach (RiftView view in players.Values)
                {
                    using (Message message = Message.Create(MessageTag.PlayerConnected, view))
                    {
                        e.Client.SendMessage(message, SendMode.Reliable);
                    }
                }
                //Add client to client list
                players.Add(e.Client, newConnection);

            //Send player joined to all players on server
            using (Message message = Message.Create(MessageTag.PlayerConnected, newConnection))
            {
                foreach (IClient client in ClientManager.GetAllClients())
                {
                    client.SendMessage(message, SendMode.Reliable);
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
                    sendTo.SendMessage(message, SendMode.Reliable);
                }
            }
        }

        void Client_PlayerMessageReceivedEvent(object sender, MessageReceivedEventArgs e)
        {
            if(e.Tag == MessageTag.SendStream)
            {            
                using (DarkRiftReader reader = e.GetMessage().GetReader())
                {
                    StreamView view = reader.ReadSerializable<StreamView>();
                    
                    using (Message message = Message.Create(MessageTag.ReceivingStream, view))
                    {
                        
                        foreach (IClient sendTo in ClientManager.GetAllClients().Except(new IClient[] { e.Client }))
                        {
                            sendTo.SendMessage(message, SendMode.Reliable);
                        }
                    }
                }                   
            }
            else if(e.Tag == MessageTag.RPC)
            {
                using (DarkRiftReader reader = e.GetMessage().GetReader())
                {
                    RPCView view = reader.ReadSerializable<RPCView>();
                    IClient[] clients = view.excludeSelf ? ClientManager.GetAllClients().Except(new IClient[] { e.Client }).ToArray() : ClientManager.GetAllClients();
                    using (Message message = Message.Create(MessageTag.RPC, view))
                    {
                        foreach (IClient sendTo in clients)
                        {
                            sendTo.SendMessage(message, SendMode.Reliable);
                        }
                    }
                }
            }
            else if (e.Tag == MessageTag.PrivateRPC)
            {
                using (DarkRiftReader reader = e.GetMessage().GetReader())
                {
                    RPCView view = reader.ReadSerializable<RPCView>();
                    RiftView target = reader.ReadSerializable<RiftView>();

                    using (Message message = Message.Create(MessageTag.RPC, view))
                    {
                        foreach (IClient sendTo in ClientManager.GetAllClients())
                        {
                            if (players[sendTo].Equals(target))
                            {
                                sendTo.SendMessage(message, SendMode.Reliable);
                            }                            
                        }
                    }
                }
            }
        }
    }
}
