using LiteNetLib;
using LiteNetLib.Utils;
using System;

namespace Server
{
    class Program : INetEventListener
    {
        private NetManager serverNetManager;

        public void Run()
        {
            serverNetManager = new NetManager(this, 100, "game");
            if (serverNetManager.Start(15000))
                Console.WriteLine("Server started listening on port 15000");
            else
            {
                Console.WriteLine("Server cold not start!");
                return;
            }

            while (serverNetManager.IsRunning)
            {
                serverNetManager.PollEvents();
                System.Threading.Thread.Sleep(15);
            }
        }

        static void Main(string[] args)
        {
            Program program = new Program();
            program.Run();

            Console.ReadKey();
        }

        public void OnPeerConnected(NetPeer peer)
        {
            Console.WriteLine($"OnPeerConnected: {peer.EndPoint.Host} : {peer.EndPoint.Port}");
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Console.WriteLine($"OnPeerConnected: {peer.EndPoint.Host} : {peer.EndPoint.Port} Reason: {disconnectInfo.Reason.ToString()}");
        }

        public void OnNetworkError(NetEndPoint endPoint, int socketErrorCode)
        {
            Console.WriteLine($"OnNetworkError: {socketErrorCode}");
        }

        public void OnNetworkReceive(NetPeer peer, NetDataReader reader)
        {
            Console.WriteLine($"OnNetworkReceive: {reader.Data.Length}");
        }

        public void OnNetworkReceiveUnconnected(NetEndPoint remoteEndPoint, NetDataReader reader, UnconnectedMessageType messageType)
        {
            Console.WriteLine($"OnNetworkReceiveUnconnected");
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
           
        }
    }
}
