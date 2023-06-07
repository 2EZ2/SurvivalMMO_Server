using DarkRift;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalMMO_PlayerPlugin
{
    public class RPCView : IDarkRiftSerializable
    {
        public RiftView sender;
        public string method = "";
        byte[] data;
        public byte[] Data { get { return data; } set { data = value; } }
        public bool excludeSelf = false;

        public void Deserialize(DeserializeEvent e)
        {
            sender = e.Reader.ReadSerializable<RiftView>();
            method = e.Reader.ReadString();
            data = e.Reader.ReadBytes();
            excludeSelf = e.Reader.ReadBoolean();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(sender);
            e.Writer.Write(method);
            e.Writer.Write(data);
            e.Writer.Write(excludeSelf);
        }
    }
}
