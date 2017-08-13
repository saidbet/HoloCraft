using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayModeMenu : Menu
{
    public void RepositionActiveCreation()
    {
        MainManager.Instance.RepositionCurrentCreation();
        GetComponentInParent<Menu>().HideMenu();
    }
}
