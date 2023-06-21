using DarkRift;
using DarkRift.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SurvivalMMO_PlayerPlugin
{
    internal class RPCManager : Plugin
    {
        public override bool ThreadSafe => true;

        public override Version Version => new Version(1, 0, 0);

        public RPCManager(PluginLoadData data): base(data)
        {
            //Subscribe for notification when a new client connects
            ClientManager.ClientConnected += ClientManager_ClientConnected;
        }

        void ClientManager_ClientConnected(object sender, ClientConnectedEventArgs e)
        {
            e.Client.MessageReceived += Client_PlayerMessageReceivedEvent;
        }

        void Client_PlayerMessageReceivedEvent(object sender, MessageReceivedEventArgs e)
        {
            using (DarkRiftReader reader = e.GetMessage().GetReader())
            {
                if (e.Tag == MessageTag.RPC)
                {
                    RPCDataView view = reader.ReadSerializable<RPCDataView>();
                    IClient[] clients = view.RepeatToClient ? ClientManager.GetAllClients() : ClientManager.GetAllClients().Except(new IClient[] { e.Client }).ToArray();
                    using (Message message = Message.Create(MessageTag.RPC, view))
                    {
                        foreach (IClient sendTo in clients)
                        {
                            sendTo.SendMessage(message, SendMode.Reliable);
                        }
                    }
                }
                else if (e.Tag == MessageTag.PrivateRPC)
                {
                    RPCDataView view = reader.ReadSerializable<RPCDataView>();

                    using (Message message = Message.Create(MessageTag.RPC, view))
                    {
                        IClient sendTo = ClientManager.GetAllClients().ToList<IClient>().Find(x => x.ID == view.TargetRiftView.Owner);

                        if (sendTo != null)
                        {
                            sendTo?.SendMessage(message, SendMode.Reliable);
                        }
                    }
                }
            }
           
            
        }
    }
}
