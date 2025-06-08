using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System.Collections.Generic;

public static class LoginService
{
    public static async Task<bool> TryLogin(string email, string password)
    {
        // check if there is a session, if not create one
        _ = await APIConnection.CheckSession(true);
        Debug.Log($"[LoginService] completed session check! session: {APIConnection.sessionId}");

        // construct user_login url
        string url = APIConnection.BuildUrl("user_login", $"sessid={APIConnection.sessionId}", $"em={email}", $"pw={password}");
        string json = await APIConnection.MakeWebRequest(url);
        Debug.Log($"Received response: {json}");
        if(json == "") { return false; }

        // handle received data (response)
        // assuming the response is in JSON format, parse the JSON into UserData class
        HandleLogin(json);
        return true;
    }

    public static async Task<bool> TrySignin(string email, string nickname, string password)
    {
        await APIConnection.CheckSession(true);

        string result = string.Empty;
        // TODO - Send the data from UI Controller

        // construct user_insert url

        // set all current account data

        // parse JSON result
        // if result = 0
        HandleLogin(result);
        return true;
    }

    private static bool HandleLogin(string json)
    {
        // set account information in AccountManager
        UserData results = JsonUtility.FromJson<UserData>(json);
        if(results == null) { return false; }

        AccountManager.Instance.SetAccount(results);
        return true;
    }

    public static bool TryLogout()
    {
        if (AccountManager.Instance.User_Id == -1) { return false; }

        AccountManager.Instance.ClearAccount();
        return true;
    }
}

[System.Serializable]
public class UserData
{
    public UserData(string nickname)
    {
        id = -1;
        this.nickname = nickname;
    }

    public int id;
    public string email;
    public string nickname;
    public string password;
    public string geboortedatum;  // date of birth
}
