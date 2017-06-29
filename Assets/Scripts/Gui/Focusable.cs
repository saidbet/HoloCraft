using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Focusable : MonoBehaviour, IFocusable
{
    public GameObject highlight;

    private void Start()
    {
        highlight.gameObject.SetActive(false);
    }

    public void OnFocusEnter()
    {
        highlight.gameObject.SetActive(true);
        highlight.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, this.transform.localPosition.z - 1);
        highlight.transform.rotation = this.transform.rotation;
        highlight.transform.localScale = this.transform.localScale;
    }

    public void OnFocusExit()
    {
        highlight.gameObject.SetActive(false);
    }
}
