using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkDiscoveryManager : NetworkDiscovery {

    public float StopBroadcastAfter = 60;
    
    public void PrepareToBroadcast(string sharingServiceIp)
    {
        Initialize();
        this.broadcastData = sharingServiceIp;
        StartAsServer();
        StartCoroutine(StopBroadcastingDelayed());
    }

    public void PrepareToListen()
    {
        Initialize();
        StartAsClient();
    }

    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        if(data != "" && SharedController.ValidateIp(data))
        {
            // Assign data (ip) as ip of the sharing service
            Debug.Log("IP Received and Valid");
            NetworkController.Instance.ConnectToServer(data);
            StopBroadcast();
        } else
        {
            Debug.Log("IP Received and NOT Valid");
        }
    }

    IEnumerator StopBroadcastingDelayed()
    {
        yield return new WaitForSeconds(StopBroadcastAfter);
        StopBroadcast();
    }
}
