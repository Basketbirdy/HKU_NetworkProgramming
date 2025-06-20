using Unity.Collections;
using UnityEngine;

public class GameEndMessage : NetworkMessage
{
    public override NetworkMessageType Type => NetworkMessageType.GAME_END;

    public ScoreEntry stats;
    public string player1Name;
    public string player2Name;
    public string winnerName;

    public override void Encode(ref DataStreamWriter writer)
    {
        base.Encode(ref writer);

        WriteScoreEntry(ref writer, stats);
        writer.WriteFixedString128(player1Name);
        writer.WriteFixedString128(player2Name);
        writer.WriteFixedString128(winnerName);
    }

    public override void Decode(ref DataStreamReader reader)
    {
        base.Decode(ref reader);

        stats = ReadScoreEntry(ref reader);
        player1Name = reader.ReadFixedString128().ToString();
        player2Name = reader.ReadFixedString128().ToString();
        winnerName = reader.ReadFixedString128().ToString();
    }
}
