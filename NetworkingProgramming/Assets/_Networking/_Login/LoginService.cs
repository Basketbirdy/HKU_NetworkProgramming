using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System.Collections.Generic;

public static class LoginService
{
    public static async Task<string> TryLogin(string email, string password)
    {
        // check if there is a session, if not create one
        await APIConnection.CheckSession(true);
        Debug.Log($"[LoginService] completed session check! session: {APIConnection.sessionId}");

        // construct user_login url
        string url = APIConnection.BuildUrl("user_login", $"sessid={APIConnection.sessionId}", $"em={email}", $"pw={password}");
        string json = await APIConnection.MakeWebRequest(url);
        Debug.Log($"Received response: {json}");

        if (APIConnection.CheckResponseForErrorCode(json))
        {
            return json;
        }

        // handle received data (response)
        // assuming the response is in JSON format, parse the JSON into UserData class
        HandleLogin(json);
        return json;
    }

    public static async Task<string> TrySignin(string email, string nickname, string dateOfBirth, string password)
    {
        await APIConnection.CheckSession(true);
        Debug.Log($"[LoginService] completed session check! session: {APIConnection.sessionId}");

        // TODO - Send the data from UI Controller

        // construct user_insert url
        string url = APIConnection.BuildUrl("user_insert", $"sessid={APIConnection.sessionId}", $"em={email}", $"nn={nickname}", $"dob={dateOfBirth}", $"pw={password}");
        string result = await APIConnection.MakeWebRequest(url, "1");

        if (APIConnection.CheckResponseForErrorCode(result))
        {
            return result;
        }

        // if successful registration log in
        await TryLogin(email, password);
        return result;
    }

    private static bool HandleLogin(string json)
    {
        // set account information in AccountManager
        UserData results = JsonUtility.FromJson<UserData>(json);
        if(results == null) { return false; }

        AccountManager.Instance.SetAccount(results);
        return true;
    }

    public static async Task<string> TryLogout()
    {
        if (AccountManager.Instance.User_Id == -1) { return string.Empty; }    // if noone is logged in, skip everything

        // check if there is a session, if not create one
        await APIConnection.CheckSession(false);
        Debug.Log($"[LoginService] completed session check! session: {APIConnection.sessionId}");

        string url = APIConnection.BuildUrl("user_logout", $"sessid={APIConnection.sessionId}");
        string result = await APIConnection.MakeWebRequest(url, "1");

        if (APIConnection.CheckResponseForErrorCode(result))
        {
            return result;
        }

        AccountManager.Instance.ClearAccount();
        return result;
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
