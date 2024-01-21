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
    private VictoryMenu victory;

    // Use this for initialization
    void Start()
    {
        pauseMenu.SetActive(false);
        player = GameObject.FindWithTag("Player").GetComponent<FirstPersonControllerFalcon>();
        controller = GameObject.FindWithTag("Player").GetComponent<HapticProbeFPS>();
        health = GameObject.FindWithTag("Player").GetComponent<PlayerHealth>();
        audioManager = GameObject.FindWithTag("AudioSystem").GetComponent<AudioManager>();
        victory = GetComponent<VictoryMenu>();  
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
            if (controller.isActive())
                controller.resetAllForces();
            PauseGame();
        }
    }

    public void PauseGame()
    {

        if (health.currentHealth() <= 0) return;
        if (victory.hasWin) return;

        pauseAudio();
        pauseMenu.SetActive(true);
        isPaused = true;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        if (victory.hasWin) return;

        pauseMenu.SetActive(false);
        resumeAudio();
        isPaused = false;
        player.lockMouse();
        Time.timeScale = 1f;
    }

    private void pauseAudio()
    {
        AudioController[] audioControllers = (AudioController[])FindObjectsOfType(typeof(AudioController));

        foreach (AudioController sounds in audioControllers)
        {
            sounds.PauseAll();
        }

        audioManager.PauseAll();
    }

    private void resumeAudio()
    {
        AudioController[] audioControllers = (AudioController[])FindObjectsOfType(typeof(AudioController));

        foreach (AudioController sounds in audioControllers)
        {
            sounds.ResumeAll();
        }

        audioManager.ResumeAll();
    }
}
