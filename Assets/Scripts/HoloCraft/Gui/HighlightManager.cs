using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighlightManager : MonoBehaviour {

    private RectTransform rect;
    private RectTransform targetRect;
    private Image visual;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        visual = GetComponent<Image>();
    }

    public void SetHighlight(GameObject target)
    {
        targetRect = target.GetComponent<RectTransform>();

        transform.position = target.transform.position;
        rect.sizeDelta = new Vector2(targetRect.rect.width, targetRect.rect.height);
        rect.pivot = targetRect.pivot;

        if(visual.enabled == false)
            visual.enabled = true;
    }

    public void HideHighlight()
    {
        if (visual.enabled == true)
            visual.enabled = false;
    }

    public void SetMaxSiblingIndex()
    {
        transform.SetSiblingIndex(transform.parent.childCount);
    }
}
