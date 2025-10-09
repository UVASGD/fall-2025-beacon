using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStateManager : MonoBehaviour
{
    //UI state manager stores if a menu is currently opened. If so, it should not be possible to open a new menu.
    public static UIStateManager i;
    private bool menuOpened = false;
    public bool MenuOpened => menuOpened; //making it read-only so it may only be changed by function call

    private void Awake()
    {
        i = this;
    }

    public bool checkMenuState()
    {
        return menuOpened;
    }

    public void onMenuOpened()
    {
        menuOpened = true;
    }
    public void onMenuClosed()
    {
        menuOpened = false;
    }
}