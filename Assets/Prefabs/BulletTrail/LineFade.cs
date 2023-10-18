using UnityEngine;
using System.Collections;

namespace Bolt.AdvancedTutorial
{
    public class LineFade : MonoBehaviour
    {
        [SerializeField] private float speed = 10f;

        private LineRenderer lr;
        private Color color;

        void Start()
        {
            lr = GetComponent<LineRenderer>();
            color = lr.startColor; // Otteniamo il colore iniziale dall'oggetto LineRenderer
        }

        void Update()
        {
            // Modifica il canale alpha del colore
            color.a = Mathf.Lerp(color.a, 0, Time.deltaTime * speed);

            // Assegna il colore modificato all'oggetto LineRenderer
            lr.startColor = color;
            lr.endColor = color;
        }
    }
}
