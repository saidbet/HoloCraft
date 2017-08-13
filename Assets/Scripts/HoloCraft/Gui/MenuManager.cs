using HoloToolkit.Unity;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuManager : Singleton<MenuManager>
{
    public Menu objectPicker;
    public Menu workspaceMenu;
    public Menu propertiesMenu;
    public Menu LoadingMenu;
    public Menu PlayModeMenu;

    public bool inMenu;

    private Direction direction;

    private void Update()
    {
        if (MainManager.Instance.currentMode == MainManager.Mode.Building)
        {
            if (CInput.GetKeyUp(workspaceMenu.toggleKey))
                ShowMenu(workspaceMenu);

            if (CInput.GetKeyUp(objectPicker.toggleKey))
                ShowMenu(objectPicker);

            if (CInput.GetKeyUp(propertiesMenu.toggleKey))
                ShowMenu(propertiesMenu);
        }
        else if(MainManager.Instance.currentMode == MainManager.Mode.Playing)
        {
            if (CInput.GetKeyUp(PlayModeMenu.toggleKey))
                ShowMenu(PlayModeMenu);
        }
    }

    public void ShowMenu(Menu menu)
    {
        menu.gameObject.SetActive(true);
    }
}
