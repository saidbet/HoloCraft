using HoloToolkit.Sharing;
using HoloToolkit.Unity.InputModule;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FurnitureMenu : BasePanel
{

    public GameObject[] containers;

    private int currentPage;

    public BaseButton nextButton;
    public BaseButton prevButton;
    public BaseButton furnitureButton;
    public BaseButton colorButton;
    public BaseButton moveButton;

    private bool colorMode = true;

    public Material defaultSpriteMat;
    public Material defaultMat;
    private Material Aluminum;

    MeshRenderer[] renderers;

    protected override void Start()
    {
        base.Start();
        currentPage = 1;
        DisplayModels();
        ShareManager.Instance.onMessageEvent += Instance_onMessageEvent;
        transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2;
    }

    private void Instance_onMessageEvent(MessageSynchronizer obj)
    {
        switch(obj.messageType.Value)
        {
            case ShareManager.VERANDA_PLACED:
                FindMaterial();
                break;

            case ShareManager.SET_ACTIVE:
                if(SharedController.GetPath(gameObject) == obj.stringData.Value)
                    SharedController.SetActive(gameObject, obj.boolData.Value);
                break;

            case ShareManager.NEXT_PAGE:
                if (SharedController.GetPath(gameObject) == obj.stringData.Value)
                    NextPage();
                break;

            case ShareManager.PREV_PAGE:
                if (SharedController.GetPath(gameObject) == obj.stringData.Value)
                    PreviousPage();
                break;

            case ShareManager.COLOR_MODE:
                ColorMode();
                break;

            case ShareManager.FURNITURES_MODE:
                FurnituresMode();
                break;

            case ShareManager.CHANGE_COLOR:
                ChangeColor(obj.vectorData.Value);
                break;
        }
    }

    public void DisplayModels()
    {
        if(colorMode)
        {
            for (int i = 0; i < containers.Length; i++)
            {
                if (ModelsHolder.Instance.colors.Count > ((currentPage - 1) * 6) + i)
                {
                    if (containers[i].GetComponent<MeshRenderer>().enabled == false)
                    {
                        containers[i].GetComponent<MeshRenderer>().enabled = true;
                        containers[i].GetComponent<Collider>().enabled = true;
                    }
                    containers[i].GetComponent<MeshRenderer>().material = defaultMat;
                    containers[i].GetComponent<MeshRenderer>().material.color = ModelsHolder.Instance.colors[((currentPage - 1) * 6) + i];
                }
                else
                {
                    containers[i].GetComponent<MeshRenderer>().enabled = false;
                    containers[i].GetComponent<Collider>().enabled = false;
                }
            }
        }
        else
        {
            for (int i = 0; i < containers.Length; i++)
            {
                if (ModelsHolder.Instance.models.Count > ((currentPage - 1) * 6) + i)
                {
                    if (containers[i].GetComponent<MeshRenderer>().enabled == false)
                    {
                        containers[i].GetComponent<MeshRenderer>().enabled = true;
                        containers[i].GetComponent<Collider>().enabled = true;
                    }
                    containers[i].GetComponent<MeshRenderer>().material = defaultSpriteMat;
                    containers[i].GetComponent<MeshRenderer>().material.SetTexture("_MainTex", ModelsHolder.Instance.models[((currentPage - 1) * 6) + i].modelImage);
                }
                else
                {
                    containers[i].GetComponent<MeshRenderer>().enabled = false;
                    containers[i].GetComponent<Collider>().enabled = false;
                }
            }
        }
    }

    public void NextPage()
    {
        if(colorMode && ModelsHolder.Instance.colors.Count <= 6 * currentPage)
        {
            return;
        }
        else if(!colorMode && ModelsHolder.Instance.models.Count <= 6 * currentPage)
        {
            return;
        }

        currentPage += 1;
        DisplayModels();
    }

    public void PreviousPage()
    {
        if (currentPage > 1)
        {
            currentPage -= 1;
            DisplayModels();
        }
    }

    public void ColorMode()
    {
        colorMode = true;
        currentPage = 1;
        DisplayModels();
    }

    public void FurnituresMode()
    {
        colorMode = false;
        currentPage = 1;
        DisplayModels();
    }

    public void FindMaterial()
    {
        renderers = SharedController.Instance.placedVeranda.GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer renderer in renderers)
        {
            Material[] materials = renderer.materials;
            if (materials != null)
            {
                foreach (Material material in materials)
                {
                    if(material.name == "Aluminum (Instance)")
                    {
                        Aluminum = material;
                    }
                }
            }
        }
    }

    public override void OnClick(BaseButton button)
    {
        if (button == nextButton)
        {
            ShareManager.Instance.SendSyncMessage(ShareManager.NEXT_PAGE, gameObject.name);
        }
        else if (button == prevButton)
        {
            ShareManager.Instance.SendSyncMessage(ShareManager.PREV_PAGE, gameObject.name);
        }
        else if (button == furnitureButton)
        {
            ShareManager.Instance.SendSyncMessage(ShareManager.FURNITURES_MODE);
        }
        else if(button == colorButton)
        {
            ShareManager.Instance.SendSyncMessage(ShareManager.COLOR_MODE);
        }
        else if(button == moveButton)
        {
            ObjectPlacingController.Instance.MoveMenu(gameObject);
        }
        else
        {
            if(colorMode)
            {
                Color color = button.GetComponent<Renderer>().material.color;
                ShareManager.Instance.SendSyncMessage(ShareManager.CHANGE_COLOR, new Vector3(color.r, color.g, color.b));
            }
            else
            {
                Model selected = ModelsHolder.Instance.models.Find(Model => Model.modelImage == button.GetComponent<MeshRenderer>().material.GetTexture("_MainTex"));
                ObjectPlacingController.Instance.SpawnModel(selected.modelPrefab);
            }
        }
    }

    public void SetActive(bool state)
    {
        if (state == true)
            transform.position = Camera.main.transform.position + Camera.main.transform.forward * 1.5f;
        ShareManager.Instance.SendSyncMessage(ShareManager.SET_ACTIVE, state, SharedController.GetPath(gameObject), true);
    }

    private void ChangeColor(Vector3 color)
    {
        if (SharedController.Instance.placedVeranda == null)
        {
            return;
        }

        if (Aluminum == null)
        {
            FindMaterial();
        }

        Color newColor = new Color(color.x, color.y, color.z);
        Aluminum.color = newColor;
    }
}
