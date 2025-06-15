using UnityEngine;
using Unity.Networking.Transport;
using System.Collections.Generic;

public enum GameState { LOBBY, PLAYING }
public enum SubGameState { ACTIVE, INACTIVE }
public class GameManager : MonoBehaviour
{
    // NETWORKING

    [SerializeField] private bool isClient;
    [SerializeField] private bool isServer;

    private ClientBehaviour client;
    private ServerBehaviour server;

    // GAME

    private Dictionary<int, string> playerList = new Dictionary<int, string>();

    [Header("State")]
    [SerializeField] private GameState gameState = GameState.LOBBY;
    [SerializeField] private SubGameState subGameState = SubGameState.INACTIVE;

    [Header("Game settings")]
    [SerializeField] private int minimumPlayers = 2;
    [SerializeField] private DeckSO defaultDeck;

    [Header("Game data")]
    [SerializeField] private Deck player1;
    [SerializeField] private Deck player2;

    [SerializeField] private CardStack hand1;
    [SerializeField] private CardStack hand2;

    private void OnEnable()
    {
        if (isClient)
        {
            client.AddMessageEventListener(OnPlayerJoinedMessage);
        }

        if (isServer)
        {
            server.AddMessageEventListener();
        }
    }

    private void OnDisable()
    {
        if (isClient)
        {
            client.RemoveMessageEventListener(OnPlayerJoinedMessage);
        }

        if (isServer)
        {
            server.RemoveMessageEventListener();
        }
    }

    private void Awake()
    {
        if (isClient)
        {
            client = FindAnyObjectByType<ClientBehaviour>();
        }

        if (isServer)
        {
            server = FindAnyObjectByType<ServerBehaviour>();
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnStartGame()
    {
        Debug.Log($"Starting game!");

        // setup game

        // send message to all connected clients to start the game
    }

    private void OnPlayerJoinedMessage(NetworkMessage message)
    {
        Debug.Log($"Received Network message in GameManager!, {message.GetType()}");

        // if network message is not of expected type, return and do nothing
        if(!TypeUtils.CompareType<PlayerJoinedMessage>(message.GetType())) { return; }
        PlayerJoinedMessage msg = message as PlayerJoinedMessage;

        if (isClient)
        {
            UpdatePlayerList(msg.playerNames);
            
            var notificationController = UIManager.Instance.GetUIControllerAs<NotificationUIController>("NotificationUIController");
            notificationController.SendNotification("Server", $"{msg.name} joined as player {msg.playerNumber}" );
        }

        if (isServer)
        {
            if(server.playerNames.Count >= minimumPlayers)
            {
                // send out notification
                var notificationMessage = new NotificationMessage()
                {
                    source = "Server",
                    message = "Enough players present to start the game! waiting for host...",
                };

                server.SendNetworkMessageAll(notificationMessage);

                // enable start game ui
                UIManager.Instance.EnableButton("StartGameButton");
            }
        }
    }

    private void UpdatePlayerList(List<string> list)
    {
        // update player list with player name
        playerList.Clear();
        for (int i = 0; i < list.Count; i++)
        {
            playerList.Add(i + 1, list[i]);
            UIManager.Instance.SetText($"player{i + 1}Text", list[i]);
        }
    }
}
