using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour {

    [SerializeField] float damage = 40f;
    private EnemyAI enemyAI;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();  
        enemyAI = GetComponent<EnemyAI>(); 
    }

    void Update() {
        Attack();
    }

    public void Attack() {
        
        //se abbastanza vicino effettua attacco o si avvicina ulteriormente
        if (enemyAI.isClose())
        {
            animator.SetBool("isAttacking", true);
        }

        else
        {
            animator.SetBool("isAttacking", false);
        }

    }

    //Metodo chiamato come evento durante animazione attacco
    public void hitEvent() {
        if (enemyAI.isClose())
        {
            Debug.Log("Danneggiato");
        }
    }

}
