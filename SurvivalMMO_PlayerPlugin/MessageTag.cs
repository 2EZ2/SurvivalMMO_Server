using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalMMO_PlayerPlugin
{
    static class MessageTag
    {
        public static readonly ushort InsantiateObject = 0;
        public static readonly ushort RemoveObject = 1;
        public static readonly ushort RPCCall = 2;
        public static readonly ushort ReceivingStream = 3;
    }
}
