using TMPro;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUIController : BaseUIController
{
    [SerializeField] private TextMeshProUGUI player1Text;
    [SerializeField] private TextMeshProUGUI player2Text;

    [SerializeField] private Button startGameButton;

    [SerializeField] private GameObject notificationPrefab;

    private void OnEnable()
    {
        //EventHandler<string>.AddListener(GlobalEvents.PLAYER_JOINED, UpdatePlayerList);
        //EventHandler<string>.AddListener(GlobalEvents.PLAYER_JOINED, ShowJoinNotification);
        //EventHandler.AddListener("PlayerMinimumReached", EnableStartButton);
    }

    private void OnDisable()
    {
        //EventHandler<string>.RemoveListener(GlobalEvents.PLAYER_JOINED, UpdatePlayerList);
        //EventHandler<string>.RemoveListener(GlobalEvents.PLAYER_JOINED, ShowJoinNotification);
        //EventHandler.RemoveListener("PlayerMinimumReached", EnableStartButton);
    }

    private void Start()
    {
        UIManager.Instance.AddReference<TextMeshProUGUI>("player1Text", player1Text);
        UIManager.Instance.AddReference<TextMeshProUGUI>("player2Text", player2Text);

        UIManager.Instance.AddReference<Button>("StartGameButton", startGameButton);
    }

    public void OnStartGameButtonPressed()
    {
        EventHandler.InvokeEvent(GlobalEvents.GAME_START);
    }
}
