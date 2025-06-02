using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public delegate void ServerNetworkMessage(ServerBehaviour server, NetworkConnection connection, NetworkMessage msg);
public delegate void ClientNetworkMessage(ClientBehaviour client, NetworkMessage msg);

public enum NetworkMessageType
{
    HANDSHAKE,
    OBJECT_POSITION,
    REMOTE_PROCEDURE_CALL
}

public static class NetworkMessageInfo
{
    public static Dictionary<NetworkMessageType, System.Type> typeMap = new Dictionary<NetworkMessageType, System.Type>
    {
        {NetworkMessageType.OBJECT_POSITION, typeof(ObjectPositionMessage) },
        {NetworkMessageType.REMOTE_PROCEDURE_CALL, typeof(RPCMessage) },
        {NetworkMessageType.HANDSHAKE, typeof(HandshakeMessage) }
    };
}

public static class NetworkMessageHandler
{
    /// <summary>
    /// messages received by the server, from clients
    /// </summary>
    public static Dictionary<NetworkMessageType, ServerNetworkMessage> clientMessageHandlers = new Dictionary<NetworkMessageType, ServerNetworkMessage>
    {
        {NetworkMessageType.OBJECT_POSITION, HandleClientObjectPosition},
        {NetworkMessageType.HANDSHAKE, HandleClientHandshake}
    };

    /// <summary>
    /// messages received by clients from the server
    /// </summary>
    public static Dictionary<NetworkMessageType, ClientNetworkMessage> serverMessageHandlers = new Dictionary<NetworkMessageType, ClientNetworkMessage>
    {
        {NetworkMessageType.OBJECT_POSITION, HandleServerObjectPosition }
    };

    // client handlers
    private static void HandleClientObjectPosition(object recipient, NetworkConnection connection, NetworkMessage networkMessage) // handle object position message sent by client, received by server
    {
        // getting data
        ServerBehaviour server = recipient as ServerBehaviour;
        ObjectPositionMessage message = networkMessage as ObjectPositionMessage;

        // handling data
        GameObject obj;
        NetworkManager.instance.Get(message.objectId, out obj);
        if(obj != null)
        {
            obj.transform.position = message.position;
        }
        else
        {
            Debug.LogWarning($"[ObjectPosition Handler] could not find networked object! id: {message.objectId}");
        }
        Debug.Log($"[Server] received object position message! objId: {message.objectId}, pos: {message.position}");

        // respond
        uint newId = message.objectId + 1;
        ObjectPositionMessage response = new ObjectPositionMessage { objectId = newId, position = Vector3.one * newId };
        server.SendNetworkMessageAll(response);
    }

    // client handlers

    private static void HandleClientHandshake(object recipient, NetworkConnection connection, NetworkMessage networkMessage)
    {
        // getting data
        ClientBehaviour client = recipient as ClientBehaviour;

        // check if player cap has been reached
        // if yes, send error back
        // if no, proceed

        // add this player to the player list
        // spawn player (does the server actually need to spawn visible objects?)
        
        // send all existing player data back (let them know who is in the lobby)  

        // send player to all other clients (just the other in this case, since
    }

    //sub client methods

    public static void HandleRPC(ClientBehaviour client, RPCMessage msg)
    {
        RPCMessage message = msg;

        // try to call the function
        try
        {
            message.methodInfo.Invoke(message.target, message.data);
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
            Debug.Log(e.StackTrace);
        }
    }

    // server handlers

    private static void HandleServerObjectPosition(object recipient, NetworkMessage networkMessage) // handle object position message sent by server, received by client
    {
        // getting data
        ClientBehaviour client = recipient as ClientBehaviour;
        ObjectPositionMessage message = networkMessage as ObjectPositionMessage;

        // handling data
        NetworkManager.instance.Get(message.objectId, out GameObject obj);
        if (obj != null)
        {
            obj.transform.position = message.position;
        }
        else
        {
            Debug.LogWarning($"[ObjectPosition Handler] could not find networked object! id: {message.objectId}");
        }
        Debug.Log($"[Client] received object position message! objId: {message.objectId}, pos: {message.position}");


        // respond
    }

    // sub server methods
    public static void HandleRPC(ServerBehaviour server, RPCMessage msg)
    {
        RPCMessage message = msg;

        // try to call the function
        try
        {
            message.methodInfo.Invoke(message.target, message.data);
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
            Debug.Log(e.StackTrace);
        }
    }
}
