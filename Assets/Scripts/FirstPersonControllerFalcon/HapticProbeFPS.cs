/*=========================================================================

  Name:        HapticProbe.cs

  Author:      David Borland, RENCI

  Description: Set position to that of the Falcon.

=========================================================================*/

using UnityEngine;
using System.Collections;

public class HapticProbeFPS : MonoBehaviour
{

    // The Falcon device
    private FalconFPS falcon;

    private Vector3 startPlayerPosition;

    //NOTA: xxxIndex e l'indice della forza di una certa tipologia (semplice, viscosità, ecc..), quindi potrò avere piu forze dello stesso tipo con indici diversi

    // Simple force
    private int simpleForceIndex = -1;
    public bool useSimpleForce = false;
    public Vector3 simpleForce;

    // Viscosity
    private int viscosityIndex = -1;
    public bool useViscosity = false;

    // Surface
    private int surfaceIndex = -1;
    public bool useSurface = false;

    // Spring
    private int springIndex = -1;
    public bool useSpring = false;

    // Intermolecular force
    private int intermolecularForceIndex = -1;
    public bool useIntermolecularForce = false;

    // Random
    private int randomForceIndex = -1;
    public bool useRandomForce = false;

    // Simula lo spostamento virtuale del controller
    private float virtualPositionX;
    private float virtualPositionY;

    // Memorizza la posizione x e y iniziale del falcon nell'ambiente, usata per verificare che abbia raggiunto un estremo del controller
    private float startPositionX;
    private float startPositionY;

    //Variabile usata per rilevare quando un bottone del falcon è stato premuto
    private bool[] buttonPressed = new bool[] { false, false, false, false };

    private float timer;

    private bool recoiling = false;
    private bool isJumping = false;
    private bool isDashing = false;
    private bool isReceivingAttack = false;
    private bool isChangingWeapon = false;

    // Use this for initialization
    void Start()
    {
        // Set Falcon
        falcon = FindObjectOfType<FalconFPS>();

        if (!falcon)
        {
            Debug.LogError("Can't find Falcon");
        }

        StartCoroutine(setInitialPosition());
        //SetPosition();

        startPositionX = transform.position.x;
        virtualPositionX = startPositionX;

        startPositionY = transform.position.y;
        virtualPositionY = startPositionY;

        //simpleForceIndex = FalconFPS.AddSimpleForce(simpleForce);

    }


    void FixedUpdate()
    {
        timer += Time.deltaTime;
        //test funzione creazione molla
        
        /*
        if (isActive() && buttonWasPressed(1))
        {
            StartCoroutine(springHapticFeedback(1));
        }
        */
        
        // Move probe		
        //SetPosition();

        /*
        // Update simple force
        if (useSimpleForce)
        {
            if (simpleForceIndex < 0)
            {
                simpleForceIndex = FalconFPS.AddSimpleForce(simpleForce);
            }
            else
            {
                FalconFPS.UpdateSimpleForce(simpleForceIndex, simpleForce);
            }
        }
        else if (simpleForceIndex >= 0)
        {
            FalconFPS.RemoveSimpleForce(simpleForceIndex);
            simpleForceIndex = -1;
        }
         
        // Update viscosity
        if (useViscosity && viscosityIndex < 0)
        {
            viscosityIndex = FalconFPS.AddViscosity(0.5f, 0.25f);
        }
        else if (!useViscosity && viscosityIndex >= 0)
        {
            FalconFPS.RemoveViscosity(viscosityIndex);
            viscosityIndex = -1;
        }

        // Update surface
        if (useSurface && surfaceIndex < 0)
        {
            surfaceIndex = FalconFPS.AddSurface(transform.position, new Vector3(0.0f, 1.0f, 0.0f), 20.0f, 0.01f);
        }
        else if (!useSurface && surfaceIndex >= 0)
        {
            FalconFPS.RemoveSurface(surfaceIndex);
            surfaceIndex = -1;
        }

        // Update spring
        if (useSpring && springIndex < 0)
        {
            springIndex = FalconFPS.AddSpring(transform.position, 2.0f, 0.01f, 0.0f, -1.0f);
        }
        else if (!useSpring && springIndex >= 0)
        {
            FalconFPS.RemoveSpring(springIndex);
            springIndex = -1;
        }

        // Update intermolecular
        if (useIntermolecularForce && intermolecularForceIndex < 0)
        {
            intermolecularForceIndex = FalconFPS.AddIntermolecularForce(transform.position, 10.0f, 0.01f, 2.0f, 4.0f);
        }
        else if (!useIntermolecularForce && intermolecularForceIndex >= 0)
        {
            FalconFPS.RemoveIntermolecularForce(intermolecularForceIndex);
            intermolecularForceIndex = -1;
        }

        // Update Random
        if (useRandomForce && randomForceIndex < 0)
        {
            randomForceIndex = FalconFPS.AddRandomForce(1.0f, 5.0f, 0.01f, 0.1f);
        }
        else if (!useRandomForce && randomForceIndex >= 0)
        {
            FalconFPS.RemoveRandomForce(randomForceIndex);
            randomForceIndex = -1;
        }
        */
    }

    /*
    //prende la posizione del controller e imposta la posizione dell'oggetto
    void SetPosition()
    {
        // Set position from Falcon
        Vector3 p = falcon.position;
        transform.position = p;
    }
    */


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
        else if ((falcon.position.x > startPositionX + 2.5) && (falcon.position.y > startPositionY + 1.5)) {
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

    public IEnumerator recoilHapticFeedback(float recoilIntensity)
    {
        if (isActive() && !recoiling)
        {
            recoiling = true;
            float intensityRecoilMultiplier = 2f;
            int recoilIndex = FalconFPS.AddSimpleForce(new Vector3(0, 0, -recoilIntensity * intensityRecoilMultiplier));
            yield return new WaitForSeconds(0.1f);
            FalconFPS.RemoveSimpleForce(recoilIndex);
            recoiling = false;
        }

    }

    public IEnumerator jumpHapticFeedback(float jumpIntensity)
    {
        //timer evita feedback salto iniziale in gioco
        if (isActive() && !isJumping && timer>1)
        {
            isJumping = true;
            float intensityJumpMultiplier = 1.5f;
            int jumpIndex = FalconFPS.AddSimpleForce(new Vector3(0, jumpIntensity * intensityJumpMultiplier, 0));
            yield return new WaitForSeconds(0.1f);
            FalconFPS.RemoveSimpleForce(jumpIndex);
            isJumping = false;
        }
    }

    public IEnumerator dashHapticFeedback(float dashIntensity)
    {
        if (isActive() && !isDashing)
        {
            isDashing = true;
            float multiplierVertical = 2;
            int runIndex = FalconFPS.AddSimpleForce(new Vector3(Input.GetAxis("Horizontal") * dashIntensity, 0, Input.GetAxis("Vertical") * dashIntensity * multiplierVertical));
            yield return new WaitForSeconds(0.1f);
            FalconFPS.RemoveSimpleForce(runIndex);
            isDashing = false;
        }
    }

    public IEnumerator springHapticFeedback(int button)
    {
        if (isActive())
        {
            int springIndex = FalconFPS.AddSpring(startPlayerPosition, 2.0f, 0.01f, 0.0f, -1.0f);

            //il bottone è mantenuto premuto
            while (getButtonState(button))
            {
                yield return null;
            }

            //il bottone è stato rilasciato
            FalconFPS.RemoveSpring(springIndex);
        }
    }

    public IEnumerator setInitialPosition() {
        if (isActive())
        {
            startPlayerPosition = transform.position;
            int springIndex = FalconFPS.AddSpring(startPlayerPosition, 1f, 0.01f, 0.0f, -1.0f);
            yield return new WaitForSeconds(.5f);
            FalconFPS.RemoveSpring(springIndex);
        }
    }

    public IEnumerator attackHapticFeedback()
    {
        if (isActive() && !isReceivingAttack)
        {
            isReceivingAttack = true;
            int springIndex = FalconFPS.AddSpring(startPlayerPosition, 2f, 0.01f, 0.0f, -1.0f);
            yield return new WaitForSeconds(.5f);
            FalconFPS.RemoveSpring(springIndex);
            isReceivingAttack = false;
        }
    }

    public IEnumerator changeWeaponHapticFeedback()
    {
        if (isActive() && !isChangingWeapon)
        {
            isChangingWeapon = true;
            int changeWeaponIndex = FalconFPS.AddSimpleForce(new Vector3(0f, 0f, -6f));
            yield return new WaitForSeconds(0.2f);
            FalconFPS.UpdateSimpleForce(changeWeaponIndex, new Vector3(0f, 0f, 0f));
            yield return new WaitForSeconds(0.2f);
            FalconFPS.UpdateSimpleForce(changeWeaponIndex, new Vector3(0f, 0f, 3f));
            yield return new WaitForSeconds(0.2f);
            FalconFPS.RemoveSimpleForce(changeWeaponIndex);
            isChangingWeapon = false;
        }
    }

}
