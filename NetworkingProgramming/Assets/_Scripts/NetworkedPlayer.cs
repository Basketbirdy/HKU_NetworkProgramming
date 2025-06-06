using UnityEngine;

public class NetworkedPlayer : NetworkedBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (isLocal)
        {
            // get client
        }

        if (isServer)
        {
            // get server
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isLocal)
        {
            // send input
        }

        if (isServer)
        {
            // process player behaviour
        }
    }
}
