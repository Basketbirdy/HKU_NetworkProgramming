using Unity.Networking.Transport;
using UnityEngine;

public class LobbyUIHandler : MonoBehaviour
{
    [SerializeField] private GameObject notificationPrefab;

    private void OnEnable()
    {
        EventHandler<string>.AddListener(GlobalEvents.PLAYER_JOINED, UpdatePlayerList);
        EventHandler<string>.AddListener(GlobalEvents.PLAYER_JOINED, ShowJoinNotification);
        EventHandler.AddListener("PlayerMinimumReached", EnableStartButton);
    }

    private void OnDisable()
    {
        EventHandler<string>.RemoveListener(GlobalEvents.PLAYER_JOINED, UpdatePlayerList);
        EventHandler<string>.RemoveListener(GlobalEvents.PLAYER_JOINED, ShowJoinNotification);
        EventHandler.RemoveListener("PlayerMinimumReached", EnableStartButton);
    }

    private void UpdatePlayerList(string name)
    {
        Debug.Log($"TODO - update player list, {name}");
    }

    private void ShowJoinNotification(string name)
    {
        ShowNotification($"{name} joined the game!");
    }

    private void EnableStartButton()
    {
        Debug.Log($"TODO - enable start game button");
    }

    private void ShowNotification(string message)
    {
        Debug.Log($"TODO - show notification, {message}");
    }
}
