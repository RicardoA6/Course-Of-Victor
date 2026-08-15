using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player
{
    public string PlayerName;
    public int CurrentHealth;
    public int MaxHealth;
    public int CurrentEnergy;
    public int MaxEnergy;
    public Deck Deck;
    public List<StatusEffect> ActiveStatusEffects = new List<StatusEffect>();

    private const int CARDS_PER_TURN = 5;

    public Player(string playerName, int maxHealth, int maxEnergy, Deck deck)
    {
        PlayerName = playerName;
        MaxHealth = maxHealth;
        CurrentHealth = maxHealth;
        MaxEnergy = maxEnergy;
        Deck = deck;
    }

    public bool IsAlive => CurrentHealth > 0;

    public void StartTurn()
    {
        CurrentEnergy = MaxEnergy;
        Deck.DrawCards(CARDS_PER_TURN);

        foreach (StatusEffect effect in ActiveStatusEffects)
        {
            effect.OnTurnStart(this);
        }
    }

    public void EndTurn()
    {
        Deck.DiscardHand();

        for (int i = ActiveStatusEffects.Count - 1; i >= 0; i--)
        {
            ActiveStatusEffects[i].OnTurnEnd(this);
            if (ActiveStatusEffects[i].IsExpired)
            {
                ActiveStatusEffects.RemoveAt(i);
            }
        }
    }

    public bool CanPlay(CardData card)
    {
        return CurrentEnergy >= card.EnergyCost;
    }

    public void PlayCard(CardData card, List<Player> targets)
    {
        if (!CanPlay(card))
        {
            Debug.LogWarning($"{PlayerName} cannot afford {card.CardName}");
            return;
        }

        CurrentEnergy -= card.EnergyCost;

        foreach (CardEffect effect in card.Effects)
        {
            foreach (Player target in targets)
            {
                effect.Apply(this, target);
            }
        }

        Deck.PlayFromHand(card);
    }

    public void TakeDamage(int amount)
    {
        amount = AbsorbWithShell(amount);
        amount = ReduceWithProtect(amount);
        CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
    }

    private int AbsorbWithShell(int amount)
    {
        if (amount <= 0)
        {
            return amount;
        }

        StatusEffect shell = ActiveStatusEffects.FirstOrDefault(e => e.EffectType == StatusEffectType.Shell);
        if (shell == null)
        {
            return amount;
        }

        int absorbed = Mathf.Min(shell.Magnitude, amount);
        shell.Magnitude -= absorbed;
        amount -= absorbed;

        if (shell.Magnitude <= 0)
        {
            ActiveStatusEffects.Remove(shell);
        }

        return amount;
    }

    private int ReduceWithProtect(int amount)
    {
        int reduction = ActiveStatusEffects
            .Where(e => e.EffectType == StatusEffectType.Protect)
            .Sum(e => e.Magnitude);

        return Mathf.Max(0, amount - reduction);
    }

    public void Heal(int amount)
    {
        CurrentHealth = Mathf.Min(MaxHealth, CurrentHealth + amount);
    }

    public void ApplyStatusEffect(StatusEffect effect)
    {
        ActiveStatusEffects.Add(effect.Clone());
    }
}
