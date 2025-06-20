using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndGameUIController : BaseUIController
{
    ScoreEntry stats;

    [Header("Scene indices")]
    [SerializeField] private int startupScene = 0;
    [Header("Components")]
    [SerializeField] private TextMeshProUGUI statsText;
    [SerializeField] private Button uploadButton;
    private IPopup popup;

    private void Start()
    {
        popup = GetComponentInChildren<IPopup>(true);
        UIManager.Instance.AddReference<BaseUIController>(GetType().ToString(), this);
        UIManager.Instance.AddReference<Button>("UploadButton", uploadButton);
    }

    public void PopulateStats(ScoreEntry stats, string player1, string player2, string winner)
    {
        this.stats = stats;
        StringBuilder builder = new StringBuilder();

        builder.AppendLine($"Winner: {winner}");
        builder.AppendLine($"participants: {player1} - {player2}");
        builder.AppendLine("");
        builder.AppendLine($"Rounds played: {stats.roundsPlayed}");
        builder.AppendLine($"Rounds tied: {stats.roundsTied}");
        builder.AppendLine("");
        builder.AppendLine($"Rocks played: {stats.rockCount}");
        builder.AppendLine($"Papers played: {stats.paperCount}");
        builder.AppendLine($"Scissors played: {stats.scissorsCount}");

        statsText.text = builder.ToString();
        builder = null;
    }

    public void Return()
    {
        SceneManager.LoadScene(startupScene);
    }

    public void TryScoreInsert()
    {
        if (string.IsNullOrEmpty(APIConnection.sessionId)){
            popup.Show("failed upload", "failed uploading your stats, you can return without uploading");
            return;
        }
        else
        {
            ScoreInsert();
        }
    }
    private async void ScoreInsert()
    {
        string response = await LeaderboardService.TrySendScore(stats);
        if (APIConnection.CheckResponseForErrorCode(response) || string.IsNullOrEmpty(response))
        {
            popup.Show(APIConnection.responseCodeHeader[response], APIConnection.responseCodeMessage[response]);
        }
        else
        {
            popup.Show($"Uploaded data!", $"Thanks for uploading your stats!");
        }
    }
}
