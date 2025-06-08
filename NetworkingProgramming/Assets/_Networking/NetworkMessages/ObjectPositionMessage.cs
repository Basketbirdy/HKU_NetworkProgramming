using Unity.Collections;
using UnityEngine;

public class ObjectPositionMessage : NetworkMessage
{
    public override NetworkMessageType Type => NetworkMessageType.OBJECT_POSITION;

    public uint objectId;
    public Vector3 position;


    public override void Encode(ref DataStreamWriter writer)
    {
        base.Encode(ref writer);    // encode mandatory data (message type, id)

        writer.WriteUInt(objectId);

        writer.WriteFloat(position.x);
        writer.WriteFloat(position.y);
        writer.WriteFloat(position.z);
    }

    public override void Decode(ref DataStreamReader reader)
    {
        base.Decode(ref reader);        // decode mandatory data (message type, id)

        objectId = reader.ReadUInt();   // whos position is this?

        float x = reader.ReadFloat();
        float y = reader.ReadFloat();
        float z = reader.ReadFloat();

        position = new Vector3(x, y, z);
    }
}
