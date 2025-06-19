using UnityEngine;

public class Card : MonoBehaviour, IInitializable<CardSO>
{
    [Header("References")]
    [SerializeField] private SpriteRenderer image;
    [SerializeField] private SpriteRenderer backdrop;
    [SerializeField] public CardSO Data { get; private set; }

    [Header("Data")]
    [SerializeField] private CardType type;

    public void Init(CardSO data)
    {
        this.Data = data;
        type = data.type;

        image.sprite = data.imageData.image;
        backdrop.color = data.cardColor;
        image.transform.position += data.imageData.offset;
        image.transform.rotation = Quaternion.Euler(data.imageData.rotation);
        image.transform.localScale = data.imageData.scale;
    }
}
