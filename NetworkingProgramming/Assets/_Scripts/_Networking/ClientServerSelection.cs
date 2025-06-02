using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientServerSelection : MonoBehaviour
{
    [Header("Scene indices")]
    [SerializeField] private int serverScene;
    [SerializeField] private int clientScene;

    [Header("Settings")]
    [SerializeField] private bool requiresLogin = true;
    private IPopup popup_Error;

    private void Awake()
    {
        popup_Error = GetComponentInChildren<IPopup>();
    }

    private void Start()
    {
        popup_Error.Close();
    }

    public void GoServer()
    {
        if (!TryServer())
        {
            
        }
    }

    public void GoClient()
    {
        if (!TryClient())
        {
            
        }
    }

    private bool TryServer()
    {
        if (!CheckLogin("host")) { return false; }

        LoadScene(serverScene);

        return true;
    }

    private bool TryClient()
    {
        if (!CheckLogin("join")) { return false; }

        LoadScene(clientScene);

        return true;
    }

    private void LoadScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    private bool CheckLogin(string subject)
    {
        if (!AccountManager.Instance.LoggedIn && requiresLogin)
        {
            popup_Error.Show($"Failed {subject}ing!", $"Not logged in. <br>please login to play");
            return false;
        }
        return true;
    }
}
