using Unity.Collections;
using UnityEngine;

public class GameStartedMessage : NetworkMessage
{
    public override NetworkMessageType Type => NetworkMessageType.GAME_STARTED;

    public CardStack hand;

    public override void Encode(ref DataStreamWriter writer)
    {
        base.Encode(ref writer);

        WriteCardStack(ref writer, hand);
    }

    public override void Decode(ref DataStreamReader reader)
    {
        base.Decode(ref reader);

        hand = ReadCardStack(ref reader);
    }
}
