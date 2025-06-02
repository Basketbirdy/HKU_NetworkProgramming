using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartupUIController : MonoBehaviour
{
    [Header("Scene indices")]
    [SerializeField] private int loginScene;
    [SerializeField] private int leaderboardScene;
    [Space]
    [SerializeField] private TextMeshProUGUI accountText;

    private void Start()
    {
        SetAccountInfo();
    }

    private void SetAccountInfo()
    {
        string msg = string.Empty;
        if (AccountManager.Instance.LoggedIn)
        {
            string nickname = AccountManager.Instance.Nickname;
            msg = $"Hi, {nickname}";
        }
        else
        {
            msg = $"<i>No login found!<i>";
        }
        accountText.text = msg;
    }

    public void GoLogin()
    {
        SceneManager.LoadScene(loginScene);
    }

    public void GoLeaderboard()
    {
        SceneManager.LoadScene(leaderboardScene);
    }

    public void GoClose()
    {
        Application.Quit();
    }
}
