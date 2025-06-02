using UnityEngine;

public static class LoginService
{
    public static bool TryLogin(string email, string password)
    {
        // TODO - Send the data from UI Controller

        // construct user_login url

        // set all current account data

        // parse JSON result
        // if result = 0
        return false;
        // else
        return true;
    }

    public static bool TrySignin(string email, string nickname, string password)
    {
        // TODO - Send the data from UI Controller

        // construct user_insert url

        // set all current account data

        // parse JSON result
        // if result = 0
        return false;
        // else
        return true;
    }
}
