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

    private Direction direction;

    private void Update()
    {
        if (MainManager.Instance.CurrentMode == MainManager.Mode.Building || MainManager.Instance.CurrentMode == MainManager.Mode.InMenu)
        {
            if (CInput.start)
                ToggleMenu(workspaceMenu);

            if (CInput.rbDown)
                ToggleMenu(objectPicker);

            if (CInput.leftStick)
                ToggleMenu(propertiesMenu);
        }
        if (MainManager.Instance.CurrentMode == MainManager.Mode.InMenu)
        {
            if (CInput.bUp)
                CloseMenus();

            direction = CInput.GetDpadDirection();
            if (direction != Direction.None)
                currentMenu.MoveSelection(direction);
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

            if (currentMenu != null)
            {
                MonoBehaviour monoObject = (MonoBehaviour)currentMenu;
                monoObject.gameObject.SetActive(false);
            }

            currentMenu = menu.GetComponent<IMenu>();
        }

        if (menu.activeSelf == false && MainManager.Instance.CurrentMode == MainManager.Mode.InMenu)
        {
            currentMenu = null;
            MainManager.Instance.CurrentMode = MainManager.Mode.Building;
        }
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
