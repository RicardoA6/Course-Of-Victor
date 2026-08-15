using UnityEngine;

public enum StatusEffectType
{
    Regen,
    Protect,
    Shell,
    Slow,
    Haste
}

[System.Serializable]
public class StatusEffect
{
    public string EffectName;
    public StatusEffectType EffectType;
    public int Duration;
    public int Magnitude;

    public bool IsExpired => Duration <= 0;

    public void OnTurnStart(Player owner)
    {
        switch (EffectType)
        {
            case StatusEffectType.Regen:
                owner.Heal(Magnitude);
                break;
            case StatusEffectType.Slow:
                owner.CurrentEnergy = Mathf.Max(0, owner.CurrentEnergy - Magnitude);
                break;
            case StatusEffectType.Haste:
                owner.CurrentEnergy += Magnitude;
                break;
        }
    }

    public void OnTurnEnd(Player owner)
    {
        Duration--;
    }

    public StatusEffect Clone()
    {
        return new StatusEffect
        {
            EffectName = EffectName,
            EffectType = EffectType,
            Duration = Duration,
            Magnitude = Magnitude
        };
    }
}
