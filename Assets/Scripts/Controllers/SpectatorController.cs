using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectatorController : MonoBehaviour {

	// Use this for initialization
	void Start () {
        ShareManager.Instance.onMessageEvent += Instance_onMessageEvent;
	}

    private void Instance_onMessageEvent(MessageSynchronizer msg)
    {
        Debug.Log("Spectator received a message: " + msg.messageType.Value);
    }
}
