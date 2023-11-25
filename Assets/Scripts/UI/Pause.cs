using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
public class Pause : MonoBehaviour
{

    public GameObject pauseMenu;
    public bool isPaused = false;

    private FirstPersonControllerFalcon player;
    private Weapon weapon;

    // Use this for initialization
    void Start()
    {
        pauseMenu.SetActive(false);
        player = GameObject.FindWithTag("Player").GetComponent<FirstPersonControllerFalcon>();
        weapon = GameObject.FindWithTag("Weapon").GetComponent<Weapon>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        isPaused = true;
        player.enabled = false;
        weapon.enabled = false;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        isPaused = false;
        player.enabled = true;
        weapon.enabled = true;
        Time.timeScale = 1f;    
    }
}
