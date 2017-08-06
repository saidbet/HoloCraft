using UnityEngine;

public class WheelManager : MonoBehaviour
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
        props = GetComponent<BlockPropertiesValues>();
        //speed = props.properties.Find(prop => prop.property == Properties.Speed).value;
        speed = 40;
        InputHandler.Instance.keyPress += Instance_keyPress;
        if (oppositeDirection == true)
        {
            speed = -speed;
        }
    }

    private void Update()
    {
        if (MainManager.Instance.CurrentMode != MainManager.Mode.Playing) return;

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
        if (MainManager.Instance.CurrentMode != MainManager.Mode.Playing) return;

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
}
