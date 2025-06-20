using Unity.Collections;
using UnityEngine;

public class DrawCardResponseMessage : NetworkMessage
{
    public override NetworkMessageType Type => NetworkMessageType.CARD_DRAW_RESPONSE;

    public uint cardTypeId;

    public override void Encode(ref DataStreamWriter writer)
    {
        base.Encode(ref writer);

        writer.WriteUInt(cardTypeId);
    }

    public override void Decode(ref DataStreamReader reader)
    {
        base.Decode(ref reader);

        cardTypeId = reader.ReadUInt();
    }
}
