using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class ServerBehaviour : MonoBehaviour
{
    NetworkDriver driver;
    NativeList<NetworkConnection> connections;

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
                    uint number = stream.ReadUInt();
                    Debug.Log($"Got {number} from a client, adding 2 to it");

                    number += 2;

                    driver.BeginSend(NetworkPipeline.Null, connections[i], out var writer);
                    writer.WriteUInt(number);
                    driver.EndSend(writer);
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
}
