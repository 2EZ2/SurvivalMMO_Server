using DarkRift;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalMMO_PlayerPlugin
{
    /// <summary>
    ///     Holds serializable data about a player.
    /// </summary>
    public class RiftView : IDarkRiftSerializable
    {
        public ushort ID { get; set; }
        public ushort Owner { get; set; }

        public RiftView()
        {
            ID = 0;
            Owner = 0;
        }

        public RiftView(ushort id, ushort owner)
        {
            ID = id;
            Owner = owner;
        }

        public void Deserialize(DeserializeEvent e)
        {
            this.ID = e.Reader.ReadUInt16();
            this.Owner = e.Reader.ReadUInt16();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(ID);
            e.Writer.Write(Owner);
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
            return (int)(this.ID * 23 + this.Owner * 29);
        }
    }
}
