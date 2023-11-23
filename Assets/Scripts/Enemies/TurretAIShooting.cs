using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretAIShooting : MonoBehaviour
{
    private Animator animator;
    private Weapon weapon;

    [Header("Settings")]
    private float stoppingDistanceShooting;
    private float InitialStoppingDistanceShooting;
    public float startWaitTime = 4;                 //  Wait time of every action
    public float timeToRotate = 2;                  //  Wait time when the enemy detect near the player without seeing

    public float viewRadius = 15;                   //  Radius of the enemy view
    public float viewAngle = 90;                    //  Angle of the enemy view
    public LayerMask playerMask;                    //  To detect the player with the raycast
    public LayerMask obstacleMask;                  //  To detect the obstacules with the raycast
    public float meshResolution = 1.0f;             //  How many rays will cast per degree
    public int edgeIterations = 4;                  //  Number of iterations to get a better performance of the mesh filter when the raycast hit an obstacule
    public float edgeDistance = 0.5f;               //  Max distance to calcule the a minumun and a maximum raycast when hits something

    bool m_playerInRange;                           //  If the player is in range of vision, state of chasing

    private Transform m_Player;

    bool isLooking = false;


    void Start()
    {
        m_playerInRange = false;
        animator = GetComponent<Animator>();

        m_Player = GameObject.FindGameObjectWithTag("Player").transform;
        weapon = GameObject.FindWithTag("Weapon").GetComponent<Weapon>();
        stoppingDistanceShooting = viewRadius;
        InitialStoppingDistanceShooting = stoppingDistanceShooting;
        randomDistanceShooting();
    }

    private void Update()
    {
        if (!isDead())
        {
            EnviromentView();                       //  Check whether or not the player is in the enemy's field of vision

            Alerted();
              
            Animations();
        }
    }

    private void Animations() {

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            StartCoroutine(lookPlayer(1f));
        }
    }


    private void Alerted()
    {
        if (weapon.hasShooted && Vector3.Distance(m_Player.position, transform.position)<=viewRadius)
        {
            StartCoroutine(lookPlayer(1f));
        }        
    }


    IEnumerator lookPlayer(float rotationTime) {
        if (!isLooking) {
            isLooking = true;
            Vector3 dir = m_Player.position - transform.position;

            Quaternion lookRotation = Quaternion.LookRotation(dir);
            lookRotation.x = 0;
            lookRotation.z = 0;

            float elapsedTime = 0;

            while (elapsedTime < rotationTime)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, (elapsedTime / rotationTime));
                elapsedTime += Time.deltaTime;

                // Yield here
                yield return null;
            }
            isLooking = false;
        }
    }


    private bool isDead() {
        if (animator.GetBool("death").Equals(true)) {
            return true;
        }
        return false;   
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, viewRadius);
    }

    public bool isViewing() {
        if (m_playerInRange && Vector3.Distance(transform.position, m_Player.position) <= stoppingDistanceShooting)
        {
            return true;
        }
        else {
            return false;
        }
    }

    private void randomDistanceShooting() {
        Random.seed = System.DateTime.Now.Millisecond;
        stoppingDistanceShooting = Random.Range(InitialStoppingDistanceShooting, InitialStoppingDistanceShooting + 10f);
    }

    void EnviromentView()
    {
        Collider[] playerInRange = Physics.OverlapSphere(transform.position, viewRadius, playerMask);   //  Make an overlap sphere around the enemy to detect the playermask in the view radius

        for (int i = 0; i < playerInRange.Length; i++)
        {
            Transform player = playerInRange[i].transform;
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToPlayer) < viewAngle / 2)
            {
                float dstToPlayer = Vector3.Distance(transform.position, player.position);          //  Distance of the enmy and the player
                if (!Physics.Raycast(transform.position, dirToPlayer, dstToPlayer, obstacleMask))
                {
                    m_playerInRange = true;             //  The player has been seeing by the enemy and then the nemy starts to chasing the player
                }
                else
                {
                    /*
                     *  If the player is behind a obstacle the player position will not be registered
                     * */
                    m_playerInRange = false;
                }
            }
            if (Vector3.Distance(transform.position, player.position) > viewRadius)
            {
                /*
                 *  If the player is further than the view radius, then the enemy will no longer keep the player's current position.
                 *  Or the enemy is a safe zone, the enemy will no chase
                 * */
                m_playerInRange = false;                //  Change the sate of chasing
            }
        }
    }
}