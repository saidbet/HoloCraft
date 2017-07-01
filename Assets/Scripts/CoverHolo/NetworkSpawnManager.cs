using HoloToolkit.Sharing.Spawning;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using HoloToolkit.Unity;
using HoloToolkit.Sharing;

public class NetworkSpawnManager : SpawnManager<SyncSpawnedObject>
{
    private int objectCreationCounter;

    public const int EVERYONE = 0;
    public const int EXPERT = 1;
    public const int CLIENT = 2;
    public const int HYBRIDE = 3;

    public GameObject defaultParent;

    public bool SyncSourceReady()
    {
        if (SyncSource == null)
            return false;
        else
            return true;
    }

    public override void Delete(SyncSpawnedObject objectToDelete)
    {
        SyncSource.RemoveObject(objectToDelete);
    }

    public void Delete(GameObject objectToDelete)
    {
        if (SyncSource == null)
        {
            Destroy(objectToDelete);
        }
        else
        {
            if (objectToDelete.GetComponent<DataModelReference>() != null)
            {
                Delete(objectToDelete.GetComponent<DataModelReference>().dataModel);
            }
        }
    }

    protected override void InstantiateFromNetwork(SyncSpawnedObject spawnedObject)
    {
        if (spawnedObject.userGroup.Value == NetworkSpawnManager.EXPERT && ShareManager.Instance.userType == NetworkSpawnManager.CLIENT)
        {
            return;
        }
        else if(spawnedObject.userGroup.Value == NetworkSpawnManager.CLIENT && ShareManager.Instance.userType == NetworkSpawnManager.EXPERT)
        {
            return;
        }

        int thisUser = SharingStage.Instance.Manager.GetLocalUser().GetID();

        if (spawnedObject.ownerId.Value != thisUser)
        {
            GameObject parent = null;
            GameObject prefab = null;

            if (!string.IsNullOrEmpty(spawnedObject.ParentPath.Value))
                parent = GameObject.Find(spawnedObject.ParentPath.Value);

            if (!string.IsNullOrEmpty(spawnedObject.prefabPath.Value))
                prefab = Resources.Load(spawnedObject.prefabPath.Value) as GameObject;

            CreatePrefabInstance(spawnedObject, prefab, parent, spawnedObject.Name.Value);
        }
    }

    protected override void RemoveFromNetwork(SyncSpawnedObject removedObject)
    {
        if (removedObject.GameObject != null)
        {
            Debug.Log("removedObject");
            Destroy(removedObject.GameObject);
            removedObject.GameObject = null;
        }
    }

    protected override void SetDataModelSource()
    {
        SyncSource = NetworkManager.Root.InstantiatedPrefabs;
    }

    /// <summary>
    /// Generates a unique name
    /// </summary>
    /// <param name="baseName"></param>
    /// <returns></returns>
    protected virtual string CreateInstanceName(string baseName)
    {
        string instanceName = string.Format("{0}{1}_{2}", baseName, objectCreationCounter.ToString(), NetworkManager.AppInstanceUniqueId);
        objectCreationCounter++;
        return instanceName;
    }

    /// <summary>
    /// Spawns content with the given parent. If no parent is specified it will be parented to the spawn manager itself.
    /// </summary>
    /// <param name="dataModel">Data model to use for spawning.</param>
    /// <param name="prefab">prefab to spawn</param>
    /// <param name="userGroup">which group of users should this be shared with</param>
    /// <param name="localPosition">Local position for the new instance.</param>
    /// <param name="localRotation">Local rotation for the new instance.</param>
    /// <param name="localScale">optional local scale for the new instance. If not specified, uses the prefabs scale.</param>
    /// <param name="parent">Parent to assign to the object.</param>
    /// <param name="synchronizersToAdd">which synchronizers should be added to the GameObject.</param>
    /// Indicates if the spawned object is owned by this device or not.
    /// An object that is locally owned will be removed from the sync system when its owner leaves the session.
    /// </param>
    /// <returns>True if spawning succeeded, false if not.</returns>
    public GameObject Spawn(SyncSpawnedObject dataModel, GameObject prefab, int userGroup, string synchronizerToAdd, GameObject parent, Vector3 localPosition, Quaternion localRotation, Vector3? localScale, bool isOwnedLocally)
    {
        if (SyncSource == null)
        {
            dataModel.synchronizerToAdd.Value = synchronizerToAdd;
            dataModel.isEnabled.Value = true;
            GameObject instance = CreatePrefabInstance(dataModel, prefab, parent==null?defaultParent:parent, prefab.name + DateTime.Now.Ticks);
            instance.transform.position = localPosition;
            instance.transform.localScale = localScale.HasValue ? localScale.Value : prefab.transform.localScale;
            return instance;
        }

        if(parent == null)
        {
            parent = defaultParent;
        }

        dataModel.Transform.Position.Value = localPosition;
        dataModel.Transform.Rotation.Value = localRotation;
        dataModel.synchronizerToAdd.Value = synchronizerToAdd;
        dataModel.isEnabled.Value = true;

        dataModel.Transform.Scale.Value = Vector3.one;
        string instanceName = "";
        string prefabPath = "";

        if (prefab == null)
        {
            instanceName = CreateInstanceName("ParentGameObject");
        }
        else
        {
            dataModel.Transform.Scale.Value = localScale.HasValue ? localScale.Value : prefab.transform.localScale;
            prefabPath = "Prefabs/" + prefab.name;
            instanceName = CreateInstanceName(prefab.name);
        }

        User owner = SharingStage.Instance.Manager.GetLocalUser();

        dataModel.Initialize(instanceName, parent.transform.name, prefabPath, userGroup, owner.GetID());

        if (!isOwnedLocally)
            owner = null;

        SyncSource.AddObject(dataModel, owner);

        return CreatePrefabInstance(dataModel, prefab, parent, dataModel.Name.Value);
    }

    public GameObject Spawn(SyncSpawnedObject dataModel, GameObject prefab, int userGroup, string synchronizerToAdd, GameObject parent, Vector3 localPosition, Quaternion localRotation, Vector3? localScale)
    {
        return Spawn(dataModel, prefab, userGroup, synchronizerToAdd, parent, localPosition, localRotation, localScale, true);
    }

    public GameObject Spawn(SyncSpawnedObject dataModel, GameObject prefab, int userGroup, string synchronizerToAdd)
    {
        return Spawn(dataModel, prefab, userGroup, synchronizerToAdd, null, Vector3.zero, Quaternion.identity, null);
    }

    public GameObject Spawn(SyncSpawnedObject dataModel, GameObject prefab, int userGroup, string synchronizerToAdd, GameObject parent)
    {
        return Spawn(dataModel, prefab, userGroup, synchronizerToAdd, parent, Vector3.zero, Quaternion.identity, null);
    }

    protected virtual GameObject CreatePrefabInstance(SyncSpawnedObject dataModel, GameObject prefabToInstantiate, GameObject parentObject, string objectName)
    {
        GameObject instance = null;
        if(prefabToInstantiate != null)
        {
            instance = Instantiate(prefabToInstantiate, dataModel.Transform.Position.Value, dataModel.Transform.Rotation.Value);
        }
        else
        {
            instance = Instantiate(new GameObject(), dataModel.Transform.Position.Value, dataModel.Transform.Rotation.Value);
        }

        instance.transform.localScale = dataModel.Transform.Scale.Value;
        instance.transform.SetParent(parentObject.transform, false);
        instance.gameObject.name = objectName;

        dataModel.GameObject = instance;

        DataModelReference modelReference = instance.EnsureComponent<DataModelReference>();
        modelReference.dataModel = dataModel;

        if (!dataModel.synchronizerToAdd.Value.Equals("None"))
        {
            AddSynchronizers(instance, dataModel);
        }

        if (SyncSource != null && SharingStage.Instance.Manager.GetLocalUser().GetID() != dataModel.ownerId.Value)
        {
            RemoveComponents(instance);
        }

        return instance;
    }

    private void RemoveComponents(GameObject instance)
    {
        List<Component> liste = new List<Component>();
        instance.GetComponents(typeof(IRemoveOnSpawn), liste);

        foreach(Component c in liste)
        {
            Destroy(c);
        }
    }

    private void AddSynchronizers(GameObject instance, SyncSpawnedObject dataModel)
    {

        if (dataModel.synchronizerToAdd.Value.Equals("Custom"))
        {
            CustomTransformSynchronizer transformSynchronizer = instance.EnsureComponent<CustomTransformSynchronizer>();
            transformSynchronizer.syncTransform = dataModel.Transform;
        }
        else
        {
            if(instance.GetComponent<Renderer>() != null)
                instance.GetComponent<Renderer>().enabled = dataModel.isEnabled.Value;

            TransformSynchronizer transformSynchronizer = instance.EnsureComponent<TransformSynchronizer>();
            transformSynchronizer.TransformDataModel = dataModel.Transform;
            transformSynchronizer.isEnabled = dataModel.isEnabled;
        }
        if (Type.GetType(dataModel.synchronizerToAdd.Value) != null)
        {
            Synchronizer synchro = instance.AddComponent(Type.GetType(dataModel.synchronizerToAdd.Value)) as Synchronizer;
            synchro.LinkData(dataModel);
        }
    }
}
