using UnityEngine;

[System.Serializable]
public abstract class CardEffect
{
    public abstract void Apply(Player source, Player target);
}

[System.Serializable]
public class DamageEffect : CardEffect
{
    public int Amount;

    public override void Apply(Player source, Player target)
    {
        target.TakeDamage(Amount);
    }
}

[System.Serializable]
public class HealEffect : CardEffect
{
    public int Amount;

    public override void Apply(Player source, Player target)
    {
        target.Heal(Amount);
    }
}

[System.Serializable]
public class ApplyStatusEffectCard : CardEffect
{
    public StatusEffect EffectToApply;

    public override void Apply(Player source, Player target)
    {
        target.ApplyStatusEffect(EffectToApply);
    }
}
