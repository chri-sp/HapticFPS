using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class DeathHandler : MonoBehaviour
{

    [SerializeField] GameObject gameOver;
    [SerializeField] GameObject hapticWorkspace;
    private FirstPersonControllerFalcon player;
    private HapticProbeFPS controller;
    private Weapon weapon;

    private void Start()
    {
        gameOver.active = false;
        player = GameObject.FindWithTag("Player").GetComponent<FirstPersonControllerFalcon>();
        weapon = GameObject.FindWithTag("Weapon").GetComponent<Weapon>();
        controller = GameObject.FindWithTag("Player").GetComponent<HapticProbeFPS>();
    }

    public void HandleDeath()
    {
        gameOver.active = true;
        Time.timeScale = 0;
        player.enabled = false;
        weapon.enabled = false;
        Destroy(hapticWorkspace);
        Destroy(controller);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
