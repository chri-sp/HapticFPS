using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRotation : MonoBehaviour {

    [SerializeField] private float rotationSpeed = 10.0f;

    private AudioSource audioSource;

    private WeaponSwitching weaponSwitching;

    // Use this for initialization
    void Start()
    {
        rotationSpeed = Random.Range(rotationSpeed, rotationSpeed * 1.5f);
        audioSource = GetComponent<AudioSource>();
        weaponSwitching = GameObject.FindWithTag("WeaponHolder").GetComponent<WeaponSwitching>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * Time.deltaTime * rotationSpeed, Space.World);
    }

    void OnTriggerEnter()
    {
        StartCoroutine(OnTrigger());
    }

    private IEnumerator OnTrigger()
    {
        if(this.name.Contains("Rifle"))
            weaponSwitching.enableRifle();
        else
            weaponSwitching.enableShotgun();

        GetComponentInChildren<MeshRenderer>().enabled = false;
        audioSource.Play();
        yield return new WaitForSeconds(.5f);
        Destroy(gameObject);
    }
}
