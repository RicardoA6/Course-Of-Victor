using System.Collections.Generic;
using UnityEngine;

public class MatchSetup : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private int startingHealth = 50;
    [SerializeField] private int startingEnergy = 3;

    // Deck power curve: 5 bands, weakest -> strongest, copy count strictly
    // tapering (6/5/4/3/2 = 20). This is an implicit "common to legendary"
    // rarity expressed purely through how often a card shows up in a hand,
    // without a literal rarity label anywhere in the data.

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

        // Band 1 (6)
        AddCopies(deck, 3, () => CreateCard("Cure", 1, TargetType.SingleAlly, new HealEffect { Amount = 10 }));
        AddCopies(deck, 3, () => CreateCard("Aero", 1, TargetType.SingleEnemy, new DamageEffect { Amount = 6 }));

        // Band 2 (5)
        AddCopies(deck, 3, () => CreateStatusCard("Regen", 2, TargetType.SingleAlly, StatusEffectType.Regen, "Regen", duration: 3, magnitude: 4));
        AddCopies(deck, 2, () => CreateStatusCard("Protect", 2, TargetType.SingleAlly, StatusEffectType.Protect, "Protect", duration: 3, magnitude: 3));

        // Band 3 (4)
        // Large duration: Shell is removed when its absorb pool is depleted (see Player.AbsorbWithShell), not by turn countdown.
        AddCopies(deck, 4, () => CreateStatusCard("Shell", 2, TargetType.SingleAlly, StatusEffectType.Shell, "Shell", duration: 99, magnitude: 15));

        // Band 4 (3)
        AddCopies(deck, 3, () => CreateCard("Holy", 3, TargetType.SingleEnemy, new DamageEffect { Amount = 20 }));

        // Band 5 (2)
        AddCopies(deck, 1, () => CreateCard("Full-Heal", 3, TargetType.SingleAlly, new HealEffect { Amount = 999 }));
        AddCopies(deck, 1, () => CreateCard("Life", 3, TargetType.DeadAlly, new HealEffect { Amount = 20 }));

        return deck;
    }

    private List<CardData> BuildBlackMageDeck()
    {
        List<CardData> deck = new List<CardData>();

        // Band 1 (6)
        AddCopies(deck, 3, () => CreateCard("Fire", 1, TargetType.SingleEnemy, new DamageEffect { Amount = 9 }));
        AddCopies(deck, 3, () => CreateCard("Ice", 1, TargetType.SingleEnemy, new DamageEffect { Amount = 7 }));

        // Band 2 (5)
        AddCopies(deck, 3, () => CreateStatusCard("Slow", 1, TargetType.SingleEnemy, StatusEffectType.Slow, "Slow", duration: 2, magnitude: 2));
        AddCopies(deck, 2, () => CreateCard("Water", 2, TargetType.AllEnemies, new DamageEffect { Amount = 6 }));

        // Band 3 (4)
        AddCopies(deck, 4, () => CreateCard("Thunder", 2, TargetType.SingleEnemy, new DamageEffect { Amount = 14 }));

        // Band 4 (3)
        AddCopies(deck, 3, () => CreateCard("Flare", 3, TargetType.SingleEnemy, new DamageEffect { Amount = 22 }));

        // Band 5 (2)
        AddCopies(deck, 2, () => CreateCard("Comet", 3, TargetType.SingleEnemy, new DamageEffect { Amount = 26 }));

        return deck;
    }

    private List<CardData> BuildRedMageDeck()
    {
        List<CardData> deck = new List<CardData>();

        // Band 1 (6) — Fire and Cure are the literal shared White/Black Mage spells,
        // reflecting Red Mage's hybrid access to basic white and black magic.
        AddCopies(deck, 2, () => CreateCard("Swordplay", 1, TargetType.SingleEnemy, new DamageEffect { Amount = 7 }));
        AddCopies(deck, 2, () => CreateCard("Fire", 1, TargetType.SingleEnemy, new DamageEffect { Amount = 9 }));
        AddCopies(deck, 2, () => CreateCard("Cure", 1, TargetType.SingleAlly, new HealEffect { Amount = 10 }));

        // Band 2 (5)
        AddCopies(deck, 2, () => CreateStatusCard("Haste", 2, TargetType.Self, StatusEffectType.Haste, "Haste", duration: 2, magnitude: 1));
        AddCopies(deck, 3, () => CreateCard("Dia", 2, TargetType.SingleEnemy, new DamageEffect { Amount = 13 }));

        // Band 3 (4)
        AddCopies(deck, 4, () => CreateCard("Blizzara", 2, TargetType.SingleEnemy, new DamageEffect { Amount = 14 }));

        // Band 4 (3)
        AddCopies(deck, 3, () => CreateCard("Cura", 2, TargetType.SingleAlly, new HealEffect { Amount = 18 }));

        // Band 5 (2)
        AddCopies(deck, 2, () => CreateCard("Grand Cross", 3, TargetType.SingleEnemy, new DamageEffect { Amount = 24 }));

        return deck;
    }

    private List<CardData> BuildThiefDeck()
    {
        List<CardData> deck = new List<CardData>();

        // Band 1 (6)
        AddCopies(deck, 3, () => CreateCard("Quick Strike", 1, TargetType.SingleEnemy, new DamageEffect { Amount = 6 }));
        AddCopies(deck, 3, () => CreateCard("Mug", 1, TargetType.SingleEnemy, new DamageEffect { Amount = 5 }, new DrainEnergyEffect { Amount = 1 }));

        // Band 2 (5)
        AddCopies(deck, 2, () => CreateCard("Trick", 1, TargetType.SingleEnemy, new DrainEnergyEffect { Amount = 2 }));
        AddCopies(deck, 3, () => CreateCard("Sneak Attack", 1, TargetType.SingleEnemy, new DamageEffect { Amount = 9 }));

        // Band 3 (4)
        AddCopies(deck, 4, () => CreateCard("Backstab", 2, TargetType.SingleEnemy, new DamageEffect { Amount = 16 }));

        // Band 4 (3)
        AddCopies(deck, 3, () => CreateCard("Assassinate", 2, TargetType.SingleEnemy, new DamageEffect { Amount = 20 }));

        // Band 5 (2)
        AddCopies(deck, 2, () => CreateCard("Vendetta", 3, TargetType.SingleEnemy, new DamageEffect { Amount = 30 }));

        return deck;
    }

    private List<CardData> BuildMonkDeck()
    {
        List<CardData> deck = new List<CardData>();

        // Band 1 (6)
        AddCopies(deck, 3, () => CreateCard("Punch", 1, TargetType.SingleEnemy, new DamageEffect { Amount = 7 }));
        AddCopies(deck, 3, () => CreateStatusCard("Meditate", 1, TargetType.Self, StatusEffectType.Regen, "Regen", duration: 3, magnitude: 5));

        // Band 2 (5)
        AddCopies(deck, 2, () => CreateStatusCard("Iron Skin", 2, TargetType.Self, StatusEffectType.Protect, "Protect", duration: 3, magnitude: 4));
        AddCopies(deck, 3, () => CreateCard("Chi Blast", 1, TargetType.SingleEnemy, new DamageEffect { Amount = 9 }));

        // Band 3 (4)
        AddCopies(deck, 4, () => CreateCard("Barrage", 2, TargetType.AllEnemies, new DamageEffect { Amount = 6 }));

        // Band 4 (3)
        AddCopies(deck, 3, () => CreateCard("Combo Strike", 2, TargetType.SingleEnemy, new DamageEffect { Amount = 18 }));

        // Band 5 (2)
        AddCopies(deck, 2, () => CreateCard("Dragon Kick", 3, TargetType.SingleEnemy, new DamageEffect { Amount = 28 }));

        return deck;
    }

    private List<CardData> BuildSoldierDeck()
    {
        List<CardData> deck = new List<CardData>();

        // Band 1 (6)
        AddCopies(deck, 3, () => CreateCard("Slash", 1, TargetType.SingleEnemy, new DamageEffect { Amount = 8 }));
        AddCopies(deck, 3, () => CreateCard("Thrust", 1, TargetType.SingleEnemy, new DamageEffect { Amount = 7 }));

        // Band 2 (5)
        AddCopies(deck, 2, () => CreateStatusCard("Guard Stance", 1, TargetType.Self, StatusEffectType.Protect, "Protect", duration: 2, magnitude: 3));
        AddCopies(deck, 3, () => CreateCard("Shield Bash", 1, TargetType.SingleEnemy, new DamageEffect { Amount = 9 }));

        // Band 3 (4)
        AddCopies(deck, 4, () => CreateCard("Cross-Slash", 2, TargetType.SingleEnemy,
            new DamageEffect { Amount = 10 },
            new ApplyStatusEffectCard { EffectToApply = MakeStatus("Slow", StatusEffectType.Slow, duration: 2, magnitude: 2) }));

        // Band 4 (3)
        AddCopies(deck, 3, () => CreateCard("Braver", 2, TargetType.SingleEnemy, new DamageEffect { Amount = 16 }));

        // Band 5 (2)
        AddCopies(deck, 2, () => CreateCard("Limit Break", 3, TargetType.SingleEnemy, new DamageEffect { Amount = 32 }));

        return deck;
    }

    private void AddCopies(List<CardData> deck, int count, System.Func<CardData> factory)
    {
        for (int i = 0; i < count; i++)
        {
            deck.Add(factory());
        }
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
