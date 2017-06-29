using HoloToolkit.Sharing;
using HoloToolkit.Sharing.Spawning;
using HoloToolkit.Sharing.VoiceChat;
using HoloToolkit.Unity;
using MixedRemoteViewCompositor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RemoteMainManager : Singleton<RemoteMainManager> {

    public List<GameObject> listPanels;
    public InputField ipSalesmanField;
    public InputField ipSpectatorField;
    public InputField ipClientField;
    public string ipSalesman;
    public string ipSpectator;
    public string ipClient;
    public InputField xAxisField;
    public InputField yAxisField;
    private string connectTo;
    public List<GameObject> iconsList;
    public GameObject noAnchors;
    public GameObject anchorsOk;
    public GameObject connectingMessage;
    public GameObject pointer;
    public GameObject pointerPrefab;
    public GameObject blipPrefab;
    public GameObject voiceChatGroup;
    public Text textButtonMuteAudio;
    public Text textButtonMuteMic;
    public Text textVoiceChat;
    private bool clicking;
    private bool trailing;
    private bool micMute;
    private bool audioEnabled;
    public const int SALESPERSON = 1;
    public const int SPECTATOR = 2;
    public const int CLIENT = 3;

    public Text trailText;
    public Slider trailSlider;
    private float defaultTrailStartTime = 0.1f;
    public float trailStartTime;

    public enum ClickMode
    {
        PLACE_ICONS,
        SHOW_POINTER
    }

    public ClickMode clickMode;

    public string ConnectTo {
        get {
            return this.connectTo;
        }
        set {
            this.connectTo = value;
        }
    }

    private void Start()
    {
        clicking = false;
        ipClientField.text = ipClient;
        ipSpectatorField.text = ipSpectator;
        ipSalesmanField.text = ipSalesman;
        iconsList = new List<GameObject>();
        clickMode = ClickMode.PLACE_ICONS;
        audioEnabled = true;
        micMute = false;
        trailStartTime = defaultTrailStartTime;
        trailText.text = trailStartTime.ToString("F1");
        trailSlider.value = trailStartTime;
    }

    public void ShowConnectingMessage(bool show)
    {
        connectingMessage.SetActive(show);
    }

    private bool CheckRaycastUI()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);
        bool uiHit = false;
        foreach (RaycastResult r in results)
        {
            if(r.gameObject.layer == 5)
            {
                uiHit = true;
            }
            else
            {
                Debug.Log("hit layer: " + r.gameObject.layer);
            }
        }

        return uiHit;
    }

    private void Update()
    {
        if(Input.GetMouseButton(0) && clickMode == ClickMode.SHOW_POINTER)
        {
            if (!clicking)
            {
                clicking = true;
                StartCoroutine(CreateTrail());
            }
            if(pointer != null)
            {
                PositionPointer();
            }
        }

        if(Input.GetMouseButtonUp(0) && clickMode == ClickMode.SHOW_POINTER)
        {
            if (clicking)
            {
                if (!trailing)
                {
                    ActivateBeacon();
                }
                else
                {
                    trailing = false;
                    ShareManager.Instance.spawnManager.Delete(pointer);
                    pointer = null;
                }
                clicking = false;
            }
        }

        if (Input.GetMouseButtonDown(0) && clickMode == ClickMode.PLACE_ICONS)
        {
            if (!CheckRaycastUI())
            {
                TryPlaceIcon();
            }
            else
            {
                Debug.Log("clicked on UI");
            }
        }

        if (Input.GetMouseButtonDown(1) && clickMode == ClickMode.PLACE_ICONS)
        {
            if (!CheckRaycastUI())
            {
                TryDeleteIcon();
            }
        }
    }

    private void ShowAnchorsPanel()
    {
        if (SharedController.Instance.anchorsList == null || SharedController.Instance.anchorsList.Count < 3) return;
        noAnchors.SetActive(false);
        anchorsOk.SetActive(true);
        xAxisField.text = Vector3.Distance(SharedController.Instance.anchorsList[0].transform.position, SharedController.Instance.anchorsList[1].transform.position) + "";
        yAxisField.text = Vector3.Distance(SharedController.Instance.anchorsList[0].transform.position, SharedController.Instance.anchorsList[2].transform.position) + "";
    }

    public void ChangeMode(int mode)
    {
        if(mode == 0)
        {
            clickMode = ClickMode.PLACE_ICONS;
            if(pointer != null)
            {
                trailing = false;
                ShareManager.Instance.spawnManager.Delete(pointer);
                pointer = null;
            }
        } else
        {
            clickMode = ClickMode.SHOW_POINTER;
        }
        
    }

    public void ToggleVoiceChat()
    {
        bool voiceChatActive = !voiceChatGroup.activeSelf;
        voiceChatGroup.SetActive(voiceChatActive);
        textVoiceChat.text = voiceChatActive ? "<i>Disable</i>" : "<i>Enable</i>";
        Camera.main.gameObject.GetComponent<MicrophoneReceiver>().enabled = voiceChatActive;
        Camera.main.gameObject.GetComponent<MicrophoneTransmitter>().enabled = voiceChatActive;
        ShareManager.Instance.SendSyncMessage(ShareManager.VOICE_CHAT, voiceChatActive ? 1 : 0);
    }

    public void ChangeTraceDelay()
    {
        trailStartTime = trailSlider.value;
        trailText.text = trailStartTime.ToString("F1");
    }

    public void DefaultTraceDelay()
    {
        trailStartTime = defaultTrailStartTime;
        trailText.text = trailStartTime.ToString("F1");
        trailSlider.value = trailStartTime;
    }

    public void ActivateBeacon()
    {
        if (!CheckRaycastUI())
        {
            Vector3 pos = Input.mousePosition;
            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(pos), out hitInfo);
            if (hit)
            {
                GameObject beacon = ShareManager.Instance.spawnManager.Spawn(new SyncSpawnedObject(), blipPrefab, NetworkSpawnManager.EVERYONE, "");
                beacon.transform.position = hitInfo.point;
                beacon.transform.forward = hitInfo.normal;
            }
        }
        
    }

    private IEnumerator CreateTrail()
    {
        yield return new WaitForSeconds(trailStartTime);
        if (pointer == null && clicking)
        {
            Vector3 pos = Input.mousePosition;
            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(pos), out hitInfo);
            if (hit)
            {
                pointer = ShareManager.Instance.spawnManager.Spawn(new SyncSpawnedObject(), pointerPrefab, NetworkSpawnManager.EVERYONE, "", null, hitInfo.point, Quaternion.identity, new Vector3(0.03294296f, 0.03294296f, 0.03294296f));
                trailing = true;
            }
            
        }
    }

    public void PositionPointer()
    {
        Vector3 pos = Input.mousePosition;
        RaycastHit hitInfo = new RaycastHit();
        bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(pos), out hitInfo);
        if (hit)
        {
            pointer.transform.position = hitInfo.point;
            pointer.transform.forward = hitInfo.normal;
        }
    }

    public void TryPlaceIcon()
    {
        Vector3 pos = Input.mousePosition;
        RaycastHit hitInfo = new RaycastHit();
        bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(pos), out hitInfo);
        if (hit)
        {
            if (hitInfo.transform.CompareTag("expertIcons"))
            {
                GameObject icon = hitInfo.transform.gameObject;
                if (iconsList.Contains(icon))
                {
                    ShareManager.Instance.spawnManager.Delete(icon);
                    iconsList.Remove(icon);
                }
            }
            else
            {
                SyncSpawnedObject obj = new SyncSpawnedObject();
                GameObject icon = ShareManager.Instance.spawnManager.Spawn(obj, (GameObject)Resources.Load("Prefabs/expert_pointer"), NetworkSpawnManager.EVERYONE, "", null, hitInfo.point, Quaternion.identity, Vector3.one);
                icon.transform.position = hitInfo.point;
                icon.transform.forward = hitInfo.normal;
                icon.AddComponent<BoxCollider>();
                Bounds bounds = icon.GetComponent<BoxCollider>().bounds;
                bounds.extents = icon.GetComponent<Renderer>().bounds.extents;
                icon.tag = "expertIcons";
                iconsList.Add(icon);
            }
        }
    }

    public void TryDeleteIcon()
    {
        Vector3 pos = Input.mousePosition;
        RaycastHit hitInfo = new RaycastHit();
        bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(pos), out hitInfo);
        if (hit && hitInfo.transform.CompareTag("expertIcons"))
        {
            GameObject icon = hitInfo.transform.gameObject;
            if (iconsList.Contains(icon))
            {
                ShareManager.Instance.spawnManager.Delete(icon);
                iconsList.Remove(icon);
            }
        }
    }

    public void DeleteIcons()
    {
        foreach(GameObject i in iconsList)
        {
            ShareManager.Instance.spawnManager.Delete(i);
        }
        iconsList = new List<GameObject>();
    }

    public void TogglePanel(int index)
    {
        ShowAnchorsPanel();

        if(listPanels != null && listPanels.Count > index)
        {
            bool active = !listPanels[index].activeSelf;
            HidePanels();
            listPanels[index].SetActive(active);
        }
    }

    public void HidePanels()
    {
        if(listPanels != null)
        {
            foreach (GameObject g in listPanels)
            {
                g.SetActive(false);
            }
        }
    }

    public void Connect(int type)
    {
        string ip = "";
        switch (type)
        {
            case SALESPERSON:
                ip = ipSalesman;
                break;
            case SPECTATOR:
                ip = ipSpectator;
                break;
            case CLIENT:
                ip = ipClient;
                break;
        }

        if (ValidateAddress(ip))
        {
            HidePanels();
            MrvcController.Instance.ConnectToAddress(ip);
        }
    }

    public void ModifyAnchors()
    {
        float newX = ValidateMeasure(xAxisField.text);
        float newY = ValidateMeasure(yAxisField.text);
        ShareManager.Instance.SendSyncMessage(ShareManager.ADJUST_ANCHORS, "", new Vector3(newX, newY, 0), 0f, 0);
    }

    private float ValidateMeasure(string text)
    {
        return float.Parse(text);
    }

    private bool ValidateAddress(string address)
    {
        string ip = address;
        string[] listIps = ip.Split('.');
        if (listIps.Length != 4)
            return false;
        int block1, block2, block3, block4;
        block1 = Int32.Parse(listIps[0]);
        block2 = Int32.Parse(listIps[1]);
        block3 = Int32.Parse(listIps[2]);
        block4 = Int32.Parse(listIps[3]);
        if ((block1 < 1 || block1 > 255) ||
            (block2 < 0 || block1 > 255) ||
            (block3 < 0 || block1 > 255) ||
            (block4 < 1 || block1 > 255))
            return false;
        return true;
    }

    public void ExitProgram()
    {
        Application.Quit();
    }
    
    public void SaveAdresses()
    {
        if(ValidateAddress(ipSalesmanField.text) && ValidateAddress(ipSpectatorField.text) && ValidateAddress(ipClientField.text))
        {
            ipSalesman = ipSalesmanField.text;
            ipSpectator = ipSpectatorField.text;
            ipClient = ipClientField.text;
            HidePanels();
        }
    }

    public void ToggleAudio()
    {
        audioEnabled = !audioEnabled;
        Camera.main.gameObject.GetComponent<MicrophoneReceiver>().enabled = audioEnabled;
        textButtonMuteAudio.text = audioEnabled ? "Mute audio" : "Enable audio";
    }

    public void ToggleMic()
    {
        micMute = !micMute;
        Camera.main.gameObject.GetComponent<MicrophoneTransmitter>().Mute = micMute;
        textButtonMuteMic.text = micMute ? "Enable microphone": "Mute microphone";
    }

    public void ResetScene()
    {
        ShareManager.Instance.SendSyncMessage(ShareManager.RESET_ALL);
        TogglePanel(2);
        DeleteIcons();
    }

}
