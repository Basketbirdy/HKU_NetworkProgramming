using UnityEngine;
using UnityEngine.SceneManagement;

public class LeaderboardUIController : MonoBehaviour
{
    [Header("Scene indices")]
    [SerializeField] private int startupScene = 0;

    [Header("Leaderboard")]
    [SerializeField] private GameObject entryPrefab;
    [SerializeField] private RectTransform entryParent;

    public void GoStartup()
    {
        SceneManager.LoadScene(startupScene);
    }

    public void CreateEntry()
    {
        // TODO - Create leaderboard entry based on all returned score database entries
        GameObject newEntry = Instantiate(entryPrefab, entryParent);
    }
}
