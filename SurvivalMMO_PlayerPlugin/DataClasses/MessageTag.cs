﻿using System;
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
        public static readonly ushort RPC = 2;
        public static readonly ushort ReceivingStream = 3;
        public static readonly ushort SendStream = 4;
        public static readonly ushort CloseConnection = 5;
        public static readonly ushort OwnershipRequest = 6;
        public static readonly ushort OwnershipTransfer = 7;
        public static readonly ushort OwnershipUpdate = 8;
        public static readonly ushort PlayerConnected = 9;
        public static readonly ushort PlayerDisconnected = 10;
        public static readonly ushort SpawnPlayer = 11;
        public static readonly ushort RemovePlayer = 12;
        public static readonly ushort PrivateRPC = 13;
    }
}
