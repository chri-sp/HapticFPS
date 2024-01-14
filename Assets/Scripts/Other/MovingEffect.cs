using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingEffect : MonoBehaviour {

    public float velocitaFluttuazioneY = 1.0f;
    public float ampiezzaFluttuazioneY = 1.0f;
    public float velocitaFluttuazioneX = 1.0f;
    public float ampiezzaFluttuazioneX = 1.0f;

    private Vector3 posizioneIniziale;

    private AudioSource audioSource;

    void Start()
    {
        posizioneIniziale = transform.position;
        ampiezzaFluttuazioneY = Random.Range(ampiezzaFluttuazioneY - .25f, ampiezzaFluttuazioneY + .25f);
        ampiezzaFluttuazioneX = Random.Range(ampiezzaFluttuazioneX - .25f, ampiezzaFluttuazioneX + .25f);
        velocitaFluttuazioneY = Random.Range(velocitaFluttuazioneY - .25f, velocitaFluttuazioneY + .25f);
        velocitaFluttuazioneX = Random.Range(velocitaFluttuazioneX - .25f, velocitaFluttuazioneX + .25f);

        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
    }

    void Update()
    {
        float movimentoFluttuanteVerticale = ampiezzaFluttuazioneY * Mathf.Sin(Time.time * velocitaFluttuazioneY);
        float movimentoFluttuanteOrizzontale = ampiezzaFluttuazioneX * Mathf.Cos(Time.time * velocitaFluttuazioneX);

        Vector3 movimento = new Vector3(movimentoFluttuanteOrizzontale, movimentoFluttuanteVerticale, 0);
        movimento = transform.rotation * movimento;
        transform.position = posizioneIniziale + movimento;
    }
}
