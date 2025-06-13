using Unity.Collections;
using UnityEngine;

public class PlayerJoinedMessage : NetworkMessage
{
    public override NetworkMessageType Type => NetworkMessageType.PLAYER_JOINED;

    public string name;

    public override void Encode(ref DataStreamWriter writer)
    {
        base.Encode(ref writer);

        writer.WriteFixedString128(name);
    }

    public override void Decode(ref DataStreamReader reader)
    {
        base.Decode(ref reader);

        name = reader.ReadFixedString128().ToString();
    }
}
