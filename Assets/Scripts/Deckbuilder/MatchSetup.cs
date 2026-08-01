using System.Collections.Generic;
using UnityEngine;

public class MatchSetup : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private int startingHealth = 50;
    [SerializeField] private int startingEnergy = 3;

    private void Start()
    {
        Player alice = new Player("Alice", startingHealth, startingEnergy, new Deck(BuildStarterDeck()));
        Player bob = new Player("Bob", startingHealth, startingEnergy, new Deck(BuildStarterDeck()));
        Player carol = new Player("Carol", startingHealth, startingEnergy, new Deck(BuildStarterDeck()));

        Team teamA = new Team("Team A", new List<Player> { alice, bob });
        Team teamB = new Team("Team B", new List<Player> { carol });

        gameManager.StartMatch(new List<Team> { teamA, teamB });
    }

    private List<CardData> BuildStarterDeck()
    {
        List<CardData> deck = new List<CardData>();

        for (int i = 0; i < 6; i++)
        {
            deck.Add(CreateCard("Strike", 1, TargetType.SingleEnemy, new DamageEffect { Amount = 6 }));
        }

        for (int i = 0; i < 4; i++)
        {
            deck.Add(CreateCard("Guard", 1, TargetType.Self, new HealEffect { Amount = 4 }));
        }

        return deck;
    }

    private CardData CreateCard(string cardName, int cost, TargetType target, CardEffect effect)
    {
        CardData card = ScriptableObject.CreateInstance<CardData>();
        card.CardName = cardName;
        card.EnergyCost = cost;
        card.Target = target;
        card.Effects.Add(effect);
        return card;
    }
}
