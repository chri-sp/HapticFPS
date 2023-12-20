﻿using System.Collections;
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
    private WeaponManager weaponManager;

    private void Start()
    {
        gameOver.SetActive(false);
        player = GameObject.FindWithTag("Player").GetComponent<FirstPersonControllerFalcon>();
        weapon = GameObject.FindWithTag("Weapon").GetComponent<Weapon>();
        controller = GameObject.FindWithTag("Player").GetComponent<HapticProbeFPS>();
        weaponManager = GameObject.FindWithTag("WeaponHolder").GetComponent<WeaponManager>();
        weaponManager.onWeaponChanged += weaponChanged;
    }

    void weaponChanged(Weapon newWeapon)
    {
        weapon = newWeapon;
    }

    public void HandleDeath()
    {
        gameOver.SetActive(true);
        Time.timeScale = 0;
        disableObjectsOnGameOver();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void disableObjectsOnGameOver() {
        player.enabled = false;
        weapon.enabled = false;
        Destroy(hapticWorkspace);
        Destroy(controller);
    }
}
