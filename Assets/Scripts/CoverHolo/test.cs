using UnityEngine;

public class test : MonoBehaviour
{

    public GameObject plane;

    private void Start()
    {
        InputHandler.Instance.keyPress += Instance_keyPress;
    }

    private void Instance_keyPress(KeyPress obj)
    {
        if (obj.button == ControllerConfig.RIGHTSTICK)
            plane.SetActive(!plane.activeSelf);
    }
}
