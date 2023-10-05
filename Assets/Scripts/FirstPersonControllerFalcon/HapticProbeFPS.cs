/*=========================================================================

  Name:        HapticProbe.cs

  Author:      David Borland, RENCI

  Description: Set position to that of the Falcon.

=========================================================================*/

using UnityEngine;

public class HapticProbeFPS : MonoBehaviour
{

    // The Falcon device
    private FalconFPS falcon;

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

    // Memorizza la posizione x iniziale del falcon nell'ambiente, usata per verificare che abbia raggiunto un estremo del controller
    private float startPositionX;

    // Use this for initialization
    void Start()
    {
        // Set Falcon
        falcon = FindObjectOfType<FalconFPS>();

        if (!falcon)
        {
            Debug.LogError("Can't find Falcon");
        }

        //SetPosition();

        startPositionX = falcon.position.x;

        virtualPositionX = startPositionX;



        simpleForceIndex = FalconFPS.AddSimpleForce(simpleForce);

    }

    //mappa che associa ad uno oggetto, l'indice assegnato dal falcon e la forza applicata sul falcon


    //aggiunge la forza alla mappa e al controller falcon
    public void AddSimpleForce(/*oggetto da cui ricavare l'indice*/)
    {
        //se l'oggetto non è gia contenuto
        //aggiungo alla mappa l'oggetto e la forza
        simpleForceIndex = FalconFPS.AddSimpleForce(simpleForce);
    }

    public void UpdateSimpleForce(/*oggetto da cui ricavare l'indice, nuova forza*/)
    {
        //se l'oggetto è presente nella mappa
        FalconFPS.UpdateSimpleForce(simpleForceIndex, simpleForce);
    }

    public void RemoveSimpleForce(/*oggetto da cui ricavare l'indice*/)
    {
        //se l'oggetto è presente nella mappa
        //rimouvere l'oggetto dalla mappa
        FalconFPS.RemoveSimpleForce(simpleForceIndex);
    }

    public void RemoveAllSimpleForces()
    {
        //aggiornare la mappa
        FalconFPS.RemoveSimpleForces();
    }


    void FixedUpdate()
    {
        // Move probe		
        //SetPosition();


        //ad ogni update aggiorno tutte le forze in gioco nella mappa
        if (useSimpleForce)
        {
            //foreach oggetti in mappa
            FalconFPS.UpdateSimpleForce(simpleForceIndex/*contenuto indice dell'oggetto nella mappa*/, simpleForce/*contenuto forza dell'oggetto nella mappa*/);
        }


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
        */

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
    }

    //prende la posizione del controller e imposta la posizione dell'oggetto
    void SetPosition()
    {
        // Set position from Falcon
        Vector3 p = falcon.position;
        transform.position = p;
    }


    //Ritorna il vettore di coordinate della posizione del controller, usato per permettere il movimento della camera
    public Vector2 getFalconPosition()
    {
        Debug.Log("Posizione falcon: " + falcon.position);


        //Arrivo al bordo destro
        //falcon.position.x > startXPosition + 4
        if (falcon.position.x > 4)
        {
            virtualPositionX = virtualPositionX + 0.1f;
        }
        //Arrivo al bordo sinistro
        //falcon.position.x > startXPosition - 4
        else if (falcon.position.x < -4)
        {
            virtualPositionX = virtualPositionX - 0.1f;
        }

        return new Vector2(falcon.position.x + virtualPositionX, falcon.position.y);
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

}
