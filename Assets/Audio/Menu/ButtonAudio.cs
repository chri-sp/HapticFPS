using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAudio : MonoBehaviour
{

    private AudioManager audioManager;
    private Animator animator;

    [HideInInspector] private bool oneTimeAudio = false;
    void Start()
    {
        audioManager = GameObject.FindWithTag("AudioSystem").GetComponent<AudioManager>();
        animator = gameObject.GetComponent<Animator>();
    }


    void Update()
    {
        if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Equals("Normal")) oneTimeAudio = false;
        else if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Equals("Highlighted")) {
            audioOnButton();
        }
    }
    void audioOnButton()
    {
        if (!oneTimeAudio)
        {
            oneTimeAudio = true;
            soundButton();
        }
    }

    void soundButton()
    {
        if (!audioManager.IsPlaying("Button"))
            audioManager.Play("Button");
    }
}
