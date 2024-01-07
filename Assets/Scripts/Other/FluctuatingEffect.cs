using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FluctuatingEffect : MonoBehaviour
{

    public float velocitaFluttuazioneVerticale = 1.0f;
    public float ampiezzaFluttuazioneVerticale = 1.0f;
    public float velocitaFluttuazioneOrizzontale = 1.0f;
    public float ampiezzaFluttuazioneOrizzontale = 1.0f;

    private Vector3 posizioneIniziale;

    void Start()
    {
        posizioneIniziale = transform.position;
        ampiezzaFluttuazioneVerticale = Random.Range(ampiezzaFluttuazioneVerticale - .5f, ampiezzaFluttuazioneVerticale + .5f);
        ampiezzaFluttuazioneOrizzontale = Random.Range(ampiezzaFluttuazioneOrizzontale - .5f, ampiezzaFluttuazioneOrizzontale + .5f);
        velocitaFluttuazioneVerticale = Random.Range(velocitaFluttuazioneVerticale - .5f, velocitaFluttuazioneVerticale + .5f);
        ampiezzaFluttuazioneOrizzontale = Random.Range(ampiezzaFluttuazioneOrizzontale - .5f, ampiezzaFluttuazioneOrizzontale + .5f);
    }

    void Update()
    {
        float movimentoFluttuanteVerticale = ampiezzaFluttuazioneVerticale * Mathf.Sin(Time.time * velocitaFluttuazioneVerticale);

        float movimentoFluttuanteOrizzontale = ampiezzaFluttuazioneOrizzontale * Mathf.Cos(Time.time * velocitaFluttuazioneOrizzontale);

        // Aggiorna la posizione dell'oggetto
        transform.position = posizioneIniziale + new Vector3(movimentoFluttuanteOrizzontale, movimentoFluttuanteVerticale, 0);
    }
}
