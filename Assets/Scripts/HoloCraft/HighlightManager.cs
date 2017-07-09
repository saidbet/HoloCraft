using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighlightManager : MonoBehaviour {

    public void SetHighlight(GameObject target)
    {
        transform.position = target.transform.position;
        GetComponent<Image>().enabled = true;
    }

    public void HideHighlight()
    {
        GetComponent<Image>().enabled = false;
    }
}
