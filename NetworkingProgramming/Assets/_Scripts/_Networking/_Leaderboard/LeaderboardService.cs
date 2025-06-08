using UnityEngine;
using System.Threading.Tasks;

public static class LeaderboardService
{
    public static async Task<bool> RequestScores()
    {
        return false;
        //// check if there is a session, if not create one
        //_ = await APIConnection.CheckSession(true);
        //Debug.Log($"[LoginService] completed session check! session: {APIConnection.sessionId}");
    }
}

[System.Serializable]
public struct ScoreEntry
{
    // winner
    public string winnerName;       // username of the user who won the match
    public string score;            // score of the user who won the match
    // TODO - other relevant score data

    // losers
    public string[] loserNames;     // usernames of the users who lost the match
    
    // other
    public string datetime;         // time match results were uploaded
}
