using Unity.VisualScripting;
using UnityEngine;

public class NetworkedPlayer : NetworkedBehaviour
{
    private Deck deck;
    private CardStack discardPile;
    private CardStack hand;

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

    private void Update()
    {
        if (isLocal)
        {

        }

        if (isServer)
        {
            
        }
    }
}
