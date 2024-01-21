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

    [SerializeField] private static bool rifle = false;

    [SerializeField] private static bool shotgun = false;

    public bool debugRifle = false;
    public bool debugShotgun = false;


    // Use this for initialization
    void Start()
    {
        selectWeapon();
        handsPosition = GameObject.FindWithTag("CharacterArms").GetComponents<InverseKinematics>();
        weaponManager = GetComponent<WeaponManager>();
        setHandsPosition();

        if (rifle)
            GameObject.FindWithTag("RifleCollectible").SetActive(false);
        if (shotgun)
            GameObject.FindWithTag("ShotgunCollectible").SetActive(false);
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

        debugWeapon();
    }

    private void debugWeapon() { 
        if (debugRifle)
            rifle = true;
        if (debugShotgun)
            shotgun = true;
    }

    public void enableRifle() { 
        rifle = true;
    }

    public void enableShotgun() {
        shotgun = true;
    }

    public void disableRifle()
    {
        rifle = false;
    }

    public void disableShotgun()
    {
        shotgun = false;
    }

    private void weaponSwitchingFalcon()
    {
        //Uso come input il falcon
        if (controller.isActive() && controller.buttonWasPressed(1))
        {
            int initialSelectedWeapon = selectedWeapon;

            selectedWeapon++;
            selectedWeapon %= transform.childCount;

            disableSwitchOnWeapon(initialSelectedWeapon);

            if (initialSelectedWeapon != selectedWeapon)
                StartCoroutine(controller.changeWeaponHapticFeedback());
        }
    }

    IEnumerator switchWeapon()
    {
        if (!isSwitching) { 
            isSwitching = true;
            Animator weaponAnimator = GameObject.FindWithTag("Weapon").GetComponent<Animator>();
            weaponAnimator.Play("PutWeapon");
            Weapon currentWeapon = weaponManager.currentWeapon();
            yield return new WaitForSeconds(.4f);      
            selectWeapon();
            setHandsPosition();    
            weaponManager.updateWeapon();
            //se non è avvenuto cambio arma effettivo riporto arma corrente in posizione iniziale
            if (currentWeapon == weaponManager.currentWeapon())
                weaponAnimator.Play("WeaponSwitched");
            isSwitching = false;
        }
        
    }

    private void weaponSwitchingByNumber()
    {

        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            selectedWeapon = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && rifle)
        {
            selectedWeapon = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && shotgun)
        {
            selectedWeapon = 2;
        }
    }

    private void weaponSwitchingScrollWheel() {

        int initialSelectedWeapon = selectedWeapon;
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

        disableSwitchOnWeapon(initialSelectedWeapon);
    }

    //evita cambio arma se non ancora sbloccata
    private void disableSwitchOnWeapon(int initialSelectedWeapon) {
        if (selectedWeapon == 1 && !rifle)
            selectedWeapon = initialSelectedWeapon;
        if (selectedWeapon == 2 && !shotgun)
            selectedWeapon = initialSelectedWeapon;
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
