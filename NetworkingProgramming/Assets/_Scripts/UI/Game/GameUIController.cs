using TMPro;
using UnityEngine;

public class GameUIController : BaseUIController
{
    [SerializeField] private TextMeshProUGUI stateIndicatorText;
    [SerializeField] private TextMeshProUGUI roundTitleText;

    private const string isMyTurn = "Awaiting input";
    private const string notMyTurn = "Waiting for opponent to make a move";

    private void Start()
    {
        UIManager.Instance.AddReference<BaseUIController>(GetType().ToString(), this);
    }

    public void SetRoundTitleText(int roundNumber)
    {
        roundTitleText.text = $"Round: {roundNumber}";
    }
    public void SetStateIndicatorText(bool myTurn)
    {
        string text = myTurn ? isMyTurn : notMyTurn;
        stateIndicatorText.text = text;
    }
}
