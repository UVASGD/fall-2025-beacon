using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] FactionSelectionMenu factionSelectionMenu;
    public void PlayPressed()
    {
        factionSelectionMenu.gameObject.SetActive(true);
        factionSelectionMenu.SpawnFactionIcons(FactionManager.i.ReturnFactionList());
    }

    public void OnCampaignStart()
    {
        //SceneManager.LoadScene("SampleScene", LoadSceneMode.Single); loading scene by string is terrifying. Please load by build menu ID for if/when we rename scenes for build
        //As punishment, I am leaving that commented for everyone to see. -David

        FactionManager.i.PrepGameplayFactions();
        
        SceneManager.LoadScene(1);
    }

    public void ExitPressed()
    {
        Application.Quit();
    }
}
