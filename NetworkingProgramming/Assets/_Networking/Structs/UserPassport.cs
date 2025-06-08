using UnityEngine;
using Unity.Networking.Transport;

/// <summary>
/// struct that holds all identifying data for each user
/// </summary>
public struct UserPassport
{
    public UserPassport(string username, NetworkConnection connection)
    {
        this.username = username;
        this.connection = connection;
    }

    public string username;
    public NetworkConnection connection;

}
