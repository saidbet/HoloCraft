using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class verandaHolder : MonoBehaviour
{
    private void Start()
    {

    }

    void Update ()
    {
        transform.Rotate(0, 6.0F * 4.0F * Time.deltaTime, 0);
    }
}
