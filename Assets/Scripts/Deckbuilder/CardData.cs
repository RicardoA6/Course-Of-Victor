using System.Collections.Generic;
using UnityEngine;

public enum TargetType
{
    SingleEnemy,
    AllEnemies,
    Self,
    SingleAlly,
    AllAllies,
    DeadAlly
}

[CreateAssetMenu(menuName = "Deckbuilder/Card")]
public class CardData : ScriptableObject
{
    public string CardName;
    public int EnergyCost;
    public TargetType Target;

    [SerializeReference]
    public List<CardEffect> Effects = new List<CardEffect>();
}
