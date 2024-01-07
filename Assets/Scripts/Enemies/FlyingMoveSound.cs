using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingMoveSound : MonoBehaviour
{

    private AudioController audioController;

    private EnemyHealth health;

    // Use this for initialization
    void Start()
    {
        audioController = GetComponent<AudioController>();
        health = GetComponent<EnemyHealth>();
        audioController.Play("footstep");
    }

    void Update() {
        if (health.fractionRemaining() <= 0) {
            audioController.StopPlaying("footstep");
        }
    
    }
}
