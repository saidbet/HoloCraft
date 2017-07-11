using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ObjectPicker : GuiMenu
{
    public BlocksArray blocksArray;
    public GameObject elementPrefab;
    public EventSystem eventSystem;
    public ColorBlock colorBlock;

    private void Awake()
    {
        guiElements = new GameObject[blocksArray.array.Length];

        for (int i = 0; i < blocksArray.array.Length; i++)
        {
            PlaceElement(i);
            guiElements[i].GetComponent<Image>().sprite = blocksArray.array[i].thumbnail;
        }

        //InputHandler.Instance.keyPress += Instance_keyPress;
        eventSystem.firstSelectedGameObject = guiElements[0];
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

    protected void PlaceElement(int index)
    {
        guiElements[index] = Instantiate(elementPrefab, transform);
        guiElements[index].GetComponent<RectTransform>().anchoredPosition = GetValidPosition(index);
        guiElements[index].GetComponent<Button>().colors = colorBlock;
    }

    protected Vector2 GetValidPosition(int index)
    {
        int x = index % nbrElementPerLine;

        int y = index / nbrElementPerLine;

        return new Vector2(x * elementCoeff, -y * elementCoeff);
    }

}
