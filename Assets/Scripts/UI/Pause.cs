using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class Pause : MonoBehaviour
{

    public GameObject pauseMenu;
    public bool isPaused = false;

    private FirstPersonControllerFalcon player;
    private PlayerHealth health;
    private HapticProbeFPS controller;

    private AudioManager audioManager;

    // Use this for initialization
    void Start()
    {
        pauseMenu.SetActive(false);
        player = GameObject.FindWithTag("Player").GetComponent<FirstPersonControllerFalcon>();
        controller = GameObject.FindWithTag("Player").GetComponent<HapticProbeFPS>();
        health = GameObject.FindWithTag("Player").GetComponent<PlayerHealth>();
        audioManager = GameObject.FindWithTag("AudioSystem").GetComponent<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isPaused && Input.GetKeyDown(KeyCode.Escape))
        {
            ResumeGame();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            controller.resetAllForces();
            PauseGame();
        }
    }

    public void PauseGame()
    {

        if (health.currentHealth() <= 0) return;

        audioManager.PauseAll();
        pauseMenu.SetActive(true);
        isPaused = true;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        audioManager.ResumeAll();
        pauseMenu.SetActive(false);
        isPaused = false;
        player.lockMouse();
        Time.timeScale = 1f;
    }
}
