using DarkRift;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace SurvivalMMO_PlayerPlugin
{
    [System.Serializable]
    public class RiftMessage : IDarkRiftSerializable
    {
        public RiftView Sender { get; set; }
        public byte[] message { get; set; }

        public RiftMessage()
        {
            Sender = new RiftView(new Guid(), 100);
        }

        public RiftMessage(RiftView view)
        {
            Sender = view;
        }

        public void Deserialize(DeserializeEvent e)
        {
            Sender = e.Reader.ReadSerializable<RiftView>();
            message = e.Reader.ReadBytes();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write<RiftView>(Sender);
            e.Writer.Write(message);
        }
    }
}
