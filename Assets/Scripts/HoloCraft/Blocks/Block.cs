using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Block : MonoBehaviour
{
    public BlockType type;
    public SnapPoint[] snapPoints;
    public Renderer[] renderers;
    public List<Material> materials;
    public List<Color> defaultColors;
    public Vector3 position;
    public Quaternion rotation;

    private void Awake()
    {
        FindMats();
    }

    private void Start()
    {
        GetComponent<BlockPropertiesValues>().CreateProperties();
    }

    public void ToggleSnapPoints(bool state)
    {
        foreach (SnapPoint snap in snapPoints)
        {
            snap.enabled = state;
            if (state == false)
                Destroy(snap.GetComponent<Rigidbody>());
            else
                snap.gameObject.AddComponent<Rigidbody>();
        }
    }

    public void FindMats()
    {
        renderers = GetComponentsInChildren<Renderer>();
        materials = new List<Material>();

        for (int i = 0; i < renderers.Length; i++)
        {
            materials.AddRange(renderers[i].materials);
        }

        BackUpColors();
    }

    public void SetMaterialColor(Color colorToSet)
    {
        foreach (Material mat in materials)
        {
            mat.color = colorToSet;
        }
    }

    private void BackUpColors()
    {
        defaultColors = new List<Color>();

        foreach (Material mat in materials)
        {
            defaultColors.Add(mat.GetColor("_Color"));
        }
    }

    public void RestoreDefaultColor()
    {
        for (int i = 0; i < materials.Count; i++)
        {
            materials[i].SetColor("_Color", defaultColors[i]);
        }
    }

    public void Hide()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].enabled = false;
        }
    }

    public void UnHide()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].enabled = true;
        }
    }

    public void Validate()
    {
        position = this.transform.localPosition;
        rotation = this.transform.localRotation;
        RestoreDefaultColor();
        ToggleSnapPoints(false);
    }

    public void ResetCreationState()
    {
        this.transform.localPosition = position;
        this.transform.localRotation = rotation;
        this.transform.localScale = Vector3.one;
    }

    public void EnablePhysics()
    {
        GetComponent<Rigidbody>().isKinematic = false;
    }

    public void DisablePhysics()
    {
        GetComponent<Rigidbody>().isKinematic = true;
        foreach (FixedJoint joint in GetComponents<FixedJoint>())
        {
            Destroy(joint);
        }
    }
}
