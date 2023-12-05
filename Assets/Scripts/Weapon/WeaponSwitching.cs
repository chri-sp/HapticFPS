using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitching : MonoBehaviour
{
    private HapticProbeFPS controller;

    private InverseKinematics[] handsPosition;

    private WeaponManager weaponManager;

    public int selectedWeapon = 0;

    private bool isSwitching = false;

    // Use this for initialization
    void Start()
    {
        selectWeapon();
        handsPosition = GameObject.FindWithTag("CharacterArms").GetComponents<InverseKinematics>();
        weaponManager = GetComponent<WeaponManager>();
        setHandsPosition();
    }

    private void setHandsPosition()
    {
        Transform rightHandPosition;
        Transform leftHandPosition;
        //Mano destra
        rightHandPosition = GameObject.FindWithTag("Weapon").transform.GetChild(0).GetChild(0).transform;
        //Mano sinistra
        leftHandPosition = GameObject.FindWithTag("Weapon").transform.GetChild(0).GetChild(1).transform;

        handsPosition[0].setHandPosition(leftHandPosition);
        handsPosition[1].setHandPosition(rightHandPosition);

        controller = GameObject.FindWithTag("Player").GetComponent<HapticProbeFPS>();
    }

    // Update is called once per frame
    void Update()
    {
        int previousSelectedWeapon = selectedWeapon;
        weaponSwitchingScrollWheel();
        weaponSwitchingByNumber();
        weaponSwitchingFalcon();

        if (previousSelectedWeapon != selectedWeapon) {
            StartCoroutine(switchWeapon());
        }
    }

    private void weaponSwitchingFalcon()
    {
        //Uso come input il falcon
        if (controller.isActive() && controller.buttonWasPressed(1))
        {
            selectedWeapon++;
            selectedWeapon %= transform.childCount;
            StartCoroutine(controller.changeWeaponHapticFeedback());
        }
    }

    IEnumerator switchWeapon()
    {
        if (!isSwitching) { 
            isSwitching = true;
            Animator weaponAnimator = GameObject.FindWithTag("Weapon").GetComponent<Animator>();
            weaponAnimator.Play("PutWeapon");
            yield return new WaitForSeconds(.4f);      
            selectWeapon();
            setHandsPosition();
            weaponManager.updateWeapon();
            isSwitching = false;
        }
        
    }

    private void weaponSwitchingByNumber()
    {

        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            selectedWeapon = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedWeapon = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            selectedWeapon = 2;
        }
    }

    private void weaponSwitchingScrollWheel() {
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            selectedWeapon++;
            selectedWeapon %= transform.childCount;
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            if (selectedWeapon <= 0)
                selectedWeapon = transform.childCount - 1;
            else
                selectedWeapon--;
        }
    }

    private void selectWeapon()
    {
        int i = 0;
        foreach (Transform weapon in transform)
        {
            if (i == selectedWeapon)
                weapon.gameObject.SetActive(true);
            else
                weapon.gameObject.SetActive(false);
            i++;
        }
    }
}
