using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.EventSystems;

public class ServerBehaviour : MonoBehaviour
{
    public static ServerBehaviour Instance { get; private set; }
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    public NetworkDriver driver;
    public NativeList<NetworkConnection> connections;

    public Dictionary<NetworkConnection, string> playerNames = new Dictionary<NetworkConnection, string>();
    public Dictionary<NetworkConnection, NetworkedPlayer> playerInstances = new Dictionary<NetworkConnection, NetworkedPlayer>();

    private int playerCap = 2;
    private Action onConnectionDropped;

    [SerializeField] private float keepAliveTickRate = 20f;
    private float keepAliveTimestamp;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        driver = NetworkDriver.Create();
        connections = new NativeList<NetworkConnection>(16, Allocator.Persistent);

        var endpoint = NetworkEndpoint.AnyIpv4; // Accepts connections on any IPv4 address
        endpoint.Port = 8005;
        if (driver.Bind(endpoint) != 0)
        {
            Debug.LogError("Failed to bind to port 7777");
            return;
        }
        driver.Listen();

        OnPlayerHostedServer(); // do player setup for server host
    }

    private void OnDestroy()
    {
        if (driver.IsCreated)
        {
            ShutDown();
        }    
    }
    private void ShutDown()
    {
        driver.Dispose();
        connections.Dispose();
    }

    // Update is called once per frame
    void Update()
    {
        KeepAlive();

        driver.ScheduleUpdate().Complete();

        // clean up past connections
        for(int i = 0; i < connections.Length; i++)
        {
            if (!connections[i].IsCreated)
            {
                connections.RemoveAtSwapBack(i);
                i--;
            }
        }

        // accept new connections
        NetworkConnection c;
        while((c = driver.Accept()) != default)
        {
            connections.Add(c);
            Debug.Log("Accepted a connection");
        }

        for(int i = 0; i < connections.Length; i++) 
        {
            DataStreamReader stream;
            NetworkEvent.Type cmd;
            while((cmd = driver.PopEventForConnection(connections[i], out stream)) != NetworkEvent.Type.Empty)
            {
                if(cmd == NetworkEvent.Type.Data) 
                {
                    NetworkMessageType messageType = (NetworkMessageType)stream.ReadUInt();

                    // process received message
                    NetworkMessage msg = (NetworkMessage)Activator.CreateInstance(NetworkMessageInfo.typeMap[messageType]);
                    msg.Decode(ref stream);

                    EventHandler<NetworkMessage>.InvokeEvent(GlobalEvents.MESSAGE_RECEIVED, msg);

                    if(NetworkMessageHandler.clientMessageHandlers.ContainsKey(messageType))
                    {
                        try
                        {
                            NetworkMessageHandler.clientMessageHandlers[messageType].Invoke(this, connections[i], msg);
                        }
                        catch
                        {
                            Debug.LogError($"[Server] Message not properly formatted! {messageType}");
                        }
                    }
                    else
                    {
                        Debug.LogError($"[Server] no message type identified!");
                    }
                }
                else if(cmd == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log("Client disconnected from the server");
                    connections[i] = default;
                    break;
                }
            }
        }
    }
    private void KeepAlive()
    {
        if(Time.time - keepAliveTimestamp > keepAliveTickRate)
        {
            keepAliveTimestamp = Time.time;
            var message = new KeepAliveMessage();
            SendNetworkMessageAll(message);
        }
    }

    // public message sending functions

    public void SendNetworkMessageAll(NetworkMessage msg) 
    {
        for(int i = 0; i < connections.Length ; i++)
        {
            var connection = connections[i];
            int result = driver.BeginSend(connection, out DataStreamWriter writer);
            if(result == 0)
            {
                msg.Encode(ref writer);
                driver.EndSend(writer);
            }
            else
            {
                Debug.LogError($"[Server] failed writing network message to all! {result}", this);
            }
        }
    }

    public void SendNetworkMessageOne(uint id, NetworkMessage msg) 
    {
        var connection = connections[(int)id];
        int result = driver.BeginSend(connection, out DataStreamWriter writer);
        if (result == 0)
        {
            msg.Encode(ref writer);
            driver.EndSend(writer);
        }
        else
        {
            Debug.LogError($"[Server] failed writing network message to all! {result}", this);
        }
    }
    public void SendNetworkMessageOne(NetworkConnection connection, NetworkMessage msg)
    {
        int result = driver.BeginSend(connection, out DataStreamWriter writer);
        if (result == 0)
        {
            msg.Encode(ref writer);
            driver.EndSend(writer);
        }
        else
        {
            Debug.LogError($"[Server] failed writing network message to all! {result}", this);
        }
    }

    // helpers
    public bool CanJoin()
    {
        // do checks to see if a player can join
        Debug.Log($"Checking if client can join! if {playerInstances.Count} < {playerCap}");
        if(playerInstances.Count >= playerCap) { return false; }

        return true;
    }

    // execute this when player hosted server starts
    private void OnPlayerHostedServer()
    {
        //// spawn server gamemanager
        //GameObject player;
        //uint networkedId = 0;
        //if (NetworkManager.Instance.Create(NetworkObjectType.PLAYER, NetworkManager.NextNetworkId, out player))
        //{
        //    NetworkedPlayer instance = player.GetComponent<NetworkedPlayer>();
        //    instance.isLocal = true;
        //    instance.isServer = true;
        //    networkedId = instance.networkId;

        //    playerNames.Add(default, AccountManager.Instance.Nickname);
        //    playerInstances.Add(default, instance);
        //}
        //else
        //{
        //    Debug.LogError("Could not spawn server manager");
        //}

    }

    public void Disconnect(NetworkConnection connection)
    {
        for(int i = 0; i < connections.Length; i++)
        {
            NetworkConnection conn = connections[i];
            if(conn != connection)
            {
                continue;
            }

            conn.Disconnect(driver);

            playerNames.Remove(conn);
            playerInstances.Remove(conn);
            conn = default;

            connections.RemoveAtSwapBack(i);
        }
    }

    // receive message event
    public void AddMessageEventListener(Action<NetworkMessage> action)
    {
        EventHandler<NetworkMessage>.AddListener(GlobalEvents.SERVER_MESSAGE, action);
    }
    public void RemoveMessageEventListener(Action<NetworkMessage> action)
    {
        EventHandler<NetworkMessage>.RemoveListener(GlobalEvents.SERVER_MESSAGE, action);
    }
}
