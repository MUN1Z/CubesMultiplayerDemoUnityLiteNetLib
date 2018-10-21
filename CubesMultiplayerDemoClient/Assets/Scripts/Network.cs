using Assets.Scripts;
using LiteNetLib;
using LiteNetLib.Utils;
using Shared.Enums;
using System.Collections.Generic;
using UnityEngine;

public class Network : MonoBehaviour, INetEventListener {

    public Player player;
    private Vector3 lastNetworkedPosition = Vector3.zero;
    private float lastDistance = 0.0f;
    const float MIN_DISTANCE_TO_SEND_POSITION = 0.001f;

    private NetDataWriter dataWriter;
    private NetManager clientNetManager;
    private NetPeer serverPeer;

    public GameObject netPlayerPrefab;
    private Dictionary<long, NetPlayer> netPlayersDictionary;

    // Use this for initialization
    void Start()
    {
        netPlayersDictionary = new Dictionary<long, NetPlayer>();
        dataWriter = new NetDataWriter();
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
        if (clientNetManager != null)
            if (clientNetManager.IsRunning)
            {
                clientNetManager.PollEvents();

                lastDistance = Vector3.Distance(lastNetworkedPosition, player.transform.position);
                if(lastDistance >= MIN_DISTANCE_TO_SEND_POSITION)
                {
                    dataWriter.Reset();
                    
                    dataWriter.Put((int)NetworkTags.PlayerPosition);
                    dataWriter.Put(player.transform.position.x);
                    dataWriter.Put(player.transform.position.y);
                    dataWriter.Put(player.transform.position.z);

                    serverPeer.Send(dataWriter, SendOptions.Sequenced);

                    lastNetworkedPosition = player.transform.position;
                }
            }

        foreach (var p in netPlayersDictionary)
        {
            if (!p.Value.gameObjectAdded)
            {
                p.Value.gameObjectAdded = true;
                p.Value.gameObject = GameObject.Instantiate(netPlayerPrefab, new Vector3(p.Value.x, p.Value.y, p.Value.z), Quaternion.identity);
            }
            else
            {
                p.Value.gameObject.transform.position = new Vector3(p.Value.x, p.Value.y, p.Value.z);

                //Debug.Log($"X: {p.Value.x} Y: {p.Value.y} Z: { p.Value.z}");
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    private void OnApplicationQuit()
    {
        if (clientNetManager != null)
            if (clientNetManager.IsRunning)
                clientNetManager.Stop();
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {
        
    }

    public void OnNetworkReceive(NetPeer peer, NetDataReader reader)
    {
        if (reader.Data == null)
            return;

        Debug.Log($"OnNetworkReceive: {reader.Data.Length}");

        if (reader.Data.Length >= 4)
        {
            NetworkTags networkTag = (NetworkTags)reader.GetInt();
            if (networkTag == NetworkTags.PlayerPositionsArray)
            {
                int lengthArr = (reader.Data.Length - 4) / (sizeof(long) + sizeof(float) * 3);

                Debug.Log("Got positions array data num : " + lengthArr);
                
                for (int i = 0; i < lengthArr; i++)
                {
                    long playerid = reader.GetLong();

                    float x = reader.GetFloat();
                    float y = reader.GetFloat();
                    float z = reader.GetFloat();

                    if (!netPlayersDictionary.ContainsKey(playerid))
                    {
                        netPlayersDictionary.Add(playerid, new NetPlayer());
                    }

                    netPlayersDictionary[playerid].x = x;
                    netPlayersDictionary[playerid].y = y;
                    netPlayersDictionary[playerid].z = z;
                }
            }
        }
    }
    
    public void OnPeerConnected(NetPeer peer)
    {
        serverPeer = peer;
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
