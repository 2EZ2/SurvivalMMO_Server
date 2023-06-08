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


        List<IClient> clientList = new List<IClient>();

        public ServerPlayerManager(PluginLoadData pluginLoadData) : base(pluginLoadData)
        {
            //Subscribe for notification when a new client connects
            ClientManager.ClientConnected += ClientManager_ClientConnected;

            //Subscribe for notifications when a new client disconnects
            ClientManager.ClientDisconnected += ClientManager_ClientDisconnected;
        }

        void ClientManager_ClientConnected(object sender, ClientConnectedEventArgs e)
        {
            //Subscribe to when this client sends messages
            e.Client.MessageReceived += Client_PlayerMessageReceivedEvent;

            lock (clientList)
            {
                clientList.Add(e.Client);
            }
            
            using(DarkRiftWriter writer = DarkRiftWriter.Create())
            {
                writer.Write(e.Client.ID);
                //Send player joined to all players on server
                using (Message message = Message.Create(MessageTag.PlayerConnected, writer))
                {
                    foreach (IClient client in ClientManager.GetAllClients())
                    {
                        client.SendMessage(message, SendMode.Reliable);
                    }
                }
            }
            
        }

        private void ClientManager_ClientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {
            lock (clientList)
            {
                clientList.Remove(e.Client);
            }

            using (DarkRiftWriter writer = DarkRiftWriter.Create())
            {
                writer.Write(e.Client.ID);
                //Send player Disconnected to all players on server
                using (Message message = Message.Create(MessageTag.PlayerDisconnected, writer))
                {
                    foreach (IClient client in ClientManager.GetAllClients())
                    {
                        client.SendMessage(message, SendMode.Reliable);
                    }
                }
            }
        }

        void Client_PlayerMessageReceivedEvent(object sender, MessageReceivedEventArgs e)
        {
            if(e.Tag == MessageTag.SendStream)
            {            
                using (DarkRiftReader reader = e.GetMessage().GetReader())
                {
                    ushort messageCount = reader.ReadUInt16(); //How many messages am i recieving?

                    //Gather Messages
                    if (messageCount > 0)
                    {
                        RiftMessage[] messages = new RiftMessage[messageCount];

                        for (int i = 0; i < messageCount; i++)
                        {
                            messages[i] = reader.ReadSerializable<RiftMessage>();
                        }

                        using (DarkRiftWriter writer = DarkRiftWriter.Create())
                        {
                            writer.Write(messages);

                            using (Message message = Message.Create(MessageTag.ReceivingStream, writer))
                            {
                                foreach (IClient sendTo in ClientManager.GetAllClients().Except(new IClient[] { e.Client }))
                                {
                                    sendTo.SendMessage(message, SendMode.Unreliable);
                                }
                            }
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
                        IClient sendTo = clientList.Find(x => x.ID == target.Owner);

                        sendTo?.SendMessage(message, SendMode.Reliable);
                    }
                }
            }
        }
    }
}
