using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelManager : MonoBehaviour {

    public WheelCollider wheelCollider;
    public GameObject wheelMesh;
    public int speed;

	void Start ()
    {
        InputHandler.Instance.keyPress += Instance_keyPress;
    }

    private void Update()
    {
        wheelMesh.transform.position = wheelCollider.transform.position;
        wheelMesh.transform.rotation = wheelCollider.transform.rotation;
    }

    private void Instance_keyPress(KeyPress obj)
    {

        if (obj.button == ControllerConfig.RIGHTTRIGGER)
            Accelerate(obj.value);
        if (obj.button == ControllerConfig.LEFTSTICK)
            Steer(obj.value);
    }

    private void Accelerate(float value)
    {
        Debug.Log("Accel");
        wheelCollider.motorTorque = value * speed;
    }

    private void Steer(float value)
    {
        wheelCollider.steerAngle = value * 45;
    }
}
