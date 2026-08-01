using System.Collections.Generic;
using UnityEngine;

public class Deck
{
    public List<CardData> DrawPile = new List<CardData>();
    public List<CardData> Hand = new List<CardData>();
    public List<CardData> DiscardPile = new List<CardData>();

    public Deck(IEnumerable<CardData> startingCards)
    {
        DrawPile.AddRange(startingCards);
        ShuffleDrawPile();
    }

    public void DrawCards(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (DrawPile.Count == 0)
            {
                ShuffleDiscardIntoDraw();
                if (DrawPile.Count == 0)
                {
                    return;
                }
            }

            int lastIndex = DrawPile.Count - 1;
            Hand.Add(DrawPile[lastIndex]);
            DrawPile.RemoveAt(lastIndex);
        }
    }

    public void PlayFromHand(CardData card)
    {
        Hand.Remove(card);
        DiscardPile.Add(card);
    }

    public void DiscardHand()
    {
        DiscardPile.AddRange(Hand);
        Hand.Clear();
    }

    public void ShuffleDiscardIntoDraw()
    {
        DrawPile.AddRange(DiscardPile);
        DiscardPile.Clear();
        ShuffleDrawPile();
    }

    private void ShuffleDrawPile()
    {
        for (int i = DrawPile.Count - 1; i > 0; i--)
        {
            int swapIndex = Random.Range(0, i + 1);
            (DrawPile[i], DrawPile[swapIndex]) = (DrawPile[swapIndex], DrawPile[i]);
        }
    }
}
