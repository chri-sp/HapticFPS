using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class KillPlayerOnCollision : MonoBehaviour
{

    private DeathHandler deathHandler;

    void Start()
    {
        deathHandler = GameObject.FindWithTag("GameEvents").GetComponent<DeathHandler>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            deathHandler.HandleDeath();
        }

    }
}
