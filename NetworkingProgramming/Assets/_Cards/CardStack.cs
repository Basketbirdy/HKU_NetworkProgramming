using UnityEngine;
using System.Collections.Generic;
using Unity.Collections;

[System.Serializable]
public class CardStack
{
    public List<CardSO> stack = new List<CardSO>();
    private int LastIndex => stack.Count - 1;
    
    public CardStack(List<CardSO> cards)
    {
        Fill(cards);
    }
    public void Fill(List<CardSO> cards)
    {
        stack = cards;
    }

    public bool Draw(out CardSO card)
    {
        card = null;
        if(stack.Count == 0) { return false; }
        
        card = stack[LastIndex];
        stack.RemoveAtSwapBack(LastIndex);
        
        return true;
    }

    public void Remove(CardSO card)
    {
        int index = stack.IndexOf(card);
        stack.RemoveAtSwapBack(index);
    }

    public void Add(CardSO card)
    {
        stack.Add(card);
    }

    public void Shuffle()
    {
        for (int i = 0; i < stack.Count - 1; i++)
        {
            int rand = UnityEngine.Random.Range(i, stack.Count);
            SwapCards(ref stack, i, rand);
        }
    }

    // helper function
    private void SwapCards(ref List<CardSO> deck, int index, int targetIndex)
    {
        if (index == targetIndex) { return; }
        //Debug.Log($"[Deck] swapping cards: deck[{index}] - {deck[index]}, deck[{targetIndex}] - {deck[targetIndex]}");

        var temp = deck[index];
        deck[index] = deck[targetIndex];
        deck[targetIndex] = temp;

        //Debug.Log($"[Deck] swapping cards: deck[{index}] - {deck[index]}, deck[{targetIndex}] - {deck[targetIndex]}");
    }


}
