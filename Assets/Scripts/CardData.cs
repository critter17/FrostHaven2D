using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CardData : ScriptableObject
{
    public List<Ability> topAction;
    public List<Ability> bottomAction;
    public Ability basicAttack;
    public Ability basicMove;
}
