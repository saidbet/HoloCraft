using UnityEngine;

public class test : MonoBehaviour
{

    public GameObject plane;

    private void Update()
    {
        if (CInput.rightStickUp)
            plane.SetActive(!plane.activeSelf);
    }
}
