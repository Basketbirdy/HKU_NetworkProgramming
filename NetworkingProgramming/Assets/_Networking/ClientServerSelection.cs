using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientServerSelection : MonoBehaviour
{
    [Header("Scene indices")]
    [SerializeField] private int serverScene;
    [SerializeField] private int clientScene;
    [Space]
    [SerializeField] private TMP_InputField ipInput;

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
            ClientBehaviour.ip = ipInput.text;

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
        if (string.IsNullOrEmpty(ipInput.text)) 
        {
            popup.Show($"missing ip!", $"The ip input field is empty. Please enter the correct ip adress before trying to join");
            return false; 
        }

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
