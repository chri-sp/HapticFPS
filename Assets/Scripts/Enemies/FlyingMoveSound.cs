using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingMoveSound : MonoBehaviour
{

    private AudioController audioController;

    // Use this for initialization
    void Start()
    {
        audioController = GetComponent<AudioController>();
        audioController.Play("footstep");
    }

}
