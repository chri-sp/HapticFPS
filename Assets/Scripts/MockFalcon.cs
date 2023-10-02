using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MockFalcon : MonoBehaviour
{


    // Displayed force
    public Vector3 force = Vector3.zero;

    // Buttons
    public bool[] buttons;

    // Use force feedback or not
    public bool useForceFeedback = true;


    private int lastForceIndex = 0;
    public List<SimpleForce> simpleForcesIndexes = new List<SimpleForce>();

    public class SimpleForce
    {

        public int index;

        public Vector3 force;

        public SimpleForce(int index, Vector3 force)
        {
            this.index = index;
            this.force = force;
        }
    }

    void Awake()
    {
        // Initialize buttons
        buttons = new bool[] { false, false, false, false };

    }



    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public int AddSimpleForce(Vector3 force)
    {
        simpleForcesIndexes.Add(new SimpleForce(lastForceIndex++, force));
        Debug.Log(simpleForcesIndexes.LastOrDefault().index + ", forza semplice: " + force);
        return simpleForcesIndexes.LastOrDefault().index;
    }

    public void UpdateSimpleForce(int i, Vector3 force)
    {

        foreach (SimpleForce forza in simpleForcesIndexes)
        {
            if (forza.index == i)
            {
                forza.force = force;
                Debug.Log(i + ", nuova forza semplice: " + force);
            }
        }


    }

    public void RemoveSimpleForce(int i)
    {

        foreach (SimpleForce forza in simpleForcesIndexes)
        {
            if (forza.index == i)
            {
                simpleForcesIndexes.Remove(forza);
                Debug.Log(i + ", forza semplice eliminata");
                break;
            }
        }

    }


    public void RemoveSimpleForces()
    {
        simpleForcesIndexes.Clear();
        Debug.Log("eliminate tutte le forze semplici");
    }
}
