using Unity.Collections;
using UnityEngine;

public class RoundStartMessage : NetworkMessage
{
    public override NetworkMessageType Type => NetworkMessageType.ROUND_START;

    public uint activePlayer;
    public uint roundNumber;

    public override void Encode(ref DataStreamWriter writer)
    {
        base.Encode(ref writer);

        writer.WriteUInt(activePlayer);
        writer.WriteUInt(roundNumber);
    }
    public override void Decode(ref DataStreamReader reader)
    {
        base.Decode(ref reader);

        activePlayer = reader.ReadUInt();
        roundNumber = reader.ReadUInt();
    }
}
