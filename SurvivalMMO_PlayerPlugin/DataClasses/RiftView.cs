using DarkRift;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalMMO_PlayerPlugin
{
    /// <summary>
    ///     Holds serializable data about a player.
    /// </summary>
    [System.Serializable]
    public class RiftView : IDarkRiftSerializable, ISerializable
    {
        public Guid ID { get; set; }
        public ushort Owner { get; set; }

        public RiftView()
        {
            ID = new Guid();
            Owner = 0;
        }

        public RiftView(Guid id, ushort owner)
        {
            ID = id;
            Owner = owner;
        }

        public void Deserialize(DeserializeEvent e)
        {
            byte[] id = e.Reader.ReadBytes();
            this.ID = new Guid(id);
            this.Owner = e.Reader.ReadUInt16();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(this.ID.ToByteArray());
            e.Writer.Write(this.Owner);
        }

        public override string ToString()
        {
            return $@"VIEW: {ID}, {Owner}";
        }
        /// <summary>
        ///     Compares an object for equality with this.
        /// </summary>
        /// <param name="obj">The object to compare with.</param>
        /// <returns>Whether the object is equal to this block.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is RiftView))
                return false;

            RiftView b = (RiftView)obj;

            return this.ID == b.ID && this.Owner == b.Owner;
        }
        /// <summary>
        ///     Basic hashcode generator based on the object id
        /// </summary>
        /// <returns>hash code.</returns>
        public override int GetHashCode()
        {
            return (int)(ID.GetHashCode() + this.Owner.GetHashCode());
        }

        public RiftView(SerializationInfo info, StreamingContext ctxt)
        {
            ID = (Guid)info.GetValue("ID", typeof(Guid));
            Owner = (ushort)info.GetValue("OWN", typeof(ushort));
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ID", ID);
            info.AddValue("OWN", Owner);
        }
    }
}
