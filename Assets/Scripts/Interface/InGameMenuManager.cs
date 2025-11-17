using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameMenuManager : MonoBehaviour
{
    public GameObject menuGameObject;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)){
            ToggleMenu();
        }
    }

    public void ToggleMenu()
    {
        if (menuGameObject != null)
            menuGameObject.SetActive(!menuGameObject.activeSelf);
    }

    public void ExitGame()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
