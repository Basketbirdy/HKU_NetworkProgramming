using UnityEngine;
using System.Collections.Generic;

public class CardStack
{
    public List<CardSO> stack = new List<CardSO>();
    
    public CardStack(List<CardSO> cards)
    {
        Fill(cards);
    }

    public CardSO Draw()
    {
        return GetLastCard();
    }

    public void Add()
    {
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
    private CardSO GetLastCard()
    {
        return stack[stack.Count - 1];
    }
    private void SwapCards(ref List<CardSO> deck, int index, int targetIndex)
    {
        if (index == targetIndex) { return; }
        Debug.Log($"[Deck] swapping cards: deck[{index}] - {deck[index]}, deck[{targetIndex}] - {deck[targetIndex]}");

        var temp = deck[index];
        deck[index] = deck[targetIndex];
        deck[targetIndex] = temp;

        Debug.Log($"[Deck] swapping cards: deck[{index}] - {deck[index]}, deck[{targetIndex}] - {deck[targetIndex]}");
    }

    private void Fill(List<CardSO> cards)
    {
        stack = cards;
    }
}
