using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseManager : MonoBehaviour
{
    public EarthHealth earthHealth;
    public GameObject loseScreen;

    public delegate void Simple();
    public event Simple onLose;

    private bool gameIsLost;

    private void Update()
    {
        if (!gameIsLost && earthHealth.GetHealth() <= 0)
            LoseGame();
    }

    void LoseGame()
    {
        gameIsLost = true;
        if(onLose != null) onLose();

        loseScreen.SetActive(true);
        Time.timeScale = 0f;
    }


    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        Time.timeScale = 1f;
    }
}
