
using HoloToolkit.Sharing.Spawning;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class test : MonoBehaviour
{
    public List<WheelManager> wheelManager;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            foreach(WheelManager wheel in wheelManager)
            {
                wheel.Startplay();
            }
        }
        MainManager.Instance.CurrentMode = MainManager.Mode.Playing;
    }
}
