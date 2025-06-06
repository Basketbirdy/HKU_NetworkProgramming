using UnityEngine;
using Unity.Networking.Transport;
using System.Collections.Generic;

public class NetworkedGameManager : NetworkedBehaviour
{
    public Dictionary<NetworkConnection, NetworkedPlayer> playerInstances = new Dictionary<NetworkConnection, NetworkedPlayer>();

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
}
