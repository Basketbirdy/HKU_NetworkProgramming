using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public delegate void ServerNetworkMessage(ServerBehaviour server, NetworkConnection connection, NetworkMessage msg);
public delegate void ClientNetworkMessage(ClientBehaviour client, NetworkMessage msg);

public enum NetworkMessageType
{
    OBJECT_POSITION
}

public static class NetworkMessageHandler
{
    /// <summary>
    /// messages received by the server, from clients
    /// </summary>
    public static Dictionary<NetworkMessageType, ServerNetworkMessage> clientMessageHandlers = new Dictionary<NetworkMessageType, ServerNetworkMessage>
    {
        {NetworkMessageType.OBJECT_POSITION, HandleClientObjectPosition}
    };

    /// <summary>
    /// messages received by clients from the server
    /// </summary>
    public static Dictionary<NetworkMessageType, ClientNetworkMessage> serverMessageHandlers = new Dictionary<NetworkMessageType, ClientNetworkMessage>
    {

    };

    private static void HandleClientObjectPosition(object sender, NetworkConnection connection, NetworkMessage networkMessage)
    {
        // getting data
        ObjectPositionMessage message = networkMessage as ObjectPositionMessage;

        // handling data
        NetworkManager.instance.Get(message.objectId, out GameObject obj);
        obj.transform.position = message.position;

        // respond
    }
}

public static class NetworkMessageInfo
{
    public static Dictionary<NetworkMessageType, System.Type> typeMap = new Dictionary<NetworkMessageType, System.Type>
    {
        {NetworkMessageType.OBJECT_POSITION, typeof(ObjectPositionMessage) }
    };
}
