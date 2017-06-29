using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapPoint : MonoBehaviour
{
    public bool colliding;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.transform.tag == "SnapPoint")
        {
            Debug.Log("colliding with" + collider.transform.name);
            MainManager.Instance.SnapColliding();
        }
    }
}
