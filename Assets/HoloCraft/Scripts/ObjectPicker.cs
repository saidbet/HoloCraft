using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[System.Serializable]
public struct Block
{
    public GameObject blockPrefab;
    public Button button;
    public Sprite blockImage;
}

public class ObjectPicker : MonoBehaviour
{
    public Block[] blocks;
    private int currentButton;
    public GameObject highlight;

    private void Start()
    {
        currentButton = 0;
        SetHighlight(blocks[currentButton].button.gameObject);
        foreach(var part in blocks)
        {
            part.button.GetComponent<Image>().sprite = part.blockImage;
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
            MainManager.Instance.ChangeObject(blocks[currentButton].blockPrefab);
    }

    private void SetHighlight(GameObject target)
    {
        highlight.transform.position = target.transform.position;
        highlight.SetActive(true);
    }

    public void GetNewPos(MainManager.Direction direction)
    {
        if(direction == MainManager.Direction.Right)
        {
            if (currentButton == 11)
            {
                currentButton = 0;
            }
            else
                currentButton += 1;
        }
        else if(direction == MainManager.Direction.Left)
        {
            if (currentButton == 0)
            {
                currentButton = 11;
            }
            else
                currentButton -= 1;
        }
        else if(direction == MainManager.Direction.Up)
        {
            if (currentButton < 3)
            {
                currentButton = currentButton + 9;
            }
            else
                currentButton -= 3;
        }
        else if(direction == MainManager.Direction.Down)
        {
            if (currentButton > 8)
            {
                currentButton = currentButton - 9;
            }
            else
                currentButton += 3;
        }

        SetHighlight(blocks[currentButton].button.gameObject);
    }
}
