using DarkRift;
using DarkRift.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalMMO_PlayerPlugin
{
    internal class SyncVarManager : Plugin
    {
        public override bool ThreadSafe => true;

        public override Version Version => new Version(1, 0, 0);

        public SyncVarManager(PluginLoadData data) : base(data)
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
                if (e.Tag == MessageTag.SyncVar)
                {
                    ushort messageCount = reader.ReadUInt16(); //How many messages am i recieving?

                    //Gather Messages
                    if (messageCount > 0)
                    {
                        VarSyncDataView[] messages = new VarSyncDataView[messageCount];

                        for (int i = 0; i < messageCount; i++)
                        {
                            messages[i] = reader.ReadSerializable<VarSyncDataView>();
                        }

                        //Write messages to everyone but client sending the message
                        using (DarkRiftWriter writer = DarkRiftWriter.Create())
                        {
                            writer.Write(messages);

                            using (Message message = Message.Create(MessageTag.SyncVar, writer))
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

        [System.Serializable]
        public class VarSyncDataView : IDarkRiftSerializable
        {
            public RiftView SenderView { get; set; }

            public string SystemType { get; set; }

            public string PropertyName { get; set; }

            public object Value { get; set; }

            public VarSyncDataView()
            {
                this.SenderView = new RiftView();
                this.SystemType = "";
                this.PropertyName = "";
                this.Value = new object();
            }

            public VarSyncDataView(RiftView sender, string componentName, string propertyName, object input)
            {
                this.SenderView = sender;
                this.SystemType = componentName;
                this.PropertyName = propertyName;
                this.Value = input;
            }

            public void Deserialize(DeserializeEvent e)
            {
                SenderView = e.Reader.ReadSerializable<RiftView>();
                SystemType = e.Reader.ReadString();
                PropertyName = e.Reader.ReadString();

                byte[] bytes = e.Reader.ReadBytes();

                IFormatter formatter = new BinaryFormatter();

                using (MemoryStream memoryStream = new MemoryStream(bytes))
                {
                    object convertedStream = formatter.Deserialize(memoryStream);

                    this.Value = ((object)convertedStream);
                }
            }

            public void Serialize(SerializeEvent e)
            {
                e.Writer.Write<RiftView>(this.SenderView);
                e.Writer.Write(this.SystemType);
                e.Writer.Write(this.PropertyName);
                e.Writer.WriteAs(typeof(object), Value);
            }
        }
    }
