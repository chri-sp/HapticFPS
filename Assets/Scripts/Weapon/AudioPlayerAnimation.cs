using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayerAnimation : MonoBehaviour {

    private AudioManager audioManager;
    
    
    // Use this for initialization
    void Start () {
        audioManager = GameObject.FindWithTag("AudioSystem").GetComponent<AudioManager>();
    }

    public void TakeWeaponAudio() {
        audioManager.Play("Take"+this.name);
    }

    public void PutWeaponAudio()
    {
        audioManager.Play("Put" + this.name);
    }

}
