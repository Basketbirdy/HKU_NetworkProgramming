using Unity.Networking.Transport;
using UnityEngine;

public class ClientBehaviour : MonoBehaviour
{
    NetworkDriver driver;
    NetworkConnection connection;

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

                uint value = 1;
                driver.BeginSend(connection, out var writer);
                writer.WriteUInt(value);
                driver.EndSend(writer);
            }
            else if(cmd == NetworkEvent.Type.Data)
            {
                uint value = stream.ReadUInt();
                Debug.Log($"Got the value {value} back from the server");

                connection.Disconnect(driver);
                connection = default;
            }
            else if(cmd == NetworkEvent.Type.Disconnect)
            {
                Debug.Log("Client got disconnected from the server");
                connection = default;
            }
        }
    }
}
