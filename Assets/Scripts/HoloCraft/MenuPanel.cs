using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuPanel : MonoBehaviour
{
    public Button[] menuItems;
    public HighlightManager highlight;
    public int currentItem;

	void Start ()
    {
        currentItem = 0;
        highlight.SetHighlight(menuItems[currentItem].gameObject);

        InputHandler.Instance.keyPress += Instance_keyPress;
    }

    private void Instance_keyPress(KeyPress obj)
    {
        if (MainManager.Instance.mode != MainManager.Mode.WorkspaceMenu) return;

        if (obj.button == ControllerConfig.DOWN)
            GetNewPos(MainManager.Direction.Down);
        if (obj.button == ControllerConfig.UP)
            GetNewPos(MainManager.Direction.Up);
        if (obj.button == ControllerConfig.A)
            menuItems[currentItem].onClick.Invoke();
    }

    private void GetNewPos(MainManager.Direction direction)
    {
        if (direction == MainManager.Direction.Down)
        {
            if (currentItem == 3)
            {
                currentItem = 0;
            }
            else
                currentItem += 1;
        }
        else if (direction == MainManager.Direction.Up)
        {
            if (currentItem == 0)
            {
                currentItem = 3;
            }
            else
                currentItem -= 1;
        }

        highlight.SetHighlight(menuItems[currentItem].gameObject);
    }

    public void MoveWorkspace()
    {
        MainManager.Instance.mode = MainManager.Mode.Moving;
        MenuManager.Instance.ToggleMenu(gameObject, false);
    }

    public void ScaleWorkspace()
    {
        MainManager.Instance.mode = MainManager.Mode.Scaling;
        MenuManager.Instance.ToggleMenu(gameObject, false);
    }
}
