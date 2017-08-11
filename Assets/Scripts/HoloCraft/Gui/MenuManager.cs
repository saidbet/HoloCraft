using HoloToolkit.Unity;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuManager : Singleton<MenuManager>
{
    public GameObject objectPicker;
    public GameObject workspaceMenu;
    public GameObject propertiesMenu;
    public GameObject LoadingMenu;

    public bool inMenu;
    public IMenu currentMenu;

    private void Start()
    {
        InputHandler.Instance.keyPress += Instance_keyPress;
    }

    private void Instance_keyPress(KeyPress obj)
    {
        if (MainManager.Instance.CurrentMode == MainManager.Mode.Building)
        {
            if (obj.button == ControllerConfig.LB)
                ToggleMenu(workspaceMenu);

            if (obj.button == ControllerConfig.RB)
                ToggleMenu(objectPicker);

            if (obj.button == ControllerConfig.LEFTSTICK)
                ToggleMenu(propertiesMenu);
        }
        else if (MainManager.Instance.CurrentMode == MainManager.Mode.InMenu)
        {
            if (obj.button == ControllerConfig.Y)
                CloseMenus();

            if (obj.button == ControllerConfig.DOWN)
                currentMenu.MoveSelection(MainManager.Direction.Down);
            if (obj.button == ControllerConfig.UP)
                currentMenu.MoveSelection(MainManager.Direction.Up);
            if (obj.button == ControllerConfig.LEFT)
                currentMenu.MoveSelection(MainManager.Direction.Left);
            if (obj.button == ControllerConfig.RIGHT)
                currentMenu.MoveSelection(MainManager.Direction.Right);
        }
    }

    public void ToggleMenu(GameObject menu)
    {
        menu.SetActive(!menu.activeSelf);
        if (menu.activeSelf == true)
        {
            MainManager.Instance.CurrentMode = MainManager.Mode.InMenu;

            Selectable firstSelectable = menu.GetComponentInChildren<Selectable>();
            if (firstSelectable != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(firstSelectable.gameObject);
            }

            currentMenu = menu.GetComponent<IMenu>();
        }

        if (menu.activeSelf == false && MainManager.Instance.CurrentMode == MainManager.Mode.InMenu)
            MainManager.Instance.CurrentMode = MainManager.Mode.Building;
    }

    public void CloseMenus()
    {
        IMenu[] menus = GetComponentsInChildren<IMenu>();
        if (menus != null || menus.Length > 0)
        {
            for (int i = 0; i < menus.Length; i++)
            {
                MonoBehaviour monoObject = (MonoBehaviour)menus[i];
                monoObject.gameObject.SetActive(false);
            }
        }

        MainManager.Instance.CurrentMode = MainManager.Mode.Building;
    }

}
