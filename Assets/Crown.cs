using HoloToolkit.Sharing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crown : MonoBehaviour
{
    private void Update()
    {
        if(GetComponent<DataModelReference>().dataModel.ownerId.Value == SharingStage.Instance.Manager.GetLocalUser().GetID())
        {
            transform.position = Camera.main.transform.position + (Camera.main.transform.up * 0.1f) + (-Camera.main.transform.forward * 0.1f);
        }
    }

}
