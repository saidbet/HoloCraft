using HoloLensXboxController;
using HoloToolkit.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPress
{
    public const int AXIS = 0;
    public const int DOWN = 1;
    public const int UP = 2;

    public int button;
    public int type;
    public float value;

    public KeyPress(int button, int type)
    {
        this.button = button;
        this.type = type;
        this.value = 0;
    }

    public KeyPress(int button, float value)
    {
        this.button = button;
        this.value = value;
        this.type = 0;
    }
}

public class InputHandler : Singleton<InputHandler>
{

    private ControllerInput controllerInput;

    public event Action<KeyPress> keyPress;

    bool leftStickUsed;
    bool rightStickXUsed;
    bool rightStickYUsed;

    float leftStickX;
    float leftStickY;
    float rightStickX;
    float rightStickY;

    float leftTrigger;
    float rightTrigger;

    //only in unity editor
    float dpadX;
    float dpadY;
    bool dpadUsed;

    protected override void Awake()
    {
        base.Awake();
        controllerInput = new ControllerInput(0, 0.10f);
    }

    void Update()
    {
        #region UNITY_WSA
#if UNITY_WSA
        if (!Application.isEditor)
        {
            controllerInput.Update();
            if (controllerInput.GetButtonDown(ControllerButton.A))
                KeyDown(ControllerConfig.A);
            if (controllerInput.GetButtonDown(ControllerButton.B))
                KeyDown(ControllerConfig.B);
            if (controllerInput.GetButtonDown(ControllerButton.X))
                KeyDown(ControllerConfig.X);
            if (controllerInput.GetButtonDown(ControllerButton.Y))
                KeyDown(ControllerConfig.Y);

            if (controllerInput.GetButtonDown(ControllerButton.LeftShoulder))
                KeyDown(ControllerConfig.LB);
            if (controllerInput.GetButtonDown(ControllerButton.RightShoulder))
                KeyDown(ControllerConfig.RB);

            if (controllerInput.GetButtonDown(ControllerButton.RightThumbstick))
                KeyDown(ControllerConfig.RIGHTSTICK);
            if (controllerInput.GetButtonDown(ControllerButton.LeftThumbstick))
                KeyDown(ControllerConfig.LEFTSTICK);

            if (controllerInput.GetButtonDown(ControllerButton.DPadUp))
                KeyDown(ControllerConfig.UP);
            if (controllerInput.GetButtonDown(ControllerButton.DPadDown))
                KeyDown(ControllerConfig.DOWN);
            if (controllerInput.GetButtonDown(ControllerButton.DPadRight))
                KeyDown(ControllerConfig.RIGHT);
            if (controllerInput.GetButtonDown(ControllerButton.DPadLeft))
                KeyDown(ControllerConfig.LEFT);

            if (controllerInput.GetButtonUp(ControllerButton.LeftShoulder))
                KeyUp(ControllerConfig.LB);
            if (controllerInput.GetButtonUp(ControllerButton.RightShoulder))
                KeyUp(ControllerConfig.RB);

            leftTrigger = controllerInput.GetAxisLeftTrigger();
            rightTrigger = controllerInput.GetAxisRightTrigger();

            if (CheckAxis(leftTrigger))
                keyPress(new KeyPress(ControllerConfig.LEFTTRIGGER, leftTrigger));
            if (CheckAxis(rightTrigger))
                keyPress(new KeyPress(ControllerConfig.RIGHTTRIGGER, rightTrigger));

            leftStickX = controllerInput.GetAxisLeftThumbstickX();
            leftStickY = controllerInput.GetAxisLeftThumbstickY();
            rightStickX = controllerInput.GetAxisRightThumbstickX();
            rightStickY = controllerInput.GetAxisRightThumbstickY();

            //continious axis input
            if (CheckAxis(leftStickX))
                keyPress(new KeyPress(ControllerConfig.LEFTSTICKX, leftStickX));

            if (CheckAxis(leftStickY))
                keyPress(new KeyPress(ControllerConfig.LEFTSTICKY, leftStickY));

            if (CheckAxis(rightStickX))
                keyPress(new KeyPress(ControllerConfig.RIGHTSTICKX, rightStickX));

            if (CheckAxis(rightStickY))
                keyPress(new KeyPress(ControllerConfig.RIGHTSTICKY, rightStickY));

            //Single input
            if (leftStickX > 0.2)
            {
                if (!leftStickUsed)
                {
                    KeyDown(ControllerConfig.LEFTSTICKRIGHT);
                    leftStickUsed = true;
                }
            }
            else if (leftStickX < 0.2)
            {
                if (!leftStickUsed)
                {
                    KeyDown(ControllerConfig.LEFTSTICKLEFT);
                    leftStickUsed = true;
                }
            }
            else if (leftStickY > 0.2)
            {
                if (!leftStickUsed)
                {
                    KeyDown(ControllerConfig.LEFTSTICKUP);
                    leftStickUsed = true;
                }
            }
            else if (leftStickY < 0.2)
            {
                if (!leftStickUsed)
                {
                    KeyDown(ControllerConfig.LEFTSTICKDOWN);
                    leftStickUsed = true;
                }
            }
            else
                leftStickUsed = false;
        }
#endif
        #endregion

        #region UNITY_EDITOR
#if UNITY_EDITOR

        if (Input.GetKeyDown(KeyCode.JoystickButton0))
            KeyDown(ControllerConfig.A);
        if (Input.GetKeyDown(KeyCode.JoystickButton1))
            KeyDown(ControllerConfig.B);
        if (Input.GetKeyDown(KeyCode.JoystickButton2))
            KeyDown(ControllerConfig.X);
        if (Input.GetKeyDown(KeyCode.JoystickButton3))
            KeyDown(ControllerConfig.Y);

        if (Input.GetKeyDown(KeyCode.JoystickButton4))
            KeyDown(ControllerConfig.LB);
        if (Input.GetKeyDown(KeyCode.JoystickButton5))
            KeyDown(ControllerConfig.RB);

        if (Input.GetKeyDown(KeyCode.JoystickButton9))
            KeyDown(ControllerConfig.RIGHTSTICK);
        if (Input.GetKeyDown(KeyCode.JoystickButton8))
            KeyDown(ControllerConfig.LEFTSTICK);

        if (Input.GetKeyUp(KeyCode.JoystickButton4))
            KeyUp(ControllerConfig.LB);
        if (Input.GetKeyUp(KeyCode.JoystickButton5))
            KeyUp(ControllerConfig.RB);

        dpadX = Input.GetAxisRaw("DpadHoriz");
        dpadY = Input.GetAxisRaw("DpadVert");

        if (dpadY == 1)
        {
            if (!dpadUsed)
            {
                KeyDown(ControllerConfig.UP);
                dpadUsed = true;
            }
        }
        else if (dpadY == -1)
        {
            if (!dpadUsed)
            {
                KeyDown(ControllerConfig.DOWN);
                dpadUsed = true;
            }
        }
        else if (dpadX == 1)
        {
            if (!dpadUsed)
            {
                KeyDown(ControllerConfig.RIGHT);
                dpadUsed = true;
            }
        }
        else if (dpadX == -1)
        {
            if (!dpadUsed)
            {
                KeyDown(ControllerConfig.LEFT);
                dpadUsed = true;
            }
        }
        else
            dpadUsed = false;


        leftStickX = Input.GetAxis("LeftStickHoriz");
        leftStickY = Input.GetAxis("LeftStickVert");
        rightStickX = Input.GetAxis("RightStickHoriz");
        rightStickY = Input.GetAxis("RightStickVert");

        //Single input
        if (leftStickX > 0.2)
        {
            if (!leftStickUsed)
            {
                KeyDown(ControllerConfig.LEFTSTICKRIGHT);
                leftStickUsed = true;
            }
        }
        else if (leftStickX < -0.2)
        {
            if (!leftStickUsed)
            {
                KeyDown(ControllerConfig.LEFTSTICKLEFT);
                leftStickUsed = true;
            }
        }
        else if (leftStickY > 0.2)
        {
            if (!leftStickUsed)
            {
                KeyDown(ControllerConfig.LEFTSTICKUP);
                leftStickUsed = true;
            }
        }
        else if (leftStickY < -0.2)
        {
            if (!leftStickUsed)
            {
                KeyDown(ControllerConfig.LEFTSTICKDOWN);
                leftStickUsed = true;
            }
        }
        else
            leftStickUsed = false;

        //Continious axis input
        if (CheckAxis(leftStickX))
            keyPress(new KeyPress(ControllerConfig.LEFTSTICKX, leftStickX));

        if (CheckAxis(leftStickY))
            keyPress(new KeyPress(ControllerConfig.LEFTSTICKY, leftStickY));

        if (CheckAxis(rightStickX))
            keyPress(new KeyPress(ControllerConfig.RIGHTSTICKX, rightStickX));

        if (CheckAxis(rightStickY))
            keyPress(new KeyPress(ControllerConfig.RIGHTSTICKY, rightStickY));


        //triggers
        leftTrigger = Input.GetAxis("LeftTrigger");
        rightTrigger = Input.GetAxis("RightTrigger");

        if (CheckAxis(leftTrigger))
            keyPress(new KeyPress(ControllerConfig.LEFTTRIGGER, leftTrigger));
        if (CheckAxis(rightTrigger))
            keyPress(new KeyPress(ControllerConfig.RIGHTTRIGGER, rightTrigger));

#endif
        #endregion
    }

    private void KeyDown(int key)
    {
        keyPress(new KeyPress(key, KeyPress.DOWN));
    }

    private void KeyUp(int key)
    {
        keyPress(new KeyPress(key, KeyPress.UP));
    }

    private bool CheckAxis(float axis)
    {
        if (axis > 0.20 || axis < -0.20)
            return true;
        else
            return false;
    }
}
