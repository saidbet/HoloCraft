using HoloLensXboxController;
using HoloToolkit.Unity;
using UnityEngine;

public class WheelManager : MonoBehaviour, IPlayable
{
    public WheelCollider wheelCollider;
    public GameObject wheelMesh;
    public float speed;
    private float accelValue;
    private float steerValue;
    private BlockPropertiesValues props;

    private ControllerInput controllerInput;

    //Configurable options
    public bool steerable;
    public bool _oppositeDirection;

    public bool OppositeDirection
    {
        get { return _oppositeDirection; }
        set
        {
            if (value == true && speed > 0)
                speed = -speed;
            else if (value == false && speed < 0)
                speed = -speed;

            _oppositeDirection = value;
        }
    }

    private void Update()
    {

        if (MainManager.Instance.CurrentMode != MainManager.Mode.Playing || wheelCollider == null)
            return;

        if (CInput.rightTrigger != 0)
        {
            wheelCollider.brakeTorque = 0;
            accelValue = CInput.rightTrigger;
            Accelerate();
        }
        else if (CInput.leftTrigger != 0)
        {
            wheelCollider.brakeTorque = 0;
            accelValue = -CInput.leftTrigger;
            Accelerate();
        }
        else
        {
            accelValue = 0;
            Accelerate();
            if (wheelCollider.rpm < 10)
                wheelCollider.brakeTorque = 2;
        }

        if (CInput.leftStickX != 0 && steerable)
        {
            steerValue = CInput.leftStickX;
            Steer();
        }
        else
        {
            steerValue = 0;
            Steer();
        }

        UpdateMeshePosition();
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
