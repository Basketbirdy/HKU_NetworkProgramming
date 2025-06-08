using System;
using System.Collections.Generic;
using System.Data;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEditor.MemoryProfiler;
using UnityEngine;

public class ClientBehaviour : MonoBehaviour
{
    public NetworkDriver driver;
    public NetworkConnection connection;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        driver = NetworkDriver.Create();

        var endpoint = NetworkEndpoint.Parse("192.168.178.63", 9000, NetworkFamily.Ipv4);
        endpoint.Port = 1511;
        connection = driver.Connect(endpoint);
    }

    private void OnDestroy()
    {
        if (driver.IsCreated)
        {
            driver.Dispose();
        }
    }

    // Update is called once per frame
    void Update()
    {
        driver.ScheduleUpdate().Complete();

        if(!connection.IsCreated)
        {
            return;
        }

        DataStreamReader stream;
        NetworkEvent.Type cmd;
        while((cmd = connection.PopEvent(driver, out stream)) != NetworkEvent.Type.Empty)
        {
            if(cmd == NetworkEvent.Type.Connect)
            {
                Debug.Log("We are now connected to the server");

                HandshakeMessage message = new HandshakeMessage()
                {
                    name = AccountManager.Instance.Nickname
                };
                SendNetworkMessage(message);

                // TODO - get player Id

                //uint value = 1;
                //driver.BeginSend(connection, out var writer);
                //writer.WriteUInt((uint)NetworkMessageType.SEND_UINT);
                //writer.WriteUInt(value);
                //driver.EndSend(writer);
            }
            else if(cmd == NetworkEvent.Type.Data)
            {
                NetworkMessageType messageType = (NetworkMessageType)stream.ReadUInt();

                // process received message
                NetworkMessage msg = (NetworkMessage)Activator.CreateInstance(NetworkMessageInfo.typeMap[messageType]);
                msg.Decode(ref stream);

                if (NetworkMessageHandler.serverMessageHandlers.ContainsKey(messageType))
                {
                    try
                    {
                        NetworkMessageHandler.serverMessageHandlers[messageType].Invoke(this, msg);
                    }
                    catch
                    {
                        Debug.LogError($"[Server] read-order does not mimic write-order!");
                    }
                }
                else
                {
                    Debug.LogError($"[Server] no message type identified!");
                }

                //uint value = stream.ReadUInt();
                //Debug.Log($"Got the value {value} back from the server");

                //connection.Disconnect(driver);
                //connection = default;
            }
            else if(cmd == NetworkEvent.Type.Disconnect)
            {
                Debug.Log("Client got disconnected from the server");
                connection = default;
            }
        }
    }

    /// <summary>
    /// prepare, write and send a network message to the server
    /// </summary>
    /// <param name="msg"></param>
    public void SendNetworkMessage(NetworkMessage msg)
    {
        int result = driver.BeginSend(connection, out DataStreamWriter writer);
        if(result == 0)
        {
            msg.Encode(ref writer);
            driver.EndSend(writer);
        }
        else
        {
            Debug.LogError($"[Client] failed writing network message! {result}", this);
        }
    }
}
