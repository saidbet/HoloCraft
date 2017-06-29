using HoloToolkit.Sharing;
using HoloToolkit.Sharing.Spawning;
using HoloToolkit.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharedController : Singleton<SharedController>
{
    public GameObject placedVeranda;
    public int currentVerandaId;
    public event Action currentVerandaIdChanged;
    public bool furnituresGuiSpawned;
    public GameObject backyardPrefab;
    public List<GameObject> anchorsList;

    private void Start()
    {
        currentVerandaId = 1;
        ShareManager.Instance.onMessageEvent += Instance_onMessageEvent;
    }

    private void Instance_onMessageEvent(MessageSynchronizer obj)
    {
        switch(obj.messageType.Value)
        {
            case ShareManager.VERANDA_PLACED:
                placedVeranda = GameObject.Find(obj.stringData.Value);
                break;

            case ShareManager.PREVIEWED_VERANDA_CHANGED:
                currentVerandaId = obj.integerData.Value;
                currentVerandaIdChanged();
                break;

            case ShareManager.ANCHORS_LIST:
                fetchAnchors(obj.stringData.Value);
                break;

        }
    }

    public static void SetActive(GameObject gameObject, bool state)
    {
        Collider[] colliders = gameObject.GetComponentsInChildren<Collider>();
        Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
        
        if(colliders != null && colliders.Length > 0)
        {
            foreach(Collider collider in colliders)
            {
                collider.enabled = state;
            }
        }
        if(renderers != null && colliders.Length > 0)
        {
            foreach(Renderer renderer in renderers)
            {
                renderer.enabled = state;
            }
        }

        BasePanel panel = gameObject.GetComponent<BasePanel>();
        if (panel != null && panel.highlight != null)
        {
            panel.highlight.GetComponent<Renderer>().enabled = false;
        }
    }

    public static string GetPath(GameObject obj)
    {
        string path = "/" + obj.name;
        while (obj.transform.parent != null)
        {
            obj = obj.transform.parent.gameObject;
            path = "/" + obj.name + path;
        }
        return path;
    }

    public void SetBackyard()
    {
        GameObject backyard = ShareManager.Instance.spawnManager.Spawn(new SyncSpawnedObject(), backyardPrefab, NetworkSpawnManager.EVERYONE, "");
    }

    public void fetchAnchors(string anchorsCSV)
    {
        string[] names = anchorsCSV.Split('*');
        if (names.Length == 3)
        {
            anchorsList = new List<GameObject>();
            foreach (string name in names)
            {
                anchorsList.Add(GameObject.Find(name));
            }
        }
    }

    public static bool ValidateIp(string ip)
    {
        string[] ipFields = ip.Split('.');
        if (ipFields.Length == 4)
        {
            int field0 = int.Parse(ipFields[0]);
            int field1 = int.Parse(ipFields[1]);
            int field2 = int.Parse(ipFields[2]);
            int field3 = int.Parse(ipFields[3]);
            if (!(field0 < 1 || field0 > 254
                || field1 < 0 || field1 > 255
                || field2 < 0 || field2 > 255
                || field3 < 1 || field3 > 254))
            {
                return true;
            }
        }
        return false;
    }
}
