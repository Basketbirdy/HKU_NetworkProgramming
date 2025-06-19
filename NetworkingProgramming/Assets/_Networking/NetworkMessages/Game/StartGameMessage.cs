using Unity.Collections;
using UnityEngine;

public class StartGameMessage : NetworkMessage
{
    public override NetworkMessageType Type => NetworkMessageType.GAME_START;

    public override void Encode(ref DataStreamWriter writer)
    {
        base.Encode(ref writer);
        
    }

    public override void Decode(ref DataStreamReader reader)
    {
        base.Decode(ref reader);

    }
}
