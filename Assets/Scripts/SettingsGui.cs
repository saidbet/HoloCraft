using HoloToolkit.Sharing;
using HoloToolkit.Sharing.VoiceChat;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsGui : BasePanel
{
    public BaseButton hirezScan;
    public BaseButton changeScene;
    public GameObject scenePanel;
    public GameObject anchorsPanel;
    public GameObject vuforiaToggle;
    public BaseButton modifyAnchors;
    public BaseButton close;
    public BaseButton restart;
    public BaseButton exit;
    public BaseButton clientScene;
    public BaseButton expertScene;
    public BaseButton hybrideScene;
    public BaseButton modifyHorizontal;
    public BaseButton modifyVertical;
    public BaseButton enableVoiceChat;
    public BaseButton backFromAnchors;
    public GameObject inputAnchorsPanel;
    public GameObject highlightPrefab;

    protected override void Start()
    {
        if (ShareManager.Instance.userType == NetworkSpawnManager.EXPERT)
        {
            if (ScanController.Instance.enableHiRezScan)
            {
                hirezScan.ChangeState(1);
            }
            else
            {
                hirezScan.ChangeState(0);
            }
            if(AnchorController.Instance.anchorsList.Count >= 3)
            {
                modifyAnchors.ChangeState(0);
            }else
            {
                modifyAnchors.ChangeState(2);
            }
        }

        changeScene.ChangeState(0);
        scenePanel.SetActive(false);
        anchorsPanel.SetActive(false);

        if (ShareManager.Instance.userType == NetworkSpawnManager.EXPERT)
        {
            MainController.Instance.guiPanel.gameObject.SetActive(false);
        }

        transform.position = Camera.main.transform.position + Camera.main.transform.forward * 3;

        highlight = Instantiate(highlightPrefab, transform);
        highlight.GetComponent<Renderer>().enabled = false;
        highlight.GetComponent<Renderer>().material.renderQueue = 3001;

        enableVoiceChat.ChangeState(MainController.Instance.voiceChatActive ? 1 : 0);
    }

    private void OnDestroy()
    {
        MainController.Instance.guiPanel.gameObject.SetActive(true);
    }

    public override void OnClick(BaseButton button)
    {
        if(button == hirezScan)
        {
            if (button.state == true)
            {
                ScanController.Instance.SetHirezScan(true);
                button.ChangeState(1);
            }
            else
            {
                ScanController.Instance.SetHirezScan(false);
                button.ChangeState(0);
            }
        }
        else if(button == changeScene)
        {
            scenePanel.SetActive(!scenePanel.activeSelf);
            anchorsPanel.SetActive(false);
        }
        else if(button == close)
        {
            ShareManager.Instance.spawnManager.Delete(this.gameObject);
        }
        else if(button == restart)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else if(button == exit)
        {
            Application.Quit();
        }
        else if(button == expertScene)
        {
            SceneManager.LoadScene("scene_expert");
        }
        else if(button == clientScene)
        {
            SceneManager.LoadScene("scene_client");
        }
        else if(button == hybrideScene)
        {
            SceneManager.LoadScene("scene_hybride");
        }
        else if(button == modifyAnchors)
        {
            ShowAnchorsPanel();
        }
        else if(button == backFromAnchors)
        {
            HideAnchorsPanel();
        } else if(button == modifyHorizontal)
        {
            ShowAnchorEdit(AnchorsInputGui.XAXIS);
        } else if(button == modifyVertical)
        {
            ShowAnchorEdit(AnchorsInputGui.YAXIS);
        } else if(button == enableVoiceChat)
        {
            ToggleVoiceChat();
        }
    }

    public void ToggleVoiceChat()
    {
        bool newState = !Camera.main.GetComponent<MicrophoneTransmitter>().enabled;
        MainController.Instance.EnableVoiceChat(newState);
        enableVoiceChat.ChangeState(newState?1:0);
    }
     
    public void ShowAnchorsPanel()
    {
        ToggleSettingsMainMenu(false);
        scenePanel.SetActive(false);
        anchorsPanel.transform.localPosition = new Vector3(-1.097f, anchorsPanel.transform.localPosition.y, anchorsPanel.transform.localPosition.z);
        anchorsPanel.SetActive(!anchorsPanel.activeSelf);
    }

    public void HideAnchorsPanel()
    {
        anchorsPanel.SetActive(!anchorsPanel.activeSelf);
        ToggleSettingsMainMenu(true);
    }

    public void ShowAnchorEdit(int type)
    {
        backFromAnchors.GetComponent<Renderer>().enabled = false;
        modifyHorizontal.GetComponent<Renderer>().enabled = false;
        modifyVertical.GetComponent<Renderer>().enabled = false;
        backFromAnchors.GetComponent<BoxCollider>().enabled = false;
        modifyHorizontal.GetComponent<BoxCollider>().enabled = false;
        modifyVertical.GetComponent<BoxCollider>().enabled = false;
        inputAnchorsPanel.SetActive(true);
        inputAnchorsPanel.transform.localPosition = new Vector3(0, inputAnchorsPanel.transform.localPosition.y, inputAnchorsPanel.transform.localPosition.z);
        inputAnchorsPanel.GetComponent<AnchorsInputGui>().ActivateBehaviour(type);

    }

    public void HideAnchorEdit()
    {
        inputAnchorsPanel.SetActive(false);
        backFromAnchors.GetComponent<Renderer>().enabled = true;
        modifyHorizontal.GetComponent<Renderer>().enabled = true;
        modifyVertical.GetComponent<Renderer>().enabled = true;
        backFromAnchors.GetComponent<BoxCollider>().enabled = true;
        modifyHorizontal.GetComponent<BoxCollider>().enabled = true;
        modifyVertical.GetComponent<BoxCollider>().enabled = true;        
    }

    public void ToggleSettingsMainMenu(bool state)
    {
        hirezScan.GetComponent<Renderer>().enabled = state;
        changeScene.GetComponent<Renderer>().enabled = state;
        modifyAnchors.GetComponent<Renderer>().enabled = state;
        close.GetComponent<Renderer>().enabled = state;
        restart.GetComponent<Renderer>().enabled = state;
        exit.GetComponent<Renderer>().enabled = state;
        hirezScan.GetComponent<BoxCollider>().enabled = state;
        changeScene.GetComponent<BoxCollider>().enabled = state;
        modifyAnchors.GetComponent<BoxCollider>().enabled = state;
        close.GetComponent<BoxCollider>().enabled = state;
        restart.GetComponent<BoxCollider>().enabled = state;
        exit.GetComponent<BoxCollider>().enabled = state;
    }
}
