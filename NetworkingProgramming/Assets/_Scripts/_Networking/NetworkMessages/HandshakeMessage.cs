using Unity.Collections;
using UnityEngine;

public class HandshakeMessage : NetworkMessage
{
    public override NetworkMessageType Type => NetworkMessageType.HANDSHAKE;

    public string nickname;

    public override void Encode(ref DataStreamWriter writer)
    {
        base.Encode(ref writer);

        writer.WriteFixedString128(nickname);
    }

    public override void Decode(ref DataStreamReader reader)
    {
        base.Decode(ref reader);

        reader.ReadFixedString128();
    }
}
