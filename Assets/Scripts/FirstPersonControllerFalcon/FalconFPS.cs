/*=========================================================================

  Name:        Falcon.cs

  Author:      David Borland, RENCI

  Description: Script for loading and interfacing with the Novint Falcon
  			   Unity plugin.

=========================================================================*/

using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class FalconFPS : MonoBehaviour
{
    // Position
    public Vector3 position = Vector3.zero;

    // Displayed force
    public Vector3 force = Vector3.zero;

    // Buttons
    public bool[] buttons;

    // Use force feedback or not
    public bool useForceFeedback = true;

    //Il falcon è attivo
    [HideInInspector] public bool isActive = false;

    // Load functions from DLL

    // Initialize HDL library
    [DllImport("FalconUnityPlugin")]
    private static extern bool Initialize();

    [DllImport("FalconUnityPlugin")]
    private static extern void CleanUp();

    // Set the workspace of the graphics scene that will be mapped to the device workspace
    [DllImport("FalconUnityPlugin")]
    private static extern void SetGraphicsWorkspace(Vector3 center, Vector3 size);

    [DllImport("FalconUnityPlugin")]
    private static extern void ResetForces();

    // Get the device position
    [DllImport("FalconUnityPlugin")]
    private static extern Vector3 GetPosition();

    // Get the displayed force
    [DllImport("FalconUnityPlugin")]
    private static extern Vector3 GetForce();

    [DllImport("FalconUnityPlugin")]
    private static extern bool GetButton(int button);

    [DllImport("FalconUnityPlugin")]
    private static extern bool UseForceFeedback(bool use);

    // Proxy position

    // Set the proxy position, to use for calculating forces when not using force feedback
    [DllImport("FalconUnityPlugin")]
    public static extern bool SetProxyPosition(Vector3 p);

    // Simple forces

    // Add a force effect. Return id for this effect
    [DllImport("FalconUnityPlugin")]
    public static extern int AddSimpleForce(Vector3 force);

    [DllImport("FalconUnityPlugin")]
    public static extern void UpdateSimpleForce(int i, Vector3 force);

    // Remove the force effect with the given id
    [DllImport("FalconUnityPlugin")]
    public static extern void RemoveSimpleForce(int i);

    [DllImport("FalconUnityPlugin")]
    public static extern void RemoveSimpleForces();

    // Viscosities
    // c: Damping coefficient
    // w: Heuristic weighting factor in the range 0 to 1 used to interpolate
    //    between the previous viscous force and the current viscous force to reduce vibration. 
    //    A value of 0 will give no weight to the current viscous force, and 1 will give full weight.
    [DllImport("FalconUnityPlugin")]
    public static extern int AddViscosity(float c, float w);

    [DllImport("FalconUnityPlugin")]
    public static extern void UpdateViscosity(int i, float c, float w);

    [DllImport("FalconUnityPlugin")]
    public static extern void RemoveViscosity(int i);

    [DllImport("FalconUnityPlugin")]
    public static extern void RemoveViscosities();

    // Surfaces
    // p: Surface contact point. Using the center of the haptic proxy results in a dilation of the surface by the proxy radius.
    // n: Surface normal
    // k: Spring constant
    // c: Damping coefficient
    [DllImport("FalconUnityPlugin")]
    public static extern int AddSurface(Vector3 p, Vector3 n, float k, float c);

    [DllImport("FalconUnityPlugin")]
    public static extern void UpdateSurface(int i, Vector3 p, Vector3 n, float k, float c);

    [DllImport("FalconUnityPlugin")]
    public static extern void RemoveSurface(int i);

    [DllImport("FalconUnityPlugin")]
    public static extern void RemoveSurfaces();

    // Springs
    // p: Position
    // k: Spring constant
    // c: Damping coefficient
    // r: Rest length
    // m: Maximum length. Negative value for no maximum.
    [DllImport("FalconUnityPlugin")]
    public static extern int AddSpring(Vector3 p, float k, float c, float r, float m);

    [DllImport("FalconUnityPlugin")]
    public static extern void UpdateSpring(int i, Vector3 p, float k, float c, float r, float m);

    [DllImport("FalconUnityPlugin")]
    public static extern void RemoveSpring(int i);

    [DllImport("FalconUnityPlugin")]
    public static extern void RemoveSprings();

    // Intermolecular forces
    // Simulating using a linear spring with bond equilibrium length equal to rest length. Instead of breaking at maximum length, however, 
    // the force curve is mirrored to achieve a continuous falloff, and thus a linear approximation of an intermolecular force curve:
    //
    // \
    //  \
    // __\_______
    //   |\    /
    //   | \  /
    //   |  \/
    //   |   | 
    //   r   m
    //
    // p: Position
    // k: Spring constant
    // c: Damping coefficient
    // r: Bond length
    // m: Maximum length, beyond which bond is "broken"
    [DllImport("FalconUnityPlugin")]
    public static extern int AddIntermolecularForce(Vector3 p, float k, float c, float r, float m);

    [DllImport("FalconUnityPlugin")]
    public static extern void UpdateIntermolecularForce(int i, Vector3 p, float k, float c, float r, float m);

    [DllImport("FalconUnityPlugin")]
    public static extern void RemoveIntermolecularForce(int i);

    [DllImport("FalconUnityPlugin")]
    public static extern void RemoveIntermolecularForces();

    // Random forces
    // minMag: Minimum force magnitude
    // maxMag: Maximum force magnitude
    // minTime: Minimum time interval to apply force
    // maxTime: Maximum time interval to apply force
    [DllImport("FalconUnityPlugin")]
    public static extern int AddRandomForce(float minMag, float maxMag, float minTime, float maxTime);

    [DllImport("FalconUnityPlugin")]
    public static extern void UpdateRandomForce(int i, float minMag, float maxMag, float minTime, float maxTime);

    [DllImport("FalconUnityPlugin")]
    public static extern void RemoveRandomForce(int i);

    [DllImport("FalconUnityPlugin")]
    public static extern void RemoveRandomForces();

    private BasicAimHelper aimHelper;

    void Awake()
    {
        // Initialize buttons
        buttons = new bool[] { false, false, false, false };

        aimHelper = GameObject.FindWithTag("MainCamera").GetComponent<BasicAimHelper>();
        try
        {
            if (Initialize())
            {
                Renderer renderer = GetComponent<Renderer>();
                SetGraphicsWorkspace(renderer.bounds.center,
                                     renderer.bounds.size);

                UpdateState();

                UseForceFeedback(useForceFeedback);

                Debug.Log("Falcon success");
                aimHelper.enabled = true;
                isActive = true;
            }
            else
            {
                Debug.Log("Falcon failure");
                aimHelper.enabled = false;
                isActive = false;
            }
        }
        catch (DllNotFoundException e)
        {

        }

    }

    void OnDestroy()
    {
        try
        {
            Debug.Log("Falcon cleaned up");
            CleanUp();
        }

        catch (DllNotFoundException e)
        {

        }

    }

    void FixedUpdate()
    {
        UpdateState();
    }

    void UpdateState()
    {
        try
        {
            // Update position
            position = GetPosition();

            // Update force
            force = GetForce();

            // Update buttons
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i] = GetButton(i);
            }
        }

        catch (DllNotFoundException e)
        {

        }

    }
}
