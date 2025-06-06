using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class NetworkedBehaviour : MonoBehaviour
{
    [Header("Networking")]
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

    protected virtual void Update()
    {
        if(isLocal)
        {

        }

        if(isServer)
        {

        }
    }
}
