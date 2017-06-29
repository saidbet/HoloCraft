using HoloToolkit.Sharing.SyncModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Sharing.Spawning;
using HoloToolkit.Sharing;

public class RulerSynchroniser : Synchronizer
{
    public SyncString text;
    public string validText;
    TextMesh mesh;

    private void Start()
    {
        text.Value = "";
        validText = "";
        mesh = GetComponent<TextMesh>();
        mesh.text = "";
    }

    private void Update()
    {
        if (!MessageObserver.Instance.offline)
        {
            if (GetComponent<DataModelReference>().dataModel.ownerId.Value == SharingStage.Instance.Manager.GetLocalUser().GetID())
            {
                if (mesh.text != text.Value)
                {
                    text.Value = mesh.text;
                }
            }
            else
            {
                if (mesh.text != text.Value)
                {
                    mesh.text = text.Value;
                }
            }
        }
    }

    public override void LinkData(SyncSpawnedObject dataModel)
    {
        SyncText syncText = (SyncText)dataModel;
        this.text = syncText.stringData;
    }
}
