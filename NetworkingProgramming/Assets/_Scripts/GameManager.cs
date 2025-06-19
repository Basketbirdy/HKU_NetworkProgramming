using UnityEngine;
using Unity.Networking.Transport;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    // NETWORKING

    [SerializeField] private bool isClient;
    [SerializeField] private bool isServer;

    private ClientBehaviour client;
    private ServerBehaviour server;

    // GAME

    [Header("Components")]
    [SerializeField] private CardHolder cardHolder;
    private CardFactory cardFactory;

    [Header("Game settings")]
    [SerializeField] private int minimumPlayers = 2;
    [SerializeField] private DeckSO defaultDeck;
    [Space]
    [SerializeField] private int startCardCount = 3;
    [SerializeField] private int startLifeCount = 3;

    [Header("Match data")]
    [SerializeField] private int activePlayer;
    private int currentRound;
    public ScoreEntry scoreData;

    [Header("Match stats")]
    [SerializeField] private int roundsPlayed;
    [SerializeField] private int roundsTied;
    private Dictionary<CardType, int> cardsPlayed = new Dictionary<CardType, int>();

    [Header("Player data")]
    private Dictionary<int, string> playerNames = new Dictionary<int, string>();
    // server data
    private Dictionary<int, Deck> playerDecks = new Dictionary<int, Deck>();
    private Dictionary<int, CardStack> playerHands = new Dictionary<int, CardStack>();                   
    private Dictionary<int, CardStack> playerDiscards = new Dictionary<int, CardStack>();                   
    private Dictionary<int, int> playerLives = new Dictionary<int, int>();

    [Header("Client game variables")]
    [SerializeField] LayerMask cardMask;

    private CardStack hand;
    private bool myTurn;


    private void Awake()
    {
        if (isClient)
        {
            client = FindAnyObjectByType<ClientBehaviour>();
            cardFactory = GetComponent<CardFactory>();
        }

        if (isServer)
        {
            server = FindAnyObjectByType<ServerBehaviour>();
        }
    }
    private void OnEnable()
    {
        if (isClient)
        {
            client.AddMessageEventListener(OnPlayerJoinedMessage, OnGameStartedMessage, OnRoundStartMessage);
        }

        if (isServer)
        {
            server.AddMessageEventListener(OnStartGameMessage);
        }
    }
    private void OnDisable()
    {
        if (isClient)
        {
            client.RemoveMessageEventListener(OnPlayerJoinedMessage, OnGameStartedMessage, OnRoundStartMessage);
        }

        if (isServer)
        {
            server.RemoveMessageEventListener(OnStartGameMessage);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (isClient)
        {
        }

        if (isServer)
        {
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isClient)
        {
            if (myTurn)
            {
                // await input :)
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    onClick(out RaycastHit hit);
                }
            }
        }

        if(isServer)
        {

        }
    }
    private bool onClick(out RaycastHit hit)
    {
        Debug.Log("Shooting ray! pew pew");;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, cardMask))
        {
            var card = hit.collider.GetComponent<Card>();
            if (card == null) { return false; }
            Debug.Log($"hit card {card.Data.id}");
        }
        return true;
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
        playerNames.Clear();
        for (int i = 0; i < list.Count; i++)
        {
            playerNames.Add(i + 1, list[i]);
            UIManager.Instance.SetText($"player{i + 1}Text", list[i]);
        }
    }

    public void OnStartGamePressed()
    {
        Debug.Log($"Start game button pressed!");

        // send start game message to the server to start the game
        StartGameMessage startGameMessage = new StartGameMessage()
        {
            
        };

        client.SendNetworkMessage(startGameMessage);
    }
    public void OnStartGameMessage(NetworkMessage message)
    {
        // if network message is not of expected type, return and do nothing
        if (!TypeUtils.CompareType<StartGameMessage>(message.GetType())) { return; }
        var msg = message as StartGameMessage;

        // do nothing if client, since only server can receive this message
        if (isClient)
        {
            
        }

        if(isServer)
        {
            Debug.Log("Setting up game!");
            GameSetup();
        }
    }
    private void GameSetup()
    {
        // setup each player
        foreach(KeyValuePair<int, string> player in playerNames)
        {
            int playerNumber = player.Key;

            // fill player deck
            playerDecks.Add(playerNumber, new Deck(defaultDeck.GetCards()));

            // shuffle deck
            playerDecks[playerNumber].Shuffle();

            // fill player hand
            List<CardSO> newHand = new List<CardSO>();
            for (int i = 0; i < startCardCount; i++)
            {
                playerDecks[playerNumber].Draw(out CardSO card);
                newHand.Add(card);
            }
            playerHands.Add(playerNumber, new CardStack(newHand));

            // set player discard pile
            playerDiscards.Add(playerNumber, new CardStack(null));

            // set players lives
            playerLives.Add(playerNumber, startLifeCount);
        }

        // setup game
        activePlayer = 1;

        // setup stats
        roundsPlayed = 0;
        roundsTied = 0;
        cardsPlayed.Add(CardType.ROCK, 0);
        cardsPlayed.Add(CardType.PAPER, 0);
        cardsPlayed.Add(CardType.SCISSORS, 0);

        // send all clients their hand
        foreach(NetworkConnection connection in server.connections)
        {
            if (!server.playerNumbers.ContainsKey(connection)) { Debug.LogWarning($"Found connection outside of game!"); continue; }
            int playerNumber = server.playerNumbers[connection];

            // create game started message
            GameStartedMessage gameStartedMessage = new GameStartedMessage()
            {
                hand = playerHands[playerNumber],
            };
            server.SendNetworkMessageOne(connection, gameStartedMessage);

        }

        NotificationMessage notifMessage = new NotificationMessage()
        {
            source = "[Server]",
            message = "Game started!",
        };
        server.SendNetworkMessageAll(notifMessage);

        // notify all clients of round start
        RoundStartMessage roundStartMessage = new RoundStartMessage()
        {
            activePlayer = (uint)activePlayer,
            roundNumber = (uint)roundsPlayed,
        };
        server.SendNetworkMessageAll(roundStartMessage);
    }

    public void OnGameStartedMessage(NetworkMessage message)
    {
        // if network message is not of expected type, return and do nothing
        if (!TypeUtils.CompareType<GameStartedMessage>(message.GetType())) { return; }
        var msg = message as GameStartedMessage;

        if (isClient)
        {
            Debug.Log($"[Client] received game started message! hand: {msg.hand.stack[0]}, {msg.hand.stack[1]}, {msg.hand.stack[2]}");
            // disable lobby ui
            UIManager.Instance.GetUIControllerAs<LobbyUIController>("LobbyUIController").HideCanvas();
            // enable game ui
            UIManager.Instance.GetUIControllerAs<GameUIController>("GameUIController").ShowCanvas();

            // spawn card objects
            foreach(CardSO card in msg.hand.stack)
            {
                cardHolder.AddCard(cardFactory.CreateCard(card));
            }
        }

        if (isServer)
        {

        }
    }

    public void OnRoundStartMessage(NetworkMessage message)
    {
        // if network message is not of expected type, return and do nothing
        if (!TypeUtils.CompareType<RoundStartMessage>(message.GetType())) { return; }
        var msg = message as RoundStartMessage;

        Debug.Log("[Client] Received round start message");

        if(isClient)
        {
            currentRound = (int)msg.roundNumber;
            myTurn = msg.activePlayer == client.playerNumber;

            UIManager.Instance.GetUIControllerAs<GameUIController>("GameUIController").SetStateIndicatorText(myTurn);
        }

        if (isServer)
        {
            
        }
    }
}