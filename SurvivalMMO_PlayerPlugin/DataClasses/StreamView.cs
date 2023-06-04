using DarkRift;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalMMO_PlayerPlugin
{
    public class StreamView : IDarkRiftSerializable
    {
        RiftView target;
        byte[] data;
        public byte[] Data { get { return data; } set { data = value; } }

        public void Deserialize(DeserializeEvent e)
        {
            target = e.Reader.ReadSerializable<RiftView>();
            data = e.Reader.ReadBytes();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(target);
            e.Writer.Write(data);
        }
    }
}
