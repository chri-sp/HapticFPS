﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HapticProbeTesting : MonoBehaviour
{

    // The Falcon device
    private Falcon falcon;

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
        falcon = FindObjectOfType<Falcon>();
        if (!falcon)
        {
            Debug.LogError("Can't find Falcon");
        }

        SetPosition();

        //centro la posizione del controller
        StartCoroutine(InitiatePosition());

        simpleForceIndex = Falcon.AddSimpleForce(simpleForce);
    }


    void FixedUpdate()
    {
        // Move probe		
        SetPosition();

        if (useSimpleForce)
        {
            if (simpleForceIndex < 0)
            {
                simpleForceIndex = Falcon.AddSimpleForce(simpleForce);
            }
            else
            {
                Falcon.UpdateSimpleForce(simpleForceIndex, simpleForce);
            }
        }
        else if (simpleForceIndex >= 0)
        {
            Falcon.RemoveSimpleForce(simpleForceIndex);
            simpleForceIndex = -1;
        }


        // Update viscosity
        if (useViscosity && viscosityIndex < 0)
        {
            viscosityIndex = Falcon.AddViscosity(0.5f, 0.25f);
        }
        else if (!useViscosity && viscosityIndex >= 0)
        {
            Falcon.RemoveViscosity(viscosityIndex);
            viscosityIndex = -1;
        }

        // Update surface
        if (useSurface && surfaceIndex < 0)
        {
            surfaceIndex = Falcon.AddSurface(transform.position, new Vector3(0.0f, 1.0f, 0.0f), 20.0f, 0.01f);
        }
        else if (!useSurface && surfaceIndex >= 0)
        {
            Falcon.RemoveSurface(surfaceIndex);
            surfaceIndex = -1;
        }

        // Update spring
        if (useSpring && springIndex < 0)
        {
            springIndex = Falcon.AddSpring(transform.position, 2.0f, 0.01f, 0.0f, -1.0f);
        }
        else if (!useSpring && springIndex >= 0)
        {
            Falcon.RemoveSpring(springIndex);
            springIndex = -1;
        }

        // Update intermolecular
        if (useIntermolecularForce && intermolecularForceIndex < 0)
        {
            intermolecularForceIndex = Falcon.AddIntermolecularForce(transform.position, 10.0f, 0.01f, 2.0f, 4.0f);
        }
        else if (!useIntermolecularForce && intermolecularForceIndex >= 0)
        {
            Falcon.RemoveIntermolecularForce(intermolecularForceIndex);
            intermolecularForceIndex = -1;
        }

        // Update Random
        if (useRandomForce && randomForceIndex < 0)
        {
            randomForceIndex = Falcon.AddRandomForce(1.0f, 5.0f, 0.01f, 0.1f);
        }
        else if (!useRandomForce && randomForceIndex >= 0)
        {
            Falcon.RemoveRandomForce(randomForceIndex);
            randomForceIndex = -1;
        }
    }

    //prende la posizione del controller e imposta la posizione dell'oggetto
    void SetPosition()
    {
        // Set position from Falcon
        Vector3 p = falcon.position;
        Debug.Log("Posizione falcon: " + p);

        //modificare transform.rotation?
        transform.position = p;
    }


    /*centro la posizione del controller partendo da posizione 
        * basso-destra-estesa, imprimendo una forza momentanea */
    IEnumerator InitiatePosition()
    {
        int InitialForcePositionIndex = Falcon.AddSimpleForce(new Vector3(0, 3, 0));
        yield return new WaitForSeconds(1);
        Falcon.RemoveSimpleForce(InitialForcePositionIndex);
    }
}
