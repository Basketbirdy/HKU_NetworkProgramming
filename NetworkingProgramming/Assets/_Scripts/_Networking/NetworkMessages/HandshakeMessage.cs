using Unity.Collections;
using UnityEngine;

public class HandshakeMessage : NetworkMessage
{
    public override NetworkMessageType Type => NetworkMessageType.HANDSHAKE;


    public string name = "";
    public uint networkId;

    public override void Encode(ref DataStreamWriter writer)
    {
        base.Encode(ref writer);


        writer.WriteFixedString128(name);
        writer.WriteUInt(networkId);
    }

    public override void Decode(ref DataStreamReader reader)
    {
        base.Decode(ref reader);

        name = reader.ReadFixedString128().ToString();
        networkId = reader.ReadUInt();
    }
}
