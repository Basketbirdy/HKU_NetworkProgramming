using Unity.Collections;
using UnityEngine;

public class DrawCardMessage : NetworkMessage
{
    public override NetworkMessageType Type => NetworkMessageType.CARD_DRAW;

    public uint playerNumber;

    public override void Encode(ref DataStreamWriter writer)
    {
        base.Encode(ref writer);

        writer.WriteUInt(playerNumber);
    }
    public override void Decode(ref DataStreamReader reader)
    {
        base.Decode(ref reader);

        playerNumber = reader.ReadUInt();
    }
}
