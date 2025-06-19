using UnityEngine;
using System.Collections.Generic;
using System;

public class Deck : CardStack
{
    public DeckSO contents;

    public Deck(List<CardSO> cards) : base(cards)
    {
    }

    //public void Refill()
    //{
    //    Fill(contents.GetCards());
    //}

    //public override bool Draw(out CardSO card)
    //{
    //    card = null;
    //    if(!base.Draw(out card))
    //    {
    //        Refill();
    //    }
    //}
}
