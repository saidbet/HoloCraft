using HoloToolkit.Sharing.Spawning;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataModelReference : MonoBehaviour
{
    public SyncSpawnedObject dataModel;

    private void OnDestroy()
    {
        try
        {
            ShareManager.Instance.spawnManager.Delete(gameObject);
        }
        catch(NullReferenceException e)
        {

        }
    }
}
