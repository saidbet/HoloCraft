using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class WorkspaceMenu : Menu
{ 
    public void StartMoveWorkspace()
    {
        GetComponentInParent<Menu>().HideMenu();
        MainManager.Instance.currentMode = MainManager.Mode.Moving;
    }

    public void StartScaleWorkspace()
    {
        GetComponentInParent<Menu>().HideMenu();
        MainManager.Instance.currentMode = MainManager.Mode.Scaling;
    }

    public void StartPlay()
    {
        MainManager.Instance.StartPlayMode();
        GetComponentInParent<Menu>().HideMenu();
    }

    public void SaveCreation()
    {
        MainManager.Instance.SaveData();
        GetComponentInParent<Menu>().HideMenu();
    }

    public void LoadCreation()
    {
        GetComponentInParent<Menu>().HideMenu();
        MenuManager.Instance.ShowMenu(MenuManager.Instance.LoadingMenu);
    }
}
