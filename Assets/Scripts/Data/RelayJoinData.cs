using System;

namespace Data
{
    public class RelayJoinData
    {
        public string JoinCode;
        public string Ipv4Address;
        public ushort Port;
        public Guid AllocationId;
        public byte[] AllocationIDBytes;
        public byte[] ConnectionDataBytes;
        public byte[] HostConnectionData;
        public byte[] Key;
    }
}