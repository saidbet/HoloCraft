using HoloToolkit.Sharing;
using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NetworkDiscoveryManager))]
public class NetworkController : Singleton<NetworkController> {

    public const string SHARING_IP = "SharingIp";
    public string foundIp;
    public int tryConnectingForSeconds = 2;
    bool connected;
    NetworkDiscoveryManager discoveryManager;
    public GameObject ipInputPanel;
    public GameObject clientPanel;
    GameObject clientIpPanel;

	void Start () {
        discoveryManager = GetComponent<NetworkDiscoveryManager>();
        foundIp = PlayerPrefs.GetString(SHARING_IP, "");
        if (foundIp != "")
        {
            ConnectToServer(foundIp);
        } else
        {
            AskForIp();
        }
    }

    public void ConnectToServer(string ip)
    {
        foundIp = ip;
        connected = false;
        SharingStage.Instance.ConnectToServer(ip, SharingStage.Instance.ServerPort);
        SharingStage.Instance.SharingManagerConnected += Instance_SharingManagerConnected;
        // Check after x seconds if connected. If not, ask for IP and display error message
        StartCoroutine(CheckIfConnected());
    }

    IEnumerator CheckIfConnected()
    {
        yield return new WaitForSeconds(tryConnectingForSeconds);
        if (!connected)
        {
            AskForIp();
        }
    }

    private void Instance_SharingManagerConnected(object sender, System.EventArgs e)
    {
        // if the sharingManager is connected, broadcast the IP for any Hololens with a missing configuration
        connected = true;
        PlayerPrefs.SetString(SHARING_IP, foundIp);
        PlayerPrefs.Save();
        discoveryManager.PrepareToBroadcast(foundIp);
        if(MainController.Instance != null)
        {
            MainController.Instance.SetPanelsReady();
        }
        else if(ClientController.Instance != null)
        {
            // Make panel appear to client
            Destroy(clientIpPanel);
            ClientController.Instance.SetPanelsReady();
        }
    }

    void AskForIp()
    {
        // Two different behaviours: If Salesperson, ask the user for IP, if any other, open a broadcastListener and wait for an answer
        if(ShareManager.Instance.userType == NetworkSpawnManager.EXPERT || ShareManager.Instance.userType == NetworkSpawnManager.HYBRIDE)
        {
            GameObject panel = Instantiate(ipInputPanel);
            panel.transform.position = Camera.main.transform.position + (Camera.main.transform.forward);

        } else
        {
            // Display message with button "stay offline"
            clientIpPanel = Instantiate(clientPanel);
            clientIpPanel.transform.position = Camera.main.transform.position + (Camera.main.transform.forward);
            discoveryManager.PrepareToListen();
        }
    }
}
