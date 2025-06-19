using UnityEngine;

public enum CardType { ROCK, PAPER, SCISSORS }

[CreateAssetMenu(menuName = "Cards/Card")]
public class CardSO : ScriptableObject
{
    [Header("Identifier")]
    public string id;

    [Header("Card")]
    public CardType type;
    public Color cardColor;

    [Header("Image")]
    public ImageData imageData;
}

[System.Serializable]
public struct ImageData
{
    public Sprite image;
    public Vector3 offset;
    public Vector3 rotation;
    public Vector3 scale;
    public Color color;
}