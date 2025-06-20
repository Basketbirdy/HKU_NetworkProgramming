using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : BaseUIController
{
    [SerializeField] private TextMeshProUGUI stateIndicatorText;
    [SerializeField] private TextMeshProUGUI roundTitleText;
    [Space]
    [SerializeField] private Button drawButton;

    private const string isMyTurn = "Awaiting input";
    private const string notMyTurn = "Waiting for opponent to make a move";

    private const string drawCard = "Draw a card";

    private void Start()
    {
        UIManager.Instance.AddReference<BaseUIController>(GetType().ToString(), this);

        UIManager.Instance.AddReference<Button>("DrawButton", drawButton);
    }

    public void SetRoundTitleText(int roundNumber)
    {
        roundTitleText.text = $"Round: {roundNumber}";
    }
    public void SetStateIndicatorWaitingText(bool myTurn)
    {
        string text = myTurn ? isMyTurn : notMyTurn;
        stateIndicatorText.text = text;
    }
    public void SetStateIndicatorDrawCardText()
    {
        stateIndicatorText.text = drawCard;
    }
}
