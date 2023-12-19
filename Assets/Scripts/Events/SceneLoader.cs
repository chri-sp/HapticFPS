using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;

public class SceneLoader : MonoBehaviour
{
    private GameObject player;
    private static Vector3 playerPosition;


    void Awake()
    {
        player = GameObject.FindWithTag("Player");
        setPositionPlayer();
    }

    private void setPositionPlayer() {
        //all'avvio della scena imposto la posizione del player del checkpoint
        if (playerPosition.Equals(Vector3.zero))
            playerPosition = player.transform.position;
        player.transform.position = playerPosition;
    }

    public void setSpawnPoint(Vector3 position)
    {
        playerPosition = position;
    }

    public void ReloadGame()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        
        Time.timeScale = 1.0f;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
