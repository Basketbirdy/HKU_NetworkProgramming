using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class APIConnection
{
    // should this be in here? is this dangerous?
    public const string urlHeader = "studenthome.hku.nl/~daniel.berghorst/";
    public const string serverLogin = "server_login";

    public const int gameId = 1;

    // should this be in here? is this dangerous?
    public static int serverId = 1;                         
    public static string serverPassword = "Password1";

    public static string sessionId;

    // check if there is a session, if not create one depending on createIfNull parameter
    public static async Task<bool> CheckSession(bool createIfNull, int serverId = 1, string serverPw = "password1")
    {
        Debug.Log($"[APIConnection] checking session! {serverId}, {serverPw}, sessionId: {sessionId}");

        if (string.IsNullOrEmpty(sessionId))
        {
            if (createIfNull) 
            {
                Debug.Log($"[APIConnection] Creating new session");
                await CreateSession(serverId, serverPw);
                return true;
            }
            else
            {
                return false;
            }
        }

        return true;
    }
    public static bool ClearSession()
    {
        if (string.IsNullOrEmpty(sessionId))
        {
            Debug.Log($"[APIConnection] session is already empty!");
            return false;
        }

        Debug.Log($"[APIConnection] clearing current session! {sessionId}");
        sessionId = string.Empty;
        return true;
    }
    public static async Task CreateSession(int serverId, string serverPw)
    {
        Debug.Log($"[APIConnect] creating session");

        string url = BuildUrl(serverLogin, $"id={serverId}", $"pw={serverPw}");
        string response = await MakeWebRequest(url);
        Debug.Log($"[APIConnection] new session id created! {response}");

        if (string.IsNullOrEmpty(response)) 
        {
            Debug.Log($"[APIConnection] failed at creating new session! json: {response}");
            return; 
        }

        ClearSession();
        sessionId = response.Replace("\"", "");
    }

    /// <summary>
    /// Creates and sends a webrequest using the specified url and returns the response in string format
    /// <br>errorResponse (default, 0): response indicating an external error</br>
    /// <br>targetResponse (default, null/empty): expected response</br>
    /// </summary>
    /// <param name="url"></param>
    /// <param name="errorResponse"></param>
    /// <param name="targetResponse"></param>
    /// <returns></returns>
    public static async Task<string> MakeWebRequest(string url, string errorResponse = "0", string targetResponse = "")
    {
        // create request
        var req = UnityWebRequest.Get(url);

        // send the request AND wait untill it finishes
        Debug.Log($"[APIConnection] sending webrequest! {url}");
        await req.SendWebRequest();
        Debug.Log($"[APIConnection] sent webrequest");

        // check if request failed
        if(req.responseCode != 200)
        {
            Debug.LogError($"[APIConnection] response code unexpected... {req.responseCode} {req.error}");
        }

        string response = req.downloadHandler.text;
        Debug.Log($"[APIConnection] response received! {response}");

        // check if response is what was expected, only works when request always returns the same output (like confirmations)
        if(!string.IsNullOrEmpty(targetResponse))
        {
            Debug.Log($"[APIConnection] checking targetResponse!");
            if(req.downloadHandler.text != targetResponse) 
            {
                Debug.Log($"[APIConnection] response text does not equal expected response");
                response = string.Empty;
            }
        }

        if(response == errorResponse) 
        {
            Debug.LogWarning($"[APIConnection] response indicates external error... {response}");
            response = string.Empty; 
        }

        Debug.Log($"[APIConnection] returning response result now! {response}");
        return response;
    }

    public static string BuildUrl(string fileName, params string[] parameters)
    {
        StringBuilder builder = new StringBuilder();

        builder.Append(urlHeader);                      // studenthome.hku.nl/...
        builder.Append($"{fileName}.php");              // [filename].php
        builder.Append("?");
        builder.Append(string.Join("&", parameters));   // param formatted like "[parameterName]=[value]" (example: "score=69")

        string url = builder.ToString();
        builder = null;

        Debug.Log($"[APIConnection] url: {url}");
        return url;
    }
}

//public class Response
//{
//    public string result;
//}

