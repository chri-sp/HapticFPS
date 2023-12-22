/*=========================================================================

  Name:        HapticProbe.cs

  Author:      David Borland, RENCI

  Description: Set position to that of the Falcon.

=========================================================================*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HapticProbeFPS : MonoBehaviour
{

    private Queue<int> surfaceIndexes = new Queue<int>();
    private Queue<int> springIndexes = new Queue<int>();
    private Queue<int> forceIndexes = new Queue<int>();
    private Queue<int> viscosityIndexes = new Queue<int>();
    private Queue<int> randomForceIndexes = new Queue<int>();
    private Queue<int> intermolecularIndexes = new Queue<int>();

    // The Falcon device
    private FalconFPS falcon;

    private Vector3 startPlayerPosition;

    //NOTA: xxxIndex e l'indice della forza di una certa tipologia (semplice, viscosità, ecc..), quindi potrò avere piu forze dello stesso tipo con indici diversi

    // Simula lo spostamento virtuale del controller
    private float virtualPositionX;
    private float virtualPositionY;

    // Memorizza la posizione x e y iniziale del falcon nell'ambiente, usata per verificare che abbia raggiunto un estremo del controller
    private float startPositionX;
    private float startPositionY;

    //Variabile usata per rilevare quando un bottone del falcon è stato premuto
    private bool[] buttonPressed = new bool[] { false, false, false, false };

    private float timer;
    private float TimerInitialLastElementQueueForce = .3f;
    private float TimerLastElementQueueForce;

    private bool recoiling = false;
    private bool isJumping = false;
    private bool isDashing = false;
    private bool isReceivingAttack = false;
    private bool isChangingWeapon = false;
    private bool isReloading = false;

    private bool isDequeingForce = false;


    // Use this for initialization
    void Start()
    {
        TimerLastElementQueueForce = TimerInitialLastElementQueueForce;
        // Set Falcon
        falcon = FindObjectOfType<FalconFPS>();

        if (!falcon)
        {
            Debug.LogError("Can't find Falcon");
        }

        StartCoroutine(setInitialPosition());

        startPositionX = transform.position.x;
        virtualPositionX = startPositionX;

        startPositionY = transform.position.y;
        virtualPositionY = startPositionY;
    }

    void FixedUpdate()
    {
        timer += Time.deltaTime;
        TimerLastElementQueueForce -= Time.deltaTime;

        lastElementOnQueueExpired();
        //Debug.Log(forceIndexes.Count);
    }
    void lastElementOnQueueExpired()
    {
        if (TimerLastElementQueueForce <= 0 && forceIndexes.Count > 0)
        {
            FalconFPS.RemoveSimpleForce(forceIndexes.Dequeue());
            reinitializeValuesCoroutine();
            TimerLastElementQueueForce = TimerInitialLastElementQueueForce;
        }
    }
    public void resetAllForces()
    {
        if (!isActive()) return;

        StopAllCoroutines();
        FalconFPS.RemoveSurfaces();
        FalconFPS.RemoveIntermolecularForces();
        FalconFPS.RemoveRandomForces();
        FalconFPS.RemoveSimpleForces();
        FalconFPS.RemoveViscosities();
        FalconFPS.RemoveSprings();
        forceIndexes.Clear();
        springIndexes.Clear();
        surfaceIndexes.Clear();
        viscosityIndexes.Clear();
        randomForceIndexes.Clear();
        intermolecularIndexes.Clear();
        reinitializeValuesCoroutine();
    }

    void reinitializeValuesCoroutine()
    {
        recoiling = false;
        isJumping = false;
        isDashing = false;
        isReceivingAttack = false;
        isChangingWeapon = false;
        isReloading = false;
        isDequeingForce = false;
    }
    IEnumerator dequeConcurrentForce()
    {
        while (isDequeingForce)
            yield return null;
        isDequeingForce = true;

        if (forceIndexes.Count > 0)
            FalconFPS.RemoveSimpleForce(forceIndexes.Dequeue());
        TimerLastElementQueueForce = TimerInitialLastElementQueueForce;

        isDequeingForce = false;
    }


    public IEnumerator recoilHapticFeedback(float recoilIntensity)
    {
        if (!isActive()) yield break;
        if (recoiling) yield break;
        recoiling = true;


        float intensityRecoilMultiplier = 2f;
        forceIndexes.Enqueue(FalconFPS.AddSimpleForce(new Vector3(0, 0, -recoilIntensity * intensityRecoilMultiplier)));
        TimerLastElementQueueForce = TimerInitialLastElementQueueForce;

        yield return new WaitForSeconds(0.1f);
        StartCoroutine(dequeConcurrentForce());


        recoiling = false;
    }

    public IEnumerator jumpHapticFeedback(float jumpIntensity)
    {

        if (!isActive()) yield break;
        if (isJumping) yield break;
        isJumping = true;


        //timer evita feedback salto iniziale in gioco
        if (timer > 1)
        {
            float intensityJumpMultiplier = 1.5f;
            forceIndexes.Enqueue(FalconFPS.AddSimpleForce(new Vector3(0, jumpIntensity * intensityJumpMultiplier, 0)));
            TimerLastElementQueueForce = TimerInitialLastElementQueueForce;

            yield return new WaitForSeconds(0.1f);
            StartCoroutine(dequeConcurrentForce());
        }


        isJumping = false;
    }

    public IEnumerator dashHapticFeedback(float dashIntensity)
    {

        if (!isActive()) yield break;
        if (isDashing) yield break;
        isDashing = true;


        float multiplierVertical = 2;
        forceIndexes.Enqueue(FalconFPS.AddSimpleForce(new Vector3(Input.GetAxis("Horizontal") * dashIntensity, 0, Input.GetAxis("Vertical") * dashIntensity * multiplierVertical)));
        TimerLastElementQueueForce = TimerInitialLastElementQueueForce;

        yield return new WaitForSeconds(0.1f);
        StartCoroutine(dequeConcurrentForce());


        isDashing = false;
    }

    public IEnumerator springHapticFeedback(int button)
    {
        if (!isActive()) yield break;

        springIndexes.Enqueue(FalconFPS.AddSpring(startPlayerPosition, 2.0f, 0.01f, 0.0f, -1.0f));

        //il bottone è mantenuto premuto
        while (getButtonState(button))
        {
            yield return null;
        }

        //il bottone è stato rilasciato
        FalconFPS.RemoveSpring(springIndexes.Dequeue());
    }

    public IEnumerator setInitialPosition()
    {
        if (!isActive()) yield break;

        startPlayerPosition = transform.position;
        springIndexes.Enqueue(FalconFPS.AddSpring(startPlayerPosition, 1f, 0.01f, 0.0f, -1.0f));

        yield return new WaitForSeconds(.5f);
        FalconFPS.RemoveSpring(springIndexes.Dequeue());
    }

    public IEnumerator attackHapticFeedback()
    {
        if (!isActive()) yield break;
        if (isReceivingAttack) yield break;
        isReceivingAttack = true;


        springIndexes.Enqueue(FalconFPS.AddSpring(startPlayerPosition, 2f, 0.01f, 0.0f, -1.0f));

        yield return new WaitForSeconds(.5f);
        FalconFPS.RemoveSpring(springIndexes.Dequeue());


        isReceivingAttack = false;
    }

    public IEnumerator changeWeaponHapticFeedback()
    {
        if (!isActive()) yield break;
        if (isChangingWeapon) yield break;
        isChangingWeapon = true;


        forceIndexes.Enqueue(FalconFPS.AddSimpleForce(new Vector3(0f, 0f, -6f)));
        TimerLastElementQueueForce = TimerInitialLastElementQueueForce;

        yield return new WaitForSeconds(0.2f);
        if (forceIndexes.Count > 0)
            FalconFPS.UpdateSimpleForce(forceIndexes.Peek(), new Vector3(0f, 0f, 0f));
        TimerLastElementQueueForce = TimerInitialLastElementQueueForce;

        yield return new WaitForSeconds(0.2f);
        if (forceIndexes.Count > 0)
            FalconFPS.UpdateSimpleForce(forceIndexes.Peek(), new Vector3(0f, 0f, 3f));
        TimerLastElementQueueForce = TimerInitialLastElementQueueForce;

        yield return new WaitForSeconds(0.2f);
        StartCoroutine(dequeConcurrentForce());


        isChangingWeapon = false;
    }

    public IEnumerator reloadHapticFeedback(Weapon weapon)
    {
        if (!isActive()) yield break;
        if (isReloading) yield break;
        isReloading = true;


        yield return new WaitForSeconds(0.1f);
        forceIndexes.Enqueue(FalconFPS.AddSimpleForce(new Vector3(0f, -3f, 0f)));
        TimerLastElementQueueForce = TimerInitialLastElementQueueForce;

        yield return new WaitForSeconds(0.2f);
        if (forceIndexes.Count > 0)
            FalconFPS.UpdateSimpleForce(forceIndexes.Peek(), new Vector3(0f, 0f, 0f));
        TimerLastElementQueueForce = TimerInitialLastElementQueueForce;

        //attesa prima di avere arma ricaricata
        yield return new WaitForSeconds(weapon.reloadTime - .3f - .2f);
        if (forceIndexes.Count > 0)
            FalconFPS.UpdateSimpleForce(forceIndexes.Peek(), new Vector3(0f, 3f, 0f));
        TimerLastElementQueueForce = TimerInitialLastElementQueueForce;

        yield return new WaitForSeconds(0.2f);
        StartCoroutine(dequeConcurrentForce());


        isReloading = false;
    }

    //Metodo usato per verificare se il falcon è attivo
    public bool isActive()
    {
        return falcon.isActive;
    }

    //Ritorna se uno specifico bottone è premuto
    public bool getButtonState(int button)
    {
        return falcon.buttons[button];
    }

    public bool buttonWasPressed(int button)
    {
        // tasto premuto
        if (!buttonPressed[button] && getButtonState(button))
        {
            buttonPressed[button] = true;
        }

        //tasto mantenuto premuto
        else if (buttonPressed[button] && getButtonState(button))
        {
            return false;
        }

        //tasto rilasciato
        else if (buttonPressed[button] && !getButtonState(button))
        {
            buttonPressed[button] = false;
        }

        //tasto non premuto
        else if (!buttonPressed[button] && !getButtonState(button))
        {
            buttonPressed[button] = false;
        }

        return buttonPressed[button];
    }

    //Ritorna il vettore di coordinate della posizione del controller nell'ambiente, usato per permettere il movimento della camera
    public Vector2 getFalconPosition()
    {
        //Debug.Log("Falcon position x: " + falcon.position.x+ "\n Falcon position y: "+ falcon.position.y);

        //Arrivo al bordo inferiore-destro
        if (falcon.position.x > startPositionX + 2 && falcon.position.y < startPositionY - 1.5)
        {
            virtualPositionX = virtualPositionX + 0.1f;
            virtualPositionY = virtualPositionY - 0.1f;
        }
        //Arrivo al bordo inferiore-sinistro
        else if (falcon.position.x < startPositionX - 2 && falcon.position.y < startPositionY - 1.5)
        {
            virtualPositionX = virtualPositionX - 0.1f;
            virtualPositionY = virtualPositionY - 0.1f;
        }
        //Arrivo al bordo superiore-sinistro
        else if ((falcon.position.x < startPositionX - 2) && (falcon.position.y > startPositionY + 1.3))
        {
            virtualPositionX = virtualPositionX - 0.1f;
            virtualPositionY = virtualPositionY + 0.1f;
        }
        //Arrivo al bordo superiore-destro
        else if ((falcon.position.x > startPositionX + 2.5) && (falcon.position.y > startPositionY + 1.5))
        {
            virtualPositionX = virtualPositionX + 0.1f;
            virtualPositionY = virtualPositionY + 0.1f;
        }
        //Arrivo al bordo destro
        else if (falcon.position.x > startPositionX + 4.2)
        {
            virtualPositionX = virtualPositionX + 0.1f;
        }
        //Arrivo al bordo sinistro
        else if (falcon.position.x < startPositionX - 4.2)
        {
            virtualPositionX = virtualPositionX - 0.1f;
        }
        //Arrivo al bordo superiore
        else if (falcon.position.y > startPositionY + 4.5)
        {
            virtualPositionY = virtualPositionY + 0.1f;
        }
        //Arrivo al bordo inferiore
        else if (falcon.position.y < startPositionY - 4)
        {
            virtualPositionY = virtualPositionY - 0.1f;
        }
        Vector2 currentPosition = new Vector2(falcon.position.x + virtualPositionX, falcon.position.y + virtualPositionY);
        return currentPosition;
    }
}
