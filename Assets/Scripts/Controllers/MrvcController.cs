using HoloToolkit.Unity;
using MixedRemoteViewCompositor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MrvcController : Singleton<MrvcController> {

    public bool activateMrvc = false;
    public MrvcManager manager;
    public VideoCompositor compositor = null;
    public bool isRemote = false;

    // Use this for initialization
    void Start () {
        if (isRemote)
        {
            Camera.main.transform.position = new Vector3(0, -5000, 0);
            ShareManager.Instance.onMessageEvent += Instance_onMessageEvent;
        }
        if(RemoteMainManager.Instance != null)
        {
            manager = GetComponent<MrvcManager>();
            if (manager != null)
            {
                RemoteMainManager.Instance.ConnectTo = manager.ConnectTo;
            }
            if (activateMrvc)
            {
                StartPlayback();
            }
            manager.PlayerStateChanged += OnPlayerStateChanged;
        }
	}

    private void Instance_onMessageEvent(MessageSynchronizer msg)
    {
        switch (msg.messageType.Value)
        {
            case ShareManager.WORLD_ANCHOR:
                UpdateWorldAnchor(msg.vectorData.Value, msg.quaternionData.Value);
                break;
            default:
                break;
        }
    }

    private void UpdateWorldAnchor(Vector3 position, Quaternion rotation)
    {
        GameObject anchor = ShareManager.Instance.spawnManager.defaultParent;
        anchor.transform.position = position;
        anchor.transform.localRotation = rotation;
    }

    public void OnPlayerStateChanged(object sender, StateChangedEventArgs<PlayerState> args)
    {
        if(args.CurrentState == PlayerState.Failed)
        {
            StartCoroutine(StartPlaybackLate());
        } else if(args.CurrentState == PlayerState.Stopping && args.CurrentState == PlayerState.Closing)
        {
            //StartCoroutine(StartPlaybackLate());
        } else if(args.CurrentState == PlayerState.Playing)
        {
            RemoteMainManager.Instance.ShowConnectingMessage(false);
        }
    }

    public void StartPlayback()
    {
        RemoteMainManager.Instance.ShowConnectingMessage(true);
        if (manager != null)
        {
            if (!manager.EnableMRC)
            {
                manager.EnableMRC = true;
            }
            if(manager.playbackEngine != null)
            {
                manager.StopPlayback();
                StartCoroutine(StartPlaybackLate());
            }
            else
            {
                manager.StartPlayback();
            }
        }
    }

    private IEnumerator StartPlaybackLate()
    {
        RemoteMainManager.Instance.ShowConnectingMessage(true);
        yield return new WaitForSeconds(3f);
        manager.StartPlayback();
    }

    public void ConnectToAddress(string ip)
    {
        manager.SetConnectorAddress(ip);
        // Hide the current view
        Camera.main.transform.position = new Vector3(0, -5000, 0);
        // Start the playback
        StartPlayback();
    }

    private IEnumerator FindVideoCompositor()
    {
        while (compositor == null)
        {
            compositor = Camera.main.GetComponent<VideoCompositor>();
            yield return new WaitForSeconds(0.2F);
        }
    }
}
