using Unity.Collections;
using UnityEngine;

public class RoundResultsMessage : NetworkMessage
{
    public override NetworkMessageType Type => NetworkMessageType.ROUND_RESULTS;

    public uint playerNumber;
    public uint winnerPlayerNumber;
    public uint newLifeCount;

    public override void Encode(ref DataStreamWriter writer)
    {
        base.Encode(ref writer);

        writer.WriteUInt(playerNumber);
        writer.WriteUInt(winnerPlayerNumber);
        writer.WriteUInt(newLifeCount);
    }

    public override void Decode(ref DataStreamReader reader)
    {
        base.Decode(ref reader);

        playerNumber = reader.ReadUInt();
        winnerPlayerNumber = reader.ReadUInt();
        newLifeCount = reader.ReadUInt();
    }
}
