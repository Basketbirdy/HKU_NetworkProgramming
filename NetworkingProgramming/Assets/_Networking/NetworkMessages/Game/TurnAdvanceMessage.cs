using Unity.Collections;
using UnityEngine;

public class TurnAdvanceMessage : NetworkMessage
{
    public override NetworkMessageType Type => NetworkMessageType.TURN_ADVANCE;

    public uint activePlayer;

    public override void Encode(ref DataStreamWriter writer)
    {
        base.Encode(ref writer);

        writer.WriteUInt(activePlayer);
    }

    public override void Decode(ref DataStreamReader reader)
    {
        base.Decode(ref reader);

        activePlayer = reader.ReadUInt();
    }
}
