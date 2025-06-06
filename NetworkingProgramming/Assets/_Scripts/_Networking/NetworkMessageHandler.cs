using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public delegate void ServerNetworkMessage(ServerBehaviour server, NetworkConnection connection, NetworkMessage msg);
public delegate void ClientNetworkMessage(ClientBehaviour client, NetworkMessage msg);

public enum NetworkMessageType
{
    HANDSHAKE,
    HANDSHAKE_RESPONSE,
    OBJECT_POSITION,
    REMOTE_PROCEDURE_CALL,
    SPAWNMESSAGE
}

public static class NetworkMessageInfo
{
    public static Dictionary<NetworkMessageType, System.Type> typeMap = new Dictionary<NetworkMessageType, System.Type>
    {
        {NetworkMessageType.OBJECT_POSITION, typeof(ObjectPositionMessage) },
        {NetworkMessageType.REMOTE_PROCEDURE_CALL, typeof(RPCMessage) },
        {NetworkMessageType.HANDSHAKE, typeof(HandshakeMessage) },
        {NetworkMessageType.HANDSHAKE_RESPONSE, typeof(HandshakeResponseMessage) },
        {NetworkMessageType.SPAWNMESSAGE, typeof(HandshakeResponseMessage) }
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
        {NetworkMessageType.OBJECT_POSITION, HandleServerObjectPosition },
        {NetworkMessageType.HANDSHAKE_RESPONSE, HandleServerHandshakeResponse},
        {NetworkMessageType.SPAWNMESSAGE, HandleServerSpawn}
    };

    // client handlers
    private static void HandleClientObjectPosition(object recipient, NetworkConnection connection, NetworkMessage networkMessage) // handle object position message sent by client, received by server
    {
        // getting data
        ServerBehaviour server = recipient as ServerBehaviour;
        ObjectPositionMessage message = networkMessage as ObjectPositionMessage;

        // handling data
        GameObject obj;
        NetworkManager.Instance.Get(message.objectId, out obj);
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
        ServerBehaviour server = recipient as ServerBehaviour;
        HandshakeMessage message = networkMessage as HandshakeMessage;

        Debug.Log($"Received handshake from client! {message.nickname}");

        // check if player cap has been reached
        if (server.ReachedPlayerCap)
        {
            // TODO - send error back
            return;
        }
        // if no, proceed

        server.playerList.Add(connection, message.nickname);

        GameObject player;
        uint networkId = 0;
        if(NetworkManager.Instance.Create(NetworkObjectType.PLAYER, NetworkManager.NextNetworkId, out player))
        {
            NetworkedPlayer playerInstance = player.GetComponent<NetworkedPlayer>();
            playerInstance.isLocal = false;
            playerInstance.isServer = true;
            networkId = playerInstance.networkId;

            server.playerInstances.Add(connection, playerInstance);

            HandshakeResponseMessage response = new HandshakeResponseMessage()
            {
                networkId = playerInstance.networkId
            };
            server.SendNetworkMessageOne(connection, response);
        }
        else
        {
            Debug.Log("Could not spawn player instance");
        }

        // send all existing player data back (let them know who is in the lobby)  
        foreach(KeyValuePair<NetworkConnection, NetworkedPlayer> pair in server.playerInstances)
        {
            if(pair.Key == connection) { continue; }  // don't send the joined players own data back

            SpawnMessage spawnMessage = new SpawnMessage()
            {
                networkId = pair.Value.networkId,
                objectType = NetworkObjectType.PLAYER,
            };

            server.SendNetworkMessageOne(connection, spawnMessage);
        }

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

    private static void HandleServerHandshakeResponse(object recipient, NetworkMessage networkMessage)
    {
        ClientBehaviour client = recipient as ClientBehaviour;
        HandshakeResponseMessage response = networkMessage as HandshakeResponseMessage;

        Debug.Log($"Received handshake response! {response.networkId}");
        GameObject obj = null;
        if(NetworkManager.Instance.Create(NetworkObjectType.PLAYER, response.networkId, out obj))
        {
            NetworkedPlayer playerInstance = obj.GetComponent<NetworkedPlayer>();
            playerInstance.isLocal = true;
            playerInstance.isServer = false;
        }
        else
        {
            Debug.Log($"Could not spawn player");
        }
    }

    private static void HandleServerSpawn(object recipient, NetworkMessage networkMessage)
    {
        ClientBehaviour client = recipient as ClientBehaviour;
        SpawnMessage message = networkMessage as SpawnMessage;

        Debug.Log($"Received spawn message! {message.networkId}, {message.Type.ToString()}");
        if(!NetworkManager.Instance.Create(message.objectType, message.networkId, out GameObject obj))
        {
            Debug.Log($"Could not spawn object with id: {message.networkId}");
        }
    }

    private static void HandleServerObjectPosition(object recipient, NetworkMessage networkMessage) // handle object position message sent by server, received by client
    {
        // getting data
        ClientBehaviour client = recipient as ClientBehaviour;
        ObjectPositionMessage message = networkMessage as ObjectPositionMessage;

        // handling data
        NetworkManager.Instance.Get(message.objectId, out GameObject obj);
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
