using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuPanel : GuiMenu
{
    public int currentItem;

    private void Awake()
    {
        //InputHandler.Instance.keyPress += Instance_keyPress;
    }

    private void Instance_keyPress(KeyPress obj)
    {
        if (MainManager.Instance.mode != MainManager.Mode.WorkspaceMenu) return;

        if (obj.button == ControllerConfig.DOWN)
            GetNewPos(MainManager.Direction.Down);
        if (obj.button == ControllerConfig.UP)
            GetNewPos(MainManager.Direction.Up);
        if (obj.button == ControllerConfig.A)
            guiElements[currentItem].GetComponent<Button>().onClick.Invoke();
    }
}
