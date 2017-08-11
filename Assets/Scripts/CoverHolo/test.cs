using UnityEngine;

public class test : MonoBehaviour
{

    public GameObject plane;

    private void Update()
    {
        if (CInput.rightStick)
            plane.SetActive(!plane.activeSelf);
    }
}
