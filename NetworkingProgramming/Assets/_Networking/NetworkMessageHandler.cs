using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.EventSystems;

public delegate void ServerNetworkMessage(ServerBehaviour server, NetworkConnection connection, NetworkMessage msg);
public delegate void ClientNetworkMessage(ClientBehaviour client, NetworkMessage msg);

public enum NetworkMessageType
{
    // general 
    REMOTE_PROCEDURE_CALL,
    KEEPALIVE,

    // general actions
    SPAWNMESSAGE,
    OBJECT_POSITION,
    NOTIFICATION,

    // specific
    HANDSHAKE,
    HANDSHAKE_RESPONSE,
    PLAYER_JOINED,

    GAME_READYTOSTART,
    GAME_START,
}

public static class NetworkMessageInfo
{
    public static Dictionary<NetworkMessageType, System.Type> typeMap = new Dictionary<NetworkMessageType, System.Type>
    {
        {NetworkMessageType.REMOTE_PROCEDURE_CALL, typeof(RPCMessage) },
        {NetworkMessageType.KEEPALIVE, typeof(KeepAliveMessage) },

        {NetworkMessageType.SPAWNMESSAGE, typeof(SpawnMessage) },
        {NetworkMessageType.OBJECT_POSITION, typeof(ObjectPositionMessage) },
        {NetworkMessageType.NOTIFICATION, typeof(NotificationMessage) },

        {NetworkMessageType.HANDSHAKE, typeof(HandshakeMessage) },
        {NetworkMessageType.HANDSHAKE_RESPONSE, typeof(HandshakeResponseMessage) },
        {NetworkMessageType.PLAYER_JOINED, typeof(PlayerJoinedMessage) }
    };
}

public static class NetworkMessageHandler
{
    /// <summary>
    /// messages received by the CLIENTS, sent by the server
    /// </summary>
    public static Dictionary<NetworkMessageType, ClientNetworkMessage> serverMessageHandlers = new Dictionary<NetworkMessageType, ClientNetworkMessage>
    {
        {NetworkMessageType.KEEPALIVE, HandleServerKeepAlive },

        {NetworkMessageType.SPAWNMESSAGE, HandleServerSpawnMessage },
        {NetworkMessageType.OBJECT_POSITION, HandleServerObjectPosition },
        {NetworkMessageType.NOTIFICATION, HandleServerNotification },

        {NetworkMessageType.HANDSHAKE_RESPONSE, HandleServerHandshakeResponse },

        {NetworkMessageType.PLAYER_JOINED, HandleServerPlayerJoined },
    };

    /// <summary>
    /// messages received by the SERVER, sent by the clients
    /// </summary>
    public static Dictionary<NetworkMessageType, ServerNetworkMessage> clientMessageHandlers = new Dictionary<NetworkMessageType, ServerNetworkMessage>
    {

        {NetworkMessageType.SPAWNMESSAGE, HandleClientSpawnMessage },
        {NetworkMessageType.OBJECT_POSITION, HandleClientObjectPosition},

        {NetworkMessageType.GAME_START, HandleClientStartGame },
        {NetworkMessageType.HANDSHAKE, HandleClientHandshake }
    };

    // messages received by the CLIENT, sent by the server
    #region Server Messages
    private static void HandleServerKeepAlive(object recipient, NetworkMessage networkMessage)
    {
        ClientBehaviour client = recipient as ClientBehaviour;
        KeepAliveMessage message = networkMessage as KeepAliveMessage;

        Debug.Log("Received keep alive message");
    }
    private static void HandleServerHandshakeResponse(object recipient, NetworkMessage networkMessage)
    {
        ClientBehaviour client = recipient as ClientBehaviour;
        HandshakeResponseMessage response = networkMessage as HandshakeResponseMessage;

        Debug.Log($"Received handshake response: {response.message}");

        if(response.message == "0")
        {
            // TODO - How do I properly disconnect a client manually?
            client.ShutDown();
            return;
        }

        client.playerNumber = (int)response.playerNumber;
    }
    private static void HandleServerPlayerJoined(object recipient, NetworkMessage networkMessage)
    {
        ClientBehaviour client = recipient as ClientBehaviour;
        var message = networkMessage as PlayerJoinedMessage;

        Debug.Log($"Received player joined message. {message.name}");
    }
    private static void HandleServerSpawnMessage(object recipient, NetworkMessage networkMessage)
    {
        ClientBehaviour client = recipient as ClientBehaviour;
        SpawnMessage message = networkMessage as SpawnMessage;

        Debug.Log($"Received message to spawn an object: {message.objectType}, with id {message.networkId}");

        GameObject obj;
        if (!NetworkManager.Instance.Create(message.objectType, message.networkId, out obj))
        {
            Debug.Log($"Could not spawn {message.objectType}, with id {message.networkId}");
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
    private static void HandleServerNotification(object recipient, NetworkMessage networkMessage)
    {
        ClientBehaviour client = recipient as ClientBehaviour;
        var message = networkMessage as NotificationMessage;

        var notificationUIController = UIManager.Instance.GetUIControllerAs<NotificationUIController>("NotificationUIController");
        notificationUIController.SendNotification(message.source, message.message);
    }
    #endregion

    // messages received by the SERVER, sent by the clients
    #region Client Messages
    private static void HandleClientHandshake(object recipient, NetworkConnection connection, NetworkMessage networkMessage) // handshake is client trying to join the server
    {
        ServerBehaviour server = recipient as ServerBehaviour;
        HandshakeMessage message = networkMessage as HandshakeMessage;

        Debug.Log($"Received handshake request");

        // check if the connection is allowed to join
        if (!server.CanJoin())
        {
            Debug.Log("New connection could not join");

            HandshakeResponseMessage response = new HandshakeResponseMessage()
            {
                message = "0", // let the connection know it has been denied
            };

            server.SendNetworkMessageOne(connection, response);

            // disconnect
            server.Disconnect(connection);

            return;
        }

        server.playerNames.Add(connection, message.name);

        // spawn server player
        GameObject player = null;
        uint networkId = 0;
        int playerNumber = 0;
        if (NetworkManager.Instance.Create(NetworkObjectType.PLAYER, NetworkManager.NextNetworkId, out player))
        {
            NetworkedPlayer playerInstance = player.GetComponent<NetworkedPlayer>();
            playerInstance.isLocal = false;
            playerInstance.isServer = true;
            playerInstance.nickname = message.name;
            playerInstance.playerNumber = server.playerNames.Count;

            networkId = playerInstance.networkId;
            playerNumber = playerInstance.playerNumber;

            server.playerInstances.Add(connection, playerInstance);

            HandshakeResponseMessage response = new HandshakeResponseMessage()
            {
                message = $"Welcome {message.name}!",
                networkId = networkId,

                playerNumber = (uint)playerNumber,
            };

            server.SendNetworkMessageOne(connection, response);
        }
        else
        {
            Debug.Log("Could not spawn player");
        }

        // send all existing players to this player
        foreach (KeyValuePair<NetworkConnection, NetworkedPlayer> pair in server.playerInstances)
        {
            if (pair.Key == connection) { continue; }

            SpawnMessage spawnMessage = new SpawnMessage()
            {
                networkId = pair.Value.networkId,
                objectType = NetworkObjectType.PLAYER,
            };

            server.SendNetworkMessageOne(connection, spawnMessage);
        }

        // send creation of this player to all other players
        if (networkId != 0)
        {
            SpawnMessage spawnMessage = new SpawnMessage()
            {
                networkId = networkId,
                objectType = NetworkObjectType.PLAYER,
            };

            server.SendNetworkMessageAll(spawnMessage);

            List<string> playerNames = new List<string>();
            foreach (KeyValuePair<NetworkConnection, string> playerPair in server.playerNames)
            {
                playerNames.Add(playerPair.Value);
            }

            // send player joined message to all
            PlayerJoinedMessage playerJoinedMessage = new PlayerJoinedMessage()
            {
                name = message.name,
                playerNumber = playerNumber,

                playerNames = playerNames,
            };

            server.SendNetworkMessageAll(playerJoinedMessage);
        }
    }
    private static void HandleClientStartGame(object recipient, NetworkConnection connection, NetworkMessage networkMessage)
    {
        ServerBehaviour server = recipient as ServerBehaviour;
        StartGameMessage message = networkMessage as StartGameMessage;

        Debug.Log("Received request to start game");
    }
    private static void HandleClientSpawnMessage(object recipient, NetworkConnection connection, NetworkMessage networkMessage)
    {
        ServerBehaviour server = recipient as ServerBehaviour;
        SpawnMessage message = networkMessage as SpawnMessage;

        Debug.Log($"Received message to spawn an object: {message.objectType}, with id {message.networkId}");

        GameObject obj;
        if (!NetworkManager.Instance.Create(message.objectType, message.networkId, out obj))
        {
            Debug.Log($"Could not spawn {message.objectType}, with id {message.networkId}");
        }
    }
    private static void HandleClientObjectPosition(object recipient, NetworkConnection connection, NetworkMessage networkMessage) // handle object position message sent by client, received by server
    {
        // getting data
        ServerBehaviour server = recipient as ServerBehaviour;
        ObjectPositionMessage message = networkMessage as ObjectPositionMessage;

        // handling data
        GameObject obj;
        NetworkManager.Instance.Get(message.objectId, out obj);
        if (obj != null)
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
    #endregion

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
