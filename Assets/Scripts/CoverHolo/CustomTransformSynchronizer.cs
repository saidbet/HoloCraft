using HoloToolkit.Sharing;
using HoloToolkit.Sharing.Spawning;
using HoloToolkit.Sharing.SyncModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomTransformSynchronizer : Synchronizer
{
    public SyncTransform syncTransform;
	
	// Update is called once per frame
	void Update () {

        if (SharingStage.Instance.Manager.GetLocalUser().GetID() == GetComponent<DataModelReference>().dataModel.Owner.GetID())
        {

            if (syncTransform.Position.Value != gameObject.transform.localPosition)
            {
                syncTransform.Position.Value = gameObject.transform.localPosition;
            }
            if (syncTransform.Rotation.Value != gameObject.transform.rotation)
            {
                syncTransform.Rotation.Value = gameObject.transform.localRotation;
            }
            if (syncTransform.Scale.Value != gameObject.transform.localScale)
            {
                syncTransform.Scale.Value = gameObject.transform.localScale;
            }
        }

        if (gameObject.transform.localPosition != syncTransform.Position.Value)
        {
            gameObject.transform.localPosition = syncTransform.Position.Value;
        }
        if (gameObject.transform.localRotation != syncTransform.Rotation.Value)
        {
            gameObject.transform.localRotation = syncTransform.Rotation.Value;
        }
        if (gameObject.transform.localScale != syncTransform.Scale.Value)
        {
            gameObject.transform.localScale = syncTransform.Scale.Value;
        }
    }
}
