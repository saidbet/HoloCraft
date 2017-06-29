using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientIpPanel : MonoBehaviour {

    public TextMesh statusConnection;
    private int dotNb;

    // Use this for initialization
    void Start () {
        dotNb = 1;
        statusConnection.text = "Please wait.";
        StartCoroutine(UpdateMessage());
    }


    IEnumerator UpdateMessage()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            dotNb++;
            if(dotNb > 3)
            {
                dotNb = 1;
            }
            string text = "Please wait";
            for(int i = 0; i < dotNb; i++)
            {
                text += ".";
            }
            statusConnection.text = text;
        }
    }

    private void OnDestroy()
    {
        StopCoroutine(UpdateMessage());
    }
}
