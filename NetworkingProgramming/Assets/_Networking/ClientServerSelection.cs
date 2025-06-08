using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientServerSelection : MonoBehaviour
{
    [Header("Scene indices")]
    [SerializeField] private int serverScene;
    [SerializeField] private int clientScene;

    [Header("Settings")]
    private IPopup popup;

    private void Awake()
    {
        popup = GetComponentInChildren<IPopup>();
    }

    private void Start()
    {
        popup.Close();
    }

    public void GoServer()
    {
        if (TryServer())
        {
            if (!AccountManager.Instance.LoggedIn) { AccountManager.Instance.SetGuest(); }
            LoadScene(serverScene);
        }
    }

    public void GoClient()
    {
        if (TryClient())
        {
            if (!AccountManager.Instance.LoggedIn) { AccountManager.Instance.SetGuest(); }
            LoadScene(clientScene);
        }
    }

    private bool TryServer()
    {
        if (!CheckLogin("host")) { return false; }

        return true;
    }

    private bool TryClient()
    {
        if (!CheckLogin("join")) { return false; }

        return true;
    }

    private void LoadScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    private bool CheckLogin(string subject)
    {
        if (!AccountManager.Instance.LoggedIn && AccountManager.Instance.RequiresLogin)
        {
            popup.Show($"Failed {subject}ing!", $"Not logged in. <br>please login to play");
            return false;
        }
        return true;
    }
}
