using HoloToolkit.Sharing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePanel : MonoBehaviour
{
    public GameObject highlight;

    public virtual void OnClick(BaseButton button) { }

    protected virtual void Start()
    {
        highlight = Instantiate(highlight, transform);
        highlight.GetComponent<Renderer>().enabled = false;
        highlight.GetComponent<Renderer>().material.renderQueue = 3001;
    }
}
