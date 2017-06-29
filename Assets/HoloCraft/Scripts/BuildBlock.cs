using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildBlock : MonoBehaviour
{
    public SnapPoint[] snapPoints;

    public void DisableSnapPoints()
    {
        foreach (SnapPoint snapPoint in snapPoints)
        {
            Destroy(snapPoint.GetComponent<Rigidbody>());
            Destroy(snapPoint);
        }
    }
}
