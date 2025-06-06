using UnityEngine;

public class AccountManager : MonoBehaviour
{
    public static AccountManager Instance { get; private set; }

    [Header("Account data")]
    public int User_Id => userData.user_id;
    public string Nickname => userData.nickname;
    public bool LoggedIn => userData != null;

    [Header("Settings")]
    [SerializeField] private bool requiresLogin = true;
    public bool RequiresLogin => requiresLogin;

    private UserData userData;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        SetGuest();

        DontDestroyOnLoad(gameObject);
    }

    public void SetGuest()
    {
        UserData guestData = new UserData("Guest");
        this.userData = guestData;
    }

    public void SetAccount(UserData userData)
    {
        this.userData = userData;
    }

    public void ClearAccount()
    {
        userData = null;
    }
}
