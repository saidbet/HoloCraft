using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PropertiesManager : Menu
{
    public List<PropertyPrefab> propertiesPrefabs;

    protected override void OnEnable()
    {
        Block currentBlock = MainManager.Instance.creator.HoveredObject;

        if (currentBlock == null) return;

        for (int i = 0; i < currentBlock.type.properties.Length; i++)
        {
            GameObject current = Instantiate(propertiesPrefabs.Find(prop => prop.property == currentBlock.type.properties[i].property).prefab, transform);
            current.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -i * 40);
        }

        base.OnEnable();
    }

    protected override void OnDisable()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        base.OnDisable();
    }
}
