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
        if (MainManager.Instance.CurrentMode != MainManager.Mode.Building &&
            MainManager.Instance.CurrentMode != MainManager.Mode.InMenu)
            return;

        if (obj.button == ControllerConfig.LB)
            ToggleMenu(workspaceMenu);

        if (obj.button == ControllerConfig.RB)
            ToggleMenu(objectPicker);

        if (obj.button == ControllerConfig.LEFTSTICK)
            ToggleMenu(propertiesMenu);
    }

    public void ToggleMenu(GameObject menu)
    {
        menu.SetActive(!menu.activeSelf);
        if (menu.activeSelf == true)
        {
            MainManager.Instance.CurrentMode = MainManager.Mode.InMenu;

            Selectable firstSelectable = menu.GetComponentInChildren<Selectable>();
            if(firstSelectable != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(firstSelectable.gameObject);
            }
        }

        if(menu.activeSelf == false && MainManager.Instance.CurrentMode == MainManager.Mode.InMenu)
            MainManager.Instance.CurrentMode = MainManager.Mode.Building;
    }

}
