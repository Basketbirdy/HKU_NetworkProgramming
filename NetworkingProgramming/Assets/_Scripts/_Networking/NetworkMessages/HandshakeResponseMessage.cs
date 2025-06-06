using Unity.Collections;
using UnityEngine;

public class HandshakeResponseMessage : NetworkMessage
{
    public override NetworkMessageType Type => NetworkMessageType.HANDSHAKE_RESPONSE;

    public string message = "";
    public uint networkId = 0;

    public override void Encode(ref DataStreamWriter writer)
    {
        base.Encode(ref writer);
        
        writer.WriteFixedString128(message);
        writer.WriteUInt(networkId);
    }

    public override void Decode(ref DataStreamReader reader)
    {
        base.Decode(ref reader);

        message = reader.ReadFixedString128().ToString();
        networkId = reader.ReadUInt();
    }
}
