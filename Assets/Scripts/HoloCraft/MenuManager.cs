using System;
using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity;
using UnityEngine;

public class MenuManager : Singleton<MenuManager>
{
    public GameObject objectPicker;
    public GameObject workspaceMenu;

    public bool inMenu;

    private void Start()
    {
        InputHandler.Instance.keyPress += Instance_keyPress;
    }

    private void Instance_keyPress(KeyPress obj)
    {
        if (MainManager.Instance.mode != MainManager.Mode.Building &&
            MainManager.Instance.mode != MainManager.Mode.WorkspaceMenu &&
            MainManager.Instance.mode != MainManager.Mode.PickerMenu)
            return;

        if (obj.button == ControllerConfig.LB)
        {
            if (obj.type == KeyPress.DOWN)
                ToggleMenu(workspaceMenu, true);
            else if (obj.type == KeyPress.UP)
                ToggleMenu(workspaceMenu, false);
        }

        if (obj.button == ControllerConfig.RB)
        {
            if (obj.type == KeyPress.DOWN)
                ToggleMenu(objectPicker, true);
            else if (obj.type == KeyPress.UP)
                ToggleMenu(objectPicker, false);
        }
    }

    public void ToggleMenu(GameObject menu, bool state)
    {
        menu.SetActive(state);
        if (state == true)
        {
            if (menu == objectPicker)
                MainManager.Instance.mode = MainManager.Mode.PickerMenu;
            else if (menu == workspaceMenu)
                MainManager.Instance.mode = MainManager.Mode.WorkspaceMenu;
        }

        if(state == false && (MainManager.Instance.mode == MainManager.Mode.PickerMenu || MainManager.Instance.mode == MainManager.Mode.WorkspaceMenu))
            MainManager.Instance.mode = MainManager.Mode.Building;
    }

}
