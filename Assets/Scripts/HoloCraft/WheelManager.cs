using System;
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
    public bool _oppositeDirection;

    public bool OppositeDirection
    {
        get {return _oppositeDirection;}
        set
        {
            if (value == true && speed > 0)
                speed = -speed;
            else if (value == false && speed < 0)
                speed = -speed;
        }
    }

    void Start()
    {

    }

    private void FixedUpdate()
    {

        if (MainManager.Instance.CurrentMode != MainManager.Mode.Playing || wheelCollider == null)
            return;

        if (Input.GetAxis("RightTrigger") != 0)
        {
            wheelCollider.brakeTorque = 0;
            accelValue = Input.GetAxis("RightTrigger");
            Accelerate();
        }
        else if (Input.GetAxis("LeftTrigger") != 0)
        {
            wheelCollider.brakeTorque = 0;
            accelValue = -Input.GetAxis("LeftTrigger");
            Accelerate();
        }
        else
        {
            accelValue = 0;
            Accelerate();
            if(wheelCollider.rpm < 10)
                wheelCollider.brakeTorque = 2;
        }

        if ((Input.GetAxis("LeftStickHoriz") != 0 && steerable))
        {
            steerValue = Input.GetAxis("LeftStickHoriz");
            Steer();
        }
        else
        {
            steerValue = 0;
            Steer();
        }
        Debug.Log(Input.GetAxis("LeftStickHoriz"));
        UpdateMeshePosition();

        if (OppositeDirection == true && speed > 0)
            speed = -speed;
        else if (OppositeDirection == false && speed < 0)
            speed = -speed;
    }

    private void UpdateMeshePosition()
    {
        Quaternion rot;
        Vector3 pos;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelMesh.transform.position = pos;
        wheelMesh.transform.rotation = rot;
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
        if (props != null)
        {
            speed = Utility.GetPropValue(props, Properties.Speed);
            steerable = Utility.GetBoolValue(props, Properties.Steerable);
            OppositeDirection = Utility.GetBoolValue(props, Properties.Direction);
        }
        else
        {
            speed = 30;
            steerable = true;
            OppositeDirection = false;
        }
    }

}
