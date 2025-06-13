using System;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class ClientBehaviour : MonoBehaviour
{
    public static ClientBehaviour Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public NetworkDriver driver;
    public NetworkConnection connection;
    [Space]
    public static string ip = "127.0.0.1";
    public ushort port = 8005;

    private Action onConnectionDropped;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        driver = NetworkDriver.Create();

        var endpoint = NetworkEndpoint.Parse(ip, port, NetworkFamily.Ipv4);
        connection = driver.Connect(endpoint);

        Debug.Log($"Attempting to connect to Server on {endpoint.Address}");
    }

    private void OnDestroy()
    {
        if (driver.IsCreated)
        {
            ShutDown();
        }
    }
    public void ShutDown()
    {
        driver.Dispose();

        connection.Disconnect(driver);
        connection = default(NetworkConnection);
    }

    // Update is called once per frame
    void Update()
    {
        driver.ScheduleUpdate().Complete();

        // check if connection is still alive
        CheckAlive();

        // handle messages
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
                connection = default(NetworkConnection);
                onConnectionDropped?.Invoke();
                ShutDown();
            }
        }
    }
    private void CheckAlive()
    {
        if(!connection.IsCreated && driver.IsCreated)
        {
            Debug.Log("Something went wrong! lost connection to the server");
            onConnectionDropped?.Invoke();
            ShutDown();
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
