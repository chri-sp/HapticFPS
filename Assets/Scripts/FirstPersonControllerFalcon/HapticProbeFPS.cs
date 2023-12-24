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

    WeaponManager weaponManager;

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

    //timer utilizzato per evitare feedback aptico ad inizio scena
    private float timer;
    
    //questo valore deve essere maggiore del tempo in cui è applicata una certa forza
    private float initialTimerLastElementQueue = .6f;
    //timer alla cui scadenza verica se è necessario rimuovere un elemento dalla coda
    private float expireDurationLastElementOnQueue_Force;
    private float expireDurationLastElementOnQueue_Spring;

    private bool isDequeingForce = false;
    private bool isDequeingSpring = false;

    private bool recoiling = false;
    private bool isJumping = false;
    private bool isDashing = false;
    private bool isReceivingAttack = false;
    private bool isChangingWeapon = false;
    private bool isReloading = false;
    private bool isReloadingFast = false;


    // Use this for initialization
    void Start()
    {
        weaponManager = GameObject.FindWithTag("WeaponHolder").GetComponent<WeaponManager>();
        reinitializeValuesCoroutine();
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
        updateTimers();
        checkLastElementsOnQueueExpired();
        //Debug.Log(forceIndexes.Count);
    }

    //aggiorna i timer presenti nello script
    void updateTimers() {
        timer += Time.deltaTime;
        expireDurationLastElementOnQueue_Force -= Time.deltaTime;
        expireDurationLastElementOnQueue_Spring -= Time.deltaTime;
    }

    //controlla se l'ultimo elemento in ogni coda deve essere rimosso, per garantire che non rimangano applicate forze sul controller
    void checkLastElementsOnQueueExpired() {
        lastElementOnQueueExpiredForce();
        lastElementOnQueueExpiredSpring();
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
        expireDurationLastElementOnQueue_Force = initialTimerLastElementQueue;
        expireDurationLastElementOnQueue_Spring = initialTimerLastElementQueue;
        recoiling = false;
        isJumping = false;
        isDashing = false;
        isReceivingAttack = false;
        isChangingWeapon = false;
        isReloading = false;
        isReloadingFast = false;
        isDequeingForce = false;
        isDequeingSpring = false;
    }

    public IEnumerator recoilHapticFeedback(float recoilIntensity)
    {
        if (!isActive()) yield break;
        if (recoiling) yield break;
        recoiling = true;


        float intensityRecoilMultiplier = 2f;
        addForce(new Vector3(0, 0, -recoilIntensity * intensityRecoilMultiplier));
        yield return new WaitForSeconds(0.1f);
        removeForce();


        recoiling = false;
    }

    public IEnumerator jumpHapticFeedback(float jumpIntensity)
    {

        if (!isActive()) yield break;
        //timer che evita feedback salto iniziale in gioco
        if (timer > 1) yield break;
        if (isJumping) yield break;
        isJumping = true;


        float intensityJumpMultiplier = 1.5f;
        addForce(new Vector3(0, jumpIntensity * intensityJumpMultiplier, 0));
        yield return new WaitForSeconds(0.1f);
        removeForce();


        isJumping = false;
    }

    public IEnumerator dashHapticFeedback(float dashIntensity)
    {

        if (!isActive()) yield break;
        if (isDashing) yield break;
        isDashing = true;


        float multiplierVertical = 2;
        addForce(new Vector3(Input.GetAxis("Horizontal") * dashIntensity, 0, Input.GetAxis("Vertical") * dashIntensity * multiplierVertical));
        yield return new WaitForSeconds(0.1f);
        removeForce();


        isDashing = false;
    }

    public IEnumerator springHapticFeedback(int button)
    {
        if (!isActive()) yield break;

        addSpring(startPlayerPosition, 2.0f, 0.01f, 0.0f, -1.0f);

        //il bottone è mantenuto premuto
        while (getButtonState(button))
        {
            yield return null;
        }

        //il bottone è stato rilasciato
        removeSpring();
    }

    public IEnumerator setInitialPosition()
    {
        if (!isActive()) yield break;

        startPlayerPosition = transform.position;
        addSpring(startPlayerPosition, 1f, 0.01f, 0.0f, -1.0f);
        yield return new WaitForSeconds(.5f);
        removeSpring();

    }

    public IEnumerator attackHapticFeedback()
    {
        if (!isActive()) yield break;
        if (isReceivingAttack) yield break;
        isReceivingAttack = true;

        addSpring(startPlayerPosition, 2f, 0.01f, 0.0f, -1.0f);
        yield return new WaitForSeconds(.5f);
        removeSpring();


        isReceivingAttack = false;
    }

    public IEnumerator changeWeaponHapticFeedback()
    {
        if (!isActive()) yield break;
        if (isChangingWeapon) yield break;
        isChangingWeapon = true;

        addForce(new Vector3(0f, 0f, -6f));
        yield return new WaitForSeconds(0.2f);
        updateForce(new Vector3(0f, 0f, 0f));
        yield return new WaitForSeconds(0.2f);
        updateForce(new Vector3(0f, 0f, 3f));
        yield return new WaitForSeconds(0.2f);
        removeForce();


        isChangingWeapon = false;
    }

    public IEnumerator reloadHapticFeedback(Weapon weapon)
    {
        if (!isActive()) yield break;
        if (isReloading) yield break;
        isReloading = true;


        yield return new WaitForSeconds(0.1f);
        addForce(new Vector3(0f, -3f, 0f));
        yield return new WaitForSeconds(0.2f);
        removeForce();
        //attesa prima di avere arma ricaricata
        yield return new WaitForSeconds(weapon.reloadTime - .3f - .2f);

        if (canReload(weapon)) {
            StartCoroutine(reloadHapticFeedbackFinished());
        }
           
        isReloading = false;
    }


    //verifico se non ho cambiato arma e non ho effettuato la ricarica veloce
    bool canReload(Weapon weapon) {
        return (weapon == weaponManager.currentWeapon() && weapon.currentAmmo!= weapon.maxAmmo);
    }


    //feedback ricarica completata
    public IEnumerator reloadHapticFeedbackFinished()
    {
        if (!isActive()) yield break;
        if (isReloadingFast) yield break;
        isReloadingFast = true;


        addForce(new Vector3(0f, 3f, 0f));
        yield return new WaitForSeconds(0.2f);
        removeForce();


        isReloadingFast = false;
    }

    //verifica e corregge la presenza di forze semplici non corrette
    void lastElementOnQueueExpiredForce()
    {
        if (expireDurationLastElementOnQueue_Force <= 0 && forceIndexes.Count > 0)
        {
            FalconFPS.RemoveSimpleForce(forceIndexes.Dequeue());
            reinitializeValuesCoroutine();
            expireDurationLastElementOnQueue_Force = initialTimerLastElementQueue;
        }
    }

    //verifica e corregge la presenza di forze di tipo molla non corrette
    void lastElementOnQueueExpiredSpring()
    {
        if (expireDurationLastElementOnQueue_Spring <= 0 && springIndexes.Count > 0)
        {
            FalconFPS.RemoveSpring(springIndexes.Dequeue());
            reinitializeValuesCoroutine();
            expireDurationLastElementOnQueue_Spring = initialTimerLastElementQueue;
        }
    }

    void removeForce()
    {
        StartCoroutine(dequeConcurrentForce());
    }

    void addForce(Vector3 force)
    {
        forceIndexes.Enqueue(FalconFPS.AddSimpleForce(force));
        expireDurationLastElementOnQueue_Force = initialTimerLastElementQueue;
    }

    void updateForce(Vector3 force)
    {
        if (forceIndexes.Count > 0)
            FalconFPS.UpdateSimpleForce(forceIndexes.Peek(), force);
        expireDurationLastElementOnQueue_Force = initialTimerLastElementQueue;
    }

    void removeSpring()
    {
        StartCoroutine(dequeConcurrentSpring());
    }

    // Springs
    // p: Position
    // k: Spring constant
    // c: Damping coefficient
    // r: Rest length
    // m: Maximum length. Negative value for no maximum.
    void addSpring(Vector3 position, float k, float c, float r, float m)
    {
        springIndexes.Enqueue(FalconFPS.AddSpring(position, k, c, r, m));
        expireDurationLastElementOnQueue_Spring = initialTimerLastElementQueue;
    }

    void updateSpring(Vector3 position, float k, float c, float r, float m)
    {
        if (springIndexes.Count > 0)
            FalconFPS.UpdateSpring(springIndexes.Peek(), position, k, c, r, m);
        expireDurationLastElementOnQueue_Force = initialTimerLastElementQueue;
    }


    IEnumerator dequeConcurrentForce()
    {
        while (isDequeingForce)
            yield return null;
        isDequeingForce = true;

        if (forceIndexes.Count > 0)
            FalconFPS.RemoveSimpleForce(forceIndexes.Dequeue());
        expireDurationLastElementOnQueue_Force = initialTimerLastElementQueue;

        isDequeingForce = false;
    }

    IEnumerator dequeConcurrentSpring()
    {
        while (isDequeingSpring)
            yield return null;
        isDequeingSpring = true;

        if (springIndexes.Count > 0)
            FalconFPS.RemoveSpring(springIndexes.Dequeue());
        expireDurationLastElementOnQueue_Spring = initialTimerLastElementQueue;

        isDequeingSpring = false;
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
