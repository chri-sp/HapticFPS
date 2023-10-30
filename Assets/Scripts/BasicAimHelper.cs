using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// @kurtdekker

public class BasicAimHelper : MonoBehaviour
{

    [Header("@kurtdekker")]
    [Header("Radius to act within (percent of minimum screen axis)")]
    public float ActiveScreenFractionRadius = 0.15f;

    [Header("Choose this to be adequate but still let you break lock.")]
    public float AimingAssistanceStrength = 10.0f;

    [Header("At what range does attraction max out?")]
    public float MinimumActiveLinearRange = 20.0f;
    [Header("How rapidly does the effect fall off on distant enemies?")]
    public float MaximumActiveLinearRange = 40.0f;

    [Header("How far can the effectiveness fall based on range?")]
    public float FloorAttractionAtMaxRange = 0.25f;

    public static BasicAimHelper I { get; private set; }

    void OnEnable()
    {
        I = this;
    }
    void OnDisable()
    {
        I = null;
    }

    static float ScreenMinimumAxis { get { return Mathf.Min(Screen.width, Screen.height); } }

    // returns true if the mouse was adjusted
    public bool AllowForAutoAim(ref float mouseX, ref float mouseY)
    {

        bool adjusted = false;

        // TODO: expects to be ON the GameObject with the Camera!
        var cam = GetComponent<Camera>();

        // TODO: assumes crosshairs are middle of the screen precisely
        Vector2 screenCrosshairs = new Vector2(Screen.width, Screen.height) / 2;

        // TODO: get these once in a far more efficient way. This is for demo only!
        var targets = FindObjectsOfType<AimhelperTarget>();

        float activeRadius = ScreenMinimumAxis * ActiveScreenFractionRadius;

        // used to only consider the drive from the closest target, not a mix of all nearby
        float closestRadius = activeRadius;

        // will ultimately end up being the closest to our aimpoint only.
        Vector2 mouseAssistanceDrive = Vector2.zero;

        foreach (var target in targets)
        {
            
            Vector3 worldDelta = target.transform.position - transform.position;

            // only lock onto stuff in front of us
            if (Vector3.Dot(transform.forward, worldDelta) > 0)
            {
                
                float targetWorldDistanceRange = worldDelta.magnitude;

                if (targetWorldDistanceRange < MaximumActiveLinearRange)
                {
                    Vector2 screenTargetPos = cam.WorldToScreenPoint(target.transform.position);

                    Vector2 screenDelta = screenTargetPos - screenCrosshairs;

                    float screenDistance = screenDelta.magnitude;

                    // any attraction at all?
                    if (screenDistance < activeRadius)
                    {
                        if (screenDistance < closestRadius)
                        {
                            
                            closestRadius = screenDistance;

                            // linear from 0 at radius to 1 at dead-on (0 distance)
                            float effectiveness = Mathf.InverseLerp(activeRadius, 0, screenDistance);

                            // TODO: you could look this up in a non-linear curve, such
                            // as by using a public AnimationCurve and setting it up.

                            // TODO: you could square or square-root this to change the "feel"

                            // After all the above, we have a value from 0 to 1 that
                            // tells us how much we want to drive the mouse.

                            // scale down by screen pixels
                            effectiveness /= ScreenMinimumAxis;

                            // master scaling amount
                            effectiveness *= AimingAssistanceStrength;

                            // how much is effectiveness reduced as you get far away?
                            float rangeEffectivenessAttenuation = Mathf.InverseLerp(
                                MaximumActiveLinearRange,
                                MinimumActiveLinearRange,
                                targetWorldDistanceRange);
                            // TODO: you might want range falloff to be non-linear, as above
                            // Re-lerp it to the floor attraction
                            rangeEffectivenessAttenuation = Mathf.Lerp(FloorAttractionAtMaxRange, 1.0f, rangeEffectivenessAttenuation);
                            // apply range attraction attenuation
                            effectiveness *= rangeEffectivenessAttenuation;

                            mouseAssistanceDrive = screenDelta * effectiveness;

                            adjusted = true;
                        }
                    }
                }
            }
        }

        if (adjusted)
        {
            mouseX += mouseAssistanceDrive.x;
            mouseY += mouseAssistanceDrive.y;
        }

        
        return adjusted;
    }
}