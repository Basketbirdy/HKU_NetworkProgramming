using UnityEngine;

public class AccountManager : MonoBehaviour
{
    public static AccountManager Instance { get; private set; }

    [Header("Account data")]
    public int User_Id => userData.id;
    public string Nickname => userData.nickname;
    public string Email => userData.email;
    public string DateOfBirth => userData.geboortedatum;
    public bool LoggedIn => userData.id != -1 && userData != null;

    [Header("Settings")]
    [SerializeField] private bool requiresLogin = true;
    public bool RequiresLogin => requiresLogin;

    [Header("Current acount")]
    [SerializeField] private UserData userData;

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

    public void ClearAccount(bool resetToGuest = true)
    {
        userData = null;
        if (resetToGuest) { SetGuest(); }
    }
}
