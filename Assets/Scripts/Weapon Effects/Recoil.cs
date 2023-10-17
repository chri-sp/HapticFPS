using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recoil : MonoBehaviour
{

	//Rotations
	private Vector3 currentRotation;
    private Vector3 targetRotation;

    //Weapon
    private Weapon weapon;

    void Start()
    {
        /*NOTA: ad ogni cambio di arma dovrò avere il nuovo riferimento 
         * allo script specifico (quindi con i suoi valori di recoil) dell'arma attiva
        */
        weapon = GameObject.Find("Pistol").GetComponent<Weapon>();
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
