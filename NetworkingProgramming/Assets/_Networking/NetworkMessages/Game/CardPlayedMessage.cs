using Unity.Collections;
using UnityEngine;

public class CardPlayedMessage : NetworkMessage
{
    public override NetworkMessageType Type => NetworkMessageType.CARD_PLAYED;

    public uint playerNumber;
    public CardSO card;

    public override void Encode(ref DataStreamWriter writer)
    {
        base.Encode(ref writer);

        writer.WriteUInt(playerNumber);
        writer.WriteUInt((uint)card.type);
    }

    public override void Decode(ref DataStreamReader reader)
    {
        base.Decode(ref reader);

        playerNumber = reader.ReadUInt();
        card = NetworkManager.Instance.GetCard((int)reader.ReadUInt());
    }
}

public class CardPlayedResponseMessage : NetworkMessage
{
    public override NetworkMessageType Type => NetworkMessageType.CARD_PLAYED_RESPONSE;

    public uint success;
    public uint activePlayer;
    public CardSO card;

    public override void Encode(ref DataStreamWriter writer)
    {
        base.Encode(ref writer);

        writer.WriteUInt(success);
        writer.WriteUInt(activePlayer);
        WriteCardSO(ref writer, card);
    }

    public override void Decode(ref DataStreamReader reader)
    {
        base.Decode(ref reader);

        success = reader.ReadUInt();
        activePlayer = reader.ReadUInt();
        card = ReadCardSO(ref reader);
    }
}
