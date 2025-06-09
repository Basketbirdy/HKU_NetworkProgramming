using Unity.Collections;
using UnityEngine;

public class GameReadyToStartMessage : NetworkMessage
{
    public override NetworkMessageType Type => NetworkMessageType.GAME_READYTOSTART;

    public uint readyToStart;   // 0 = false, 1 = true

    public override void Encode(ref DataStreamWriter writer)
    {
        base.Encode(ref writer);

        writer.WriteUInt(readyToStart);
    }

    public override void Decode(ref DataStreamReader reader)
    {
        base.Decode(ref reader);

        readyToStart = reader.ReadUInt();
    }
}
