using UnityEngine;

public class Card : MonoBehaviour, IInitializable<CardSO>
{
    [Header("References")]
    [SerializeField] private SpriteRenderer image;
    [SerializeField] private CardSO data;

    [Header("Data")]
    [SerializeField] private CardType type;

    public void Init(CardSO data)
    {
        type = data.type;

        image.sprite = data.imageData.image;
        image.transform.position += data.imageData.offset;
        image.transform.rotation = Quaternion.Euler(data.imageData.rotation);
        image.transform.localScale = data.imageData.scale;
    }
}
