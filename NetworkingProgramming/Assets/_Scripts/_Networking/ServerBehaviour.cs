using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class ServerBehaviour : MonoBehaviour
{
    public NetworkDriver driver;
    public NativeList<NetworkConnection> connections;
    private Dictionary<NetworkConnection, int> playerIds = new Dictionary<NetworkConnection, int>();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        driver = NetworkDriver.Create();
        connections = new NativeList<NetworkConnection>(16, Allocator.Persistent);

        var endpoint = NetworkEndpoint.AnyIpv4.WithPort(7777);
        if(driver.Bind(endpoint) != 0)
        {
            Debug.LogError("Failed to bind to port 7777");
            return;
        }
        driver.Listen();
    }

    private void OnDestroy()
    {
        if (driver.IsCreated)
        {
            driver.Dispose();
            connections.Dispose();
        }    
    }

    // Update is called once per frame
    void Update()
    {
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

    // public message sending functions

    public void SendNetworkMessageAll(NetworkMessage msg) 
    {
        for(int i = 0; i < connections.Length ; i++)
        {
            var connection = connections[i];
            driver.BeginSend(connection, out DataStreamWriter writer);
            msg.Encode(ref writer);
            driver.EndSend(writer);
        }
    }
    public void SendNetworkMessageOne(uint id, NetworkMessage msg) 
    {
        var connection = connections[(int)id];
        driver.BeginSend(connection, out DataStreamWriter writer);
        msg.Encode(ref writer);
        driver.EndSend(writer);
    }
}
