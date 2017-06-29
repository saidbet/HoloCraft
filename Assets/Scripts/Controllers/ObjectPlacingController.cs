using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Sharing.Spawning;
using HoloToolkit.Sharing;

public class ObjectPlacingController : Singleton<ObjectPlacingController>, IInputClickHandler
{
    public enum EditState
    {
        firstPlacing,
        placing,
        rotating,
        scaling,
        placingMenu,
        none
    }

    private EditState currentState;

    public EditState CurrentState
    {
        get
        {
            return currentState;
        }
        set
        {
            if(currentObject != null)
            {
                if (value == EditState.none)
                {
                    ToggleEditing(true);
                }
                else
                {
                    ToggleEditing(false);
                }
            }

            currentState = value;
        }
    }

    public bool IsEditing
    {
        get
        {
            return !(CurrentState == EditState.none);
        }
    }

    private GameObject currentObject;
    private Vector3 extent;
    public ModelEditingMenu modelEditingMenu;
    public GameObject editingMenuPrefab;

    void Start()
    {
        InputManager.Instance.AddGlobalListener(this.gameObject);
        CurrentState = EditState.none;
        modelEditingMenu = Instantiate(editingMenuPrefab).GetComponent<ModelEditingMenu>();
    }

    void Update() {
        
        if(MrvcController.Instance != null && MrvcController.Instance.isRemote)
        {
            return;
        }

        if (CurrentState == EditState.placing || CurrentState == EditState.firstPlacing)
        {
            currentObject.transform.position = new Vector3(GazeManager.Instance.HitPosition.x, GazeManager.Instance.HitPosition.y + extent.y, GazeManager.Instance.HitPosition.z);
        }
        else if (CurrentState == EditState.rotating)
        {
            Vector3 positionForRotation = GazeManager.Instance.HitPosition;
            positionForRotation.y = currentObject.transform.position.y;
            currentObject.transform.LookAt(positionForRotation);
        }
        else if(CurrentState == EditState.placingMenu)
        {
            currentObject.transform.position = Camera.main.transform.position + (Camera.main.transform.forward * 2);
        }
    }

    public void SpawnModel(GameObject objectToPlace)
    {
        currentObject = ShareManager.Instance.spawnManager.Spawn(new SyncSpawnedObject(), objectToPlace, NetworkSpawnManager.EVERYONE, "");
        MeshRenderer mesh = currentObject.GetComponent<MeshRenderer>();
        extent = mesh.bounds.min;
        Vector3 center = mesh.bounds.center;
        CurrentState = EditState.firstPlacing;
    }

    public void MoveMenu(GameObject menu)
    {
        currentObject = new GameObject();
        currentObject.transform.SetParent(ShareManager.Instance.spawnManager.defaultParent.transform);
        currentObject.transform.position = GazeManager.Instance.HitPosition;
        menu.transform.SetParent(currentObject.transform);
        CurrentState = EditState.placingMenu;
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        if (CurrentState != EditState.none)
        {
            if (CurrentState == EditState.firstPlacing)
            {
                CurrentState = EditState.none;
            }
            else if (CurrentState == EditState.placingMenu)
            {
                CurrentState = EditState.none;
                currentObject.transform.GetChild(0).SetParent(ShareManager.Instance.spawnManager.defaultParent.transform);
                Destroy(currentObject);
            }
            else
            {
                CurrentState = EditState.none;
            }
        }

        if (GazeManager.Instance.IsGazingAtObject && GazeManager.Instance.HitObject.CompareTag("Furniture"))
        {

        }
        else
        {
            modelEditingMenu.SetActive(false);
        }
    }

    private void ToggleEditing(bool state)
    {
        BaseButton[] allButtons = transform.GetComponentsInChildren<BaseButton>();
        foreach (BaseButton button in allButtons)
        {
            Debug.Log("All buttons" + state);
            button.enabled = state;
        }

        if(state)
        {
            currentObject.layer = 0;
            currentObject.EnsureComponent<BoxCollider>().enabled = true;
            currentObject.EnsureComponent<Rigidbody>().detectCollisions = true;
            currentObject.transform.tag = "Furniture";
            currentObject.EnsureComponent<FurnitureController>().enabled = true;

            if(SharedController.Instance.placedVeranda != null)
            {
                SharedController.Instance.placedVeranda.GetComponent<Collider>().enabled = true;
            }

            if (ShareManager.Instance.userType == NetworkSpawnManager.EXPERT)
                MainController.Instance.guiPanel.gameObject.SetActive(false);

        }
        else
        {
            currentObject.layer = 2;
            if(currentObject.GetComponent<BoxCollider>() != null)
            {
                Destroy(currentObject.GetComponent<BoxCollider>());
            }
            if(currentObject.GetComponent<Rigidbody>() != null)
            {
                Destroy(currentObject.GetComponent<Rigidbody>());
            }

            if(SharedController.Instance.placedVeranda != null)
            {
                SharedController.Instance.placedVeranda.GetComponent<Collider>().enabled = false;
            }

            currentObject.transform.tag = "Untagged";

            if (ShareManager.Instance.userType == NetworkSpawnManager.EXPERT)
                MainController.Instance.guiPanel.gameObject.SetActive(true);
        }
    }

    public void ChangeState(GameObject model, EditState state)
    {
        currentObject = model;
        CurrentState = state;
    }

    public void Delete(GameObject model)
    {
        ShareManager.Instance.spawnManager.Delete(model);
    }
}
