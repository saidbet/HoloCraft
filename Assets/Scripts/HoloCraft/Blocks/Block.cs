using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public BlockType type;
    public SnapPoint[] snapPoints;
    public Renderer[] renderers;
    public List<Material> materials;
    public List<Color> defaultColors;
    public List<Property> properties;

    private void Start()
    {
        properties = new List<Property>();

        foreach(PropertyValue prop in type.properties)
        {
            properties.Add(new Property(prop.property, prop.value));
        }
    }

    public void DisableSnapPoints()
    {
        foreach (SnapPoint snap in snapPoints)
        {
            Destroy(snap.GetComponent<Rigidbody>());
            Destroy(snap);
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
        for(int i = 0; i<renderers.Length; i++)
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
}
