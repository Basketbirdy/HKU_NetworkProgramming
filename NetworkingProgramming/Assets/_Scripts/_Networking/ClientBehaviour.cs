using System;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class ClientBehaviour : MonoBehaviour
{

    public NetworkDriver driver;
    public NetworkConnection connection;

    public int playerId;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        driver = NetworkDriver.Create();

        var endpoint = NetworkEndpoint.LoopbackIpv4.WithPort(7777);
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

        Unity.Collections.DataStreamReader stream;
        NetworkEvent.Type cmd;
        while((cmd = connection.PopEvent(driver, out stream)) != NetworkEvent.Type.Empty)
        {
            if(cmd == NetworkEvent.Type.Connect)
            {
                Debug.Log("We are now connected to the server");

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

    public void SendNetworkMessageOne(uint id, NetworkMessage msg)
    {
        driver.BeginSend(connection, out DataStreamWriter writer);
        msg.Encode(ref writer);
        driver.EndSend(writer);
    }
}
