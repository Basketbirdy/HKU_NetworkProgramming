using UnityEngine;
using System.Collections.Generic;

public struct DeckItem
{
    public CardSO card;
    public int count;
}

[CreateAssetMenu(menuName = "Cards/Deck")]
public class DeckSO : ScriptableObject
{
    public List<DeckItem> contents = new List<DeckItem>(); 

    public List<CardSO> GetCards()
    {
        List<CardSO> cards = new List<CardSO>();

        foreach (DeckItem item in contents)
        {
            for(int i = 0; i < item.count; i++)
            {
                cards.Add(item.card);
            }
        }

        return cards;
    }
}
