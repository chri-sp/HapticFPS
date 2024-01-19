﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recoil : MonoBehaviour
{

	//Rotations
	private Vector3 currentRotation;
    private Vector3 targetRotation;

    //Weapon
    private Weapon weapon;
    private WeaponManager weaponManager;

    void Start()
    {
        weapon = GameObject.FindWithTag("Weapon").GetComponent<Weapon>();
        weaponManager = GameObject.FindWithTag("WeaponHolder").GetComponent<WeaponManager>();
        weaponManager.onWeaponChanged += weaponChanged;
    }

    void weaponChanged(Weapon newWeapon) {
        weapon = newWeapon;
    }

    void Update () {
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, weapon.returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, weapon.snappiness * Time.fixedDeltaTime);
        transform.localRotation = Quaternion.Euler(currentRotation);
    }

    public void RecoilFire() {
        targetRotation += new Vector3(weapon.recoilX, Random.Range(-weapon.recoilY, weapon.recoilY), Random.Range(-weapon.recoilZ, weapon.recoilZ));
    }
}
