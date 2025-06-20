using Unity.Collections;
using UnityEngine;

public class TurnEndMessage : NetworkMessage
{
    public override NetworkMessageType Type => NetworkMessageType.TURN_END;

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
