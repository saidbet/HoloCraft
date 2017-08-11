
using HoloToolkit.Sharing.Spawning;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class test : MonoBehaviour
{

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            MainManager.Instance.CurrentMode = MainManager.Mode.Playing;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log(MainManager.Instance.creation.creationsList.nbrCreations);
            Debug.Log(MainManager.Instance.creation.creationsList.creations.Count);
        }
    }
}
