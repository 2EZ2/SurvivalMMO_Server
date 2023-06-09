using DarkRift;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalMMO_PlayerPlugin
{
    public class RPCDataView : IDarkRiftSerializable
    {
        public RiftView SenderView { get; set; }
        public RiftView TargetRiftView { get; set; }

        public string SystemType { get; set; }

        public string MethodName { get; set; }

        public bool RepeatToClient { get; set; }

        public object[] Inputs { get; set; }

        public RPCDataView()
        {
            this.SenderView = new RiftView();
            this.TargetRiftView = new RiftView();
            this.SystemType = "";
            this.MethodName = "";
            this.RepeatToClient = true;
            this.Inputs = new object[1];
        }

        public RPCDataView(RiftView sender, RiftView targetRiftView, string componentName, string methodName, bool repeatToClient, object[] inputs)
        {
            this.SenderView = sender;
            this.TargetRiftView = targetRiftView;
            this.SystemType = componentName;
            this.MethodName = methodName;
            this.RepeatToClient = repeatToClient;
            this.Inputs = inputs;
        }

        public void Deserialize(DeserializeEvent e)
        {
            SenderView = e.Reader.ReadSerializable<RiftView>();
            TargetRiftView = e.Reader.ReadSerializable<RiftView>();
            SystemType = e.Reader.ReadString();
            MethodName = e.Reader.ReadString();
            RepeatToClient = e.Reader.ReadBoolean();

            byte[] bytes = e.Reader.ReadBytes();

            IFormatter formatter = new BinaryFormatter();

            object convertedStream;

            using (MemoryStream memoryStream = new MemoryStream(bytes))
            {
                convertedStream = formatter.Deserialize(memoryStream);

                this.Inputs = ((object[])convertedStream);
            }
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write<RiftView>(this.SenderView);
            e.Writer.Write<RiftView>(this.TargetRiftView);
            e.Writer.Write(this.SystemType);
            e.Writer.Write(this.MethodName);
            e.Writer.Write(this.RepeatToClient);
            e.Writer.WriteAs(typeof(object), Inputs);
        }
    }
}
