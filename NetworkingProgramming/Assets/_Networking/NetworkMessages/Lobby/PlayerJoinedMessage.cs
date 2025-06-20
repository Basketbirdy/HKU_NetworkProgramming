using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class PlayerJoinedMessage : NetworkMessage
{
    public override NetworkMessageType Type => NetworkMessageType.PLAYER_JOINED;

    public string name;         // name of the player that joined
    public uint playerNumber;    // number of the player that joined
    public uint userId;

    public List<string> playerNames = new List<string>();   // list of all existing players

    public override void Encode(ref DataStreamWriter writer)
    {
        base.Encode(ref writer);

        writer.WriteFixedString128(name);
        writer.WriteUInt(playerNumber);
        writer.WriteUInt(userId);

        writer.WriteUInt((uint)playerNames.Count);
        for (int i = 0; i < playerNames.Count; i++)
        {
            writer.WriteFixedString128(playerNames[i]);
        }
    }

    public override void Decode(ref DataStreamReader reader)
    {
        base.Decode(ref reader);

        name = reader.ReadFixedString128().ToString();
        playerNumber = reader.ReadUInt();
        userId = reader.ReadUInt();

        uint playerCount = reader.ReadUInt();
        for(int i = 0;  i < playerCount; i++)
        {
            playerNames.Add(reader.ReadFixedString128().ToString());
        }
    }
}
