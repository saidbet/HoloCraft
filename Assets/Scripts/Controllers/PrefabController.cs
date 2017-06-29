using HoloToolkit.Sharing.Spawning;
using HoloToolkit.Sharing.SyncModel;
using HoloToolkit.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabController : Singleton<PrefabController> {

    Dictionary<string, GameObject> prefabPool;
    public GameObject placeHolderPrefab;
    public string placeHolderName;
    public bool debugMode = true;

	void Start () {
        if (debugMode && placeHolderPrefab != null)
        {
            AddPrefabToPool(placeHolderName, placeHolderPrefab);
        }
	}

    private void InitializeDictionary()
    {

    }

    public void AddPrefabToPool(string key, GameObject prefab)
    {
        if(prefabPool == null)
        {
            prefabPool = new Dictionary<string, GameObject>();
        }

        if (!prefabPool.ContainsKey(key))
        {
            prefabPool.Add(key, prefab);
        }
    }

    public GameObject QueryPool(string key)
    {
        if (prefabPool != null && prefabPool.ContainsKey(key))
        {
            return prefabPool[key];
        } else
        {
            return null;
        }
    }

    public void MoveNetworkVeranda(Transform destination)
    {
        
    }
}
