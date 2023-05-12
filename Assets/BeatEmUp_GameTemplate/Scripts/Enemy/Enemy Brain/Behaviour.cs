using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Behaviour
{
    public abstract void updateBehaviour(GameObject enemy, EnemyAI ai);
}
