using UnityEngine;
using Unity.Networking.Transport;
using System.Collections.Generic;

public class NetworkedGameManager : NetworkedBehaviour
{
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
        if (isLocal)
        {

        }

        if (isServer)
        {
            EventHandler<int>.AddListener(GlobalEvents.PLAYER_JOINED, OnPlayerJoined);
        }
    }

    private void OnDisable()
    {
        if (isLocal)
        {

        }

        if (isServer)
        {
            EventHandler<int>.RemoveListener(GlobalEvents.PLAYER_JOINED, OnPlayerJoined);
        }
    }

    protected override void Start()
    {
        base.Start();

        if (isLocal)
        {

        }

        if (isServer)
        {

        }
    }

    protected override void Update()
    {
        if (isLocal)
        {

        }

        if (isServer)
        {

        }
    }

    private void OnPlayerJoined(int count)
    {
        if (isLocal)
        {

        }

        if (isServer)
        {
            // display player joined notification
            if (count >= minimumPlayers)
            {
                // enable start game ui
            }
        }
    }
}
