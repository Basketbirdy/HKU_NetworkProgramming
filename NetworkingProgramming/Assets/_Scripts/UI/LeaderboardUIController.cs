using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LeaderboardUIController : MonoBehaviour
{
    [Header("Scene indices")]
    [SerializeField] private int startupScene = 0;

    [Header("Statistics UI")]
    [SerializeField] private TextMeshProUGUI gamesPlayedText;
    [SerializeField] private TextMeshProUGUI avgRoundsText;
    [SerializeField] private TextMeshProUGUI avgTiedText;
    [SerializeField] private TextMeshProUGUI mostPopularText;
    [Space]
    [SerializeField] private TextMeshProUGUI topPlayerNameText;
    [SerializeField] private TextMeshProUGUI topPlayerAvgRoundsText;

    [Header("Leaderboard UI")]
    [SerializeField] private GameObject entryPrefab;
    [SerializeField] private RectTransform entryParent;

    private IPopup popup;

    private void Awake()
    {
        popup = GetComponentInChildren<IPopup>();
    }

    private void Start()
    {
        TryUpdatePage();
    }

    public void GoStartup()
    {
        SceneManager.LoadScene(startupScene);
    }

    public void TryUpdatePage()
    {
        if(AccountManager.Instance.User_Id == -1)
        {
            popup.Show("Failed!", "Please log in to view statistics");
            return;
        }

        UpdatePage();
    }

    private async void UpdatePage()
    {
        // try to retrieve data necessary for determining stats
        Debug.Log("Attempting to retrieve statistics");
        string response = await LeaderboardService.TryGetStats();

        // check if response does not return an error, if so show error popup with related information
        if (APIConnection.CheckResponseForErrorCode(response) || string.IsNullOrEmpty(response))
        {
            popup.Show(APIConnection.responseCodeHeader[response], APIConnection.responseCodeMessage[response]);
            return;
        }

        PopulateStats(response);
    }

    private void PopulateStats(string response)
    {
        var s = JsonUtility.FromJson<GameStats>(response);

        gamesPlayedText.text = s.gameCount.ToString();

        avgRoundsText.text = s.avgRoundPlayed.ToString("0.0");
        avgTiedText.text = s.avgRoundTied.ToString("0.0");

        // determine most popular card
        CardType popularType;
        if (s.rockTotal >= s.paperTotal && s.rockTotal >= s.scissorsTotal)
        {
            popularType = CardType.ROCK;
        }
        else if (s.paperTotal >= s.rockTotal && s.paperTotal >= s.scissorsTotal)
        {
            popularType = CardType.PAPER;
        }
        else
        {
            popularType = CardType.SCISSORS;
        }
        mostPopularText.text = popularType.ToString();

        topPlayerNameText.text = s.topUser.ToString();
        topPlayerAvgRoundsText.text = s.topUserAvg.ToString();
    }

    public void CreateEntry()
    {
        // TODO - Create leaderboard entry based on all returned score database entries
        GameObject newEntry = Instantiate(entryPrefab, entryParent);
    }
}

public struct GameStats
{
    public int gameCount;

    public float avgRoundPlayed;
    public float avgRoundTied;

    public int rockTotal;
    public int paperTotal;
    public int scissorsTotal;

    public string topUser;
    public int topUserAvg;
}
