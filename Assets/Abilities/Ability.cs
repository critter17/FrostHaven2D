using System;
using UnityEngine;

public abstract class Ability : ScriptableObject, IAbility
{
    public AbilityType abilityType;
    public int value;
    public int range;
    public int numberOfTargets;
    public bool persistent;
    public bool lose;
    public bool remove;

    public abstract void UseAbility();
    public abstract void StopAbility();
}

public enum AbilityType
{
    Move,
    Attack,
    Shield,
    Retaliate
}
