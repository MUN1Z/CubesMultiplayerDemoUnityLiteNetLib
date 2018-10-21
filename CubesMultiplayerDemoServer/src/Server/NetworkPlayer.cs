using LiteNetLib;

namespace Server
{
    public class NetworkPlayer
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public NetPeer NetPeer { get; set; }
        public bool Moved { get; set; }


        public NetworkPlayer(NetPeer peer)
        {
            NetPeer = peer;

            X = 0.0f;
            Y = 0.0f;
            Z = 0.0f;

            Moved = false;
        }
    }
}
