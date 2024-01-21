using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Victory : MonoBehaviour {

    private VictoryMenu menu;
    private AudioSource audioSource;

    void Start()
    {
        menu = GameObject.FindWithTag("GameEvents").GetComponent<VictoryMenu>();
        audioSource = GetComponent<AudioSource>();
    }


    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player"))
        {
            audioSource.Play();
            menu.OpenVictoryMenu();
        }        
    }

}
