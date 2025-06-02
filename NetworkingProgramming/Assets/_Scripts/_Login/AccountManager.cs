using UnityEngine;

public class AccountManager : MonoBehaviour
{
    public static AccountManager Instance { get; private set; }

    [Header("Account data")]
    public int User_Id => userData.user_id;
    public string Nickname => userData.nickname;
    public bool LoggedIn => userData != null;

    private UserData userData;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
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
