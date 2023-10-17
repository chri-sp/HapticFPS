using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBreathing : MonoBehaviour {

	[Header("Weapon Breathing")]
	public Transform weapons;

	public float swayAmountA = 1;
	public float swayAmountB = 2;
	public float swayScale = 100;
	public float swayLerpSpeed = 14;

	public float swayTime;

	public Vector3 swayPosition;

    private Vector3 initialWeaponPosition;

    // Use this for initialization
    void Start () {
        initialWeaponPosition = weapons.localPosition;
    }
	
	// Update is called once per frame
	void Update () {
		CalculateWeaponSwayBreathing();
	}

	private void CalculateWeaponSwayBreathing ()
	{
		var targetPosition = LissajousCurve(swayTime, swayAmountA, swayAmountB) / swayScale;

		swayPosition = Vector3.Lerp(swayPosition, targetPosition, Time.smoothDeltaTime* swayLerpSpeed);

		swayTime += Time.deltaTime;

        weapons.localPosition = initialWeaponPosition + swayPosition;
    }


    private Vector3 LissajousCurve(float Time, float A, float B)
    {
        return new Vector3(Mathf.Sin(Time), A * Mathf.Sin(B * Time + Mathf.PI));
    }
}
