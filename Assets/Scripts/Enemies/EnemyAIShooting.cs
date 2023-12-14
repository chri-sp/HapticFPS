using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SocialPlatforms;

public class EnemyAIShooting : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;               //  Nav mesh agent component
    private Animator animator;
    private EnemyHealth healt;
    private Weapon weapon;
    private WeaponManager weaponManager;

    [Header("Settings")]
    public float stoppingDistanceShooting;
    private float InitialStoppingDistanceShooting;
    public float startWaitTime = 4;                 //  Wait time of every action
    public float timeToRotate = 2;                  //  Wait time when the enemy detect near the player without seeing
    public float speedWalk = 6;                     //  Walking speed, speed in the nav mesh agent
    public float speedRun = 9;                      //  Running speed

    public float viewRadius = 15;                   //  Radius of the enemy view
    public float viewAngle = 90;                    //  Angle of the enemy view
    public LayerMask playerMask;                    //  To detect the player with the raycast
    public LayerMask obstacleMask;                  //  To detect the obstacules with the raycast
    public float meshResolution = 1.0f;             //  How many rays will cast per degree
    public int edgeIterations = 4;                  //  Number of iterations to get a better performance of the mesh filter when the raycast hit an obstacule
    public float edgeDistance = 0.5f;               //  Max distance to calcule the a minumun and a maximum raycast when hits something


    public Transform[] waypoints;                   //  All the waypoints where the enemy patrols
    int m_CurrentWaypointIndex;                     //  Current waypoint where the enemy is going to

    Vector3 playerLastPosition = Vector3.zero;      //  Last position of the player when was near the enemy
    Vector3 m_PlayerPosition;                       //  Last position of the player when the player is seen by the enemy

    float m_WaitTime;                               //  Variable of the wait time that makes the delay
    float m_TimeToRotate;                           //  Variable of the wait time to rotate when the player is near that makes the delay
    bool m_playerInRange;                           //  If the player is in range of vision, state of chasing
    bool m_PlayerNear;                              //  If the player is near, state of hearing
    bool m_IsPatrol;                                //  If the enemy is patrol, state of patroling
    bool m_CaughtPlayer;                            //  if the enemy has caught the player

    private Transform m_Player;

    bool isLooking = false;

    [Header("Dodge settings")]
    //dodgeDuration < resetDodgeDelay
    public float dodgeProbability = 1;
    public float dodgeDistance;
    public float dodgeDuration = .2f;
    Camera FirstPersonCamera;
    private float resetDodgeDelay = 1f;
    private float resetDodgeTimer;
    private bool isDodging = false;

    void Start()
    {
        m_PlayerPosition = Vector3.zero;
        m_IsPatrol = true;
        m_CaughtPlayer = false;
        m_playerInRange = false;
        m_PlayerNear = false;
        m_WaitTime = startWaitTime;                 //  Set the wait time variable that will change
        m_TimeToRotate = timeToRotate;

        m_CurrentWaypointIndex = 0;                 //  Set the initial waypoint
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        navMeshAgent.isStopped = false;
        navMeshAgent.speed = speedWalk;             //  Set the navemesh speed with the normal speed of the enemy
        navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);    //  Set the destination to the first waypoint
        m_Player = GameObject.FindGameObjectWithTag("Player").transform;
        healt = GetComponent<EnemyHealth>();
        weapon = GameObject.FindWithTag("Weapon").GetComponent<Weapon>();
        InitialStoppingDistanceShooting = stoppingDistanceShooting;
        randomDistanceShooting();
        FirstPersonCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        resetDodgeTimer = resetDodgeDelay;
        weaponManager = GameObject.FindWithTag("WeaponHolder").GetComponent<WeaponManager>();
        weaponManager.onWeaponChanged += weaponChanged;
    }
    void weaponChanged(Weapon newWeapon)
    {
        weapon = newWeapon;
    }

    private void Update()
    {
        dodgetTimer();
        if (!isDead())
        {
            EnviromentView();                       //  Check whether or not the player is in the enemy's field of vision

            if (!m_IsPatrol)
            {
                Chasing();
                StartCoroutine(Dodge());
            }
            else
            {
                Patroling();
                Alerted();
            }

            isJumping();
            Animations();
        }
    }

    private void Animations()
    {
        animator.SetFloat("speed", navMeshAgent.desiredVelocity.sqrMagnitude);

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            Stop();
            StartCoroutine(lookPlayer(1f));
        }
    }
    private IEnumerator Dodge()
    {
        //aumento probabilità dodge se nemico ha meno vita
        if (healt.getHit())
        {
            incrementDodgeProbability((1 - healt.fractionRemaining()) / 4);
        }


        float noDodegRadius = 5f;

        //se nemico è troppo vicino al player non effettua dodge
        if (Vector3.Distance(transform.position, m_Player.position) > noDodegRadius)
        {
            RaycastHit hit;
            Vector3 directionToEnemy = FirstPersonCamera.transform.forward;

            //se nemico vede il player ed è passato il tempo di attesa effettua dodge
            if (isViewing() && resetDodgeTimer <= 0 && Physics.Raycast(FirstPersonCamera.transform.position, directionToEnemy, out hit) && !isDodging)
            {
                isDodging = true;
                yield return new WaitForSeconds(.05f);
                //se il player sta mirando a questo nemico
                GameObject enemyIsViewedByPlayer = null;
                if (hit.collider.gameObject.GetComponentInParent<EnemyHealth>() != null)
                {
                    enemyIsViewedByPlayer = hit.collider.gameObject.GetComponentInParent<EnemyHealth>().gameObject;
                }

                if (enemyIsViewedByPlayer == transform.gameObject)
                {
                    if (Random.value <= dodgeProbability)
                    {
                        //cerco una direzione ed una distanza giusta per un massimo di 4 volte
                        Vector3 direction = chooseDodgeDirection();

                        for (int i = 0; i < 4; i++)
                        {
                            if (!canDodge(direction, dodgeDistance))
                            {
                                direction = chooseDodgeDirection();
                            }
                            else
                                break;
                        }

                        //la distanza di schivata viene scelta randomicamente basandosi sulla distanza massima percorribile
                        direction = direction / Random.Range(1f, 2f);

                        StartCoroutine(DodgeMove(direction * dodgeDistance));

                    }
                    resetDodgeTimer = resetDodgeDelay;
                }
                isDodging = false;
            }
        }
    }

    private Vector3 chooseDodgeDirection()
    {
        Vector3 direction;
        float movementChoice = Random.value;
        // Nemico esegue una schivata
        if (movementChoice <= .45f)
        {
            direction = transform.right;
        }
        else if (movementChoice <= .9f)
        {
            direction = -transform.right;
        }
        else
        {
            direction = transform.forward;
        }
        return direction;
    }

    bool canDodge(Vector3 direction, float distance)
    {
        if (passDodgeCollision(direction, distance) && passGroundDodge(direction, distance))
            return true;
        else
            return false;
    }

    bool passDodgeCollision(Vector3 direction, float distance)
    {
        RaycastHit hit;
        Vector3 position = transform.position;
        position.y = position.y + .5f;

        return !Physics.Raycast(position, direction, out hit, distance);
    }

    bool passGroundDodge(Vector3 direction, float distance)
    {
        RaycastHit hit;

        //ottengo posizione attuale oggetto
        Vector3 position = transform.position;
        position.y = position.y + .5f;

        //verifico se esiste un elemento su cui potrà poggiare l'oggetto
        Vector3 finalPosition = position + (direction * distance);
        Vector3 checkGroundPosition = finalPosition;
        checkGroundPosition.y = checkGroundPosition.y - .7f;
        return Physics.Linecast(finalPosition, checkGroundPosition);

    }

    IEnumerator DodgeMove(Vector3 direction)
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = transform.position + direction;

        float countTime = 0;
        while (countTime <= dodgeDuration)
        {
            float percentTime = countTime / dodgeDuration;
            transform.position = Vector3.Lerp(startPos, endPos, percentTime);
            yield return null; // wait for next frame
            countTime += Time.deltaTime;
        }
        // because of the frame rate and the way we are running LERP,
        // the last timePercent in the loop may not = 1
        // therefore, this line ensures we end exactly where desired.
        transform.position = endPos;
    }


    void dodgetTimer()
    {
        if (resetDodgeTimer > 0)
        {
            resetDodgeTimer -= Time.deltaTime;
        }
    }

    private void incrementDodgeProbability(float moreProbability)
    {
        dodgeProbability += moreProbability;
    }
    private void Alerted()
    {
        float currentSpeed;
        if (weapon.hasShooted && Vector3.Distance(transform.position, m_Player.position) < viewRadius)
        {
            currentSpeed = navMeshAgent.speed;
            Stop();
            StartCoroutine(lookPlayer(1f));
        }
        else if (healt.getHit())
        {
            currentSpeed = navMeshAgent.speed;
            Stop();
            StartCoroutine(lookPlayer(2f));
            StartCoroutine(resumeSpeed(2f, currentSpeed));
        }
    }
    IEnumerator resumeSpeed(float seconds, float speed)
    {
        yield return new WaitForSeconds(seconds);
        Move(speed);
    }


    IEnumerator lookPlayer(float rotationTime)
    {
        if (!isLooking)
        {
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

    private void isJumping()
    {
        if (navMeshAgent.isOnOffMeshLink)
        {
            animator.SetBool("jump", true);
            StartCoroutine(lookPlayer(1f));
        }
        else
        {
            animator.SetBool("jump", false);
        }
    }

    private bool isDead()
    {
        if (animator.GetBool("death").Equals(true))
        {
            Stop();
            return true;
        }
        return false;
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, viewRadius);
    }

    public bool isViewing()
    {
        if (m_playerInRange && Vector3.Distance(transform.position, m_Player.position) <= stoppingDistanceShooting)
        {
            Stop();
            return true;
        }
        else
        {
            //randomDistanceShooting();
            return false;
        }
    }

    private void randomDistanceShooting()
    {
        Random.seed = System.DateTime.Now.Millisecond;
        stoppingDistanceShooting = Random.Range(InitialStoppingDistanceShooting, InitialStoppingDistanceShooting + 10f);
    }
    private void Chasing()
    {
        if (!isViewing())
        {
            //  The enemy is chasing the player
            m_PlayerNear = false;                       //  Set false that hte player is near beacause the enemy already sees the player
            playerLastPosition = Vector3.zero;          //  Reset the player near position

            if (weapon.hasShooted && Vector3.Distance(transform.position, m_Player.position) < viewRadius)
            {
                m_PlayerPosition = m_Player.position;
            }

            if (!m_CaughtPlayer)
            {
                Move(speedRun);
                navMeshAgent.SetDestination(m_PlayerPosition);          //  set the destination of the enemy to the player location
            }
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)    //  Control if the enemy arrive to the player location
            {
                if (m_WaitTime <= 0 && !m_CaughtPlayer && Vector3.Distance(transform.position, m_Player.position) >= 6f)
                {
                    //  Check if the enemy is not near to the player, returns to patrol after the wait time delay
                    m_IsPatrol = true;
                    m_PlayerNear = false;
                    Move(speedWalk);
                    m_TimeToRotate = timeToRotate;
                    m_WaitTime = startWaitTime;
                    navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
                }
                else
                {
                    if (Vector3.Distance(transform.position, m_Player.position) >= navMeshAgent.stoppingDistance)
                        //  Wait if the current position is not the player position
                        Stop();
                    m_WaitTime -= Time.deltaTime;
                }
            }
        }
    }

    private void Patroling()
    {
        if (m_PlayerNear)
        {
            //  Check if the enemy detect near the player, so the enemy will move to that position
            if (m_TimeToRotate <= 0)
            {
                Move(speedWalk);
                LookingPlayer(playerLastPosition);
            }
            else
            {
                //  The enemy wait for a moment and then go to the last player position
                Stop();
                m_TimeToRotate -= Time.deltaTime;
            }
        }
        else
        {
            m_PlayerNear = false;           //  The player is no near when the enemy is platroling
            playerLastPosition = Vector3.zero;
            navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);    //  Set the enemy destination to the next waypoint
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                //  If the enemy arrives to the waypoint position then wait for a moment and go to the next
                if (m_WaitTime <= 0)
                {
                    NextPoint();
                    Move(speedWalk);
                    m_WaitTime = startWaitTime;
                }
                else
                {
                    Stop();
                    m_WaitTime -= Time.deltaTime;
                }
            }
        }
    }

    private void OnAnimatorMove()
    {

    }

    public void NextPoint()
    {
        m_CurrentWaypointIndex = (m_CurrentWaypointIndex + 1) % waypoints.Length;
        navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
    }

    void Stop()
    {
        navMeshAgent.isStopped = true;
        navMeshAgent.speed = 0;
    }

    void Move(float speed)
    {
        navMeshAgent.isStopped = false;
        navMeshAgent.speed = speed;
    }

    void CaughtPlayer()
    {
        m_CaughtPlayer = true;
    }

    void LookingPlayer(Vector3 player)
    {
        navMeshAgent.SetDestination(player);
        if (Vector3.Distance(transform.position, player) <= 0.3)
        {
            if (m_WaitTime <= 0)
            {
                m_PlayerNear = false;
                Move(speedWalk);
                navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
                m_WaitTime = startWaitTime;
                m_TimeToRotate = timeToRotate;
            }
            else
            {
                Stop();
                m_WaitTime -= Time.deltaTime;
            }
        }
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
                    m_IsPatrol = false;                 //  Change the state to chasing the player
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
            if (m_playerInRange)
            {
                /*
                 *  If the enemy no longer sees the player, then the enemy will go to the last position that has been registered
                 * */
                m_PlayerPosition = player.transform.position;       //  Save the player's current position if the player is in range of vision
            }
        }
    }
}