using UnityEngine;
using UnityEngine.SceneManagement;

public static class LoginService
{
    public static bool TryLogin(string email, string password)
    {
        string result = string.Empty;
        // TODO - Send the data from UI Controller

        // construct user_login url

        // set all current account data

        // parse JSON result
        // if result = 0
        return false;
        // else
        HandleLogin(result);
        return true;
    }

    public static bool TrySignin(string email, string nickname, string password)
    {
        string result = string.Empty;
        // TODO - Send the data from UI Controller

        // construct user_insert url

        // set all current account data

        // parse JSON result
        // if result = 0
        return false;
        // else
        HandleLogin(result);
        return true;
    }

    private static void HandleLogin(string json)
    {
        // set account information in AccountManager
        var results = JsonUtility.FromJson(json, typeof(UserData));

        UserData uData = results as UserData;
        AccountManager.Instance.SetAccount(uData);
    }
}

public class UserData
{
    public UserData(string nickname)
    {
        user_id = -1;
        this.nickname = nickname;
    }

    // TODO - recreate database columns
    public int user_id;
    public string nickname;
}
