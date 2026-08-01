[System.Serializable]
public class StatusEffect
{
    public string EffectName;
    public int Duration;
    public int Magnitude;

    public bool IsExpired => Duration <= 0;

    public void OnTurnStart(Player owner)
    {
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
            Duration = Duration,
            Magnitude = Magnitude
        };
    }
}
