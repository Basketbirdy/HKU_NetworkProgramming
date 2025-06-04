using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class NetworkedBehaviour : MonoBehaviour
{
    public uint networkId = 0;

    public bool isLocal;
    public bool isServer;

    protected ServerBehaviour server;
    protected ClientBehaviour client;

    protected virtual void Start()
    {
        if (isLocal)
        {
            client = FindFirstObjectByType<ClientBehaviour>();
        }

        if (isServer)
        {
            server = FindFirstObjectByType<ServerBehaviour>();
        }
    } 
}
