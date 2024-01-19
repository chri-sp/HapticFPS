using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VictoryMenu : MonoBehaviour {

    private SceneLoader loader;
    public GameObject victoryMenu;
    private Pause pause;
    private WeaponSwitching weaponSwitching;

    private static float timerLevel = 0;
    [SerializeField] private Text timerText;

    public bool hasWin = false;
    // Use this for initialization
    void Start()
    {
        victoryMenu.SetActive(false);
        loader = GetComponent<SceneLoader>();
        pause = GetComponent<Pause>();
        weaponSwitching = GameObject.FindWithTag("WeaponHolder").GetComponent<WeaponSwitching>();
    }

    void Update() {

        timerLevel += Time.deltaTime;
    }

    // Update is called once per frame
    public void OpenVictoryMenu()
    {
        weaponSwitching.disableRifle();
        weaponSwitching.disableShotgun();
        pause.isPaused = true;
        hasWin = true;
        victoryMenu.SetActive(true);
        TimeSpan t = TimeSpan.FromSeconds(timerLevel);
        string timer = string.Format("{0:D2}:{1:D2}",
                t.Minutes,
                t.Seconds);
        timerText.text = "Time\n"+ timer;
        timerLevel = 0;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void restartGame()
    {
        loader.setSpawnPoint(new Vector3(0, 284, 0));
        loader.ReloadGame();
    }
}
