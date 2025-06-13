using UnityEngine;
using Unity.Networking.Transport;

public enum GameState { LOBBY, PLAYING }
public enum SubGameState { ACTIVE, INACTIVE }
public class GameManager : MonoBehaviour
{
    [SerializeField] private bool isClient;
    [SerializeField] private bool isServer;

    private ServerBehaviour server;
    private ClientBehaviour client;

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
        EventHandler<NetworkMessage>.AddListener(GlobalEvents.MESSAGE_RECEIVED, OnPlayerJoined);

        if (isClient)
        {

        }

        if (isServer)
        {
            

        }
    }

    private void OnDisable()
    {
        EventHandler<NetworkMessage>.RemoveListener(GlobalEvents.MESSAGE_RECEIVED, OnPlayerJoined);

        if (isClient)
        {
            
        }

        if (isServer)
        {
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
            client = FindAnyObjectByType<ClientBehaviour>();
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
        // send message to all connected clients to start the game
    }

    private void OnPlayerJoined(NetworkMessage message)
    {
        Debug.Log($"Received Network message in GameManager!, {message.GetType()}");

        // if network message is not of expected type, return and do nothing
        if(message.GetType() != typeof(PlayerJoinedMessage)) { return; }
        PlayerJoinedMessage msg = message as PlayerJoinedMessage;

        if (isClient)
        {
            // update player list with player name
            Debug.Log($"updating list: {msg.playerNumber} with {msg.name}");
            UIManager.Instance.SetText($"player{msg.playerNumber}Text", msg.name);
        }

        if (isServer)
        {
            if(server.playerNames.Count >= minimumPlayers)
            {
                // enable start game button
            }
        }
    }

    private void StartGame()
    {

    }
}
