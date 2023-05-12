using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Brain : MonoBehaviour
{
    public List<Behaviour> behaviours;

    private void Start()
    {
        behaviours = new List<Behaviour>();
        behaviours.Add(new Idle_Behaviour());
        behaviours.Add(new Attack_Behaviour());
    }

    private void Update()
    {
        foreach (var behaviour in behaviours)
        {
            behaviour.updateBehaviour(this.gameObject, GetComponent<EnemyAI>());
        }
    }
}
