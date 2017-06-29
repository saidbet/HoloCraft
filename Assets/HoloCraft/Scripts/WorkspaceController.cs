using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkspaceController : MonoBehaviour
{

    private void Start()
    {
        InputHandler.Instance.keyPress += Instance_keyPress;
    }

    private void Instance_keyPress(KeyPress obj)
    {
        if (MainManager.Instance.mode == MainManager.Mode.Building)
        {

            if (obj.button == ControllerConfig.RIGHTSTICKX)
                RotateWorkspace(MainManager.Axis.X, obj.value);
            if (obj.button == ControllerConfig.RIGHTSTICKY)
                RotateWorkspace(MainManager.Axis.Y, obj.value);
            if (obj.button == ControllerConfig.RIGHTSTICK)
                ResetRotation();
        }
        else if (MainManager.Instance.mode == MainManager.Mode.Moving)
        {
            if (obj.button == ControllerConfig.LEFTSTICKX)
            {
                if (obj.value > 0)
                    MoveWorkspace(MainManager.Direction.Right);
                else
                    MoveWorkspace(MainManager.Direction.Left);
            }
            if (obj.button == ControllerConfig.LEFTSTICKY)
            {
                if(obj.value > 0)
                    MoveWorkspace(MainManager.Direction.Up);
                else
                    MoveWorkspace(MainManager.Direction.Down);
            }
            if (obj.button == ControllerConfig.LEFTSTICK)
                ResetPosition();
            if (obj.button == ControllerConfig.Y)
                MoveWorkspace(MainManager.Direction.Forward);
            if (obj.button == ControllerConfig.X)
                MoveWorkspace(MainManager.Direction.Backward);
            if (obj.button == ControllerConfig.A)
                MainManager.Instance.mode = MainManager.Mode.Building;
        }
        else if(MainManager.Instance.mode == MainManager.Mode.Scaling)
        {
            if (obj.button == ControllerConfig.UP)
                ScaleWorkspace(MainManager.Direction.Up);
            if (obj.button == ControllerConfig.DOWN)
                ScaleWorkspace(MainManager.Direction.Down);
            if (obj.button == ControllerConfig.A)
                MainManager.Instance.mode = MainManager.Mode.Building;
        }
    }

    private void RotateWorkspace(MainManager.Axis axis, float value)
    {
        if(axis == MainManager.Axis.X)
        {
            transform.Rotate(0, -value*4, 0, Space.World);
        }
        else if(axis == MainManager.Axis.Y)
        {
            transform.Rotate(-value*4, 0, 0, Space.World);
        }
    }

    private void MoveWorkspace(MainManager.Direction direction)
    {
        Vector3 translation = Vector3.zero;
        switch (direction)
        {
            case MainManager.Direction.Left:
                translation = -Camera.main.transform.right;
                break;
            case MainManager.Direction.Right:
                translation = Camera.main.transform.right;
                break;
            case MainManager.Direction.Up:
                translation = Camera.main.transform.up;
                break;
            case MainManager.Direction.Down:
                translation = -Camera.main.transform.up;
                break;
            case MainManager.Direction.Backward:
                translation = -Camera.main.transform.forward;
                break;
            case MainManager.Direction.Forward:
                translation = Camera.main.transform.forward;
                break;
        }

        Vector3 newTranslation = Vector3.zero;

        newTranslation.x = MainManager.Round(translation.x);
        newTranslation.y = MainManager.Round(translation.y);
        newTranslation.z = MainManager.Round(translation.z);

        transform.position += newTranslation/4;
    }

    private void ResetRotation()
    {
        transform.localRotation = Quaternion.identity;
    }

    private void ResetPosition()
    {
        transform.position = Vector3.zero;
    }

    private void ScaleWorkspace(MainManager.Direction direction)
    {
        if (direction == MainManager.Direction.Up)
            transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
        else if (direction == MainManager.Direction.Down)
            transform.localScale -= new Vector3(0.1f, 0.1f, 0.1f);
    }
}
