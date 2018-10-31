using LiteNetLib;
using LiteNetLib.Utils;
using Shared.Enums;
using System;
using System.Collections.Generic;

namespace Server
{
    class Program : INetEventListener
    {
        NetDataWriter dataWriter;
        private NetManager serverNetManager;

        private Dictionary<long, NetworkPlayer> networkPlayersDictionary;

        public void Run()
        {
            try
            {
                dataWriter = new NetDataWriter();
                networkPlayersDictionary = new Dictionary<long, NetworkPlayer>();

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

                    SendPlayerPositions();

                    System.Threading.Thread.Sleep(15);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        static void Main(string[] args)
        {
            Program program = new Program();
            program.Run();

            Console.ReadKey();
        }

        public void SendPlayerPositions()
        {
            try
            {
                Dictionary<long, NetworkPlayer> sendPosDict = new Dictionary<long, NetworkPlayer>(networkPlayersDictionary);

                foreach (var sendToPlayer in sendPosDict)
                {
                    if (sendToPlayer.Value == null)
                        continue;

                    dataWriter.Reset();
                    dataWriter.Put((int)NetworkTags.PlayerPositionsArray);

                    int amountPlayersMoved = 0;

                    foreach (var posPlayers in sendPosDict)
                    {
                        if (sendToPlayer.Key == posPlayers.Key)
                            continue;

                        if (!posPlayers.Value.Moved)
                            continue;

                        dataWriter.Put(posPlayers.Key);

                        dataWriter.Put(posPlayers.Value.X);
                        dataWriter.Put(posPlayers.Value.Y);
                        dataWriter.Put(posPlayers.Value.Z);

                        amountPlayersMoved++;
                    }

                    if (amountPlayersMoved > 0)
                        sendToPlayer.Value.NetPeer.Send(dataWriter, SendOptions.Sequenced);
                }

                foreach (var player in networkPlayersDictionary)
                    player.Value.Moved = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void OnPeerConnected(NetPeer peer)
        {
            try
            {
                Console.WriteLine($"OnPeerConnected: {peer.EndPoint.Host} : {peer.EndPoint.Port}");

                NetDataWriter netDataWriter = new NetDataWriter();
                netDataWriter.Reset();
                netDataWriter.Put((int)NetworkTags.PlayerPositionsArray);

                foreach (var p in networkPlayersDictionary)
                {
                    netDataWriter.Put(p.Key);

                    netDataWriter.Put(p.Value.X);
                    netDataWriter.Put(p.Value.Y);
                    netDataWriter.Put(p.Value.Z);
                }

                peer.Send(netDataWriter, SendOptions.ReliableOrdered);

                if (!networkPlayersDictionary.ContainsKey(peer.ConnectId))
                    networkPlayersDictionary.Add(peer.ConnectId, new NetworkPlayer(peer));

                networkPlayersDictionary[peer.ConnectId].Moved = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            try
            {
                Console.WriteLine($"OnPeerConnected: {peer.EndPoint.Host} : {peer.EndPoint.Port} Reason: {disconnectInfo.Reason.ToString()}");

                if (networkPlayersDictionary.ContainsKey(peer.ConnectId))
                    networkPlayersDictionary.Remove(peer.ConnectId);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void OnNetworkError(NetEndPoint endPoint, int socketErrorCode)
        {
            try
            {
                Console.WriteLine($"OnNetworkError: {socketErrorCode}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void OnNetworkReceive(NetPeer peer, NetDataReader reader)
        {
            try
            {
                if (reader.Data == null)
                    return;

                NetworkTags networkTag = (NetworkTags)reader.GetInt();
                if (networkTag == NetworkTags.PlayerPosition)
                {
                    float x = reader.GetFloat();
                    float y = reader.GetFloat();
                    float z = reader.GetFloat();

                    Console.WriteLine($"Got position packet : {x} | {y} | {z}");

                    networkPlayersDictionary[peer.ConnectId].X = x;
                    networkPlayersDictionary[peer.ConnectId].Y = y;
                    networkPlayersDictionary[peer.ConnectId].Z = z;

                    networkPlayersDictionary[peer.ConnectId].Moved = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void OnNetworkReceiveUnconnected(NetEndPoint remoteEndPoint, NetDataReader reader, UnconnectedMessageType messageType)
        {
            try
            {
                Console.WriteLine($"OnNetworkReceiveUnconnected");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
            try
            {
                Console.WriteLine($"OnNetworkLatencyUpdate");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
