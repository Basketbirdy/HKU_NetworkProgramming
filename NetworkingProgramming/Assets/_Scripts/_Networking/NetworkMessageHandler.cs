using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class NetworkMessageHandler
{
    public delegate void NetworkMessage(object handler, NetworkConnection connection, DataStreamReader reader);


    public static Dictionary<NetworkMessageType, NetworkMessage> networkMessageHandlers = new Dictionary<NetworkMessageType, NetworkMessage>
    {
        { NetworkMessageType.POSITION, NetworkMessageHandler.HandlePosition },
        { NetworkMessageType.SEND_UINT, NetworkMessageHandler.HandleUInt },
        { NetworkMessageType.SHOW_UINT, NetworkMessageHandler.HandleShowingUInt }
    };

    public static void HandlePosition(object handler, NetworkConnection connection, DataStreamReader stream)
    {
        float x = stream.ReadUInt();
        float y = stream.ReadUInt();
        float z = stream.ReadUInt();

        Vector3 position = new Vector3(x, y, z);

        ServerBehaviour server = handler as ServerBehaviour;    // for if I want to send a response
    }

    public static void HandleUInt(object handler, NetworkConnection connection, DataStreamReader stream)
    {
        uint x = stream.ReadUInt();
        Debug.Log($"Got {x} from a client, adding 2 to it");

        x += 2;

        // respond
        ServerBehaviour server = handler as ServerBehaviour;
        server.driver.BeginSend(NetworkPipeline.Null, connection, out DataStreamWriter writer);

        writer.WriteUInt((uint)NetworkMessageType.SHOW_UINT);
        writer.WriteUInt(x);

        server.driver.EndSend(writer);
    }

    public static void HandleShowingUInt(object handler, NetworkConnection connection, DataStreamReader stream)
    {
        uint x = stream.ReadUInt();
        Debug.Log($"Received {x}");
    }
}
