using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using HoloToolkit.Sharing;

//Classe de base pour les boutons implémentant les carac communes à tous les boutons
public class BaseButton : MonoBehaviour, IInputClickHandler, IFocusable
{
    public float disableTime = 1;
    protected float disabledFor;
    protected bool clickDisabled;
    public Texture2D stateOn;
    public Texture2D stateOff;
    public Texture2D stateDisabled;
    public bool state;
    public BasePanel panel;
    Vector3 defaultScale;

    private void Awake()
    {
        defaultScale = transform.localScale;
        ChangeState(0);
    }

    protected void Start()
    {
        panel = GetComponentInParent<BasePanel>();
        ShareManager.Instance.onMessageEvent += Instance_onMessageEvent;
    }

    private void Instance_onMessageEvent(MessageSynchronizer obj)
    {
        switch(obj.messageType.Value)
        {
            case ShareManager.HIGHLIGHT:
                if(SharedController.GetPath(gameObject) == obj.stringData.Value)
                    ShowHighlight(obj.boolData.Value);
                break;
            case ShareManager.CLICKED:
                if (SharedController.GetPath(gameObject) == obj.stringData.Value)
                    Clicked();
                break;
        }
    }

    protected void OnDisable()
    {
        transform.localScale = defaultScale;
    }

    private void Update()
    {
        if (clickDisabled)
        {
            disabledFor -= Time.deltaTime;
            if(disabledFor < 0)
            {
                clickDisabled = false;
            }
        }
    }

    public void ChangeState(int setState)
    {
        if (setState == 0)
        {
            GetComponent<MeshRenderer>().material.SetTexture("_MainTex", stateOn);
            this.state = true;
            this.enabled = true;
        }
        else if (setState == 1)
        {
            GetComponent<MeshRenderer>().material.SetTexture("_MainTex", stateOff);
            this.state = false;
            this.enabled = true;
        }
        else if (setState == 2)
        {
            GetComponent<MeshRenderer>().material.SetTexture("_MainTex", stateDisabled);
            this.enabled = false;
        }
    }

    //Effet lors du clic sur un bouton
    public IEnumerator ScaleEffect()
    {
        transform.localScale = new Vector3(defaultScale.x - (defaultScale.x/10), defaultScale.y - (defaultScale.y / 10), defaultScale.z);
        yield return new WaitForSeconds(0.1f);
        transform.localScale = defaultScale;
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        if (!clickDisabled)
        {
            panel.OnClick(this);
            ShareManager.Instance.SendSyncMessage(ShareManager.CLICKED, SharedController.GetPath(gameObject));
        }
    }

    public void OnFocusEnter()
    {
        ShareManager.Instance.SendSyncMessage(ShareManager.HIGHLIGHT, true, SharedController.GetPath(gameObject), false);
    }

    private void ShowHighlight(bool state)
    {
        if (panel.highlight != null)
        {
            if (state == true)
            {
                panel.highlight.GetComponent<Renderer>().enabled = true;
                panel.highlight.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                panel.highlight.transform.rotation = transform.rotation;
                if(panel.highlight.transform.parent != transform.parent)
                {
                    panel.highlight.transform.SetParent(gameObject.transform.parent);
                }
                panel.highlight.transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, 0.0005f);
            }
            else
                panel.highlight.GetComponent<Renderer>().enabled = false;

        }
    }

    public void OnFocusExit()
    {
        ShareManager.Instance.SendSyncMessage(ShareManager.HIGHLIGHT, false, SharedController.GetPath(gameObject), false);
    }

    private void Clicked()
    {
        disabledFor = disableTime;
        clickDisabled = true;
        StartCoroutine(ScaleEffect());
    }

    private void OnDestroy()
    {
        if(ShareManager.Instance != null)
        {
            ShareManager.Instance.onMessageEvent -= Instance_onMessageEvent;
        }
    }
}
