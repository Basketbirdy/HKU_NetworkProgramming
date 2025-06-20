using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public abstract class NetworkMessage
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

    public void WriteVector3(ref DataStreamWriter writer, Vector3 vector)
    {
        writer.WriteUInt((uint)vector.x);
        writer.WriteUInt((uint)vector.y);
        writer.WriteUInt((uint)vector.z);
    }
    public Vector3 ReadVector3(ref DataStreamReader reader) 
    {
        Vector3 v;
        v.x = reader.ReadUInt();
        v.y = reader.ReadUInt();
        v.z = reader.ReadUInt();
        return v;
    }

    public void WriteColor(ref DataStreamWriter writer, Color color)
    {
        writer.WriteUInt((uint)color.r);
        writer.WriteUInt((uint)color.g);
        writer.WriteUInt((uint)color.b);
        writer.WriteUInt((uint)color.a);
    }
    public Color ReadColor(ref DataStreamReader reader) 
    {
        Color c;
        c.r = reader.ReadUInt();
        c.g = reader.ReadUInt();
        c.b = reader.ReadUInt();
        c.a = reader.ReadUInt();
        return c;
    }

    // class functions
    public void WriteCardStack(ref DataStreamWriter writer, CardStack cardStack)
    {
        writer.WriteUInt((uint)cardStack.stack.Count);
        for(int i = 0; i < cardStack.stack.Count; i++)
        {
            writer.WriteUInt((uint)cardStack.stack[i].type);
        }
    }
    public CardStack ReadCardStack(ref DataStreamReader reader)
    {
        List<CardSO> cards = new List<CardSO>();
        uint count = reader.ReadUInt();

        for(int i = 0; i < count; i++) 
        {
            cards.Add(NetworkManager.Instance.GetCard((int)reader.ReadUInt()));
        }
        return new CardStack(cards);
    }

    public void WriteCardSO(ref DataStreamWriter writer, CardSO card)
    {
        writer.WriteUInt((uint)card.type);
    }
    public CardSO ReadCardSO(ref DataStreamReader reader)
    {
        return NetworkManager.Instance.GetCard((int)reader.ReadUInt());
    }

    public void WriteBoolean(ref DataStreamWriter writer, bool boolean)
    {
        uint value;
        if (boolean) { value = 1; }
        else { value = 0; }
        writer.WriteUInt(value);
    }
    public bool ReadBoolean(ref DataStreamReader reader)
    {
        uint value = reader.ReadUInt();
        if (value == 1) { return true; }
        else { return false; }
    }
}
