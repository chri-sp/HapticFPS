using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMusic : MonoBehaviour
{
    private AudioManager audioManager;

    void Start()
    {
        audioManager = GameObject.FindWithTag("AudioSystem").GetComponent<AudioManager>();
        audioManager.Play("BGMusic");
    }

}
