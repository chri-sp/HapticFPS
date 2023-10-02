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

        simpleForceIndex = FalconFPS.AddSimpleForce(simpleForce);


    }


    void FixedUpdate()
    {
        // Move probe		
        //SetPosition();

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
    }

    //prende la posizione del controller e imposta la posizione dell'oggetto
    void SetPosition()
    {
        // Set position from Falcon
        Vector3 p = falcon.position;
        //Blocco la coordinata della profondita per simulare il movimento della visuale, su-giu-destra-sinistra
        //p.z = 100;
        transform.position = p;
    }


    //Metodo usato per ottenere la posizione del falcon permettendo il movimento della camera
    public Vector2 getFalconPosition()
    {
        return new Vector2(falcon.position.x, falcon.position.y);

        /*  TODO se il falcon raggiunge il bordo aumento di 1 unita la 
         * coordinata corrispondente, questo per permettere di girare 
         * la visuale anche quando ho raggiunto un estremo del controller
         * (quindi ho 4 if uno per ogni estremo)
         */
    }

    //Metodo usato per verificare se il falcon è attivo
    public bool isActive()
    {
        return falcon.isActive;
    }

}
