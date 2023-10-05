using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] HapticProbeFPS controller;
    [SerializeField] Camera FirstPersonCamera;
    [SerializeField] float range = 100f;
    [SerializeField] float damage = 30f;

    void Update()
    {
        //Uso come input il falcon
        if (controller.isActive() && controller.getButtonState(0))
        {
            Shoot();
        }
        //Uso come input il mouse
        else if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(FirstPersonCamera.transform.position, FirstPersonCamera.transform.forward, out hit, range))
        {
            Debug.Log("Colpito: " + hit.transform.name);

            EnemyHealth target = hit.transform.GetComponent<EnemyHealth>();
            //evito NullReference se colpisco un oggetto senza lo script EnemyHealth 
            if (target == null) return;
            target.TakeDamage(damage);
        }

    }
}
