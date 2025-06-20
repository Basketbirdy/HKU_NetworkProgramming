using UnityEngine;
using System.Threading.Tasks;

public static class LeaderboardService
{
    public static async Task<string> TryGetStats()
    {
        // check if there is a session, if not create one
        _ = await APIConnection.CheckSession(true);
        Debug.Log($"[LoginService] completed session check! session: {APIConnection.sessionId}");

        // construct a leaderboard_get_averages url
        string url = APIConnection.BuildUrl("leaderboard_get_averages", $"sessid={APIConnection.sessionId}", $"gameid={APIConnection.gameId}");
        string response = await APIConnection.MakeWebRequest(url);
        Debug.Log($"Received response: {response}");

        return response;
    }

    public static async Task<string> TrySendScore(ScoreEntry entry)
    {
        // check if there is a session, if not create one
        _ = await APIConnection.CheckSession(true);
        Debug.Log($"[LoginService] completed session check! session: {APIConnection.sessionId}");

        // construct score_insert url
        // server id and player 1 id can be deduced from current session variables if someone is logged in
        string url = APIConnection.BuildUrl("score_insert", $"sessid={APIConnection.sessionId}", $"gameid={APIConnection.gameId}", $"score={entry.score}", $"pl2id={entry.player2Id}", $"plwid={entry.playerWinnerId}"
                                            , $"rplayed={entry.roundsPlayed}", $"rtied={entry.roundsTied}", $"crock={entry.rockCount}", $"cpaper={entry.paperCount}", $"cscissors={entry.scissorsCount}");

        string response = await APIConnection.MakeWebRequest(url, "1");

        return response;
    }
}

[System.Serializable]
public struct ScoreEntry
{
    public int gameId;

    public int player1Id;   // $_SESSION["user_id"], only the host will submit a score, so player 1 is always the current logged in user
    public int player2Id;
    public int playerWinnerId;

    public int score;   // might remain unused, since my game doesn't assign a traditional score number, but ranks based on average rounds played (fewer rounds = faster matches)

    public int roundsPlayed;
    public int roundsTied;

    public int rockCount;
    public int paperCount;
    public int scissorsCount;
}
