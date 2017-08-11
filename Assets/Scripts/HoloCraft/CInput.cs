using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CInput
{
    public static bool aDown;
    public static bool bDown;
    public static bool xDown;
    public static bool yDown;

    public static bool aUp;
    public static bool bUp;
    public static bool xUp;
    public static bool yUp;

    public static bool aHold;
    public static bool bHold;
    public static bool xHold;
    public static bool yHold;

    public static bool rbDown;
    public static bool lbDown;

    public static bool rbUp;
    public static bool lbUp;

    public static bool back;
    public static bool start;

    public static float dpadX;
    public static float dpadY;

    public static bool rightStick;
    public static bool leftStick;

    public static float leftStickX;
    public static float leftStickY;

    public static float rightStickX;
    public static float rightStickY;

    public static float leftTrigger;
    public static float rightTrigger;

    public static bool leftStickUsed;
    public static bool rightStickUsed;
    public static bool dpadUsed;

    public static Direction GetRightAxisDirection()
    {
        if (rightStickX > 0.2)
        {
            if (!rightStickUsed)
            {
                rightStickUsed = true;
                return Direction.Right;
            }
        }
        else if (rightStickX < -0.2)
        {
            if (!rightStickUsed)
            {
                rightStickUsed = true;
                return Direction.Left;
            }
        }
        else if (rightStickY > 0.2)
        {
            if (!rightStickUsed)
            {
                rightStickUsed = true;
                return Direction.Up;
            }
        }
        else if (rightStickY < -0.2)
        {
            if (!rightStickUsed)
            {
                rightStickUsed = true;
                return Direction.Down;
            }
        }
        else
            rightStickUsed = false;

        return Direction.None;
    }

    public static Direction GetLeftAxisDirection()
    {
        if (leftStickX > 0.2)
        {
            if (!leftStickUsed)
            {
                leftStickUsed = true;
                return Direction.Right;
            }
        }
        else if (leftStickX < -0.2)
        {
            if (!leftStickUsed)
            {
                leftStickUsed = true;
                return Direction.Left;
            }
        }
        else if (leftStickY > 0.2)
        {
            if (!leftStickUsed)
            {
                leftStickUsed = true;
                return Direction.Up;
            }
        }
        else if (leftStickY < -0.2)
        {
            if (!leftStickUsed)
            {
                leftStickUsed = true;
                return Direction.Down;
            }
        }
        else
            leftStickUsed = false;

        return Direction.None;
    }

    public static Direction GetDpadDirection()
    {
        if (dpadY == 1)
        {
            if (!dpadUsed)
            {
                dpadUsed = true;
                return Direction.Up;
            }
        }
        else if (dpadY == -1)
        {
            if (!dpadUsed)
            {
                dpadUsed = true;
                return Direction.Down;
            }
        }
        else if (dpadX == 1)
        {
            if (!dpadUsed)
            {
                dpadUsed = true;
                return Direction.Right;
            }
        }
        else if (dpadX == -1)
        {
            if (!dpadUsed)
            {
                dpadUsed = true;
                return Direction.Left;
            }
        }
        else
            dpadUsed = false;

        return Direction.None;
    }
}
