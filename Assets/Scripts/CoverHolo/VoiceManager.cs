using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceManager : MonoBehaviour
{
    public void ScaleUp()
    {
        VuforiaController.Instance.currentModel.GetComponent<Follower>().ScaleUp();
    }

    public void ScaleDown()
    {
        VuforiaController.Instance.currentModel.GetComponent<Follower>().ScaleDown();
    }

    public void Detach()
    {
        VuforiaController.Instance.currentModel.GetComponent<Follower>().Detach();
    }

    

}
