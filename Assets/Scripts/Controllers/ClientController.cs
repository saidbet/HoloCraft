using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using HoloToolkit.Sharing.Spawning;
using HoloToolkit.Unity;

public class ClientController : Singleton<ClientController>
{
    public bool verandaPlaced;
    public List<GameObject> selectedWalls;
    public GameObject clientGuiPanelPrefab;
    public GameObject clientGuiPanel;
    public GameObject crownPrefab;

    private void Start()
    {
        ShareManager.Instance.onMessageEvent += Instance_onMessageEvent;
        selectedWalls = new List<GameObject>();
    }

    public void SetPanelsReady()
    {
        if (ShareManager.Instance.userType == NetworkSpawnManager.CLIENT)
        {
            StartCoroutine(SpawnGuiPanel());
        }
    }

    private IEnumerator SpawnGuiPanel()
    {
        int timer = 50;
        do
        {
            if (ShareManager.Instance.spawnManager != null && ShareManager.Instance.spawnManager.SyncSourceReady())
            {
                clientGuiPanel = ShareManager.Instance.spawnManager.Spawn(new SyncPanel(), clientGuiPanelPrefab, NetworkSpawnManager.EVERYONE, "");
                clientGuiPanel.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 1.5f;
                ShareManager.Instance.spawnManager.Spawn(new SyncSpawnedObject(), crownPrefab, NetworkSpawnManager.EVERYONE, "");
            }

            yield return new WaitForSeconds(0.1f);
            timer -= 1;
        }
        while (clientGuiPanel == null && timer > 0);

        if (clientGuiPanel == null)
        {
            clientGuiPanel = ShareManager.Instance.spawnManager.Spawn(new SyncPanel(), clientGuiPanelPrefab, NetworkSpawnManager.EVERYONE, "");
            clientGuiPanel.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 1;
        }
    }

    private void Instance_onMessageEvent(MessageSynchronizer syncMessage)
    {
        switch (syncMessage.messageType.Value)
        {
            case ShareManager.RESET_ALL:
                //TODO
                break;
            default:
                break;
        }
    }
}
