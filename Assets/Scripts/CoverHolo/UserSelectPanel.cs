using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UserSelectPanel : BasePanel
{

    public BaseButton expertButton;
    public BaseButton clientButton;
    public BaseButton fullButton;
    public BaseButton spectateButton;

    public override void OnClick(BaseButton button)
    {
        if(button == expertButton)
        {
            SceneManager.LoadScene("scene_expert", LoadSceneMode.Single);
        }
        else if(button == clientButton)
        {
            SceneManager.LoadScene("scene_client", LoadSceneMode.Single);
        }
        else if (button == fullButton)
        {
            SceneManager.LoadScene("scene1", LoadSceneMode.Single);
        }
        else if (button == spectateButton)
        {
            SceneManager.LoadScene("Spectator", LoadSceneMode.Single);
        }
    }
}
