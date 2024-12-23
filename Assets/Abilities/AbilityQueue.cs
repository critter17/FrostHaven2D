using System.Collections.Generic;
using UnityEngine;

public class AbilityQueue : MonoBehaviour
{
    private Queue<int> abilityQueue = new Queue<int>();

    public void AddAbility(int ability)
    {
        abilityQueue.Enqueue(ability);
    }
}
