using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class CardHolder : MonoBehaviour
{
    [SerializeField] List<GameObject> cards = new List<GameObject>();
    [SerializeField] private Vector2 totalOffset;
    [SerializeField] private float cardWidth;
    [SerializeField] private float spacing;
    [Space]
    [SerializeField] private Transform myCard;
    [SerializeField] private Transform theirCard;
    private GameObject myCardObj;
    private GameObject theirCardObj;

    public void AddCard(GameObject obj)
    {
        if (!cards.Contains(obj))
        {
            cards.Add(obj);
            obj.transform.parent = transform;
        }

        ReorderCards();
    }

    public void RemoveCard(GameObject obj)
    {
        if (cards.Contains(obj))
        {
            int index = cards.IndexOf(obj);
            cards.RemoveAtSwapBack(index);
        }

        ReorderCards();
    }

    public void PlayCard(Card card)
    {
        RemoveCard(card.gameObject);
        card.gameObject.transform.parent = myCard;
        card.gameObject.transform.localPosition = Vector3.zero;
        myCardObj = card.gameObject;
        card.Disable();
    }

    public void ShowTheirCard(Card card)
    {
        Debug.Log(card);
        Debug.Log(card.gameObject);
        Debug.Log(card.gameObject.transform);
        Debug.Log(theirCard);
        card.gameObject.transform.parent = theirCard;
        card.gameObject.transform.localPosition = Vector3.zero;
        myCardObj = card.gameObject;
        card.Disable();
    }

    private void ReorderCards()
    {
        totalOffset = new Vector2((cardWidth + spacing) * ((float)cards.Count / 2f) - ((cardWidth + spacing) / 2), 0f) * -1f;
        for(int i = 0; i < cards.Count; i++)
        {
            Vector3 offset = totalOffset + new Vector2((cardWidth + spacing) * i, 0);
            cards[i].transform.position = transform.position + offset;
        }
    }

    private void OnDrawGizmos()
    {
        int count = 8;
        totalOffset = new Vector2((cardWidth + spacing) * ((float)count/2f) - ((cardWidth + spacing) / 2), 0f) * -1f;
        for (int i = 0; i < count; i++)
        {
            Vector3 offset = totalOffset + new Vector2((cardWidth + spacing) * i, 0);
            Gizmos.DrawSphere(transform.position + offset, .1f);
        }
    }
}
