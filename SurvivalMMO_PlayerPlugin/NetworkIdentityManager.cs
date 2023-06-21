using DarkRift;
using DarkRift.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalMMO_PlayerPlugin
{
    internal class NetworkIdentityManager : Plugin
    {
        public override bool ThreadSafe => true;

        public override Version Version => new Version(1, 0, 0);

        public static Dictionary<RiftView, ushort> NetworkIdentityCache = new Dictionary<RiftView, ushort>();

        public NetworkIdentityManager(PluginLoadData pluginLoadData) : base(pluginLoadData)
        {
            //Subscribe for notification when a new client connects
            ClientManager.ClientConnected += ClientManager_ClientConnected;

            //Subscribe for notifications when a new client disconnects
            ClientManager.ClientDisconnected += ClientManager_ClientDisconnected;
        }

        void ClientManager_ClientConnected(object sender, ClientConnectedEventArgs e)
        {
            e.Client.MessageReceived += Client_PlayerMessageReceivedEvent;

            foreach (var item in NetworkIdentityCache)
            {
                using (DarkRiftWriter writer = DarkRiftWriter.Create())
                {
                    writer.Write(item.Key);
                    writer.Write((ushort)item.Value);

                    using (Message message = Message.Create(MessageTag.InsantiateObject, writer))
                    {
                        e.Client.SendMessage(message, SendMode.Reliable);
                    }
                }
            }
        }

        private void ClientManager_ClientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {
            foreach (var item in NetworkIdentityCache)
            {
                if(item.Value == e.Client.ID)
                {
                    NetworkIdentityCache.Remove(item.Key);
                }
            }
        }

        void Client_PlayerMessageReceivedEvent(object sender, MessageReceivedEventArgs e)
        {
            if (e.Tag == MessageTag.InsantiateObject)
            {
                using (DarkRiftReader reader = e.GetMessage().GetReader())
                {
                    ushort index = reader.ReadUInt16();

                    RiftView newView = new RiftView(UniqueIDGenerator.generateID(), e.Client.ID);

                    Console.WriteLine($@"New object created {newView.Owner} : {newView.ID.ToString("N")}");

                    NetworkIdentityCache.Add(newView, index);

                    using (DarkRiftWriter writer = DarkRiftWriter.Create())
                    {
                        writer.Write(newView);
                        writer.Write((ushort)index);

                        using (Message message = Message.Create(MessageTag.InsantiateObject, writer))
                        {
                            foreach (IClient sendTo in ClientManager.GetAllClients())
                            {
                                sendTo.SendMessage(message, SendMode.Reliable);
                            }
                        }
                    }
                }
            }
            else if(e.Tag == MessageTag.RemoveObject)
            {
                using (DarkRiftReader reader = e.GetMessage().GetReader())
                {
                    RiftView newView = reader.ReadSerializable<RiftView>();

                    if (NetworkIdentityCache.ContainsKey(newView))
                    {
                        NetworkIdentityCache.Remove(newView);
                    }

                    using (Message message = Message.Create(MessageTag.RemoveObject, newView))
                    {
                        foreach (IClient sendTo in ClientManager.GetAllClients())
                        {
                            sendTo.SendMessage(message, SendMode.Reliable);
                        }
                    }
                }                   
            }
        }


    }
}
