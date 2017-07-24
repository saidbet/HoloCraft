using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropertiesManager : MonoBehaviour
{
    public List<PropertyPrefab> propertiesPrefabs;

    private void OnEnable()
    {
        Block currentBlock = MainManager.Instance.HoveredObject;
        if (currentBlock == null) return;

        for (int i = 0; i < currentBlock.type.properties.Length; i++)
        {
            GameObject current = Instantiate(propertiesPrefabs.Find(prop => prop.property == currentBlock.type.properties[i].property).prefab, transform);
            current.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -i * 40);
        }
    }

    private void OnDisable()
    {
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }


}
