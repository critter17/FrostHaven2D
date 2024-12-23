using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class MoveAbility : Ability
{
    public bool jump;
    public bool flying;

    public override void UseAbility()
    {
        GridManager gridManager = FindObjectOfType<GridManager>();
        gridManager.StartMoveAbility(value);
    }

    public override void StopAbility()
    {
        GridManager gridManager = FindObjectOfType<GridManager>();
        gridManager.StopMoveAbility();
    }
}
