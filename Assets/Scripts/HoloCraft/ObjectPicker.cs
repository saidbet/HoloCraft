using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ObjectPicker : MonoBehaviour
{
    public BlocksArray blocksArray;
    public Button[] buttons;
    private int currentIndex;
    public HighlightManager highlight;

    private void Start()
    {
        currentIndex = 0;
        highlight.SetHighlight(buttons[currentIndex].gameObject);

        for(int i = 0; i < blocksArray.array.Length; i++)
        {
            buttons[i].image.sprite = blocksArray.array[i].thumbnail;
        }

        InputHandler.Instance.keyPress += Instance_keyPress;
    }

    private void Instance_keyPress(KeyPress obj)
    {
        if (MainManager.Instance.mode != MainManager.Mode.PickerMenu) return;

        if (obj.button == ControllerConfig.DOWN)
            GetNewPos(MainManager.Direction.Down);
        if (obj.button == ControllerConfig.UP)
            GetNewPos(MainManager.Direction.Up);
        if (obj.button == ControllerConfig.LEFT)
            GetNewPos(MainManager.Direction.Left);
        if (obj.button == ControllerConfig.RIGHT)
            GetNewPos(MainManager.Direction.Right);
        if (obj.button == ControllerConfig.A)
        {
            MainManager.Instance.ChangeObject(blocksArray.array[currentIndex].prefab);
        }
    }

    public void GetNewPos(MainManager.Direction direction)
    {
        if(direction == MainManager.Direction.Right)
        {
            if (currentIndex == 11)
            {
                currentIndex = 0;
            }
            else
                currentIndex += 1;
        }
        else if(direction == MainManager.Direction.Left)
        {
            if (currentIndex == 0)
            {
                currentIndex = 11;
            }
            else
                currentIndex -= 1;
        }
        else if(direction == MainManager.Direction.Up)
        {
            if (currentIndex < 3)
            {
                currentIndex = currentIndex + 9;
            }
            else
                currentIndex -= 3;
        }
        else if(direction == MainManager.Direction.Down)
        {
            if (currentIndex > 8)
            {
                currentIndex = currentIndex - 9;
            }
            else
                currentIndex += 3;
        }

        highlight.SetHighlight(buttons[currentIndex].gameObject);
    }
}
