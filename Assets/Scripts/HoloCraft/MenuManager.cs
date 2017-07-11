using System;
using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuManager : Singleton<MenuManager>
{
    public GameObject objectPicker;
    public GameObject workspaceMenu;
    public GameObject propertiesMenu;

    public bool inMenu;

    private void Start()
    {
        InputHandler.Instance.keyPress += Instance_keyPress;
    }

    private void Instance_keyPress(KeyPress obj)
    {
        if (MainManager.Instance.mode != MainManager.Mode.Building &&
            MainManager.Instance.mode != MainManager.Mode.WorkspaceMenu &&
            MainManager.Instance.mode != MainManager.Mode.PickerMenu &&
            MainManager.Instance.mode != MainManager.Mode.PropertiesMenu)
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

        if (obj.button == ControllerConfig.LEFTSTICK)
                ToggleMenu(propertiesMenu, !propertiesMenu.activeSelf);
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
            else if (menu == propertiesMenu)
                MainManager.Instance.mode = MainManager.Mode.PropertiesMenu;

            Selectable firstSelectable = menu.GetComponentInChildren<Selectable>();
            if(firstSelectable != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(firstSelectable.gameObject);
            }
        }

        if(state == false && (MainManager.Instance.mode == MainManager.Mode.PickerMenu || MainManager.Instance.mode == MainManager.Mode.WorkspaceMenu || MainManager.Instance.mode == MainManager.Mode.PropertiesMenu))
            MainManager.Instance.mode = MainManager.Mode.Building;
    }

}
