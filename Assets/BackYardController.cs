using HoloToolkit.Sharing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackYardController : MonoBehaviour
{
    private void Start()
    {
        if(SharingStage.Instance.Manager.GetLocalUser().GetID() == GetComponent<DataModelReference>().dataModel.ownerId.Value)
        {
            transform.position = (SharedController.Instance.anchorsList[0].transform.position / 2) + (SharedController.Instance.anchorsList[1].transform.position / 2);
        }
    }

}
