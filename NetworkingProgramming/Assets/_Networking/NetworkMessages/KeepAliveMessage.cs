using Unity.Collections;
using UnityEngine;

public class KeepAliveMessage : NetworkMessage
{
    public override NetworkMessageType Type => NetworkMessageType.KEEPALIVE;

    public override void Encode(ref DataStreamWriter writer)
    {
        base.Encode(ref writer);
    }

    public override void Decode(ref DataStreamReader reader)
    {
        base.Decode(ref reader);
    }
}
