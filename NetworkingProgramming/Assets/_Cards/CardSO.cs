using UnityEngine;

public enum CardType { ROCK, PAPER, SCISSORS }

[CreateAssetMenu(menuName = "Cards/Card")]
public class CardSO : ScriptableObject
{
    public CardType type;
}
