using DarkRift;
using DarkRift.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalMMO_PlayerPlugin
{
    internal class SerializedStreamManager : Plugin
    {
        public override bool ThreadSafe => true;

        public override Version Version => new Version(1, 0, 0);

        public SerializedStreamManager(PluginLoadData pluginLoadData) : base(pluginLoadData)
        {
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
                if (e.Tag == MessageTag.SendStream)
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

                        //Write messages to everyone but client sending the message
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
        }
    }
}