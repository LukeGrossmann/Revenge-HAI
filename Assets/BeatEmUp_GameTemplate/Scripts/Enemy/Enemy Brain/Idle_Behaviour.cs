using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle_Behaviour : Behaviour
{
    public override void updateBehaviour(GameObject enemy, EnemyAI ai)
    {
        // if the enemy is a certain distance from the player go into idle.
        //if (ai.range >= RANGE.FARRANGE)
        //{
        //    ai.SetEnemyTactic(ENEMYTACTIC.STANDSTILL);
        //}
    }
}
