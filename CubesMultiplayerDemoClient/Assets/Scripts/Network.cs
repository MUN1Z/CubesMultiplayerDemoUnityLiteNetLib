using LiteNetLib;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class Network : MonoBehaviour, INetEventListener {

    private NetManager clientNetManager;
    
    // Use this for initialization
    void Start()
    {
        EventBasedNetListener listener = new EventBasedNetListener();
        clientNetManager = new NetManager(listener);

        if (clientNetManager.Start())
        {
            clientNetManager.Connect("127.0.0.1", 7171, "");
            Debug.LogError("Client net manager started!");

        }
        else
            Debug.LogError("Could not start client net manager!");

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnConnectionRequest(ConnectionRequest request)
    {
        
    }

    public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
    {
        Debug.LogError($"OnNetworkError: {socketError}");
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {
        
    }

    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
    {
        Debug.LogError($"OnNetworkReceive: {reader.AvailableBytes}");
    }

    public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
    {
        Debug.LogError($"OnNetworkReceive: {reader.AvailableBytes}");
    }

    public void OnPeerConnected(NetPeer peer)
    {
        Debug.LogError($"OnPeerConnected: {peer.EndPoint.Address} : {peer.EndPoint.Port}");
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        Debug.LogError($"OnPeerConnected: {peer.EndPoint.Address} : {peer.EndPoint.Port} Reason: {disconnectInfo.Reason.ToString()}");
    }
}
