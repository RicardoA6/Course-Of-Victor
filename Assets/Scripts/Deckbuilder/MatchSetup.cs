using System.Collections.Generic;
using UnityEngine;

public class MatchSetup : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private int startingHealth = 50;
    [SerializeField] private int startingEnergy = 3;

    private void Start()
    {
        Player alice = new Player("Alice (White Mage)", startingHealth, startingEnergy, new Deck(BuildWhiteMageDeck()));
        Player bob = new Player("Bob (Thief)", startingHealth, startingEnergy, new Deck(BuildThiefDeck()));
        Player carol = new Player("Carol (Soldier)", startingHealth, startingEnergy, new Deck(BuildSoldierDeck()));

        Player dana = new Player("Dana (Black Mage)", startingHealth, startingEnergy, new Deck(BuildBlackMageDeck()));
        Player ethan = new Player("Ethan (Red Mage)", startingHealth, startingEnergy, new Deck(BuildRedMageDeck()));
        Player farah = new Player("Farah (Monk)", startingHealth, startingEnergy, new Deck(BuildMonkDeck()));

        Team teamA = new Team("Team A", new List<Player> { alice, bob, carol });
        Team teamB = new Team("Team B", new List<Player> { dana, ethan, farah });

        gameManager.StartMatch(new List<Team> { teamA, teamB });
    }

    private List<CardData> BuildWhiteMageDeck()
    {
        List<CardData> deck = new List<CardData>();

        for (int i = 0; i < 4; i++)
        {
            deck.Add(CreateCard("Cure", 1, TargetType.SingleAlly, new HealEffect { Amount = 10 }));
        }

        for (int i = 0; i < 2; i++)
        {
            deck.Add(CreateStatusCard("Regen", 2, TargetType.SingleAlly, StatusEffectType.Regen, "Regen", duration: 3, magnitude: 4));
        }

        for (int i = 0; i < 2; i++)
        {
            deck.Add(CreateStatusCard("Protect", 2, TargetType.SingleAlly, StatusEffectType.Protect, "Protect", duration: 3, magnitude: 3));
        }

        // Large duration: Shell is removed when its absorb pool is depleted (see Player.AbsorbWithShell), not by turn countdown.
        deck.Add(CreateStatusCard("Shell", 2, TargetType.SingleAlly, StatusEffectType.Shell, "Shell", duration: 99, magnitude: 15));

        deck.Add(CreateCard("Holy", 3, TargetType.SingleEnemy, new DamageEffect { Amount = 20 }));
        deck.Add(CreateCard("Full-Heal", 3, TargetType.SingleAlly, new HealEffect { Amount = 999 }));
        deck.Add(CreateCard("Life", 3, TargetType.DeadAlly, new HealEffect { Amount = 20 }));

        return deck;
    }

    private List<CardData> BuildBlackMageDeck()
    {
        List<CardData> deck = new List<CardData>();

        for (int i = 0; i < 4; i++)
        {
            deck.Add(CreateCard("Fire", 1, TargetType.SingleEnemy, new DamageEffect { Amount = 9 }));
        }

        for (int i = 0; i < 2; i++)
        {
            deck.Add(CreateCard("Ice", 1, TargetType.SingleEnemy, new DamageEffect { Amount = 7 }));
        }

        for (int i = 0; i < 2; i++)
        {
            deck.Add(CreateStatusCard("Slow", 1, TargetType.SingleEnemy, StatusEffectType.Slow, "Slow", duration: 2, magnitude: 2));
        }

        for (int i = 0; i < 2; i++)
        {
            deck.Add(CreateCard("Thunder", 2, TargetType.SingleEnemy, new DamageEffect { Amount = 14 }));
        }

        deck.Add(CreateCard("Water", 2, TargetType.AllEnemies, new DamageEffect { Amount = 6 }));
        deck.Add(CreateCard("Comet", 3, TargetType.SingleEnemy, new DamageEffect { Amount = 26 }));

        return deck;
    }

    private List<CardData> BuildRedMageDeck()
    {
        List<CardData> deck = new List<CardData>();

        for (int i = 0; i < 3; i++)
        {
            deck.Add(CreateCard("Swordplay", 1, TargetType.SingleEnemy, new DamageEffect { Amount = 7 }));
        }

        for (int i = 0; i < 2; i++)
        {
            deck.Add(CreateCard("Firebolt", 1, TargetType.SingleEnemy, new DamageEffect { Amount = 8 }));
        }

        for (int i = 0; i < 2; i++)
        {
            deck.Add(CreateCard("Cure", 1, TargetType.SingleAlly, new HealEffect { Amount = 6 }));
        }

        deck.Add(CreateStatusCard("Haste", 2, TargetType.Self, StatusEffectType.Haste, "Haste", duration: 2, magnitude: 1));

        for (int i = 0; i < 2; i++)
        {
            deck.Add(CreateCard("Dia", 2, TargetType.SingleEnemy, new DamageEffect { Amount = 13 }));
        }

        return deck;
    }

    private List<CardData> BuildThiefDeck()
    {
        List<CardData> deck = new List<CardData>();

        for (int i = 0; i < 4; i++)
        {
            deck.Add(CreateCard("Quick Strike", 1, TargetType.SingleEnemy, new DamageEffect { Amount = 6 }));
        }

        for (int i = 0; i < 2; i++)
        {
            deck.Add(CreateCard("Mug", 1, TargetType.SingleEnemy, new DamageEffect { Amount = 5 }, new DrainEnergyEffect { Amount = 1 }));
        }

        for (int i = 0; i < 2; i++)
        {
            deck.Add(CreateCard("Trick", 1, TargetType.SingleEnemy, new DrainEnergyEffect { Amount = 2 }));
        }

        for (int i = 0; i < 2; i++)
        {
            deck.Add(CreateCard("Backstab", 2, TargetType.SingleEnemy, new DamageEffect { Amount = 16 }));
        }

        return deck;
    }

    private List<CardData> BuildMonkDeck()
    {
        List<CardData> deck = new List<CardData>();

        for (int i = 0; i < 4; i++)
        {
            deck.Add(CreateCard("Punch", 1, TargetType.SingleEnemy, new DamageEffect { Amount = 7 }));
        }

        for (int i = 0; i < 2; i++)
        {
            deck.Add(CreateCard("Barrage", 2, TargetType.AllEnemies, new DamageEffect { Amount = 6 }));
        }

        for (int i = 0; i < 2; i++)
        {
            deck.Add(CreateStatusCard("Meditate", 1, TargetType.Self, StatusEffectType.Regen, "Regen", duration: 3, magnitude: 5));
        }

        for (int i = 0; i < 2; i++)
        {
            deck.Add(CreateStatusCard("Iron Skin", 2, TargetType.Self, StatusEffectType.Protect, "Protect", duration: 3, magnitude: 4));
        }

        return deck;
    }

    private List<CardData> BuildSoldierDeck()
    {
        List<CardData> deck = new List<CardData>();

        for (int i = 0; i < 4; i++)
        {
            deck.Add(CreateCard("Slash", 1, TargetType.SingleEnemy, new DamageEffect { Amount = 8 }));
        }

        for (int i = 0; i < 2; i++)
        {
            deck.Add(CreateCard("Braver", 2, TargetType.SingleEnemy, new DamageEffect { Amount = 16 }));
        }

        for (int i = 0; i < 2; i++)
        {
            deck.Add(CreateCard("Cross-Slash", 2, TargetType.SingleEnemy,
                new DamageEffect { Amount = 10 },
                new ApplyStatusEffectCard { EffectToApply = MakeStatus("Slow", StatusEffectType.Slow, duration: 2, magnitude: 2) }));
        }

        for (int i = 0; i < 2; i++)
        {
            deck.Add(CreateCard("Limit Break", 3, TargetType.SingleEnemy, new DamageEffect { Amount = 32 }));
        }

        return deck;
    }

    private CardData CreateCard(string cardName, int cost, TargetType target, params CardEffect[] effects)
    {
        CardData card = ScriptableObject.CreateInstance<CardData>();
        card.CardName = cardName;
        card.EnergyCost = cost;
        card.Target = target;
        card.Effects.AddRange(effects);
        return card;
    }

    private CardData CreateStatusCard(string cardName, int cost, TargetType target, StatusEffectType effectType, string effectName, int duration, int magnitude)
    {
        StatusEffect status = MakeStatus(effectName, effectType, duration, magnitude);
        return CreateCard(cardName, cost, target, new ApplyStatusEffectCard { EffectToApply = status });
    }

    private StatusEffect MakeStatus(string effectName, StatusEffectType effectType, int duration, int magnitude)
    {
        return new StatusEffect
        {
            EffectName = effectName,
            EffectType = effectType,
            Duration = duration,
            Magnitude = magnitude
        };
    }
}
