using Unity.Collections;
using UnityEngine;

public class HandshakeResponseMessage : NetworkMessage
{
    public override NetworkMessageType Type => NetworkMessageType.HANDSHAKE_RESPONSE;

    public uint networkId;

    public override void Encode(ref DataStreamWriter writer)
    {
        base.Encode(ref writer);

        writer.WriteUInt(networkId);
    }

    public override void Decode(ref DataStreamReader reader)
    {
        base.Decode(ref reader);

        networkId = reader.ReadUInt();
    }
}
