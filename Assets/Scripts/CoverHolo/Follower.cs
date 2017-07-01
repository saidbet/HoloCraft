using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Follower : MonoBehaviour, IInputClickHandler
{
    public GameObject target;
    private bool detached;

    private void Update()
    {
        if(detached == false)
        {
            transform.position = target.transform.position;
            transform.rotation = target.transform.rotation;
        }
    }

    public void ScaleUp()
    {
        this.transform.localScale += new Vector3(5f, 5f, 5f);
    }

    public void ScaleDown()
    {
        this.transform.localScale -= new Vector3(0.5f, 0.5f, 0.5f);
    }

    public void Detach()
    {
        detached = true;
        VuforiaController.Instance.currentModel = null;
        VuforiaController.Instance.currentPrefab = null;
        InfoDisplay.Instance.UpdateText("Detached");
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        if(detached)
        {
            ShareManager.Instance.spawnManager.Delete(gameObject);
        }
    }
}
