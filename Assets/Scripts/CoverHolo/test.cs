
using HoloToolkit.Sharing.Spawning;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class test : MonoBehaviour
{

    private void Start()
    {
        InputHandler.Instance.keyPress += Instance_keyPress;
    }

    private void Instance_keyPress(KeyPress obj)
    {
        if (obj.button == ControllerConfig.Y)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
