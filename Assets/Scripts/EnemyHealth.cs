using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{

    private Animator animator;
    private Transform aimTarget;
    [SerializeField] float hitPoints = 100f;


    void Start() { 
        animator = GetComponent<Animator>();
        aimTarget= gameObject.transform.Find("AimTarget");
    }


    public void TakeDamage(float damage)
    {
       hitPoints -= damage;
        animator.SetTrigger("shot");
        if (hitPoints <= 0)
        {
            Destroy(transform.root.gameObject, 3f);
            aimTarget.gameObject.SetActive(false);
            animator.SetBool("death", true); 
        }
    }
}
