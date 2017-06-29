using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using HoloToolkit.Unity;
using HoloToolkit.Sharing.Spawning;

public class VerandaPreview : MonoBehaviour, IInputClickHandler
{
    public GameObject verandaInfos;
    private GameObject verandaPreview;
    public bool previewActivated;
    public bool infoDisplayed;
    public float verandaPreviewScale;
    public bool expert;

	void Start () {
        SharedController.Instance.currentVerandaIdChanged += Instance_currentVerandaIdChanged;
        ShareManager.Instance.onMessageEvent += Instance_onMessageEvent;
        previewActivated = false;
        infoDisplayed = false;
    }

    private void Instance_onMessageEvent(MessageSynchronizer obj)
    {
        if (obj.messageType.Value == ShareManager.HIDE_PREVIEW)
        {
            HideVeranda();
        }
    }

    private void Instance_currentVerandaIdChanged()
    {
        LoadVeranda();
    }

    public void LoadVeranda()
    {
        if (expert && ShareManager.Instance.userType == NetworkSpawnManager.CLIENT) return;

        GameObject prefab = CloudDataManager.Instance.GetPrefab(SharedController.Instance.currentVerandaId);

        if(verandaPreview != null)
        {
            Destroy(verandaPreview);
        }

        verandaPreview = Instantiate(prefab);
        verandaPreview.transform.position = transform.position;
        verandaPreview.transform.SetParent(transform);

        if (verandaPreview != null)
        {
            GameObject anchor1 = verandaPreview.transform.Find("O").gameObject;
            GameObject anchor2 = verandaPreview.transform.Find("X").gameObject;
            float tailleMax = Vector3.Distance(transform.position, transform.parent.position);
            float tailleVeranda = Vector3.Distance(anchor1.transform.localPosition, anchor2.transform.localPosition);
            float scale = tailleMax / tailleVeranda;

            verandaPreview.transform.localScale = new Vector3(scale, scale, scale) * verandaPreviewScale;

            if(verandaInfos != null)
            {
                verandaPreviewPanel preview = verandaInfos.GetComponent<verandaPreviewPanel>();
                preview.updatePreviewText(verandaPreview.name, "Length\t3M\nWidth\t5M\nHeight\t3M", "Wood\nStone", "Units sold in the past quarter: 100");
            }

            if(previewActivated == false)
                DisplayVeranda();
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (previewActivated && GazeManager.Instance.HitObject != verandaPreview)
        {
            transform.Rotate(0, 6.0F * 4.0F * Time.deltaTime, 0);
        }
	}

    private void DisplayVeranda()
    {
        if(ShareManager.Instance.userType != NetworkSpawnManager.CLIENT && verandaInfos != null)
        {
            verandaInfos.SetActive(true);
            infoDisplayed = true;
        }

        previewActivated = true;
    }

    public void HideVeranda()
    {
        Destroy(verandaPreview);
        verandaInfos.SetActive(false);
        previewActivated = false;

        if(infoDisplayed == true)
        {
            verandaInfos.SetActive(false);
            infoDisplayed = false;
        }
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        if ((GazeManager.Instance.HitObject.Equals(verandaPreview) || 
            GazeManager.Instance.HitObject.CompareTag("PreviewInfos")) && 
            MainController.Instance.State == MainController.RunningStates.scanningDone &&
            ShareManager.Instance.userType != NetworkSpawnManager.CLIENT)
        {
            MainController.Instance.StartPlacingAnchors();
        }
    }
}
