using HoloToolkit.Sharing.SyncModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Sharing.Spawning;
using HoloToolkit.Sharing;

/// <summary>
/// AnchorSynchronizer: Class attached to any anchor instantiated on the network.
/// Each anchor is responsible for its own ruler. The ruler should be a descendant of the object possessing the World Anchor.
/// </summary>
public class AnchorSynchronizer : Synchronizer
{
    private SyncSpawnedObject dataModel;
    public SyncVector3 color;
    private MeshRenderer renderer;

    private void Start()
    {
        renderer = GetComponentInChildren<MeshRenderer>();
    }

    private void Update()
    {
        if (!MessageObserver.Instance.offline)
        {
            if (SharingStage.Instance.Manager.GetLocalUser().GetID() == dataModel.Owner.GetID())
            {
                if (renderer.material.color != GetColor(color))
                {
                    color.Value = GetVector(renderer.material.color);
                }
            }
            else
            {
                if (renderer.material.color != GetColor(color))
                {
                    renderer.material.color = GetColor(color);
                }
            }
        }
    }

    public override void LinkData(SyncSpawnedObject dataModel)
    {
        this.dataModel = dataModel;
        SyncAnchor syncAnchor = (SyncAnchor)dataModel;
        this.color = syncAnchor.color;
    }

    private Color GetColor(SyncVector3 color)
    {
        return new Color(color.Value.x, color.Value.y, color.Value.z);
    }

    private Vector3 GetVector(Color color)
    {
        return new Vector3(color.r, color.g, color.b);
    }
}

