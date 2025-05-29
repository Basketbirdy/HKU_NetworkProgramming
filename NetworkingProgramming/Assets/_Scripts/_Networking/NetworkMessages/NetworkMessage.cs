using Unity.Collections;
using UnityEngine;

public abstract class NetworkMessage : MonoBehaviour
{
    private static uint nextId = 0;
    public static uint NextId = nextId++;

    public abstract NetworkMessageType Type { get; }
    public uint Id { get; private set; } = NextId;

    public virtual void Encode(ref DataStreamWriter writer)
    {
        writer.WriteUInt((uint)Type);   // always start with what kind of message
        writer.WriteUInt(Id);           // send message id
    }
    public virtual void Decode(ref DataStreamReader reader)
    {
        // reads message type outside of decode function
        Id = reader.ReadUInt();
    }
}
