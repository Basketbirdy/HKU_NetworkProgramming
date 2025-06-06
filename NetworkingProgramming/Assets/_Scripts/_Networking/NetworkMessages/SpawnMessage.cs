using Unity.Collections;
using UnityEngine;

public class SpawnMessage : NetworkMessage
{
    public override NetworkMessageType Type => NetworkMessageType.SPAWNMESSAGE;

    public uint networkId;
    public NetworkObjectType objectType;

    public override void Encode(ref DataStreamWriter writer)
    {
        base.Encode(ref writer);

        writer.WriteUInt(networkId);
        writer.WriteUInt((uint)objectType);
    }

    public override void Decode(ref DataStreamReader reader)
    {
        base.Decode(ref reader);

        networkId = reader.ReadUInt();
        objectType = (NetworkObjectType)reader.ReadUInt();
    }
}
