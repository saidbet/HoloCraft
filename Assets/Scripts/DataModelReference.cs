using HoloToolkit.Sharing.Spawning;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataModelReference : MonoBehaviour
{
    public SyncSpawnedObject dataModel;

    private void OnDestroy()
    {
        ShareManager.Instance.spawnManager.Delete(gameObject);
    }
}
