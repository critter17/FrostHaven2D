using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ScenarioManager : MonoBehaviour
{
    List<Ability> abilitiesToUse = new List<Ability>();
    int abilityIndex = 0;

    public void AddAbilities(List<Ability> abilities)
    {
        foreach (var ability in abilities)
        {
            abilitiesToUse.Add(ability);
        }

        UseNextAbility();
    }

    public void UseNextAbility()
    {
        if (abilityIndex >= abilitiesToUse.Count)
        {
            Debug.Log("All actions complete");
            abilitiesToUse[abilityIndex - 1].StopAbility();
            abilitiesToUse.Clear();
            return;
        }

        Ability currentAbility = abilitiesToUse[abilityIndex++];
        currentAbility.UseAbility();
    }
}
