using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DarkRift;
using DarkRift.Server;

namespace SurvivalMMO_PlayerPlugin
{
    public class ServerObjectsManager : Plugin
    {
        HashSet<RiftView> objects = new HashSet<RiftView>();

        public ServerObjectsManager(PluginLoadData pluginLoadData) : base(pluginLoadData)
        {
            //Subscribe for notification when a new client connects
            ClientManager.ClientConnected += ClientManager_ClientConnected;

            //Subscribe for notifications when a new client disconnects
            ClientManager.ClientDisconnected += ClientManager_ClientDisconnected;
        }

        public override bool ThreadSafe => throw new NotImplementedException();

        public override Version Version => throw new NotImplementedException();

        void ClientManager_ClientConnected(object sender, ClientConnectedEventArgs e)
        {
            //Subscribe to when this client sends messages
            e.Client.MessageReceived += Client_PlayerMessageReceivedEvent;
        }

        private void ClientManager_ClientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {
            // Delete or reassign objects when player client leaves         
        }

        void Client_PlayerMessageReceivedEvent(object sender, MessageReceivedEventArgs e)
        {

        }


    }

    /// <summary>
    ///     Holds serializable data about a player.
    /// </summary>
    public class RiftView : IDarkRiftSerializable
    {
        public ushort ID { get; set; }
        public ushort Owner { get; set; }

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
        ///     Basic hashcode generator based on the object id
        /// </summary>
        /// <returns>hash code.</returns>
        public override int GetHashCode()
        {
            return (int)(this.ID * 23 + this.Owner * 29);
        }
    }
}
