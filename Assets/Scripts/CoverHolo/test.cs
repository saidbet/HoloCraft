
using HoloToolkit.Sharing.Spawning;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class test : MonoBehaviour
{
    public WheelManager wheelManager;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            MainManager.Instance.CurrentMode = MainManager.Mode.Playing;
            wheelManager.Startplay();
        }
    }
}
