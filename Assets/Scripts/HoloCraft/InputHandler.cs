using HoloLensXboxController;
using HoloToolkit.Unity;
using System;
using UnityEngine;

public class InputHandler : MonoBehaviour
{

    private ControllerInput controllerInput;

    protected void Awake()
    {
        controllerInput = new ControllerInput(0, 0.10f);
    }

    void Update()
    {
        controllerInput.Update();

#if UNITY_EDITOR
        CInput.aDown = Input.GetKeyDown(KeyCode.JoystickButton0);
        CInput.aUp = Input.GetKeyUp(KeyCode.JoystickButton0);
        CInput.aHold = Input.GetKey(KeyCode.JoystickButton0);

        CInput.bDown = Input.GetKeyDown(KeyCode.JoystickButton1);
        CInput.bUp = Input.GetKeyUp(KeyCode.JoystickButton1);
        CInput.bHold = Input.GetKey(KeyCode.JoystickButton1);

        CInput.xDown = Input.GetKeyDown(KeyCode.JoystickButton2);
        CInput.xUp = Input.GetKeyUp(KeyCode.JoystickButton2);
        CInput.xHold = Input.GetKey(KeyCode.JoystickButton2);

        CInput.yDown = Input.GetKeyDown(KeyCode.JoystickButton3);
        CInput.yUp = Input.GetKeyUp(KeyCode.JoystickButton3);
        CInput.yHold = Input.GetKey(KeyCode.JoystickButton3);

        CInput.lbDown = Input.GetKeyDown(KeyCode.JoystickButton4);
        CInput.lbUp = Input.GetKeyUp(KeyCode.JoystickButton4);

        CInput.rbDown = Input.GetKeyDown(KeyCode.JoystickButton5);
        CInput.rbUp = Input.GetKeyUp(KeyCode.JoystickButton5);

        CInput.backDown = Input.GetKeyDown(KeyCode.JoystickButton6);
        CInput.backUp = Input.GetKeyUp(KeyCode.JoystickButton6);

        CInput.startDown = Input.GetKeyDown(KeyCode.JoystickButton7);
        CInput.startUp = Input.GetKeyUp(KeyCode.JoystickButton7);

        CInput.leftStickDown = Input.GetKeyDown(KeyCode.JoystickButton8);
        CInput.leftStickUp = Input.GetKeyUp(KeyCode.JoystickButton8);

        CInput.rightStickDown = Input.GetKeyDown(KeyCode.JoystickButton9);
        CInput.rightStickUp = Input.GetKeyUp(KeyCode.JoystickButton9);

        CInput.leftStickX = Input.GetAxis("LeftStickHoriz");
        CInput.leftStickY = Input.GetAxis("LeftStickVert");

        CInput.rightStickX = Input.GetAxis("RightStickHoriz");
        CInput.rightStickY = Input.GetAxis("RightStickVert");

        CInput.dpadX = Input.GetAxis("DpadHoriz");
        CInput.dpadY = Input.GetAxis("DpadVert");

        CInput.leftTrigger = Input.GetAxis("LeftTrigger");
        CInput.rightTrigger = Input.GetAxis("RightTrigger");

#elif UNITY_WSA
        CInput.aDown = controllerInput.GetButtonDown(ControllerButton.A);
        CInput.aUp = controllerInput.GetButtonUp(ControllerButton.A);
        CInput.aHold = controllerInput.GetButton(ControllerButton.A);

        CInput.bDown = controllerInput.GetButtonDown(ControllerButton.B);
        CInput.bUp = controllerInput.GetButtonUp(ControllerButton.B);
        CInput.bHold = controllerInput.GetButton(ControllerButton.B);

        CInput.xDown = controllerInput.GetButtonDown(ControllerButton.X);
        CInput.xUp = controllerInput.GetButtonUp(ControllerButton.X);
        CInput.xHold = controllerInput.GetButton(ControllerButton.X);

        CInput.yDown = controllerInput.GetButtonDown(ControllerButton.Y);
        CInput.yUp = controllerInput.GetButtonUp(ControllerButton.Y);
        CInput.yHold = controllerInput.GetButton(ControllerButton.Y);

        CInput.lbDown = controllerInput.GetButtonDown(ControllerButton.LeftShoulder);
        CInput.lbUp = controllerInput.GetButtonUp(ControllerButton.LeftShoulder);

        CInput.rbDown = controllerInput.GetButtonDown(ControllerButton.RightShoulder);
        CInput.rbUp = controllerInput.GetButtonUp(ControllerButton.RightShoulder);

        CInput.backDown = controllerInput.GetButtonDown(ControllerButton.View);
        CInput.backUp = controllerInput.GetButtonUp(ControllerButton.View);

        CInput.startDown = controllerInput.GetButtonDown(ControllerButton.Menu);
        CInput.startUp = controllerInput.GetButtonUp(ControllerButton.Menu);

        CInput.leftStickDown = controllerInput.GetButtonDown(ControllerButton.LeftThumbstick);
        CInput.leftStickUp = controllerInput.GetButtonUp(ControllerButton.LeftThumbstick);

        CInput.rightStickDown = controllerInput.GetButtonDown(ControllerButton.RightThumbstick);
        CInput.rightStickUp = controllerInput.GetButtonUp(ControllerButton.RightThumbstick);

        CInput.leftStickX = controllerInput.GetAxisLeftThumbstickX();
        CInput.leftStickY = controllerInput.GetAxisLeftThumbstickY();

        CInput.rightStickX = controllerInput.GetAxisRightThumbstickX();
        CInput.rightStickY = controllerInput.GetAxisRightThumbstickY();

        CInput.dpadUp = controllerInput.GetButton(ControllerButton.DPadUp);
        CInput.dpadDown = controllerInput.GetButton(ControllerButton.DPadDown);
        CInput.dpadLeft = controllerInput.GetButton(ControllerButton.DPadLeft);
        CInput.dpadRight = controllerInput.GetButton(ControllerButton.DPadRight);

        CInput.leftTrigger = controllerInput.GetAxisLeftTrigger();
        CInput.rightTrigger = controllerInput.GetAxisRightTrigger();
#endif

    }
}
