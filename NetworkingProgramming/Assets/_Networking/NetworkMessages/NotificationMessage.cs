using Unity.Collections;
using UnityEngine;

public class NotificationMessage : NetworkMessage
{
    public override NetworkMessageType Type => NetworkMessageType.NOTIFICATION;

    public string source;
    public string message;

    public override void Encode(ref DataStreamWriter writer)
    {
        base.Encode(ref writer);

        writer.WriteFixedString128(source);
        writer.WriteFixedString128(message);
    }

    public override void Decode(ref DataStreamReader reader)
    {
        base.Decode(ref reader);

        source = reader.ReadFixedString128().ToString();
        message = reader.ReadFixedString128().ToString();
    }
}
