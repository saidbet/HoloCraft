﻿using System;
using UnityEngine;
using HoloToolkit.Unity;

public class WheelManager : MonoBehaviour, IPlayable
{
    public WheelCollider wheelCollider;
    public GameObject wheelMesh;
    public float speed;
    private float accelValue;
    private float steerValue;
    private BlockPropertiesValues props;

    //Configurable options
    public bool steerable;
    public bool oppositeDirection;

    void Start()
    {
        InputHandler.Instance.keyPress += Instance_keyPress;
        if (oppositeDirection == true)
        {
            speed = -speed;
        }
    }

    private void Update()
    {
        if (MainManager.Instance.CurrentMode != MainManager.Mode.Playing || wheelCollider == null)
        {
            Debug.Log("WheelCollider is null");
            return;
        }

        UpdateMeshePosition();
        if (accelValue != 0)
        {
            accelValue = 0;
            Accelerate();
        }

        if (steerValue != 0)
        {
            accelValue = 0;
            Accelerate();
        }
    }

    private void UpdateMeshePosition()
    {
        Quaternion rot;
        Vector3 pos;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelMesh.transform.position = pos;
        wheelMesh.transform.rotation = rot;
    }

    private void Instance_keyPress(KeyPress obj)
    {
        if (MainManager.Instance.CurrentMode != MainManager.Mode.Playing || wheelCollider == null)
        {
            Debug.Log("wheelCollider is null " + this);
            return;
        }

        Debug.Log(this.name +" "+ wheelCollider);

        if (obj.button == ControllerConfig.RIGHTTRIGGER)
        {
            accelValue = obj.value;
            Accelerate();
        }
        else if (obj.button == ControllerConfig.LEFTTRIGGER)
        {
            accelValue = -obj.value;
            Accelerate();
        }
        if (obj.button == ControllerConfig.LEFTSTICKX && steerable)
        {
            steerValue = obj.value;
            Steer();
        }
    }

    private void Accelerate()
    {
        wheelCollider.motorTorque = accelValue * speed;
    }

    private void Steer()
    {
        wheelCollider.steerAngle = steerValue * 45;
    }

    public void Startplay()
    {
        Rigidbody rb = this.gameObject.EnsureComponent<Rigidbody>();
        props = GetComponent<BlockPropertiesValues>();
        if(props != null)
        {
            speed = Utility.GetPropValue(props, Properties.Speed);
            steerable = Utility.GetBoolValue(props, Properties.Steerable);
            oppositeDirection = Utility.GetBoolValue(props, Properties.Direction);
        }
    }

    public void SetWheelParameters()
    {
        wheelCollider.mass = 1;

        var spring = new JointSpring();
        spring.damper = 4.5f;
        spring.spring = 35;
        spring.targetPosition = 0;
        wheelCollider.suspensionSpring = spring;

        var frictionCurve = new WheelFrictionCurve();
        frictionCurve.extremumSlip = 0.4f;
        frictionCurve.extremumValue = 1;
        frictionCurve.asymptoteSlip = 0.8f;
        frictionCurve.asymptoteValue = 0.5f;
        frictionCurve.stiffness = 10;
        wheelCollider.forwardFriction = frictionCurve;
        wheelCollider.sidewaysFriction = frictionCurve;
    }

    private void OnDestroy()
    {
        InputHandler.Instance.keyPress -= Instance_keyPress;
    }
}
