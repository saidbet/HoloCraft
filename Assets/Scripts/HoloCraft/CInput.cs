using UnityEngine;

public static class CInput
{
    public enum Key
    {
        A, B, X, Y, RB, LB, Back, Start, R3, L3
    }

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

    public static bool backDown;
    public static bool startDown;

    public static bool backUp;
    public static bool startUp;

    public static float dpadX;
    public static float dpadY;

    public static bool dpadUp;
    public static bool dpadDown;
    public static bool dpadLeft;
    public static bool dpadRight;

    public static bool rightStickDown;
    public static bool leftStickDown;

    public static bool rightStickUp;
    public static bool leftStickUp;

    public static float leftStickX;
    public static float leftStickY;

    public static float rightStickX;
    public static float rightStickY;

    public static float leftTrigger;
    public static float rightTrigger;

    public static bool leftStickUsed;
    public static bool rightStickUsed;
    public static bool dpadUsed;

    public static float timer;

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
#if UNITY_EDITOR
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
#elif UNITY_WSA
        if (dpadUp)
        {
            if (!dpadUsed)
            {
                dpadUsed = true;
                return Direction.Up;
            }
        }
        else if (dpadDown)
        {
            if (!dpadUsed)
            {
                dpadUsed = true;
                return Direction.Down;
            }
        }
        else if (dpadRight)
        {
            if (!dpadUsed)
            {
                dpadUsed = true;
                return Direction.Right;
            }
        }
        else if (dpadLeft)
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
#endif
    }

    public static bool GetKeyDown(Key key)
    {
        bool returnValue;

        if (timer < 0)
        {
            switch (key)
            {
                case Key.A:
                    returnValue = aDown;
                    break;
                case Key.B:
                    returnValue = bDown;
                    break;
                case Key.X:
                    returnValue = xDown;
                    break;
                case Key.Y:
                    returnValue = yDown;
                    break;
                case Key.Back:
                    returnValue = backDown;
                    break;
                case Key.Start:
                    returnValue = startDown;
                    break;
                case Key.R3:
                    returnValue = rightStickDown;
                    break;
                case Key.L3:
                    returnValue = leftStickDown;
                    break;
                case Key.RB:
                    returnValue = rbDown;
                    break;
                case Key.LB:
                    returnValue = lbDown;
                    break;
                default:
                    return false;
            }
            if (returnValue == true)
                timer = 0.5f;
        }
        else
        {
            timer -= Time.deltaTime;
            returnValue = false;
        }
        return returnValue;
    }

    public static bool GetKeyUp(Key key)
    {
        bool returnValue;

        if (timer < 0)
        {
            switch (key)
            {
                case Key.A:
                    returnValue = aUp;
                    break;
                case Key.B:
                    returnValue = bUp;
                    break;
                case Key.X:
                    returnValue = xUp;
                    break;
                case Key.Y:
                    returnValue = yUp;
                    break;
                case Key.Back:
                    returnValue = backUp;
                    break;
                case Key.Start:
                    returnValue = startUp;
                    break;
                case Key.R3:
                    returnValue = rightStickUp;
                    break;
                case Key.L3:
                    returnValue = leftStickUp;
                    break;
                case Key.RB:
                    returnValue = rbUp;
                    break;
                case Key.LB:
                    returnValue = lbUp;
                    break;
                default:
                    return false;
            }
            if (returnValue == true)
                timer = 0.5f;
        }
        else
        {
            timer -= Time.deltaTime;
            returnValue = false;
        }
        return returnValue;
    }

    public static bool GetSubmitKey()
    {
        return GetKeyUp(Key.A);
    }
}
