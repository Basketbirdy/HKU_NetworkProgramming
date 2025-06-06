using NUnit.Framework;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public delegate void ServerNetworkMessage(ServerBehaviour server, NetworkConnection connection, NetworkMessage msg);
public delegate void ClientNetworkMessage(ClientBehaviour client, NetworkMessage msg);

public enum NetworkMessageType
{
    OBJECT_POSITION,
    REMOTE_PROCEDURE_CALL,

    HANDSHAKE,
    HANDSHAKE_RESPONSE,

    GAME_START,
}

public static class NetworkMessageInfo
{
    public static Dictionary<NetworkMessageType, System.Type> typeMap = new Dictionary<NetworkMessageType, System.Type>
    {
        {NetworkMessageType.OBJECT_POSITION, typeof(ObjectPositionMessage) },
        {NetworkMessageType.REMOTE_PROCEDURE_CALL, typeof(RPCMessage) },

        {NetworkMessageType.HANDSHAKE, typeof(HandshakeMessage) },
        {NetworkMessageType.HANDSHAKE_RESPONSE, typeof(HandshakeResponseMessage) }
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
        {NetworkMessageType.GAME_START, HandleClientStartGame },
        {NetworkMessageType.HANDSHAKE, HandleClientHandshake }
    };

    /// <summary>
    /// messages received by clients from the server
    /// </summary>
    public static Dictionary<NetworkMessageType, ClientNetworkMessage> serverMessageHandlers = new Dictionary<NetworkMessageType, ClientNetworkMessage>
    {
        {NetworkMessageType.OBJECT_POSITION, HandleServerObjectPosition },
        {NetworkMessageType.HANDSHAKE_RESPONSE, HandleServerHandshakeResponse }
    };

    // client handlers
    private static void HandleClientHandshake(object recipient, NetworkConnection connection, NetworkMessage networkMessage) // handshake is client trying to join the server
    {
        ServerBehaviour server = recipient as ServerBehaviour;
        HandshakeMessage message = networkMessage as HandshakeMessage;

        Debug.Log($"Received handshake request");

        // check if the connection is allowed to join
        if (server.CheckJoin())
        {
            Debug.Log("New connection could not join");
            return;
        }

        // spawn server gamemanager
        GameObject gm = null;
        uint networkId = 0;
        if(NetworkManager.Instance.Create(ObjectType.GAMEMANAGER, NetworkManager.NextNetworkId, out gm)){
            NetworkedGameManager gameManagerInstance = gm.GetComponent<NetworkedGameManager>();
            gameManagerInstance.isLocal = false;
            gameManagerInstance.isServer = true;
            networkId = gameManagerInstance.networkId;

            server.playerGameManagers.Add(connection, gameManagerInstance);

            HandshakeResponseMessage response = new HandshakeResponseMessage()
            {
                message = $"Welcome {message.name}!",
                networkId = networkId,
            };

            server.SendNetworkMessageOne(connection, response);
        }
        else
        {
            Debug.Log("Could not spawn player");
        }

        // send all existing gamemanager to this manager

        // send creation of this gamemanager to all other game managers
    }

        private static void HandleClientStartGame(object recipient, NetworkConnection connection, NetworkMessage networkMessage)
    {
        ServerBehaviour server = recipient as ServerBehaviour;
        StartGameMessage message = networkMessage as StartGameMessage;

        Debug.Log("Received request to start game");
    }

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
    private static void HandleServerHandshakeResponse(object recipient, NetworkMessage networkMessage)
    {
        ClientBehaviour client = recipient as ClientBehaviour;
        HandshakeResponseMessage response = networkMessage as HandshakeResponseMessage;

        Debug.Log("Received handshake response");
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
