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

    private void Start()
    {
        controllerInput = new ControllerInput(0, 0.10f);
    }

    private void FixedUpdate()
    {

        if (MainManager.Instance.CurrentMode != MainManager.Mode.Playing || wheelCollider == null)
            return;

        if (controllerInput.GetAxisRightTrigger() != 0)
        {
            wheelCollider.brakeTorque = 0;
            accelValue = controllerInput.GetAxisRightTrigger();
            Accelerate();
        }
        else if (controllerInput.GetAxisLeftTrigger() != 0)
        {
            wheelCollider.brakeTorque = 0;
            accelValue = -controllerInput.GetAxisLeftTrigger();
            Accelerate();
        }
        else
        {
            accelValue = 0;
            Accelerate();
            if (wheelCollider.rpm < 10)
                wheelCollider.brakeTorque = 2;
        }

        if (controllerInput.GetAxisLeftThumbstickX() != 0 && steerable)
        {
            steerValue = controllerInput.GetAxisLeftThumbstickX();
            Steer();
        }
        else
        {
            steerValue = 0;
            Steer();
        }

        UpdateMeshePosition();
    }

    private void Update()
    {
        controllerInput.Update();
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
