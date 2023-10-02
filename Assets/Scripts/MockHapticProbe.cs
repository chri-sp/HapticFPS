using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MockHapticProbe : MonoBehaviour
{

    // The Falcon device
    private MockFalcon falcon;

    // Simple force
    private int simpleForceIndex = -1;
    public bool useSimpleForce = true;
    public Vector3 simpleForce;



    // Use this for initialization
    void Start()
    {
        falcon = FindObjectOfType<MockFalcon>();

        StartCoroutine(InitiatePosition());

        simpleForceIndex = falcon.AddSimpleForce(simpleForce);

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Update simple force
        if (useSimpleForce)
        {
            if (simpleForceIndex < 0)
            {
                simpleForceIndex = falcon.AddSimpleForce(simpleForce);
            }
            else
            {
                falcon.UpdateSimpleForce(simpleForceIndex, simpleForce);
            }
        }
        else if (simpleForceIndex >= 0)
        {
            falcon.RemoveSimpleForce(simpleForceIndex);
            simpleForceIndex = -1;
        }
    }

    IEnumerator InitiatePosition()
    {
        simpleForceIndex = falcon.AddSimpleForce(new Vector3(0, 4, 0));
        yield return new WaitForSeconds(1);
        falcon.RemoveSimpleForce(0);
    }

}
