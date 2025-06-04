using UnityEngine;
using System.Collections.Generic;
using System;

public class Deck : CardStack
{
    private DeckSO contents;

    public Deck(List<CardSO> cards) : base(cards)
    {
    }
}
