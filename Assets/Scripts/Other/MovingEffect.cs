using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingEffect : MonoBehaviour {

    public float velocitaY = 1.0f;
    public float ampiezzaY = 1.0f;
    public float velocitaX = 1.0f;
    public float ampiezzaX = 1.0f;

    private Vector3 posizioneIniziale;

    private AudioSource audioSource;

    void Start()
    {
        posizioneIniziale = transform.position;
        ampiezzaY = Random.Range(ampiezzaY - .25f, ampiezzaY + .25f);
        ampiezzaX = Random.Range(ampiezzaX - .25f, ampiezzaX + .25f);
        velocitaY = Random.Range(velocitaY - .25f, velocitaY + .25f);
        velocitaX = Random.Range(velocitaX - .25f, velocitaX + .25f);

        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
    }

    void Update()
    {
        float movimentoFluttuanteVerticale = ampiezzaY * Mathf.Sin(Time.time * velocitaY);
        float movimentoFluttuanteOrizzontale = ampiezzaX * Mathf.Cos(Time.time * velocitaX);

        Vector3 movimento = new Vector3(movimentoFluttuanteOrizzontale, movimentoFluttuanteVerticale, 0);
        movimento = transform.rotation * movimento;
        transform.position = posizioneIniziale + movimento;
    }
}
