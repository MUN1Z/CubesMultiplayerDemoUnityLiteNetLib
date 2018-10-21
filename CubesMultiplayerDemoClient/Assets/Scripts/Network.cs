using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

public class Network : MonoBehaviour, INetEventListener {

    private NetManager clientNetManager;
    
    // Use this for initialization
    void Start()
    {
        clientNetManager = new NetManager(this, "game");

        if (clientNetManager.Start())
        {
            clientNetManager.Connect("localhost", 15000);
            Debug.Log("Client net manager started!");
        }
        else
            Debug.LogError("Could not start client net manager!");

    }

    private void FixedUpdate()
    {
        if (clientNetManager.IsRunning)
            clientNetManager.PollEvents();
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    
    public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {
        
    }

    public void OnNetworkReceive(NetPeer peer, NetDataReader reader)
    {
        Debug.Log($"OnNetworkReceive: {reader.Data.Length}");
    }
    
    public void OnPeerConnected(NetPeer peer)
    {
        Debug.Log($"OnPeerConnected: {peer.EndPoint.Host} : {peer.EndPoint.Port}");
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        Debug.Log($"OnPeerConnected: {peer.EndPoint.Host} : {peer.EndPoint.Port} Reason: {disconnectInfo.Reason.ToString()}");
    }

    public void OnNetworkError(NetEndPoint endPoint, int socketErrorCode)
    {
        Debug.LogError($"OnNetworkError: {socketErrorCode}");
    }

    public void OnNetworkReceiveUnconnected(NetEndPoint remoteEndPoint, NetDataReader reader, UnconnectedMessageType messageType)
    {
        Debug.Log($"OnNetworkReceive: {reader.Data.Length}");
    }
}
