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
    [SerializeField] private int startCardCount = 2;
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
    private bool drawnCard;

    private bool hasPlayedCard;
    private Card playedCard;


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
            
            scoreData = new ScoreEntry();
            scoreData.player1Id = AccountManager.Instance.User_Id;
        }
    }
    private void OnEnable()
    {
        if (isClient)
        {
            client.AddMessageEventListener(OnPlayerJoinedMessage, OnGameStartedMessage, OnRoundStartMessage, OnDrawCardResponseMessage, OnCardPlayedResponseMessage, OnTurnAdvanceMessage);
        }

        if (isServer)
        {
            server.AddMessageEventListener(OnStartGameMessage, OnDrawCardMessage, OnCardPlayedMessage);
        }
    }
    private void OnDisable()
    {
        if (isClient)
        {
            client.RemoveMessageEventListener(OnPlayerJoinedMessage, OnGameStartedMessage, OnRoundStartMessage, OnDrawCardResponseMessage, OnCardPlayedResponseMessage, OnTurnAdvanceMessage);
        }

        if (isServer)
        {
            server.RemoveMessageEventListener(OnStartGameMessage, OnDrawCardMessage, OnCardPlayedMessage);
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
                if (Input.GetKeyDown(KeyCode.Mouse0) && drawnCard && !playedCard)
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

            // TODO - Play this card and send advance turn message
            CardPlayedMessage cardPlayedMessage = new CardPlayedMessage()
            {
                playerNumber = (uint)client.playerNumber,
                card = card.Data,
            };
            client.SendNetworkMessage(cardPlayedMessage);

            hasPlayedCard = true;
            playedCard = card;
            //hand.Remove(card.Data);
            //cardHolder.PlayCard(card.gameObject);

            //drawnCard = false;
            //UIManager.Instance.DisableButton("DrawButton");
        }
        return true;
    }
    private void OnCardPlayedMessage(NetworkMessage message)
    {
        // if network message is not of expected type, return and do nothing
        if (!TypeUtils.CompareType<CardPlayedMessage>(message.GetType())) { return; }
        var msg = message as CardPlayedMessage;

        int playerNumber = (int)msg.playerNumber;
        NetworkConnection clientConnection = GetConnectionByPlayerNumber(playerNumber);

        if (isServer)
        {
            uint _success;
            if(!playerHands.ContainsKey(playerNumber) || playerHands[playerNumber] == null)
            {
                _success = 0;
            }
            else
            {
                _success = 1;
            }

            var cardPlayedResponseMessage = new CardPlayedResponseMessage()
            {
                success = _success,
                activePlayer = (uint)activePlayer,
                card = msg.card,
            };

            server.SendNetworkMessageAll(cardPlayedResponseMessage);

            if(_success == 1)
            {
                cardsPlayed[playedCard.Data.type]++;

                // advance 
                OnAdvanceTurn();
            }
        }
    }
    private void OnAdvanceTurn()
    {
        Debug.Log("Advancing turn");

        int newActivePlayer = activePlayer + 1;
        if (newActivePlayer > playerNames.Count)
        {
            OnEndRound();
            return;
        }
        else { activePlayer = newActivePlayer; }

        TurnAdvanceMessage turnAdvanceMessage = new TurnAdvanceMessage()
        {
            activePlayer = (uint)activePlayer,
        };
        server.SendNetworkMessageAll(turnAdvanceMessage);
    }
    private void OnTurnAdvanceMessage(NetworkMessage message)
    {
        // if network message is not of expected type, return and do nothing
        if (!TypeUtils.CompareType<TurnAdvanceMessage>(message.GetType())) { return; }
        var msg = message as TurnAdvanceMessage;

        if (isClient)
        {
            myTurn = msg.activePlayer == client.playerNumber;

            if (myTurn) 
            {
                UIManager.Instance.GetUIControllerAs<GameUIController>("GameUIController").SetStateIndicatorDrawCardText(); 
                UIManager.Instance.EnableButton("DrawButton");
            }
            else
            {
                UIManager.Instance.DisableButton("DrawButton");
            }
        }

        if (isServer)
        {

        }
    }
    private void OnEndRound()
    {
        Debug.Log("Ending round");
    }
    private void OnCardPlayedResponseMessage(NetworkMessage message)
    {
        // if network message is not of expected type, return and do nothing
        if (!TypeUtils.CompareType<CardPlayedResponseMessage>(message.GetType())) { return; }
        var msg = message as CardPlayedResponseMessage;

        if(isClient)
        {
            myTurn = msg.activePlayer == client.playerNumber;
            bool success = msg.success == 0 ? false : true;

            if (myTurn)
            {

                if (success)
                {
                    hand.Remove(playedCard.Data);
                    cardHolder.PlayCard(playedCard);

                    drawnCard = false;
                    UIManager.Instance.DisableButton("DrawButton");
                }
                else
                {
                    hasPlayedCard = false;
                }
            }
            else
            {
                if (success)
                {
                    var cardObj = cardFactory.CreateCard(msg.card);
                    cardHolder.ShowTheirCard(cardObj.GetComponent<Card>());
                    cardObj.transform.rotation = Quaternion.Euler(0, 180, 0);
                }
            }
        }
    }

    public void OnDrawCardPressed()
    {
        // send draw card request message to server
        // await response message with cardSO
        if(drawnCard) { return; }

        Debug.Log("Drawing card");
        DrawCardMessage drawCardMessage = new DrawCardMessage()
        {
            playerNumber = (uint)client.playerNumber,
        };

        client.SendNetworkMessage(drawCardMessage);
    }
    private void OnDrawCardMessage(NetworkMessage message)
    {
        // if network message is not of expected type, return and do nothing
        if (!TypeUtils.CompareType<DrawCardMessage>(message.GetType())) { return; }
        var msg = message as DrawCardMessage;

        if (isServer)
        {
            int playerNumber = (int)msg.playerNumber;
            playerDecks[playerNumber].Draw(out CardSO card);
            playerDiscards[playerNumber].Add(card);

            DrawCardResponseMessage drawCardResponseMessage = new DrawCardResponseMessage()
            {
                playerNumber = (uint)msg.playerNumber,
                cardTypeId = (uint)card.type,
            };

            NetworkConnection clientConnection = GetConnectionByPlayerNumber(playerNumber);
            server.SendNetworkMessageOne(clientConnection, drawCardResponseMessage);
        }
    }
    private void OnDrawCardResponseMessage(NetworkMessage message)
    {
        // if network message is not of expected type, return and do nothing
        if (!TypeUtils.CompareType<DrawCardResponseMessage>(message.GetType())) { return; }
        var msg = message as DrawCardResponseMessage;

        if (isClient)
        {
            // add card to holder
            var card = NetworkManager.Instance.GetCard((int)msg.cardTypeId);
            hand.Add(card);
            cardHolder.AddCard(cardFactory.CreateCard(card));

            drawnCard = true;

            UIManager.Instance.DisableButton("DrawButton");
            UIManager.Instance.GetUIControllerAs<GameUIController>("GameUIController").SetStateIndicatorWaitingText(myTurn);
        }
    }

    private void OnPlayerJoinedMessage(NetworkMessage message)
    {
        Debug.Log($"Received Network message in GameManager!, {message.GetType()}");

        // if network message is not of expected type, return and do nothing
        if(!TypeUtils.CompareType<PlayerJoinedMessage>(message.GetType())) { return; }
        PlayerJoinedMessage msg = message as PlayerJoinedMessage;

        if (isClient)
        {
            scoreData.player2Id = (int)msg.userId;
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
            playerDiscards.Add(playerNumber, new CardStack(new List<CardSO>()));

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
            hand = msg.hand;
            
            Debug.Log($"[Client] received game started message! hand: {msg.hand.stack[0]}, {msg.hand.stack[1]}");
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

            if (myTurn) { UIManager.Instance.GetUIControllerAs<GameUIController>("GameUIController").SetStateIndicatorDrawCardText(); }
            else
            {
                UIManager.Instance.DisableButton("DrawButton");
            }
        }

        if (isServer)
        {
            
        }
    }

    public NetworkConnection GetConnectionByPlayerNumber(int playerNumber)
    {
        return server.playerNumbers.Single(s => s.Value == playerNumber).Key;
    }
}