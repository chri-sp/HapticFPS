using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FluctuatingEffect : MonoBehaviour
{

    public float velocitaVerticale = 1.0f;
    public float ampiezzaVerticale = 1.0f;
    public float velocitaOrizzontale = 1.0f;
    public float ampiezzaOrizzontale = 1.0f;

    private Vector3 posizioneIniziale;

    private AudioSource audioSource;

    void Start()
    {
        posizioneIniziale = transform.position;
        ampiezzaVerticale = Random.Range(ampiezzaVerticale - .5f, ampiezzaVerticale + .5f);
        ampiezzaOrizzontale = Random.Range(ampiezzaOrizzontale - .5f, ampiezzaOrizzontale + .5f);
        velocitaVerticale = Random.Range(velocitaVerticale - .5f, velocitaVerticale + .5f);
        ampiezzaOrizzontale = Random.Range(ampiezzaOrizzontale - .5f, ampiezzaOrizzontale + .5f);

        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
    }

    void Update()
    {
        float movimentoFluttuanteVerticale = ampiezzaVerticale * Mathf.Sin(Time.time * velocitaVerticale);

        float movimentoFluttuanteOrizzontale = ampiezzaOrizzontale * Mathf.Cos(Time.time * velocitaOrizzontale);

        // Aggiorna la posizione dell'oggetto
        transform.position = posizioneIniziale + new Vector3(movimentoFluttuanteOrizzontale, movimentoFluttuanteVerticale, 0);
    }
}
