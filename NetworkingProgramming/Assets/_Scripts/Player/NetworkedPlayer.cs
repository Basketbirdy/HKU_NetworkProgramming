using Unity.VisualScripting;
using UnityEngine;

public class NetworkedPlayer : NetworkedBehaviour
{
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

    protected void Update()
    {
        base.Update();

        if (isLocal)
        {

        }

        if (isServer)
        {
            
        }
    }
}
