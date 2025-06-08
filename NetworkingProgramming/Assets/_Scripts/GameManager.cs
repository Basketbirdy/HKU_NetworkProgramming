using UnityEngine;

public enum GameState { LOBBY, PLAYING }
public enum SubGameState { ACTIVE, INACTIVE }
public class GameManager : MonoBehaviour
{
    private ServerBehaviour server;

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
        EventHandler<int>.AddListener(GlobalEvents.PLAYER_JOINED, OnPlayerJoined);   
    }

    private void OnDisable()
    {
        EventHandler<int>.RemoveListener(GlobalEvents.PLAYER_JOINED, OnPlayerJoined);   
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

    private void OnPlayerJoined(int count)
    {
        // display player joined notification
        if(count >= minimumPlayers)
        {
            // enable start game ui
        }
    }
}
