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

#if UNITY_WSA

#endif

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

        CInput.back = Input.GetKeyDown(KeyCode.JoystickButton6);

        CInput.start = Input.GetKeyDown(KeyCode.JoystickButton7);

        CInput.leftStick = Input.GetKeyDown(KeyCode.JoystickButton8);

        CInput.rightStick = Input.GetKeyDown(KeyCode.JoystickButton9);

        CInput.leftStickX = Input.GetAxis("LeftStickHoriz");
        CInput.leftStickY = Input.GetAxis("LeftStickVert");

        CInput.rightStickX = Input.GetAxis("RightStickHoriz");
        CInput.rightStickY = Input.GetAxis("RightStickVert");

        CInput.dpadX = Input.GetAxis("DpadHoriz");
        CInput.dpadY = Input.GetAxis("DpadVert");

        CInput.leftTrigger = Input.GetAxis("LeftTrigger");
        CInput.rightTrigger = Input.GetAxis("RightTrigger");
#endif
    }
}
