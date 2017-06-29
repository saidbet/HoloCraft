
using HoloToolkit.Sharing.Spawning;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public GameObject prefab;
    public const string AssetBundlesOutputPath = "/AssetBundles/";
    public string assetName = "";

    public GameObject furnitureGuiPrefab;
    public GameObject TestObject;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            ShareManager.Instance.spawnManager.Spawn(new SyncSpawnedObject(), prefab, 0, "");
        }
        else if (Input.GetKeyDown(KeyCode.V))
        {
            ShareManager.Instance.SendSyncMessage(ShareManager.DISPLAY_VERANDA, 2);
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            ShareManager.Instance.SendSyncMessage(ShareManager.CHANGE_VERANDA, 1);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            ShareManager.Instance.SendSyncMessage(ShareManager.CHANGE_VERANDA, 1);
        }
        else if (Input.GetKeyDown(KeyCode.G))
        {
            ShareManager.Instance.SendSyncMessage(ShareManager.PREVIEWED_VERANDA_CHANGED, 1);
        } else if (Input.GetKeyDown(KeyCode.S))
        {
            //CustomMeshSaver.Instance.SaveModels("test01");
        }

    }

}
