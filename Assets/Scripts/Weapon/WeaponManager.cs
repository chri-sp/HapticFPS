using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour {
	private Weapon weapon;
	public delegate void OnWeaponChanged(Weapon weapon);
	public event OnWeaponChanged onWeaponChanged;

    public void updateWeapon() {
        weapon = GameObject.FindWithTag("Weapon").GetComponent<Weapon>();
        onWeaponChanged.Invoke(weapon);
    }

	// Use this for initialization
	void Start () {
		weapon = GameObject.FindWithTag("Weapon").GetComponent<Weapon>();
    }

	public Weapon currentWeapon() { 
		return weapon;
	}
	
}
